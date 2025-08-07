using ApiCondominio.Domain.Entities;
using ApiCondominio.Domain.Interfaces;
using ApiCondominio.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiCondominio.Infrastructure.Repositories;

public class MoradorRepository(ApplicationDbContext context) : IMoradorRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Morador>> GetAllAsync() => await _context.Moradors.Include(m => m.Imovel).ToListAsync();

    public async Task<(IEnumerable<Morador> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize, string orderBy, string direction)
    {
        IQueryable<Morador> query = _context.Moradors.Include(m => m.Imovel).AsQueryable();

        if (!string.IsNullOrEmpty(orderBy))
        {
            query = direction.ToUpper() switch
            {
                "DESC" => query.OrderByDescending(e => EF.Property<object>(e, orderBy)),
                _ => query.OrderBy(e => EF.Property<object>(e, orderBy)),
            };
        }

        int totalCount = await query.CountAsync();

        List<Morador> items = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Morador?> GetByIdAsync(int id) => await _context.Moradors.Include(m => m.Imovel).FirstOrDefaultAsync(m => m.id == id);

    public async Task AddAsync(Morador morador)
    {
        _context.Moradors.Add(morador);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Morador morador)
    {
        _context.Moradors.Update(morador);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Morador? morador = await _context.Moradors.FindAsync(id);
        if (morador is not null)
        {
            _context.Moradors.Remove(morador);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByMoradorIdAsync(int id)
    {
        bool exists = await _context.Moradors.AnyAsync(m => m.id == id);
        return exists;
    }
}
