
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CortexCommerce.Repositorio.Contexto;

namespace CortexCommerce.Repositorio
{
    public class CortexCommerceContextoFactory
        : IDesignTimeDbContextFactory<CortexCommerceContexto>
    {
        public CortexCommerceContexto CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CortexCommerceContexto>();

            optionsBuilder.UseSqlServer(
                "Server=NOTE286\\SQLEXPRESS02;Database=CortexCommerceDb;Trusted_Connection=True;TrustServerCertificate=True"
            );

            return new CortexCommerceContexto(optionsBuilder.Options);
        }
    }
}