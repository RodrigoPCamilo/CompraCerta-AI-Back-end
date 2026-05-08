using System.Collections.Generic;
using System.Linq;

namespace CompraCertaAI.Service.Models
{
    /// <summary>
    /// Templates de prompt para a IA — versão melhorada.
    ///
    /// MELHORIAS v2:
    ///  1. Preços de oferta mais realistas e específicos por produto
    ///  2. Busca semântica expandida — interpreta intenção do usuário
    ///  3. Instruções mais rigorosas para evitar preço zero
    /// </summary>
    public static class AiPromptTemplates
    {
        // Mapeamento categoria → descrição detalhada para a IA
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

        // Lojas por tipo de produto
        private static readonly Dictionary<string, string> LojasPorTema = new(
            StringComparer.OrdinalIgnoreCase)
        {
            ["fitness"]      = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["equipamentos"] = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["livros"]       = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["roupas"]       = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["moda"]         = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["beleza"]       = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
            ["games"]        = "Amazon, Kabum, Magazine Luiza, Shopee, Americanas",
            ["tecnologia"]   = "Amazon, Kabum, Magazine Luiza, Shopee, Mercado Livre",
            ["casa"]         = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas",
        };

        // ──────────────────────────────────────────────────────────────────────
        // MELHORIA 1: Faixas de preço muito mais específicas e realistas
        // Baseadas em preços reais do mercado brasileiro (2025)
        // ──────────────────────────────────────────────────────────────────────
        private static readonly Dictionary<string, string> FaixasPreco = new(
            StringComparer.OrdinalIgnoreCase)
        {
            ["fitness"] =
                "halter 5kg: R$ 50-80 | halter 10kg: R$ 90-140 | kettlebell 16kg: R$ 150-220 | " +
                "whey protein 900g: R$ 80-130 | creatina: R$ 40-90 | tapete yoga: R$ 50-120 | " +
                "esteira elétrica: R$ 900-2500 | bicicleta ergométrica: R$ 600-2000 | " +
                "tênis corrida: R$ 200-600 | luva academia: R$ 40-120",

            ["games"] =
                "PS5: R$ 3.499-4.299 | Xbox Series X: R$ 3.799-4.499 | Xbox Series S: R$ 1.999-2.599 | " +
                "Nintendo Switch OLED: R$ 2.299-2.799 | Nintendo Switch: R$ 1.799-2.199 | " +
                "jogo PS5/Xbox: R$ 149-349 | jogo Nintendo: R$ 199-349 | " +
                "controle DualSense: R$ 399-499 | controle Xbox: R$ 349-449 | " +
                "headset gamer: R$ 200-800",

            ["tecnologia"] =
                "iPhone 15: R$ 5.499-6.499 | iPhone 14: R$ 3.999-4.799 | " +
                "Galaxy S24: R$ 4.299-5.499 | Galaxy A55: R$ 1.899-2.299 | Moto G85: R$ 1.299-1.699 | " +
                "MacBook Air M2: R$ 7.999-9.499 | notebook i5: R$ 2.499-3.999 | notebook i7: R$ 3.999-6.499 | " +
                "AirPods Pro: R$ 1.599-1.999 | JBL Flip 7: R$ 499-699 | Sony WH-1000XM5: R$ 1.399-1.799 | " +
                "smartwatch entry: R$ 299-599 | Apple Watch SE: R$ 2.499-3.199",

            ["casa"] =
                "utensílio cozinha: R$ 30-150 | panela: R$ 80-300 | airfryer: R$ 299-699 | " +
                "microondas: R$ 399-799 | liquidificador: R$ 80-350 | cafeteira: R$ 150-600 | " +
                "geladeira: R$ 1.899-4.999 | lavadora: R$ 1.499-3.999 | ar condicionado 9000 BTU: R$ 1.299-1.899 | " +
                "sofá 3 lugares: R$ 800-3.500 | cadeira escritório: R$ 400-1.500",

            ["livros"] =
                "livro nacional: R$ 35-75 | livro importado: R$ 60-150 | " +
                "box de livros: R$ 80-250 | mangá volume: R$ 25-50 | HQ: R$ 40-120",

            ["moda"] =
                "tênis Nike/Adidas básico: R$ 250-499 | tênis premium: R$ 500-900 | " +
                "camiseta: R$ 60-150 | calça jeans: R$ 120-300 | jaqueta: R$ 200-600 | " +
                "vestido: R$ 100-350 | bolsa: R$ 150-800",

            ["beleza"] =
                "perfume 50ml: R$ 150-400 | perfume 100ml: R$ 250-700 | " +
                "hidratante facial: R$ 50-200 | protetor solar: R$ 40-120 | " +
                "shampoo premium: R$ 30-80 | kit skincare: R$ 150-500",
        };

