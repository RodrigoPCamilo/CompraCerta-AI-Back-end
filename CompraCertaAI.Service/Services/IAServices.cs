using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using CompraCertaAI.Service.Interface;

namespace CompraCertaAI.Service.Services
{
    public class AiService : IIAService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetAiResponseAsync(string prompt)
        {
            var url = _config["Groq:ApiUrl"] ?? "https://api.groq.com/openai/v1/chat/completions";
            var apiKey = _config["Groq:ApiKey"];
            var model = _config["Groq:Model"] ?? "llama-3.3-70b-versatile";

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new Exception("Chave da Groq não configurada.");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CompraCertaAI");

            var requestBody = new
            {
                model,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Você é um assistente especializado em produtos de e-commerce brasileiro. Sempre responda SOMENTE com um array JSON válido, sem markdown, sem explicações, sem blocos de código. Sua resposta deve começar com [ e terminar com ]."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                temperature = 0.3,
                // 4000 tokens = espaço suficiente para 10 produtos com todos os campos
                max_tokens = 4000,
                // Forçar resposta JSON puro
                response_format = new { type = "json_object" }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro Groq: {response.StatusCode} - {error}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                return "[]";

            var contentStr = choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "[]";

            // Se a resposta for um objeto JSON com uma propriedade de array, extrair o array
            // (acontece quando response_format = json_object e o modelo retorna { "produtos": [...] })
            contentStr = contentStr.Trim();
            if (contentStr.StartsWith("{"))
            {
                try
                {
                    using var innerDoc = JsonDocument.Parse(contentStr);
                    // Procura qualquer propriedade que seja um array
                    foreach (var prop in innerDoc.RootElement.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            contentStr = prop.Value.GetRawText();
                            break;
                        }
                    }
                }
                catch
                {
                    // mantém o contentStr original
                }
            }

            return contentStr;
        }
    }
}