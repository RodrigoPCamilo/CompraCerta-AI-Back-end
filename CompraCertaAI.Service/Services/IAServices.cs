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
                        role = "user",
                        content = prompt
                    }
                },
                temperature = 0.2,
                max_tokens = 1000
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

            var contentElement = choices[0]
                .GetProperty("message")
                .GetProperty("content");

            return contentElement.GetString() ?? "[]";
        }
    }
}
