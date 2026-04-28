using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CompraCertaAI.Service.Interface;

namespace CompraCertaAI.Service.Services
{
    public class AiService : IIAService
    {
        private readonly HttpClient         _httpClient;
        private readonly IConfiguration     _config;
        private readonly ILogger<AiService> _logger;

        public AiService(HttpClient httpClient, IConfiguration config, ILogger<AiService> logger)
        {
            _httpClient = httpClient;
            _config     = config;
            _logger     = logger;
        }

        public async Task<string> GetAiResponseAsync(string prompt)
        {
            var url    = _config["Groq:ApiUrl"] ?? "https://api.groq.com/openai/v1/chat/completions";
            var apiKey = _config["Groq:ApiKey"];
            var model  = _config["Groq:Model"]  ?? "llama-3.1-8b-instant";

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "Groq:ApiKey não configurado. Obtenha em https://console.groq.com");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CompraCertaAI/1.0");

            var requestBody = new
            {
                model,
                messages = new[]
                {
                    new
                    {
                        role    = "system",
                        content = "Responda APENAS com JSON array válido. " +
                                  "Sem texto fora do array. Comece com [ e termine com ]."
                    },
                    new { role = "user", content = prompt }
                },
                temperature = 0.4,
                max_tokens  = 1800
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8, "application/json");

            _logger.LogInformation("Groq request: model={Model}", model);

            var response = await _httpClient.PostAsync(url, content);

            // Rate limit por minuto — aguarda e tenta uma vez
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                var errBody  = await response.Content.ReadAsStringAsync();
                var segundos = ExtrairSegundos(errBody);

                // Se for limite DIÁRIO (TPD), não adianta esperar — falha direto
                if (errBody.Contains("tokens per day") || errBody.Contains("TPD"))
                {
                    _logger.LogError("Groq TPD esgotado. Aguarde até amanhã ou troque a chave.");
                    throw new HttpRequestException($"Groq TPD esgotado: {errBody[..Math.Min(150, errBody.Length)]}");
                }

                _logger.LogWarning("Groq rate limit TPM — aguardando {S}s", segundos);
                await Task.Delay(TimeSpan.FromSeconds(segundos));

                content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync(url, content);
            }

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                _logger.LogError("Groq {Status}: {Err}",
                    (int)response.StatusCode,
                    err.Length > 200 ? err[..200] : err);
                throw new HttpRequestException($"Groq {(int)response.StatusCode}: {err}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("choices", out var choices) ||
                choices.GetArrayLength() == 0)
                return "[]";

            var result = (choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "[]").Trim();

            // Remove markdown se o modelo adicionou
            if (result.StartsWith("```"))
            {
                result = Regex.Replace(result, @"^```[a-zA-Z]*\n?", "").TrimStart();
                result = Regex.Replace(result, @"\n?```$", "").TrimEnd();
                result = result.Trim();
            }

            if (result.StartsWith("{"))
            {
                try
                {
                    using var inner = JsonDocument.Parse(result);
                    foreach (var prop in inner.RootElement.EnumerateObject())
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        { result = prop.Value.GetRawText(); break; }
                }
                catch { }
            }

            _logger.LogInformation("Groq OK — {Len} chars", result.Length);
            return result;
        }

        private static int ExtrairSegundos(string body)
        {
            try
            {
                var m = Regex.Match(body, @"try again in ([\d.]+)s");
                if (m.Success && double.TryParse(
                    m.Groups[1].Value,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var s))
                    return (int)Math.Ceiling(s) + 1;
            }
            catch { }
            return 12;
        }
    }
}