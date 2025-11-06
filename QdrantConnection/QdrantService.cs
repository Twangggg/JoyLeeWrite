using System.Net.Http;
using System.Net.Http.Json;

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

        // 🧱 Tạo collection (nếu chưa có)
        public async Task CreateCollectionAsync(int vectorSize)
        {
            // *** PHƯƠNG PHÁP ĐẢM BẢO HOẠT ĐỘNG (Sử dụng Dictionary) ***

            // 1. Tạo cấu hình cho bộ vector (size và distance)
            var vectorParams = new { size = vectorSize, distance = "Cosine" };

            // 2. Sử dụng Dictionary để tạo object có tên vector (vd: "vector")
            var vectorConfig = new Dictionary<string, object>
    {
        { "vector", vectorParams }
    };

            // 3. Tạo final payload
            var finalPayload = new
            {
                vectors = vectorConfig
            };

            var response = await _httpClient.PutAsJsonAsync($"/collections/{_collectionName}", finalPayload);

            // Xử lý lỗi (giữ lại logic bạn đã sửa để bỏ qua 409 Conflict)
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != System.Net.HttpStatusCode.Conflict)
                {
                    throw new HttpRequestException($"Qdrant API call failed: {response.StatusCode} - {errorContent}");
                }
            }
        }

        // 📥 Thêm vectors vào Qdrant
        public async Task InsertVectorsAsync(List<float[]> vectors, List<TextChunk> chunks)
        {
            var points = vectors.Select((v, i) => new
            {
                id = chunks[i].Index,

                // 🚨 PHẦN QUAN TRỌNG NHẤT: Bọc mảng vector (v) vào một Dictionary
                // với key là tên vector: "vector"
                vector = new Dictionary<string, float[]>{
            {"vector", v} // Key phải là "vector"
        },

                payload = chunks[i].Metadata
            }).ToList();

            var payload = new { points };

            // Endpoint để chèn điểm
            var response = await _httpClient.PutAsJsonAsync($"/collections/{_collectionName}/points", payload);

            // Nếu bạn đang dùng EnsureSuccessStatusCode(), hãy bắt lỗi chính xác
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Ghi log chi tiết để thấy lỗi
                System.Diagnostics.Debug.WriteLine($"LỖI CHÈN VÀO QDRANT: {errorContent}");
                throw new HttpRequestException($"Lỗi chèn điểm Qdrant: {response.StatusCode} - {errorContent}");
            }
        }

        // 🔍 Tìm kiếm vector gần nhất
        public async Task<List<(float Score, Dictionary<string, object> Payload)>> SearchAsync(float[] queryVector, int topK = 5)
        {
            var payload = new
            {
                vector = queryVector,
                limit = topK
            };

            var response = await _httpClient.PostAsJsonAsync($"/collections/{_collectionName}/points/search", payload);
            var result = await response.Content.ReadFromJsonAsync<QdrantSearchResponse>();

            return result?.Result?
                .Select(r => (r.Score, r.Payload))
                .ToList() ?? new();
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
