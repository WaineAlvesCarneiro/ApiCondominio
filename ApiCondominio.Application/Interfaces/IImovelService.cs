using ApiCondominio.Application.DTOs;
using ApiCondominio.Domain.Common;

namespace ApiCondominio.Application.Interfaces;

public interface IImovelService
{
    Task<Result<IEnumerable<ImovelDto>>> GetAllAsync();
    Task<Result<PagedResultDto<ImovelDto>>> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction);
    Task<Result<ImovelDto>> GetByIdAsync(int id);
    Task<Result<ImovelDto>> AddAsync(ImovelDto imovelDto);
    Task<Result> UpdateAsync(int id, ImovelDto imovelDto);
    Task<Result> DeleteAsync(int id);
}