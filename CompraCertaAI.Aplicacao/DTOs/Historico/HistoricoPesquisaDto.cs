using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompraCertaAI.Aplicacao.DTOs.Historico
{
    public class HistoricoPesquisaDto
    {
        public int Id { get; set; }
        public string Query { get; set; }
        public DateTime SearchDate { get; set; }
    }
}