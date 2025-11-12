using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.QdrantConnection
{
    public class RAGQueryService
    {
        private readonly OllamaEmbeddingService _embeddingService;
        private readonly QdrantService _qdrantService;
        private readonly OllamaChatService _chatService;

        public RAGQueryService()
        {
            _embeddingService = new OllamaEmbeddingService("nomic-embed-text");
            _qdrantService = new QdrantService("joylee_chunks");
            _chatService = new OllamaChatService("llama3.1"); // model sinh của bạn
        }

        public async Task<string> AskAsync(string userQuery)
        {
            var queryVector = await _embeddingService.GetEmbeddingAsync(userQuery);

            // 2) Tìm chunk liên quan trong Qdrant
            var searchResults = await _qdrantService.SearchAsync(queryVector, topK: 5);

            var contextBuilder = new StringBuilder();
            foreach (var r in searchResults)
            {
                Debug.WriteLine("=== RESULT ===");
                Debug.WriteLine($"Score: {r.Score}");

                Debug.WriteLine("Payload JSON:");
                Debug.WriteLine(
                    Newtonsoft.Json.JsonConvert.SerializeObject(
                        r.Payload,
                        Newtonsoft.Json.Formatting.Indented
                    )
                );
                if (r.Payload.TryGetValue("text", out var textObj))
                {
                    contextBuilder.AppendLine(textObj.ToString());
                }
            }
                


            var context = contextBuilder.ToString();

            // 4) Tạo prompt dạng RAG
            var finalPrompt =
 $@"You are an AI assistant that must answer strictly based on the following context.

--- CONTEXT START ---
{context}
--- CONTEXT END ---

User question: {userQuery}

Guidelines:
- Use only the information found in the context.
- Do NOT invent any details that are not present in the context.
- If the context does not contain the answer, reply with: ""The provided documents do not contain the answer.""
- Provide clear, concise, and helpful answers.";
            // 5) Gửi prompt vào mô hình Ollama để lấy câu trả lời
            var answer = await _chatService.GenerateAsync(finalPrompt);

            return answer;
        }
    }
}
