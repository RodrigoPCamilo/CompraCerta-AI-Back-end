namespace CompraCertaAI.Dominio.Entidades
{
    public class UsuarioCategoria
    {
        public int UsuarioId { get; private set; }
        public int CategoriaId { get; private set; }

        public Usuario Usuario { get; set; }
        public Categoria Categoria { get; set; }

        protected UsuarioCategoria() { }

        public UsuarioCategoria(int usuarioId, int categoriaId)
        {
            UsuarioId = usuarioId;
            CategoriaId = categoriaId;
        }
    }
}
