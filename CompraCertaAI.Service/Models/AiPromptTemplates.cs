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
Você é um mecanismo inteligente de recomendação de produtos para uma plataforma de descoberta de ofertas no Brasil.

Sua tarefa é recomendar EXATAMENTE 10 produtos reais e de qualidade com bases nas categorias favoritas do usuário.

Categorias favoritas do usuário:
{categoriasTexto}

INSTRUÇÕES OBRIGATÓRIAS:
1. PREÇO: O campo ""precoOferta"" DEVE estar SEMPRE preenchido com o preço REAL em reais brasileiros.
   - Formato CORRETO: ""R$ 199,90"", ""R$ 1.299,00"", ""R$ 59,99""
   - Formato INCORRETO: vazio, null, ""---"", ""Consulte"", valores sem unidade monetária
   - **DEVE conter o prefixo 'R$ ' seguido de valor numérico com sempre 2 casas decimais**
   - Preços devem ser realistas para cada categoria e loja

2. PRODUTOS: Retorne apenas produtos reais de e-commerce brasileiro que existem:
   - Amazon.com.br
   - Mercado Livre
   - Magazine Luiza
   - Shopee
   - Kabum

3. QUALIDADE: Cada produto deve ter:
   - Nome descritivo (entre 10 e 100 caracteres)
   - Preço preenchido (obrigatório em formato BRL)
   - Descrição breve (máximo 200 caracteres)
   - URL de imagem válida
   - Link de produto válido

4. VALIDAÇÃO FINAL:
   - Revise cada item antes de retornar
   - Confirme que TODOS possuem preço preenchido
   - Um item SEM preço preenchido será descartado

Exemplo CORRETO:
[
  {{
    ""nomeProduto"": ""Smartphone Samsung Galaxy A15 4G 128GB"",
    ""precoOferta"": ""R$ 799,99"",
    ""descricao"": ""Smartphone com tela 6.5 polegadas e bateria de 5000mAh"",
    ""imagemUrl"": ""https://images.unsplash.com/photo-1511707267537-b85faf00021e"",
    ""loja"": ""Amazon"",
    ""linkProduto"": ""https://www.amazon.com.br/s?k=samsung+galaxy+a15""
  }}
]

Retorne SOMENTE um array JSON válido. SEM explicações ou comentários.
";
        }

        public static string BuildSearchPrompt(string query)
        {
            var queryLimpa = string.IsNullOrWhiteSpace(query) ? "" : query.Trim();

            return $@"
Você é um mecanismo de busca de produtos para uma plataforma de ofertas do Brasil.

O usuário realizou a seguinte busca:
{queryLimpa}

Sua tarefa é retornar até 10 produtos REAIS e relevantes relacionados à busca, com preços atualizados.

INSTRUÇÕES OBRIGATÓRIAS:
1. PREÇO: O campo ""precoOferta"" DEVE estar SEMPRE preenchido com o preço REAL em reais brasileiros.
   - Formato CORRETO: ""R$ 199,90"", ""R$ 1.299,00"", ""R$ 59,99""
   - Formato INCORRETO: vazio, null, ""---"", ""Consulte"", valores sem unidade monetária
   - **DEVE conter o prefixo 'R$ ' seguido de valor numérico com SEMPRE 2 casas decimais**
   - Preços devem ser realistas e corresponder ao produto descrito

2. BUSCA: Retorne apenas produtos que correspondem EXATAMENTE à busca do usuário
   - Relevância máxima
   - Produtos reais de lojas confiáveis (Amazon, Mercado Livre, Magazine Luiza, Shopee, Kabum)

3. QUALIDADE: Cada produto deve ter:
   - Nome descritivo que corresponde à busca
   - Preço preenchido (obrigatório em formato BRL)
   - Descrição breve
   - URL de imagem válida
   - Link de produto válido

Exemplo CORRETO:
[
  {{
    ""nomeProduto"": ""Headphone Bluetooth Sony WH-1000XM5"",
    ""precoOferta"": ""R$ 1.499,00"",
    ""descricao"": ""Headphone com cancelamento de ruído ativo e bateria de 40h"",
    ""imagemUrl"": ""https://images.unsplash.com/photo-1505740420928-5e560c06d30e"",
    ""loja"": ""Amazon"",
    ""linkProduto"": ""https://www.amazon.com.br/s?k=sony+headphone+wh1000xm5""
  }}
]

Retorne SOMENTE um array JSON válido. SEM explicações ou comentários.
";
        }

        public static string BuildCategorySeedPrompt(string categoria, int quantidade)
        {
            var categoriaLimpa = string.IsNullOrWhiteSpace(categoria) ? "Produtos Gerais" : categoria.Trim();
            var quantidadeAjustada = quantidade <= 0 ? 1 : quantidade;

            return $@"
Você é um gerador de dados de produtos para uma plataforma de e-commerce brasileira.

sua tarefa é retornar UMA LISTA com EXATAMENTE {quantidadeAjustada} produtos REAIS para esta categoria:

Categoria: {categoriaLimpa}

INSTRUÇÕES OBRIGATÓRIAS:
1. PREÇO: O campo ""precoOferta"" DEVE estar SEMPRE preenchido com o preço REAL em reais brasileiros.
   - Formato CORRETO: ""R$ 199,90"", ""R$ 1.299,00"", ""R$ 59,99""
   - Formato INCORRETO: vazio, null, ""---"", ""Consulte"", valores numéricos sem R$
   - **DEVE conter SEMPRE o prefixo 'R$ ' seguido de valor numérico com SEMPRE 2 casas decimais**
   - Preços devem ser REALISTAS e compatíveis com produtos brasileiros atuais

2. QUANTIDADE: Retorne exatamente {quantidadeAjustada} produtos reais e distintos da categoria
   - Produtos conhecidos e populares
   - Marcas reais e confiáveis
   - Preços condizentes com o mercado brasileiro

3. QUALIDADE OBRIGATÓRIA: Cada produto DEVE ter:
   - Nome descritivo e completo (marca + modelo/tipo)
   - Preço preenchido em BRL (R$ X,XX) — NÃO DEIXAR VAZIO
   - Descrição funcional breve (características principais)
   - URL de imagem válida
   - Loja real (Amazon, Mercado Livre, Magazine Luiza, Shopee, Kabum, etc.)
   - Link do produto válido para a loja

4. VALIDAÇÃO: Um item SEM preço preenchido será rejeito e não importado.
   - Se não conseguir preço real para um produto, NÃO inclua na lista
   - Qualidade sobre quantidade: prefira menos itens com preços reais a preencher com fabricados

Exemplo CORRETO para {categoriaLimpa}:
[
  {{
    ""nomeProduto"": ""Smartphone Samsung Galaxy A15 128GB"",
    ""precoOferta"": ""R$ 749,00"",
    ""descricao"": ""Smartphone com processador Exynos, tela AMOLED 6.5 polegadas e câmera de 50MP"",
    ""imagemUrl"": ""https://images.unsplash.com/photo-1511707267537-b85faf00021e"",
    ""loja"": ""Amazon"",
    ""linkProduto"": ""https://www.amazon.com.br/s?k=samsung+galaxy+a15""
  }}
]

🔴 **CRÍTICO**: Retorne SOMENTE um array JSON válido. Sem exceções, sem explicações, sem comentários extras.
";
        }
    }
}