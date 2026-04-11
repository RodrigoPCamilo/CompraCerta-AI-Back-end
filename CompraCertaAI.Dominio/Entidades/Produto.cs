using System;

namespace CompraCertaAI.Dominio.Entidades
{
    public class Produto
    {
        public int Id { get; private set; }
        public string NomeProduto { get; private set; }
        public string Descricao { get; private set; }
        public string ImagemUrl { get; private set; }
        public string Loja { get; private set; }
        public string LinkProduto { get; private set; }
        public int CategoriaId { get; private set; }
        public bool Ativo { get; private set; }

        public Categoria Categoria { get; set; }

        protected Produto() { }

        public Produto(
            string nomeProduto,
            string descricao,
            string imagemUrl,
            string loja,
            string linkProduto,
            int categoriaId)
        {
            if (string.IsNullOrWhiteSpace(nomeProduto))
                throw new ArgumentException("Nome do produto é obrigatório.");
            if (string.IsNullOrWhiteSpace(loja))
                throw new ArgumentException("Loja é obrigatória.");
            if (string.IsNullOrWhiteSpace(linkProduto))
                throw new ArgumentException("Link do produto é obrigatório.");

            NomeProduto = nomeProduto;
            Descricao = descricao ?? string.Empty;
            ImagemUrl = imagemUrl ?? string.Empty;
            Loja = loja;
            LinkProduto = linkProduto;
            CategoriaId = categoriaId;
            Ativo = true;
        }
    }
}