        private const string LojasGeral = "Amazon, Mercado Livre, Magazine Luiza, Shopee, Americanas, Kabum";

        // ──────────────────────────────────────────────────────────────────────
        // MELHORIA 2: Prompt de recomendação — garante preços por produto real
        // ──────────────────────────────────────────────────────────────────────
        public static string BuildRecommendationPrompt(IEnumerable<string> categorias, int limiteOverride = 0)
        {
            var cats = categorias?
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim()).Distinct().ToList()
                ?? new List<string>();

            if (!cats.Any()) cats = new List<string> { "Tecnologia" };

            var tema  = string.Join(", ", cats);
            var lojas = ResolverLojas(string.Join(" ", cats));
            var count = limiteOverride > 0 ? limiteOverride : lojas.Split(',').Length * 2;
            var ex    = lojas.Split(',')[0].Trim();

            return
$@"Retorne JSON array com {count} produtos de {tema} em promoção no Brasil, 2 por loja ({lojas}).
Cada objeto: nomeProduto(marca+modelo real), precoOferta(R$), precoOriginal(maior), desconto(%), descricao(max 60 chars sobre o proprio produto), imagemUrl(""), loja, linkProduto("").
Somente JSON, sem texto extra.
Exemplo: [{{""nomeProduto"":""Adidas Ultraboost 22"",""precoOferta"":""R$ 499,90"",""precoOriginal"":""R$ 699,90"",""desconto"":""29%"",""descricao"":""Tênis Adidas para corrida"",""imagemUrl"":"""",""loja"":""{ex}"",""linkProduto"":""""}}]";
        }

        // ──────────────────────────────────────────────────────────────────────
        // MELHORIA 3: Prompt de busca — interpreta intenção semântica do usuário
        // Expande termos ambíguos e garante produtos relevantes
        // ──────────────────────────────────────────────────────────────────────
        public static string BuildSearchPrompt(string query)
        {
            var q     = string.IsNullOrWhiteSpace(query) ? "produto" : query.Trim();
            var lojas = ResolverLojas(q);
            var faixa = ResolverFaixaPreco(q);
            var count = lojas.Split(',').Length * 2;
            var primeiraLoja = lojas.Split(',')[0].Trim();

            // Expansão semântica da busca — interpreta intenções comuns
            var expansao = ExpandirBusca(q);

            var linhaInterpretacao = expansao != q ? $"INTERPRETAÇÃO: {expansao}" : string.Empty;

            return
$@"BUSCA DO USUÁRIO: ""{q}""
{linhaInterpretacao}

Liste EXATAMENTE {count} produtos de ""{q}"" em PROMOÇÃO no Brasil.
2 produtos de cada loja: {lojas}.

REGRAS CRÍTICAS — seguir à risca:
1. TODOS os {count} produtos DEVEM ser sobre ""{q}"". A descricao de cada item DEVE descrever seu proprio nomeProduto — NUNCA misture descricoes entre produtos
2. Nome SEMPRE com marca real incluída (ex: ""Samsung Galaxy S24"", ""Nike Air Max 270"")
3. precoOferta: preço REAL de oferta em BRL. Referência: {faixa}
4. precoOriginal: OBRIGATÓRIO maior que precoOferta
5. desconto: calcule a % real entre precoOriginal e precoOferta
6. descricao: descreva ESPECIFICAMENTE o nomeProduto listado — a descricao DEVE bater com o produto do campo nomeProduto. NAO misture descricoes. Max 80 chars
7. NUNCA retorne precoOferta zerado, null ou ""Consultar""
8. imagemUrl: SEMPRE vazio """"
9. linkProduto: SEMPRE vazio """"

JSON array de EXATAMENTE {count} objetos, sem nenhum texto fora do array:
[{{""nomeProduto"":""nome com marca"",""precoOferta"":""R$ 199,90"",""precoOriginal"":""R$ 249,90"",""desconto"":""20%"",""descricao"":""descrição útil"",""imagemUrl"":"""",""loja"":""{primeiraLoja}"",""linkProduto"":""""}}]";
        }

        public static string BuildCategorySeedPrompt(string categoria, int quantidade)
        {
            var cat = string.IsNullOrWhiteSpace(categoria) ? "Tecnologia" : categoria.Trim();
            return BuildRecommendationPrompt(new[] { cat });
        }

        // ──────────────────────────────────────────────────────────────────────
        // MELHORIA: Expansão semântica da busca
        // Converte termos ambíguos em intenções claras para a IA
        // ──────────────────────────────────────────────────────────────────────
        private static string ExpandirBusca(string query)
        {
            var q = query.ToLowerInvariant().Trim();

            // Abreviações e gírias comuns
            if (q is "ps5" or "playstation 5")      return "PlayStation 5 console + jogos + acessórios";
            if (q is "ps4" or "playstation 4")      return "PlayStation 4 console + jogos + controles";
            if (q is "xbox")                         return "Xbox Series X/S console + jogos + controles";
            if (q is "switch" or "nintendo switch") return "Nintendo Switch OLED/Lite console + jogos";
            if (q.Contains("iphone"))               return $"Apple {query} smartphone com especificações e preço real";
            if (q.Contains("galaxy s"))             return $"Samsung {query} smartphone com especificações";
            if (q is "notebook" or "laptop")        return "notebooks de diferentes marcas e faixas de preço";
            if (q is "fone" or "headphone")         return "fones de ouvido com e sem fio, diferentes marcas";
            if (q is "smart tv" or "tv")            return "Smart TVs 4K de diferentes tamanhos e marcas";
            if (q is "fitness" or "academia")       return "equipamentos de fitness e academia: halteres, esteira, whey protein";
            if (q is "tênis" or "tenis")            return "tênis esportivos e casuais de marcas como Nike, Adidas, New Balance";
            if (q.Contains("livro"))                return $"livros: {query} — títulos com autor e editora";
            if (q.Contains("perfume"))              return $"perfumes: {query} — com volume e família olfativa";
            if (q.Contains("airfryer") || q.Contains("fritadeira")) return "fritadeiras sem óleo airfryer de diferentes capacidades";

            return query;
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

            // Tenta detectar categoria pelo nome do produto na busca
            if (t.Contains("iphone") || t.Contains("galaxy") || t.Contains("smartphone") ||
                t.Contains("notebook") || t.Contains("fone") || t.Contains("smart tv"))
                return FaixasPreco["tecnologia"];
            if (t.Contains("tênis") || t.Contains("tenis") || t.Contains("roupa") ||
                t.Contains("adidas") || t.Contains("nike"))
                return FaixasPreco["moda"];
            if (t.Contains("ps5") || t.Contains("xbox") || t.Contains("nintendo") ||
                t.Contains("jogo") || t.Contains("game"))
                return FaixasPreco["games"];
            if (t.Contains("whey") || t.Contains("halter") || t.Contains("academia") ||
                t.Contains("fitness") || t.Contains("esteira"))
                return FaixasPreco["fitness"];

            return "R$ 50-500 (use preço real de mercado para o produto específico)";
        }
    }
}