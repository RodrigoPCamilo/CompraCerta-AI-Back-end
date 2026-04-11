
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CompraCertaAI.Repositorio.Contexto;

namespace CompraCertaAI.Repositorio
{
    public class CompraCertaAIContextoFactory
        : IDesignTimeDbContextFactory<CompraCertaAIContexto>
    {
        public CompraCertaAIContexto CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CompraCertaAIContexto>();

            optionsBuilder.UseSqlServer(
                "Server=NOTE286\\SQLEXPRESS02;Database=CompraCertaAIDb;Trusted_Connection=True;TrustServerCertificate=True"
            );

            return new CompraCertaAIContexto(optionsBuilder.Options);
        }
    }
}