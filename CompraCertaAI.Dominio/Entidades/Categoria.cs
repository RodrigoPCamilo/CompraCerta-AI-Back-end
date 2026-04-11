using System;
using System.Collections.Generic;

namespace CompraCertaAI.Dominio.Entidades
{
    public class Categoria
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public bool Ativa { get; private set; }

        public ICollection<Usuario> UsuariosFavoritos { get; set; } = new List<Usuario>();
        public ICollection<UsuarioCategoria> UsuarioCategorias { get; set; } = new List<UsuarioCategoria>();
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();

        protected Categoria() { }

        public Categoria(string nome, string descricao)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome da categoria é obrigatório.");

            Nome = nome;
            Descricao = descricao ?? string.Empty;
            Ativa = true;
        }
    }
}
