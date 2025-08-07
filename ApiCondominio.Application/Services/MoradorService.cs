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
            id = m.id,
            nome = m.nome,
            celular = m.celular,
            email = m.email,
            isProprietario = m.isProprietario,
            dataEntrada = m.dataEntrada,
            dataSaida = m.dataSaida,
            dataInclusao = m.dataInclusao,
            dataAlteracao = m.dataAlteracao,
            imovelId = m.imovelId,
            imovelDto = new ImovelDto
            {
                id = m.Imovel.id,
                bloco = m.Imovel.bloco,
                apartamento = m.Imovel.apartamento,
                boxGaragem = m.Imovel.boxGaragem
            }
        });

        return Result<IEnumerable<MoradorDto>>.Success(dtos);
    }

    public async Task<Result<PagedResultDto<MoradorDto>>> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction)
    {
        (IEnumerable<Morador> items, int totalCount) = await _repository.GetAllPagedAsync(page, linesPerPage, orderBy, direction);

        IEnumerable<MoradorDto> dtos = items.Select(m => new MoradorDto
        {
            id = m.id,
            nome = m.nome,
            celular = m.celular,
            email = m.email,
            isProprietario = m.isProprietario,
            dataEntrada = m.dataEntrada,
            dataSaida = m.dataSaida,
            dataInclusao = m.dataInclusao,
            dataAlteracao = m.dataAlteracao,
            imovelId = m.imovelId,
            imovelDto = new ImovelDto
            {
                id = m.Imovel.id,
                bloco = m.Imovel.bloco,
                apartamento = m.Imovel.apartamento,
                boxGaragem = m.Imovel.boxGaragem
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
            id = m.id,
            nome = m.nome,
            celular = m.celular,
            email = m.email,
            isProprietario = m.isProprietario,
            dataEntrada = m.dataEntrada,
            dataSaida = m.dataSaida,
            dataInclusao = m.dataInclusao,
            dataAlteracao = m.dataAlteracao,
            imovelId = m.imovelId,
            imovelDto = new ImovelDto
            {
                id = m.Imovel.id,
                bloco = m.Imovel.bloco,
                apartamento = m.Imovel.apartamento,
                boxGaragem = m.Imovel.boxGaragem
            }
        };

        return Result<MoradorDto>.Success(dto);
    }

    public async Task<Result<MoradorDto>> AddAsync(MoradorDto dto)
    {
        Imovel? imovel = await _imovelRepository.GetByIdAsync(dto.imovelId);
        if (imovel is null)
            return Result<MoradorDto>.Failure("Imóvel informado não existe.");

        Morador morador = new Morador
        {
            nome = dto.nome,
            celular = dto.celular,
            email = dto.email,
            isProprietario = dto.isProprietario,
            dataEntrada = dto.dataEntrada,
            dataSaida = null,
            dataInclusao = dto.dataInclusao,//DateTime.UtcNow,
            dataAlteracao = null,
            imovelId = dto.imovelId
        };

        await _repository.AddAsync(morador);

        dto.id = morador.id;
        dto.dataInclusao = morador.dataInclusao;
        dto.imovelDto = new ImovelDto
        {
            id = imovel.id,
            bloco = imovel.bloco,
            apartamento = imovel.apartamento,
            boxGaragem = imovel.boxGaragem
        };

        return Result<MoradorDto>.Success(dto, "Morador criado com sucesso.");
    }

    public async Task<Result> UpdateAsync(int id, MoradorDto dto)
    {
        Morador? morador = await _repository.GetByIdAsync(id);
        if (morador is null)
            return Result.Failure("Morador não encontrado");

        Imovel? imovel = await _imovelRepository.GetByIdAsync(dto.imovelId);
        if (imovel is null)
            return Result.Failure("Imóvel não encontrado");

        morador.nome = dto.nome;
        morador.celular = dto.celular;
        morador.email = dto.email;
        morador.isProprietario = dto.isProprietario;
        morador.dataEntrada = DateTime.SpecifyKind(dto.dataEntrada, DateTimeKind.Utc);

        morador.dataInclusao = dto.dataInclusao != default
            ? DateTime.SpecifyKind(dto.dataInclusao, DateTimeKind.Utc)
            : morador.dataInclusao;

        morador.dataSaida = dto.dataSaida.HasValue
            ? DateTime.SpecifyKind(dto.dataSaida.Value, DateTimeKind.Utc)
            : null;

        morador.dataAlteracao = dto.dataAlteracao;//DateTime.UtcNow;

        morador.imovelId = dto.imovelId;


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
