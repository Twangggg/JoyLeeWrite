using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JoyLeeWrite.QdrantConnection
{
    public class OllamaChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public OllamaChatService(string model)
        {
            _model = model;
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:11434") };
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            var request = new
            {
                model = _model,
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/generate", content);
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<OllamaGenerateResponse>(json);
            return result.response;
        }
    }

    public class OllamaGenerateResponse
    {
        public string response { get; set; }
    }
}
