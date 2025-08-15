using ApiCondominio.Application.DTOs;
using ApiCondominio.Application.Interfaces;
using ApiCondominio.Domain.Entities;
using ApiCondominio.Domain.Interfaces;
using ApiCondominio.Domain.Common;

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
            DataAlteracao = m?.DataAlteracao,
            ImovelId = m.ImovelId,
            ImovelDto = new ImovelDto
            {
                Id = m.ImovelId,
                Bloco = m.Bloco,
                Apartamento = m.Apartamento,
                BoxGaragem = m.BoxGaragem
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
                Id = m.ImovelId,
                Bloco = m.Bloco,
                Apartamento = m.Apartamento,
                BoxGaragem = m.BoxGaragem
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
        Morador? morador = await _repository.GetByIdAsync(id);
        if (morador is null)
            return Result<MoradorDto>.Failure("Morador não encontrado.");

        MoradorDto dto = new()
        {
            Id = morador.Id,
            Nome = morador.Nome,
            Celular = morador.Celular,
            Email = morador.Email,
            IsProprietario = morador.IsProprietario,
            DataEntrada = morador.DataEntrada,
            DataSaida = morador.DataSaida,
            DataInclusao = morador.DataInclusao,
            DataAlteracao = morador.DataAlteracao,
            ImovelId = morador.ImovelId,
            ImovelDto = new ImovelDto
            {
                Id = morador.ImovelId,
                Bloco = morador.Bloco,
                Apartamento = morador.Apartamento,
                BoxGaragem = morador.BoxGaragem
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
            DataInclusao = dto.DataInclusao,
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
        Imovel? imovel = await _imovelRepository.GetByIdAsync(dto.ImovelId);
        if (imovel is null)
            return Result<MoradorDto>.Failure("Imóvel informado não existe.");

        Morador? morador = await _repository.GetByIdAsync(id);
        if (morador is null)
            return Result.Failure("Morador não encontrado");

        morador.Nome = dto.Nome;
        morador.Celular = dto.Celular;
        morador.Email = dto.Email;
        morador.IsProprietario = dto.IsProprietario;
        morador.DataEntrada = dto.DataEntrada;
        morador.DataInclusao = dto.DataInclusao;
        morador.DataSaida = dto.DataSaida;
        morador.DataAlteracao = dto.DataAlteracao;
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
