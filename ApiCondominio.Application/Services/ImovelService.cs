using ApiCondominio.Application.DTOs;
using ApiCondominio.Application.Interfaces;
using ApiCondominio.Application.Shared;
using ApiCondominio.Domain.Entities;
using ApiCondominio.Domain.Interfaces;

namespace ApiCondominio.Application.Services;

public class ImovelService(IImovelRepository repository, IMoradorRepository moradorRepository) : IImovelService
{
    private readonly IImovelRepository _repository = repository;
    private readonly IMoradorRepository _moradorRepository = moradorRepository;

    public async Task<Result<IEnumerable<ImovelDto>>> GetAllAsync()
    {
        IEnumerable<Imovel?> imoveis = await _repository.GetAllAsync();

        IEnumerable<ImovelDto> dtos = imoveis.Select(imovel => new ImovelDto
        {
            id = imovel.id,
            bloco = imovel.bloco,
            apartamento = imovel.apartamento,
            boxGaragem = imovel.boxGaragem
        });

        return Result<IEnumerable<ImovelDto>>.Success(dtos);
    }

    public async Task<Result<PagedResultDto<ImovelDto>>> GetAllPagedAsync(int page, int pageSize, string orderBy, string direction)
    {
        (IEnumerable<Imovel> items, int totalCount) = await _repository.GetAllPagedAsync(page, pageSize, orderBy, direction);

        IEnumerable<ImovelDto> dtos = items.Select(imovel => new ImovelDto
        {
            id = imovel.id,
            bloco = imovel.bloco,
            apartamento = imovel.apartamento,
            boxGaragem = imovel.boxGaragem
        });

        PagedResultDto<ImovelDto> paged = new PagedResultDto<ImovelDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = page,
            PageSize = pageSize
        };

        return Result<PagedResultDto<ImovelDto>>.Success(paged);
    }

    public async Task<Result<ImovelDto>> GetByIdAsync(int id)
    {
        Imovel? imovel = await _repository.GetByIdAsync(id);
        if (imovel is null)
            return Result<ImovelDto>.Failure("Imóvel não encontrado.");

        ImovelDto dto = new ImovelDto
        {
            id = imovel.id,
            bloco = imovel.bloco,
            apartamento = imovel.apartamento,
            boxGaragem = imovel.boxGaragem
        };

        return Result<ImovelDto>.Success(dto);
    }

    public async Task<Result<ImovelDto>> AddAsync(ImovelDto dto)
    {
        Imovel imovel = new Imovel
        {
            bloco = dto.bloco,
            apartamento = dto.apartamento,
            boxGaragem = dto.boxGaragem
        };

        await _repository.AddAsync(imovel);

        dto.id = imovel.id;
        return Result<ImovelDto>.Success(dto, "Imóvel criado com sucesso.");
    }

    public async Task<Result> UpdateAsync(int id, ImovelDto dto)
    {
        Imovel? imovel = await _repository.GetByIdAsync(id);

        if (imovel is null)
            return Result.Failure("Imóvel não encontrado.");

        imovel.bloco = dto.bloco;
        imovel.apartamento = dto.apartamento;
        imovel.boxGaragem = dto.boxGaragem;

        await _repository.UpdateAsync(imovel);
        return Result.Success("Imóvel atualizado com sucesso.");
    }

    public async Task<Result> DeleteAsync(int id)
    {
        bool possuiMorador = await _moradorRepository.ExistsByMoradorIdAsync(id);
        if (possuiMorador)
            return Result.Failure("Morador não encontrado");

        Imovel? imovel = await _repository.GetByIdAsync(id);
        if (imovel is null)
            return Result.Failure("Imóvel não encontrado");

        await _repository.DeleteAsync(id);
        return Result.Success("Imóvel deletado com sucesso.");
    }
}
