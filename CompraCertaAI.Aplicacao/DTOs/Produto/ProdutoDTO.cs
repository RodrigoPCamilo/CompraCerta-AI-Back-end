namespace CompraCertaAI.Aplicacao.DTOs.Produto
{
    public class ProdutoDTO
    {
        /// <summary>
        /// Null quando o produto é gerado pela IA (não persistido no banco).
        /// O front-end deve tratar id null/0 normalmente.
        /// </summary>
        public int? Id { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string PrecoOferta { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string ImagemUrl { get; set; } = string.Empty;
        public string Loja { get; set; } = string.Empty;
        public string LinkProduto { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
    }
}