namespace CompraCertaAI.Aplicacao.DTOs.Produto
{
    public class ProdutoDTO
    {
        public int?   Id            { get; set; }
        public string NomeProduto   { get; set; } = string.Empty;
        public string PrecoOferta   { get; set; } = string.Empty;
        public string PrecoOriginal { get; set; } = string.Empty;
        public string Desconto      { get; set; } = string.Empty;
        public string Descricao     { get; set; } = string.Empty;
        public string ImagemUrl     { get; set; } = string.Empty;
        public string Loja          { get; set; } = string.Empty;
        public string LinkProduto   { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
    }
}
