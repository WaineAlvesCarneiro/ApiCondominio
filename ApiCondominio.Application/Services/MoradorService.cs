using ApiCondominio.Application.DTOs;
using ApiCondominio.Application.Interfaces;
using ApiCondominio.Application.Shared;
using ApiCondominio.Domain.Entities;
using ApiCondominio.Domain.Interfaces;

namespace ApiCondominio.Application.Services;

public class MoradorService(IMoradorRepository repository, IImovelRepository imovelRepository) : IMoradorService
{
    private readonly IMoradorRepository _repository = repository;
    private readonly IImovelRepository _imovelRepository = imovelRepository;

    public async Task<Result<IEnumerable<MoradorDto>>> GetAllAsync()
    {
        IEnumerable<Morador> moradores = await _repository.GetAllAsync();

        IEnumerable<MoradorDto> dtos = moradores.Select(m => new MoradorDto
        {
            Id = m.Id,
            Nome = m.Nome,
            Celular = m.Celular,
            Email = m.Email,
            IsProprietario = m.IsProprietario,
            DataEntrada = m.DataEntrada,
            DataSaida = m.DataSaida,
            DataInclusao = m.DataInclusao,
            DataAlteracao = m.DataAlteracao,
            ImovelId = m.ImovelId,
            ImovelDto = new ImovelDto
            {
                Id = m.Imovel.Id,
                Bloco = m.Imovel.Bloco,
                Apartamento = m.Imovel.Apartamento,
                BoxGaragem = m.Imovel.BoxGaragem
            }
        });

        return Result<IEnumerable<MoradorDto>>.Success(dtos);
    }

    public async Task<Result<PagedResultDto<MoradorDto>>> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction)
    {
        (IEnumerable<Morador> items, int totalCount) = await _repository.GetAllPagedAsync(page, linesPerPage, orderBy, direction);

        IEnumerable<MoradorDto> dtos = items.Select(m => new MoradorDto
        {
            Id = m.Id,
            Nome = m.Nome,
            Celular = m.Celular,
            Email = m.Email,
            IsProprietario = m.IsProprietario,
            DataEntrada = m.DataEntrada,
            DataSaida = m.DataSaida,
            DataInclusao = m.DataInclusao,
            DataAlteracao = m.DataAlteracao,
            ImovelId = m.ImovelId,
            ImovelDto = new ImovelDto
            {
                Id = m.Imovel.Id,
                Bloco = m.Imovel.Bloco,
                Apartamento = m.Imovel.Apartamento,
                BoxGaragem = m.Imovel.BoxGaragem
            }
        });

        PagedResultDto<MoradorDto> paged = new PagedResultDto<MoradorDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = page,
            LinesPerPage = linesPerPage
        };

        return Result<PagedResultDto<MoradorDto>>.Success(paged);
    }

    public async Task<Result<MoradorDto>> GetByIdAsync(int id)
    {
        Morador? m = await _repository.GetByIdAsync(id);
        if (m is null)
            return Result<MoradorDto>.Failure("Morador não encontrado.");

        MoradorDto dto = new MoradorDto
        {
            Id = m.Id,
            Nome = m.Nome,
            Celular = m.Celular,
            Email = m.Email,
            IsProprietario = m.IsProprietario,
            DataEntrada = m.DataEntrada,
            DataSaida = m.DataSaida,
            DataInclusao = m.DataInclusao,
            DataAlteracao = m.DataAlteracao,
            ImovelId = m.ImovelId,
            ImovelDto = new ImovelDto
            {
                Id = m.Imovel.Id,
                Bloco = m.Imovel.Bloco,
                Apartamento = m.Imovel.Apartamento,
                BoxGaragem = m.Imovel.BoxGaragem
            }
        };

        return Result<MoradorDto>.Success(dto);
    }

    public async Task<Result<MoradorDto>> AddAsync(MoradorDto dto)
    {
        Imovel? imovel = await _imovelRepository.GetByIdAsync(dto.ImovelId);
        if (imovel is null)
            return Result<MoradorDto>.Failure("Imóvel informado não existe.");

        Morador morador = new Morador
        {
            Nome = dto.Nome,
            Celular = dto.Celular,
            Email = dto.Email,
            IsProprietario = dto.IsProprietario,
            DataEntrada = dto.DataEntrada,
            DataSaida = null,
            DataInclusao = dto.DataInclusao,//DateTime.UtcNow,
            DataAlteracao = null,
            ImovelId = dto.ImovelId
        };

        await _repository.AddAsync(morador);

        dto.Id = morador.Id;
        dto.DataInclusao = morador.DataInclusao;
        dto.ImovelDto = new ImovelDto
        {
            Id = imovel.Id,
            Bloco = imovel.Bloco,
            Apartamento = imovel.Apartamento,
            BoxGaragem = imovel.BoxGaragem
        };

        return Result<MoradorDto>.Success(dto, "Morador criado com sucesso.");
    }

    public async Task<Result> UpdateAsync(int id, MoradorDto dto)
    {
        Morador? morador = await _repository.GetByIdAsync(id);
        if (morador is null)
            return Result.Failure("Morador não encontrado");

        Imovel? imovel = await _imovelRepository.GetByIdAsync(dto.ImovelId);
        if (imovel is null)
            return Result.Failure("Imóvel não encontrado");

        morador.Nome = dto.Nome;
        morador.Celular = dto.Celular;
        morador.Email = dto.Email;
        morador.IsProprietario = dto.IsProprietario;
        morador.DataEntrada = DateTime.SpecifyKind(dto.DataEntrada, DateTimeKind.Utc);

        morador.DataInclusao = dto.DataInclusao != default
            ? DateTime.SpecifyKind(dto.DataInclusao, DateTimeKind.Utc)
            : morador.DataInclusao;

        morador.DataSaida = dto.DataSaida.HasValue
            ? DateTime.SpecifyKind(dto.DataSaida.Value, DateTimeKind.Utc)
            : null;

        morador.DataAlteracao = dto.DataAlteracao;//DateTime.UtcNow;

        morador.ImovelId = dto.ImovelId;


        await _repository.UpdateAsync(morador);
        return Result.Success("Morador atualizado com sucesso.");
    }

    public async Task<Result> DeleteAsync(int id)
    {
        Morador? morador = await _repository.GetByIdAsync(id);
        if (morador is null)
            return Result.Failure("Morador não encontrado.");

        await _repository.DeleteAsync(id);
        return Result.Success("Morador deletado com sucesso.");
    }

    public async Task<bool> ExistsByMoradorIdAsync(int id)
    {
        bool exists = await _repository.ExistsByMoradorIdAsync(id);
        return exists;
    }
}
