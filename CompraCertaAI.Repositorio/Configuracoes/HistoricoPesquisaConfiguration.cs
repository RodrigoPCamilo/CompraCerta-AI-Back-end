using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompraCertaAI.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompraCertaAI.Repositorio.Configuracoes
{
    public class  HistoricoPesquisaConfiguration : IEntityTypeConfiguration<HistoricoPesquisa>
    {
        public void Configure(EntityTypeBuilder<HistoricoPesquisa> builder)
        {
            builder.ToTable("HistoricoPesquisas").HasKey(h => h.Id);;
            builder.Property(h => h.Query).HasColumnName("Query").HasMaxLength(500).IsRequired();
            builder.Property(h => h.SearchDate).HasColumnName("SearchDate").IsRequired();

            builder.HasOne(h => h.Usuario)
                .WithMany(u => u.HistoricoPesquisas)
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}