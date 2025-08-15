using Microsoft.EntityFrameworkCore;
using ApiCondominio.Domain.Entities;

namespace ApiCondominio.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Imovel> Imovels => Set<Imovel>();
    public DbSet<Morador> Moradors => Set<Morador>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTime>()
            .HaveColumnType("timestamp without time zone");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}