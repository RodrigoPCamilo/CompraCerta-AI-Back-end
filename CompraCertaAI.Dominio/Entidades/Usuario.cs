using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompraCertaAI.Dominio.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string SenhaHash { get; private set; }
        public DateTime DataCriacao { get; private set; }

        public ICollection<Categoria> CategoriasFavoritas { get; private set; } = new List<Categoria>();
        public ICollection<UsuarioCategoria> UsuarioCategorias { get; set; } = new List<UsuarioCategoria>();
        public ICollection<HistoricoPesquisa> HistoricoPesquisas { get; set; } = new List<HistoricoPesquisa>();

        protected Usuario()
        {

        }
        public Usuario(string nome, string email, string senha)
        {
            Validar(nome, email);
            Nome = nome;
            Email = email;
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
            DataCriacao = DateTime.UtcNow;
        }

        public bool ValidarSenha(string senha)
        {
            return BCrypt.Net.BCrypt.Verify(senha, SenhaHash);
        }

        public void DefinirCategoriasFavoritas(ICollection<Categoria> categorias)
        {
            if (categorias == null)
                throw new ArgumentException("Categorias favoritas inválidas.");

            if (categorias.Count > 5)
                throw new ArgumentException("Máximo de 5 categorias favoritas permitidas.");

            CategoriasFavoritas = categorias;
        }

        public void AtualizarDados(string nome, string email)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome inválido");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email inválido");

            Nome = nome;
            Email = email;
        }

        private void Validar(string nome, string email)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome obrigatório.");

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Email inválido.");
        }
    }
}