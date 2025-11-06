using System.Threading.Tasks;

namespace JoyLeeWrite.QdrantConnection
{
    public class EmbeddingPipeline
    {
        private readonly ChunkService _chunkService;
        private readonly OllamaEmbeddingService _embeddingService;
        private readonly QdrantService _qdrantService;

        public EmbeddingPipeline()
        {
            _chunkService = new ChunkService();
            _embeddingService = new OllamaEmbeddingService("nomic-embed-text");
            _qdrantService = new QdrantService("joylee_chunks");
        }

        public async Task ProcessDocumentAsync(string documentText, string documentId)
        {
            Console.WriteLine("🚀 Chunking document...");
            var chunkResult = _chunkService.ChunkTextAdvanced(documentText, documentId: documentId);

            Console.WriteLine("🧠 Getting embeddings...");
            var embeddings = new List<float[]>();
            foreach (var chunk in chunkResult.Chunks)
            {
                var vector = await _embeddingService.GetEmbeddingAsync(chunk.Text);
                embeddings.Add(vector);
            }

            Console.WriteLine("🗄️ Creating Qdrant collection...");
            await _qdrantService.CreateCollectionAsync(embeddings.First().Length);

            Console.WriteLine("📥 Inserting into Qdrant...");
            await _qdrantService.InsertVectorsAsync(embeddings, chunkResult.Chunks);

            Console.WriteLine($"✅ Done! Indexed {chunkResult.Chunks.Count} chunks into Qdrant.");
        }
    }
}
