using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace JoyLeeWrite.QdrantConnection
{
    public class QdrantService
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionName;

        public QdrantService(string collectionName = "joylee_chunks")
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:6333") };
            _collectionName = collectionName;
        }



        public async Task CreateCollectionAsync(int vectorSize)
        {
            // 1. Tạo cấu hình cho bộ vector (size: 768, distance: Cosine)
            var vectorParams = new { size = vectorSize, distance = "Cosine" };

            // 2. Định nghĩa tên vector là "vector"
            var vectorConfig = new Dictionary<string, object>
    {
        { "vector", vectorParams } // Key phải là "vector" để khớp với Insert/Search
    };

            // 3. ✨ THÊM CẤU HÌNH OPTIMIZER ✨
            // Đặt indexing_threshold thấp (ví dụ: 100) để kích hoạt index hóa
            // ngay cả khi chỉ có vài chục điểm được chèn.
            var optimizerConfig = new
            {
                indexing_threshold = 100,
                // Có thể thêm một số cài đặt khác nếu cần (tùy chọn)
                default_segment_number = 4
            };

            // 4. Tạo final payload chứa cả Vectors và Optimizer Config
            var finalPayload = new
            {
                vectors = vectorConfig,
                optimizer_config = optimizerConfig // Đã thêm cấu hình optimizer vào yêu cầu tạo collection
            };

            // 5. Gửi yêu cầu PUT tới Qdrant API
            var response = await _httpClient.PutAsJsonAsync($"/collections/{_collectionName}", finalPayload);

            // 6. Xử lý lỗi (bỏ qua lỗi 409 Conflict nếu collection đã tồn tại)
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // 409 Conflict: collection đã tồn tại, đây là hành vi bình thường
                if (response.StatusCode != System.Net.HttpStatusCode.Conflict)
                {
                    throw new HttpRequestException($"Qdrant API call failed: {response.StatusCode} - {errorContent}");
                }
                else
                {
                    // Ghi log để xác nhận collection đã tồn tại (nếu cần)
                    System.Diagnostics.Debug.WriteLine($"Collection '{_collectionName}' đã tồn tại (409 Conflict).");
                }
            }
        }

        public async Task InsertVectorsAsync(List<float[]> vectors, List<TextChunk> chunks)
        {
            var points = vectors.Select((v, i) => new
            {
                id = chunks[i].Index,

                vector = new Dictionary<string, float[]>
                {
                    ["vector"] = v
                },

                payload = new Dictionary<string, object>(chunks[i].Metadata)
                {
                    ["text"] = chunks[i].Text
                }
            }).ToList();

            var payload = new { points };

            var response = await _httpClient.PutAsJsonAsync(
                $"/collections/{_collectionName}/points",
                payload);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Insert error: {error}");
            }
        }


        public async Task<List<(float Score, Dictionary<string, object> Payload)>> SearchAsync(float[] queryVector, int topK = 5)
        {
            var payload = new
            {
                vector = new
                {
                    name = "vector",
                    vector = queryVector
                },
                limit = topK,
                with_payload = true
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"/collections/{_collectionName}/points/search",
                payload
            );

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Qdrant Search Error: {json}");

            var result = JsonSerializer.Deserialize<QdrantSearchResponse>(json,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

            return result?.Result?
                .Select(r => (r.Score, r.Payload))
                .ToList()
                ?? new();
        }



        private class QdrantSearchResponse
        {
            public List<QdrantPointResult> Result { get; set; } = new();
        }

        private class QdrantPointResult
        {
            public float Score { get; set; }
            public Dictionary<string, object> Payload { get; set; } = new();
        }
    }
}
