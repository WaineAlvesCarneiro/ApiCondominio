using ApiCondominio.Application.DTOs;
using ApiCondominio.Application.Interfaces;
using ApiCondominio.Domain.Entities;
using ApiCondominio.Domain.Interfaces;
using ApiCondominio.Domain.Common;

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
            Id = imovel.Id,
            Bloco = imovel.Bloco,
            Apartamento = imovel.Apartamento,
            BoxGaragem = imovel.BoxGaragem
        });

        return Result<IEnumerable<ImovelDto>>.Success(dtos);
    }

    public async Task<Result<PagedResultDto<ImovelDto>>> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction)
    {
        (IEnumerable<Imovel> items, int totalCount) = await _repository.GetAllPagedAsync(page, linesPerPage, orderBy, direction);

        IEnumerable<ImovelDto> dtos = items.Select(imovel => new ImovelDto
        {
            Id = imovel.Id,
            Bloco = imovel.Bloco,
            Apartamento = imovel.Apartamento,
            BoxGaragem = imovel.BoxGaragem
        });

        PagedResultDto<ImovelDto> paged = new PagedResultDto<ImovelDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = page,
            LinesPerPage = linesPerPage
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
            Id = imovel.Id,
            Bloco = imovel.Bloco,
            Apartamento = imovel.Apartamento,
            BoxGaragem = imovel.BoxGaragem
        };

        return Result<ImovelDto>.Success(dto);
    }

    public async Task<Result<ImovelDto>> AddAsync(ImovelDto dto)
    {
        Imovel imovel = new Imovel
        {
            Bloco = dto.Bloco,
            Apartamento = dto.Apartamento,
            BoxGaragem = dto.BoxGaragem
        };

        await _repository.AddAsync(imovel);

        dto.Id = imovel.Id;
        return Result<ImovelDto>.Success(dto, "Imóvel criado com sucesso.");
    }

    public async Task<Result> UpdateAsync(int id, ImovelDto dto)
    {
        Imovel? imovel = await _repository.GetByIdAsync(id);

        if (imovel is null)
            return Result.Failure("Imóvel não encontrado.");

        imovel.Bloco = dto.Bloco;
        imovel.Apartamento = dto.Apartamento;
        imovel.BoxGaragem = dto.BoxGaragem;

        await _repository.UpdateAsync(imovel);
        return Result.Success("Imóvel atualizado com sucesso.");
    }

    public async Task<Result> DeleteAsync(int id)
    {
        bool possuiMorador = await _moradorRepository.ExistsByMoradorIdAsync(id);
        if (possuiMorador)
            return Result.Failure("Não é possível excluir o imóvel, pois existem moradores vinculados.");

        Imovel? imovel = await _repository.GetByIdAsync(id);
        if (imovel is null)
            return Result.Failure("Imóvel não encontrado");

        await _repository.DeleteAsync(id);
        return Result.Success("Imóvel deletado com sucesso.");
    }
}
