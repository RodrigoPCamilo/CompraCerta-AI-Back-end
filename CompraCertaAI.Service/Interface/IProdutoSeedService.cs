using System.Threading.Tasks;

namespace CompraCertaAI.Service.Interface
{
    public interface IProdutoSeedService
    {
        Task SeedProdutosIniciaisAsync(int minimoPorCategoria = 10);
    }
}
