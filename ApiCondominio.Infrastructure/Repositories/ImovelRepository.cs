using ApiCondominio.Domain.Entities;
using ApiCondominio.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiCondominio.Infrastructure.Repositories;

public class ImovelRepository(ApplicationDbContext context) : IImovelRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Imovel>> GetAllAsync()
    {
        string sql = "SELECT id, apartamento, bloco, box_garagem FROM public.imovel";
        List<Imovel> retorno = await _context.Set<Imovel>().FromSqlRaw(sql).ToListAsync();

        return retorno.OrderBy(x => x.Id).ToList();
    }

    public async Task<(IEnumerable<Imovel> Items, int TotalCount)> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction)
    {
        string order = string.IsNullOrEmpty(orderBy) ? "id" : orderBy;
        string dir = direction?.ToUpper() == "DESC" ? "DESC" : "ASC";

        string[] allowedColumns = ["id", "apartamento", "bloco", "box_garagem"];
        if (!allowedColumns.Contains(order.ToLower()))
            order = "id";

        int offset = page * linesPerPage;

        string sqlPaged = $@"SELECT id, apartamento, bloco, box_garagem FROM public.imovel ORDER BY {order} {dir} LIMIT {{0}} OFFSET {{1}}";

        string sqlCount = "SELECT COUNT(*) FROM public.imovel";

        List<Imovel> items = await _context.Imovels.FromSqlRaw(sqlPaged, linesPerPage, offset).ToListAsync();

        int totalCount = await _context.Database.ExecuteSqlRawAsync(sqlCount);

        totalCount = await _context.Imovels.CountAsync();

        return (items, totalCount);
    }

    public async Task<Imovel?> GetByIdAsync(int id)
    {
        string sql = "SELECT id, apartamento, bloco, box_garagem FROM public.imovel WHERE id = {0}";
        Imovel? retorno = await _context.Set<Imovel>().FromSqlRaw(sql, id).AsNoTracking().FirstOrDefaultAsync();

        return retorno;
    }

    public async Task AddAsync(Imovel imovel)
    {
        string sql = @"INSERT INTO public.imovel (apartamento, bloco, box_garagem)
            VALUES ({0}, {1}, {2})";

        int newId = await _context.Database.ExecuteSqlRawAsync(sql, imovel.Apartamento, imovel.Bloco, imovel.BoxGaragem);

        imovel.Id = newId;
    }

    public async Task UpdateAsync(Imovel imovel)
    {
        string sql = @"UPDATE public.imovel SET apartamento = {0}, bloco = {1}, box_garagem = {2} WHERE id = {3}";

        await _context.Database.ExecuteSqlRawAsync(sql, imovel.Apartamento, imovel.Bloco, imovel.BoxGaragem, imovel.Id);
    }

    public async Task DeleteAsync(int id)
    {
        string sql = "DELETE FROM public.imovel WHERE id = {0}";
        await _context.Database.ExecuteSqlRawAsync(sql, id);
    }
}
