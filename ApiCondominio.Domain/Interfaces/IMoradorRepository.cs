namespace ApiCondominio.Domain.Interfaces;

public interface IMoradorRepository
{
    Task<IEnumerable<Morador>> GetAllAsync();
    Task<(IEnumerable<Morador> Items, int TotalCount)> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction);
    Task<Morador?> GetByIdAsync(int id);
    Task AddAsync(Morador imovel);
    Task UpdateAsync(Morador imovel);
    Task DeleteAsync(int id);
    Task<bool> ExistsByMoradorIdAsync(int id);
}
