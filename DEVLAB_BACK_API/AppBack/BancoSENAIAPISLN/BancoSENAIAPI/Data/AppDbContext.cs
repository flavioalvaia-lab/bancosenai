using BancoSENAIAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Carteira> Carteiras { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<DocumentoMetadado> DocumentosMetadados
        {
            get; set;
        }
    }
}