using Microsoft.EntityFrameworkCore;
using ApiCondominio.Domain.Interfaces;
using ApiCondominio.Domain.Entities;
using ApiCondominio.Persistence;

namespace ApiCondominio.Infrastructure.Repositories;

public class ImovelRepository(ApplicationDbContext context) : IImovelRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Imovel>> GetAllAsync() => await _context.Imovels.ToListAsync();

    public async Task<(IEnumerable<Imovel> Items, int TotalCount)> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction)
    {
        IQueryable<Imovel> query = _context.Imovels.AsQueryable();

        if (!string.IsNullOrEmpty(orderBy))
        {
            query = direction.ToUpper() switch
            {
                "DESC" => query.OrderByDescending(e => EF.Property<object>(e, orderBy)),
                _ => query.OrderBy(e => EF.Property<object>(e, orderBy)),
            };
        }

        int totalCount = await query.CountAsync();

        List<Imovel> items = await query
            .Skip(page * linesPerPage)
            .Take(linesPerPage)
            .ToListAsync();

        return (items, totalCount);
    }

    public Task<Imovel?> GetByIdAsync(int id) => _context.Imovels.FindAsync(id).AsTask();

    public async Task AddAsync(Imovel imovel)
    {
        _context.Imovels.Add(imovel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Imovel imovel)
    {
        _context.Imovels.Update(imovel);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Imovel? imovel = await _context.Imovels.FindAsync(id);
        if (imovel is not null)
        {
            _context.Imovels.Remove(imovel);
            await _context.SaveChangesAsync();
        }
    }
}