using System.Collections.Generic;
using System.Linq;

namespace CompraCertaAI.Service.Models
{
    public static class AiPromptTemplates
    {
        public static string BuildRecommendationPrompt(IEnumerable<string> categorias)
        {
            var categoriasLimpas = categorias?
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .ToList() ?? new List<string>();

            var categoriasTexto = categoriasLimpas.Any()
                ? string.Join(", ", categoriasLimpas)
                : "Sem categorias favoritas informadas.";

            return $@"
Você é um mecanismo inteligente de recomendação de produtos para uma plataforma de descoberta de ofertas.

Sua tarefa é recomendar até 10 produtos com base nas categorias favoritas do usuário.

Categorias favoritas do usuário:
{categoriasTexto}

Regras:
- Recomende no máximo 10 produtos
- Os produtos devem vir de lojas conhecidas e confiáveis
- Priorize produtos populares e com boa relação custo-benefício
- Evite produtos duplicados
- Os produtos devem ser relevantes para as categorias fornecidas
- Considere lojas populares no Brasil como Amazon, Mercado Livre, Magazine Luiza, Shopee e Kabum

Retorne SOMENTE um array JSON no seguinte formato:

[
  {{
    ""nomeProduto"": """",
    ""descricao"": """",
    ""imagemUrl"": """",
    ""loja"": """",
    ""linkProduto"": """"
  }}
]

Não inclua explicações, comentários ou texto fora do JSON.
Retorne apenas o JSON.
";
        }

        public static string BuildSearchPrompt(string query)
        {
            var queryLimpa = string.IsNullOrWhiteSpace(query) ? "" : query.Trim();

            return $@"
Você é um mecanismo de busca de produtos para uma plataforma de ofertas.

O usuário realizou a seguinte busca:

{queryLimpa}

Sua tarefa é retornar até 10 produtos relevantes relacionados à busca.

Regras:
- Retorne produtos reais e populares
- Priorize produtos vendidos em lojas confiáveis
- Evite produtos duplicados
- Priorize produtos com boa avaliação ou popularidade
- Os resultados devem ser altamente relevantes para a busca do usuário
- Considere lojas populares no Brasil como Amazon, Mercado Livre, Magazine Luiza, Shopee e Kabum

Retorne SOMENTE um array JSON no formato:

[
  {{
    ""nomeProduto"": """",
    ""descricao"": """",
    ""imagemUrl"": """",
    ""loja"": """",
    ""linkProduto"": """"
  }}
]

Não inclua explicações ou textos fora do JSON.
Retorne apenas o JSON.
";
        }
    }
}