namespace CompraCertaAI.Aplicacao.DTOs.Produto
{
    public class BuscaProdutosDto
    {
        public string Query { get; set; }
        public int? CategoriaId { get; set; }
        public int UsuarioId { get; set; }
    }
}
