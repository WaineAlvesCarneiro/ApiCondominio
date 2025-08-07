using Microsoft.EntityFrameworkCore;
using ApiCondominio.Domain.Entities;

namespace ApiCondominio.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Imovel> Imovels => Set<Imovel>();
    public DbSet<Morador> Moradors => Set<Morador>();
}