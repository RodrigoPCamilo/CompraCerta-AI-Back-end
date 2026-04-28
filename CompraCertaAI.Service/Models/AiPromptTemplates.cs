using System.Collections.Generic;
using System.Linq;

namespace CompraCertaAI.Service.Models
{
    public static class AiPromptTemplates
    {
        // Mapeamento das categorias do banco → descrição detalhada para a IA
        // Garante que "Fitness" gere produtos de fitness, não eletrônicos
        private static readonly Dictionary<string, string> MapCategoria = new(
            StringComparer.OrdinalIgnoreCase)
        {
            ["Tecnologia"] = "eletrônicos, smartphones, notebooks, tablets, fones de ouvido, smartwatches, gadgets tecnológicos",
            ["Games"]      = "jogos para PS5/Xbox/Nintendo/PC, consoles PlayStation/Xbox/Nintendo Switch, controles e acessórios gamer",
            ["Casa"]       = "decoração, móveis, utensílios domésticos, organização, cama mesa banho, eletrodomésticos",
            ["Livros"]     = "livros de ficção, romance, fantasia, técnicos, educação, autoajuda, mangá, HQ",
            ["Fitness"]    = "equipamentos de academia, halteres, kettlebell, esteira, bicicleta ergométrica, tapete yoga, suplementos proteína whey, roupas esportivas, tênis para corrida",
            ["Moda"]       = "roupas, calçados, tênis, bolsas, acessórios de moda",
            ["Beleza"]     = "cosméticos, perfumes, skincare, maquiagem, cuidados pessoais",
            ["Esportes"]   = "artigos esportivos, bolas, raquetes, material esportivo",
        };

        // Lojas por tipo de produto — evita Kabum em fitness/moda
        private static readonly Dictionary<string, string> LojasPorTema = new(
            StringComparer.OrdinalIgnoreCase)
        {
            ["fitness"]      = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["equipamentos"] = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["livros"]       = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["roupas"]       = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["moda"]         = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["games"]        = "Amazon, Kabum, Magazine Luiza, Shopee, Americanas",
            ["tecnologia"]   = "Amazon, Kabum, Magazine Luiza, Shopee, Mercado Livre",
            ["casa"]         = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
        };

        // Faixas de preço por tema para guiar a IA
        private static readonly Dictionary<string, string> FaixasPreco = new(
            StringComparer.OrdinalIgnoreCase)
        {
            ["fitness"]    = "halteres: R$ 50-300, whey protein: R$ 80-200, tênis esportivo: R$ 150-600, esteira: R$ 800-3000",
            ["games"]      = "jogos: R$ 150-350, consoles: R$ 2.500-5.000, acessórios: R$ 100-500",
            ["tecnologia"] = "smartphone: R$ 800-6000, notebook: R$ 2000-8000, fone: R$ 100-1500",
            ["casa"]       = "utensílio: R$ 30-200, decoração: R$ 50-500, eletrodoméstico: R$ 200-3000",
            ["livros"]     = "livros: R$ 20-80, box: R$ 80-300",
            ["moda"]       = "tênis: R$ 150-800, roupa: R$ 50-300",
        };

        private const string LojasGeral = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas, Kabum";

        public static string BuildRecommendationPrompt(IEnumerable<string> categorias)
        {
            var cats = categorias?
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim()).Distinct().ToList()
                ?? new List<string>();

            if (!cats.Any()) cats = new List<string> { "Tecnologia" };

            // Expande cada categoria para sua descrição detalhada
            var temasExpandidos = cats.Select(c =>
                MapCategoria.TryGetValue(c, out var desc) ? desc : c).ToList();

            var tema = string.Join(", ", temasExpandidos);
            var lojas = ResolverLojas(string.Join(" ", cats));
            var faixa = ResolverFaixaPreco(string.Join(" ", cats));
            var count = lojas.Split(',').Length * 2;

            return
$@"Liste EXATAMENTE {count} produtos em promoção no Brasil sobre: {tema}.
2 produtos de cada loja: {lojas}.

REGRAS OBRIGATÓRIAS:
1. Produtos DEVEM ser relacionados ao tema: {tema}
2. Nome com marca (ex: ""Adidas Superstar"", ""Sony PlayStation 5"")  
3. precoOferta: valor real em BRL. Referência: {faixa}
4. precoOriginal: maior que precoOferta
5. NUNCA deixe precoOferta zerado ou nulo

JSON array de {count} itens:
[{{""nomeProduto"":""nome com marca"",""precoOferta"":""R$ 99,90"",""precoOriginal"":""R$ 149,90"",""desconto"":""33%"",""descricao"":""descrição do produto"",""imagemUrl"":"""",""loja"":""{lojas.Split(',')[0].Trim()}"",""linkProduto"":""""}}]";
        }

        public static string BuildSearchPrompt(string query)
        {
            var q     = string.IsNullOrWhiteSpace(query) ? "produto" : query.Trim();
            var lojas = ResolverLojas(q);
            var faixa = ResolverFaixaPreco(q);
            var count = lojas.Split(',').Length * 2;

            return
$@"BUSCA: ""{q}""
Liste EXATAMENTE {count} produtos de ""{q}"" em promoção no Brasil, 2 de cada loja: {lojas}.

REGRAS:
1. TODOS os {count} produtos DEVEM ser sobre ""{q}""
2. Nome com marca sempre incluída
3. precoOferta: valor real em BRL. Referência: {faixa}
4. precoOriginal: maior que precoOferta
5. NUNCA use preço zero

JSON array de {count} itens:
[{{""nomeProduto"":""nome com marca"",""precoOferta"":""R$ 99,90"",""precoOriginal"":""R$ 149,90"",""desconto"":""33%"",""descricao"":""descrição"",""imagemUrl"":"""",""loja"":""{lojas.Split(',')[0].Trim()}"",""linkProduto"":""""}}]";
        }

        public static string BuildCategorySeedPrompt(string categoria, int quantidade)
        {
            var cat = string.IsNullOrWhiteSpace(categoria) ? "Tecnologia" : categoria.Trim();
            return BuildRecommendationPrompt(new[] { cat });
        }

        private static string ResolverLojas(string tema)
        {
            var t = tema.ToLowerInvariant();
            foreach (var (key, lojas) in LojasPorTema)
                if (t.Contains(key.ToLower())) return lojas;
            return LojasGeral;
        }

        private static string ResolverFaixaPreco(string tema)
        {
            var t = tema.ToLowerInvariant();
            foreach (var (key, faixa) in FaixasPreco)
                if (t.Contains(key.ToLower())) return faixa;
            return "R$ 50-500";
        }
    }
}
