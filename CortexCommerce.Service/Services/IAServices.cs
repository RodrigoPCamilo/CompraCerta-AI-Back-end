using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using CortexCommerce.Service.Interface;

namespace CortexCommerce.Service.Services
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
            var url = "https://models.inference.ai.azure.com/chat/completions";
            var token = _config["GitHubModels:Token"];

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Token do GitHub Models não configurado.");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CortexCommerce");

            var dataAtual = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var requestBody = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = $@"
Você é um ASSISTENTE ESPECIALISTA EM E-COMMERCE, curadoria de produtos e comparação de preços.

CONTEXTO TEMPORAL:
- Data atual de referência: {dataAtual}
- Pesquise se há produtos no estoque, se os preços estão atualizados, e se as lojas estão ativas.

OBJETIVO:
Ajudar usuários a encontrar os MELHORES produtos do mercado com foco em custo-benefício, nos dias atuais.

REGRAS:
- Recomende 2 a 5 produtos
- Informe nome, descrição curta, preço médio e link 
- o link do produto quero que seja a pagina dele por exemplo:
.Amazon: Echo Dot (5ª Geração) com Relógio o link seria: https://www.amazon.com.br/s?k=Echo+Dot+%285%C2%AA+Gera%C3%A7%C3%A3o%29+com+Rel%C3%B3gio&__mk_pt_BR=%C3%85M%C3%85%C5%BD%C3%95%C3%91&crid=2LW9XBLQRU9WB&sprefix=echo+dot+5%C2%AA+gera%C3%A7%C3%A3o+com+rel%C3%B3gio%2Caps%2C190&ref=nb_sb_noss_2
.Mercado Livre: Echo Dot (5ª Geração) com Relógio o link seria: https://lista.mercadolivre.com.br/echo-dot-5%C2%AA-gera%C3%A7%C3%A3o-com-rel%C3%B3gio#D[A:Echo%20Dot%20(5%C2%AA%20Gera%C3%A7%C3%A3o)%20com%20Rel%C3%B3gio] 
.Magazine Luiza : Echo Dot (5ª Geração) com Relógio o link seria: https://www.magazineluiza.com.br/busca/echo+dot+5a+geracao+com+relogio/
.Shopee: Echo Dot (5ª Geração) com Relógio o link seria: https://shopee.com.br/search?keyword=echo%20dot%20(5%C2%AA%20gera%C3%A7%C3%A3o)%20com%20rel%C3%B3gio

- Responda em português do Brasil
- NÃO diga que é uma IA
"
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                max_tokens = 800
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
                throw new Exception($"Erro IA: {response.StatusCode} - {error}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            return doc
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?? "Sem resposta da IA.";
        }
    }
}
// - Priorize Amazon, Mercado Livre, Magalu, Shopee