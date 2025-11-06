using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
namespace JoyLeeWrite.QdrantConnection { 
    public class OllamaEmbeddingService { 
        private readonly HttpClient _httpClient;
        private readonly string _modelName; 
        public OllamaEmbeddingService(string modelName = "nomic-embed-text") 
        { 
            _httpClient = new HttpClient 
            { 
                BaseAddress = new Uri("http://localhost:11434") 
            }; 
            _modelName = modelName; 
        } 
        public async Task<float[]> GetEmbeddingAsync(string text) { 
            var payload = new { model = _modelName, prompt = text }; 
            var response = await _httpClient.PostAsJsonAsync("/api/embeddings", payload); 
            if (!response.IsSuccessStatusCode) 
            { 
                var error = await response.Content.ReadAsStringAsync(); 
                throw new Exception($"Ollama embedding failed: {error}"); 
            }
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            JsonElement root = doc.RootElement;

            float[] vector;

            // ✅ Kiểm tra các cấu trúc JSON có thể có
            if (root.TryGetProperty("embedding", out var singleEmbedding))
            {
                // Trường hợp cũ: {"embedding": [...]}
                vector = singleEmbedding.EnumerateArray().Select(v => v.GetSingle()).ToArray();
            }
            else if (root.TryGetProperty("embeddings", out var embeddingsArray))
            {
                // Trường hợp mới: {"embeddings": [[...]]}
                vector = embeddingsArray[0].EnumerateArray().Select(v => v.GetSingle()).ToArray();
            }
            else if (root.TryGetProperty("data", out var dataArray))
            {
                // Một số model embedding khác: {"data": [{"embedding": [...]}]}
                vector = dataArray[0].GetProperty("embedding").EnumerateArray().Select(v => v.GetSingle()).ToArray();
            }
            else
            {
                throw new Exception("❌ Không tìm thấy embedding trong phản hồi của Ollama.");
            }

            return vector;

        }
    } 
}