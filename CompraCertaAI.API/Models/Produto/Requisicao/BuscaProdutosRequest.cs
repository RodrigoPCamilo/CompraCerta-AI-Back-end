namespace CompraCertaAI.API.Models.Produto.Requisicao
{
    public class BuscaProdutosRequest
    {
        public string Query { get; set; }
        public int? CategoriaId { get; set; }
    }
}
