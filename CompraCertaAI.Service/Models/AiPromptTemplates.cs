using System.Collections.Generic;
using System.Linq;

namespace CompraCertaAI.Service.Models
{
    public static class AiPromptTemplates
    {
        private const string LojasBrasil =
            "Amazon, Mercado Livre, Magazine Luiza (Magalu), Shopee, Americanas, Kabum";

        public static string BuildRecommendationPrompt(IEnumerable<string> categorias)
        {
            var categoriasLimpas = categorias?
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .ToList() ?? new List<string>();

            var categoriasTexto = categoriasLimpas.Any()
                ? string.Join(", ", categoriasLimpas)
                : "Eletrônicos, Moda, Casa";

            return $@"Retorne exatamente 10 produtos em oferta do e-commerce brasileiro, baseados nas categorias: {categoriasTexto}.

Lojas permitidas: {LojasBrasil}.

Regras obrigatórias:
- Retorne EXATAMENTE 10 itens no array
- Cada item deve ter: nomeProduto, precoOferta (formato ""R$ 0,00""), descricao, imagemUrl, loja, linkProduto
- linkProduto deve ser a URL de busca da loja pelo produto (ex: https://www.amazon.com.br/s?k=produto)
- imagemUrl use URLs do Unsplash relacionadas ao produto (ex: https://images.unsplash.com/photo-ID?w=400)
- Distribua os produtos entre as lojas permitidas
- Preços devem ser realistas para o mercado brasileiro atual

Retorne SOMENTE o array JSON, sem explicações:
[
  {{
    ""nomeProduto"": ""Nome completo do produto"",
    ""precoOferta"": ""R$ 0,00"",
    ""descricao"": ""Breve descrição do produto"",
    ""imagemUrl"": ""https://images.unsplash.com/photo-ID?w=400"",
    ""loja"": ""Nome da loja"",
    ""linkProduto"": ""https://url-de-busca-na-loja""
  }}
]";
        }

        public static string BuildSearchPrompt(string query)
        {
            var queryLimpa = string.IsNullOrWhiteSpace(query) ? "produto" : query.Trim();

            return $@"Retorne exatamente 10 produtos em oferta relacionados à busca: ""{queryLimpa}"".

Lojas permitidas: {LojasBrasil}.

Regras obrigatórias:
- Retorne EXATAMENTE 10 itens no array
- Todos os produtos devem ser relevantes para a busca ""{queryLimpa}""
- Cada item deve ter: nomeProduto, precoOferta (formato ""R$ 0,00""), descricao, imagemUrl, loja, linkProduto
- linkProduto deve ser a URL de busca da loja pelo produto (ex: https://www.amazon.com.br/s?k={System.Uri.EscapeDataString(queryLimpa)})
- imagemUrl use URLs do Unsplash relacionadas ao produto
- Distribua os produtos entre as lojas: Amazon, Mercado Livre, Shopee, Magazine Luiza, Americanas
- Preços devem ser realistas para o mercado brasileiro atual

Retorne SOMENTE o array JSON, sem explicações:
[
  {{
    ""nomeProduto"": ""Nome completo do produto"",
    ""precoOferta"": ""R$ 0,00"",
    ""descricao"": ""Breve descrição do produto"",
    ""imagemUrl"": ""https://images.unsplash.com/photo-ID?w=400"",
    ""loja"": ""Nome da loja"",
    ""linkProduto"": ""https://url-de-busca-na-loja""
  }}
]";
        }

        public static string BuildCategorySeedPrompt(string categoria, int quantidade)
        {
            var categoriaLimpa = string.IsNullOrWhiteSpace(categoria) ? "Produtos Gerais" : categoria.Trim();
            var qtd = quantidade <= 0 ? 10 : quantidade;

            return $@"Retorne exatamente {qtd} produtos em oferta da categoria ""{categoriaLimpa}"" do e-commerce brasileiro.

Lojas permitidas: {LojasBrasil}.

Regras obrigatórias:
- Retorne EXATAMENTE {qtd} itens no array
- Cada item deve ter: nomeProduto, precoOferta (formato ""R$ 0,00""), descricao, imagemUrl, loja, linkProduto
- linkProduto deve ser a URL de busca da loja pelo produto
- imagemUrl use URLs do Unsplash relacionadas ao produto
- Distribua entre as lojas disponíveis
- Preços devem ser realistas para o mercado brasileiro atual

Retorne SOMENTE o array JSON, sem explicações:
[
  {{
    ""nomeProduto"": ""Nome completo do produto"",
    ""precoOferta"": ""R$ 0,00"",
    ""descricao"": ""Breve descrição do produto"",
    ""imagemUrl"": ""https://images.unsplash.com/photo-ID?w=400"",
    ""loja"": ""Nome da loja"",
    ""linkProduto"": ""https://url-de-busca-na-loja""
  }}
]";
        }
    }
}