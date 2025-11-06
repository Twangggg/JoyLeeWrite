using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JoyLeeWrite.QdrantConnection
{
    /// <summary>
    /// 🎯 CHUNK SERVICE TỐI ƯU CHO EMBEDDINGS
    /// Khắc phục các vấn đề: chunk size không ổn định, thiếu metadata, không tối ưu cho RAG
    /// </summary>
    public class ChunkService
    {
        private readonly int _defaultChunkSize;
        private readonly int _defaultOverlap;
        private readonly int _minChunkSize;
        private readonly int _maxChunkSize;

        public ChunkService(
            int defaultChunkSize = 500,   // 🎯 Tối ưu cho Gemini embedding
            int defaultOverlap = 50)      // 🔗 10% overlap giữ ngữ cảnh
        {
            if (defaultChunkSize <= 0)
                throw new ArgumentException("Chunk size must be positive", nameof(defaultChunkSize));

            if (defaultOverlap < 0)
                throw new ArgumentException("Overlap cannot be negative", nameof(defaultOverlap));

            if (defaultOverlap >= defaultChunkSize)
                throw new ArgumentException("Overlap must be less than chunk size", nameof(defaultOverlap));

            _defaultChunkSize = defaultChunkSize;
            _defaultOverlap = defaultOverlap;

            // 🆕 Giới hạn linh hoạt để tránh chunks quá nhỏ/lớn
            _minChunkSize = (int)(defaultChunkSize * 0.5);  // Tối thiểu 50%
            _maxChunkSize = (int)(defaultChunkSize * 1.5);  // Tối đa 150%
        }

        /// <summary>
        /// 🎯 PHƯƠNG THỨC CHÍNH: Chunk với metadata đầy đủ
        /// Trả về ChunkResult thay vì List<string> để có thêm thông tin
        /// </summary>
        public ChunkResult ChunkTextAdvanced(
            string text,
            int? chunkSize = null,
            int? overlap = null,
            string? documentId = null,
            Dictionary<string, object>? metadata = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("⚠️ Empty or null text provided");
                return new ChunkResult { Chunks = new List<TextChunk>() };
            }

            int actualChunkSize = chunkSize ?? _defaultChunkSize;
            int actualOverlap = overlap ?? _defaultOverlap;

            // Validate
            if (actualOverlap >= actualChunkSize)
            {
                Console.WriteLine($"⚠️ Overlap ({actualOverlap}) >= ChunkSize ({actualChunkSize}), adjusting to {actualChunkSize / 4}");
                actualOverlap = actualChunkSize / 4; // Giảm xuống 25% thay vì 50%
            }

            var chunks = new List<TextChunk>();

            // Text quá ngắn - trả về 1 chunk
            if (text.Length <= actualChunkSize)
            {
                chunks.Add(CreateChunk(text, 0, 0, text.Length, documentId, metadata));
                Console.WriteLine($"✅ Text is short, created 1 chunk ({text.Length} chars)");
                return new ChunkResult { Chunks = chunks };
            }

            // Split theo câu
            var sentences = SplitIntoSentences(text);
            Console.WriteLine($"📝 Split into {sentences.Count} sentences");

            var currentSentences = new List<string>();
            int currentLength = 0;
            int chunkIndex = 0;
            int globalPosition = 0;

            foreach (var sentence in sentences)
            {
                var sentenceLength = sentence.Length;

                // 🎯 LOGIC MỚI: Kiểm tra cả min và max size
                bool shouldSplit = currentLength + sentenceLength > actualChunkSize
                                   && currentLength >= _minChunkSize
                                   && currentSentences.Count > 0;

                if (shouldSplit)
                {
                    // Lưu chunk hiện tại
                    var chunkText = string.Join(" ", currentSentences).Trim();
                    var chunk = CreateChunk(
                        chunkText,
                        chunkIndex++,
                        globalPosition,
                        chunkText.Length,
                        documentId,
                        metadata
                    );
                    chunks.Add(chunk);
                    globalPosition += chunkText.Length;

                    // 🆕 OVERLAP THÔNG MINH: Giữ lại câu cuối hoặc N chars
                    var overlapSentences = GetOverlapSentences(currentSentences, actualOverlap);
                    currentSentences = overlapSentences;
                    currentLength = currentSentences.Sum(s => s.Length);
                }

                // Thêm câu mới
                currentSentences.Add(sentence);
                currentLength += sentenceLength;

                // 🆕 XỬ LÝ CHUNK QUÁ LỚN: Nếu 1 câu quá dài
                if (sentenceLength > _maxChunkSize)
                {
                    Console.WriteLine($"⚠️ Very long sentence detected ({sentenceLength} chars), splitting further");

                    // Lưu các câu trước đó trước
                    if (currentSentences.Count > 1)
                    {
                        currentSentences.RemoveAt(currentSentences.Count - 1); // Bỏ câu dài ra
                        if (currentSentences.Count > 0)
                        {
                            var chunkText = string.Join(" ", currentSentences).Trim();
                            chunks.Add(CreateChunk(chunkText, chunkIndex++, globalPosition, chunkText.Length, documentId, metadata));
                            globalPosition += chunkText.Length;
                        }
                    }

                    // Chia câu dài thành sub-chunks
                    var subChunks = SplitLongSentence(sentence, actualChunkSize, actualOverlap);
                    foreach (var subChunk in subChunks)
                    {
                        chunks.Add(CreateChunk(subChunk, chunkIndex++, globalPosition, subChunk.Length, documentId, metadata));
                        globalPosition += subChunk.Length;
                    }

                    currentSentences.Clear();
                    currentLength = 0;
                }
            }

            // Lưu chunk cuối
            if (currentSentences.Count > 0)
            {
                var chunkText = string.Join(" ", currentSentences).Trim();
                chunks.Add(CreateChunk(chunkText, chunkIndex, globalPosition, chunkText.Length, documentId, metadata));
            }

            var result = new ChunkResult
            {
                Chunks = chunks,
                Stats = CalculateStats(chunks)
            };

            Console.WriteLine($"✅ Created {chunks.Count} chunks");
            Console.WriteLine($"   📊 Avg: {result.Stats.AverageLength:F0} chars, Min: {result.Stats.MinLength}, Max: {result.Stats.MaxLength}");
            Console.WriteLine($"   🎯 Quality Score: {result.Stats.QualityScore:F2}/10");

            return result;
        }

        /// <summary>
        /// 🆕 Tạo TextChunk với metadata đầy đủ
        /// </summary>
        private TextChunk CreateChunk(
            string text,
            int index,
            int startPos,
            int length,
            string? documentId,
            Dictionary<string, object>? baseMetadata)
        {
            var chunk = new TextChunk
            {
                Index = index,
                Text = text,
                StartPosition = startPos,
                EndPosition = startPos + length,
                CharCount = text.Length,
                WordCount = text.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length,
                TokenEstimate = EstimateTokens(text),
                DocumentId = documentId,
                Metadata = new Dictionary<string, object>()
            };

            // Copy base metadata
            if (baseMetadata != null)
            {
                foreach (var kvp in baseMetadata)
                {
                    chunk.Metadata[kvp.Key] = kvp.Value;
                }
            }

            // Add chunk-specific metadata
            chunk.Metadata["chunk_index"] = index;
            chunk.Metadata["char_count"] = chunk.CharCount;
            chunk.Metadata["word_count"] = chunk.WordCount;

            return chunk;
        }

        /// <summary>
        /// 🆕 Lấy overlap sentences thông minh
        /// </summary>
        private List<string> GetOverlapSentences(List<string> sentences, int targetOverlapChars)
        {
            var overlap = new List<string>();
            int overlapChars = 0;

            // Lấy câu từ cuối lên đầu cho đến khi đủ overlap
            for (int i = sentences.Count - 1; i >= 0 && overlapChars < targetOverlapChars; i--)
            {
                overlap.Insert(0, sentences[i]);
                overlapChars += sentences[i].Length;
            }

            return overlap;
        }

        /// <summary>
        /// 🆕 Chia câu quá dài thành sub-chunks
        /// </summary>
        private List<string> SplitLongSentence(string sentence, int maxSize, int overlap)
        {
            var chunks = new List<string>();
            var words = sentence.Split(' ');
            var currentChunk = new StringBuilder();

            foreach (var word in words)
            {
                if (currentChunk.Length + word.Length + 1 > maxSize && currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString().Trim());

                    // Overlap: giữ lại vài từ cuối
                    var lastWords = GetLastWords(currentChunk.ToString(), overlap);
                    currentChunk.Clear();
                    currentChunk.Append(lastWords);
                    if (lastWords.Length > 0) currentChunk.Append(" ");
                }

                if (currentChunk.Length > 0) currentChunk.Append(" ");
                currentChunk.Append(word);
            }

            if (currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString().Trim());
            }

            return chunks;
        }

        /// <summary>
        /// 🆕 Lấy N từ cuối cùng
        /// </summary>
        private string GetLastWords(string text, int maxChars)
        {
            if (text.Length <= maxChars) return text;

            var words = text.Split(' ');
            var result = new List<string>();
            int length = 0;

            for (int i = words.Length - 1; i >= 0 && length < maxChars; i--)
            {
                result.Insert(0, words[i]);
                length += words[i].Length + 1;
            }

            return string.Join(" ", result);
        }

        /// <summary>
        /// 📊 Ước tính số tokens (1 token ≈ 4 chars tiếng Anh)
        /// </summary>
        private int EstimateTokens(string text)
        {
            // Gemini: ~4 chars per token cho tiếng Anh
            // Tiếng Việt có thể khác: ~3-3.5 chars per token
            return (int)Math.Ceiling(text.Length / 4.0);
        }

        /// <summary>
        /// Split text into sentences (improved)
        /// </summary>
        private List<string> SplitIntoSentences(string text)
        {
            // Bảo vệ abbreviations
            var protectedText = text
                .Replace("Mr.", "Mr<DOT>")
                .Replace("Mrs.", "Mrs<DOT>")
                .Replace("Ms.", "Ms<DOT>")
                .Replace("Dr.", "Dr<DOT>")
                .Replace("Prof.", "Prof<DOT>")
                .Replace("Sr.", "Sr<DOT>")
                .Replace("Jr.", "Jr<DOT>")
                .Replace("vs.", "vs<DOT>")
                .Replace("etc.", "etc<DOT>")
                .Replace("i.e.", "i<DOT>e<DOT>")
                .Replace("e.g.", "e<DOT>g<DOT>");

            // Split với regex mạnh hơn
            var regex = new Regex(@"(?<=[.!?])\s+(?=[A-Z\""'])", RegexOptions.Multiline);
            var sentences = regex.Split(protectedText);

            return sentences
                .Select(s => s.Replace("<DOT>", ".").Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        /// <summary>
        /// 📊 Calculate detailed stats
        /// </summary>
        private ChunkStats CalculateStats(List<TextChunk> chunks)
        {
            if (chunks.Count == 0)
                return new ChunkStats();

            var lengths = chunks.Select(c => c.CharCount).ToList();
            var tokens = chunks.Select(c => c.TokenEstimate).ToList();

            // 🆕 Quality score: đánh giá độ đồng đều của chunks
            var avgLength = lengths.Average();
            var variance = lengths.Select(l => Math.Pow(l - avgLength, 2)).Average();
            var stdDev = Math.Sqrt(variance);
            var coefficientOfVariation = stdDev / avgLength;

            // Score 10 = hoàn hảo đồng đều, 0 = rất không đồng đều
            var qualityScore = Math.Max(0, 10 * (1 - coefficientOfVariation));

            return new ChunkStats
            {
                TotalChunks = chunks.Count,
                AverageLength = (int)avgLength,
                MinLength = lengths.Min(),
                MaxLength = lengths.Max(),
                TotalCharacters = lengths.Sum(),
                AverageTokens = (int)tokens.Average(),
                TotalTokens = tokens.Sum(),
                StandardDeviation = (int)stdDev,
                QualityScore = qualityScore
            };
        }

        /// <summary>
        /// 🔄 BACKWARD COMPATIBILITY: Method cũ vẫn hoạt động
        /// </summary>
        public List<string> ChunkText(string text, int? chunkSize = null, int? overlap = null)
        {
            var result = ChunkTextAdvanced(text, chunkSize, overlap);
            return result.Chunks.Select(c => c.Text).ToList();
        }

        /// <summary>
        /// Get statistics (legacy support)
        /// </summary>
        public ChunkStats GetChunkStats(List<string> chunks)
        {
            if (chunks == null || chunks.Count == 0)
                return new ChunkStats();

            var lengths = chunks.Select(c => c.Length).ToList();
            var avgLength = lengths.Average();
            var variance = lengths.Select(l => Math.Pow(l - avgLength, 2)).Average();

            return new ChunkStats
            {
                TotalChunks = chunks.Count,
                AverageLength = (int)avgLength,
                MinLength = lengths.Min(),
                MaxLength = lengths.Max(),
                TotalCharacters = lengths.Sum(),
                StandardDeviation = (int)Math.Sqrt(variance)
            };
        }
    }

    /// <summary>
    /// 🆕 TextChunk với metadata đầy đủ
    /// </summary>
    public class TextChunk
    {
        public int Index { get; set; }
        public string Text { get; set; } = string.Empty;
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public int CharCount { get; set; }
        public int WordCount { get; set; }
        public int TokenEstimate { get; set; }
        public string? DocumentId { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();

        public override string ToString()
        {
            return $"Chunk #{Index}: {CharCount} chars, ~{TokenEstimate} tokens, [{StartPosition}-{EndPosition}]";
        }
    }

    /// <summary>
    /// 🆕 ChunkResult với stats
    /// </summary>
    public class ChunkResult
    {
        public List<TextChunk> Chunks { get; set; } = new();
        public ChunkStats Stats { get; set; } = new();
    }

    /// <summary>
    /// 📊 Enhanced ChunkStats
    /// </summary>
    public class ChunkStats
    {
        public int TotalChunks { get; set; }
        public int AverageLength { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public int TotalCharacters { get; set; }
        public int StandardDeviation { get; set; }

        // 🆕 Token estimates
        public int AverageTokens { get; set; }
        public int TotalTokens { get; set; }

        // 🆕 Quality metrics
        public double QualityScore { get; set; } // 0-10

        public override string ToString()
        {
            return $"Chunks: {TotalChunks}, Avg: {AverageLength}±{StandardDeviation} chars, " +
                   $"Range: [{MinLength}-{MaxLength}], Quality: {QualityScore:F1}/10, " +
                   $"~{TotalTokens} tokens total";
        }
    }
}

