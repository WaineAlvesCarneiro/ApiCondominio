using ApiCondominio.Domain.Entities;

namespace ApiCondominio.Domain.Interfaces;

public interface IImovelRepository
{
    Task<IEnumerable<Imovel>> GetAllAsync();
    Task<(IEnumerable<Imovel> Items, int TotalCount)> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction);
    Task<Imovel?> GetByIdAsync(int id);
    Task AddAsync(Imovel imovel);
    Task UpdateAsync(Imovel imovel);
    Task DeleteAsync(int id);
}
