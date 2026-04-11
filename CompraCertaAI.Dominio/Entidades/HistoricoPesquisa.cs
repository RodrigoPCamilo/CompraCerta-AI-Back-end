using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompraCertaAI.Dominio.Entidades
{
    public class HistoricoPesquisa
    {
        public int Id { get; private set; }

        public int UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; }

        public string Query { get; private set; }
        public DateTime SearchDate { get; private set; }

        protected HistoricoPesquisa() { }

        public HistoricoPesquisa(int usuarioId, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query obrigatória.");

            UsuarioId = usuarioId;
            Query = query;
            SearchDate = DateTime.UtcNow;
        }
    }
}