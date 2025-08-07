using ApiCondominio.Application.DTOs;
using ApiCondominio.Application.Shared;

namespace ApiCondominio.Application.Interfaces;

public interface IMoradorService
{
    Task<Result<IEnumerable<MoradorDto>>> GetAllAsync();
    Task<Result<PagedResultDto<MoradorDto>>> GetAllPagedAsync(int page, int pageSize, string orderBy, string direction);
    Task<Result<MoradorDto>> GetByIdAsync(int id);
    Task<Result<MoradorDto>> AddAsync(MoradorDto moradorDto);
    Task<Result> UpdateAsync(int id, MoradorDto moradorDto);
    Task<Result> DeleteAsync(int id);
    Task<bool> ExistsByMoradorIdAsync(int id);
}
