using ApiCondominio.Application.DTOs;
using ApiCondominio.Application.Interfaces;
using ApiCondominio.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace ApiCondominio.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ImovelController(IImovelService service) : ControllerBase
{
    private readonly IImovelService _service = service;

    /// <summary>
    /// Lista todos os imóveis.
    /// </summary>
    /// <response code="200">Lista retornada com sucesso.</response>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Result<IEnumerable<ImovelDto>> result = await _service.GetAllAsync();

        if (!result.Sucesso)
            return Conflict(new { sucesso = false, erro = result.Mensagem });

        return Ok(new { sucesso = true, dados = result.Dados });
    }

    /// <summary>
    /// Lista os imóveis com paginação.
    /// </summary>
    /// <param name="page">Página atual</param>
    /// <param name="pageSize ">Itens por página</param>
    /// <param name="orderBy">Campo de ordenação</param>
    /// <param name="direction">Direção ASC ou DESC</param>
    /// <response code="200">Lista paginada retornada com sucesso.</response>
    [HttpGet("paginado")]
    public async Task<IActionResult> GetAllPagedAsync(
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10,
        [FromQuery] string orderBy = "bloco",
        [FromQuery] string direction = "ASC")
    {
        Result<PagedResultDto<ImovelDto>> result = await _service.GetAllPagedAsync(page, pageSize, orderBy, direction);

        if (!result.Sucesso)
            return Conflict(new { sucesso = false, erro = result.Mensagem });

        return Ok(new { sucesso = true, dados = result.Dados });
    }

    /// <summary>
    /// Retorna um imóvel por ID.
    /// </summary>
    /// <response code="200">Imóvel encontrado.</response>
    /// <response code="404">Imóvel não encontrado.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        Result<ImovelDto> result = await _service.GetByIdAsync(id);

        if (!result.Sucesso)
            return NotFound(new { sucesso = false, erro = result.Mensagem });

        return Ok(new { sucesso = true, dados = result.Dados });
    }

    /// <summary>
    /// Cadastra um novo imóvel.
    /// </summary>
    /// <response code="201">Imóvel criado com sucesso.</response>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ImovelDto dto)
    {
        Result<ImovelDto> result = await _service.AddAsync(dto);

        if (!result.Sucesso)
            return Conflict(new { sucesso = false, erro = result.Mensagem });

        return CreatedAtAction(nameof(GetById), new { id = result.Dados!.Id }, new
        {
            sucesso = true,
            dados = result.Dados
        });
    }

    /// <summary>
    /// Atualiza os dados de um imóvel existente.
    /// </summary>
    /// <response code="204">Imóvel atualizado com sucesso.</response>
    /// <response code="404">Imóvel não encontrado.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] ImovelDto dto)
    {
        Result result = await _service.UpdateAsync(id, dto);

        if (!result.Sucesso)
            return NotFound(new { sucesso = false, erro = result.Mensagem });

        return NoContent();
    }

    /// <summary>
    /// Deleta um imóvel por ID.
    /// </summary>
    /// <response code="204">Imóvel deletado com sucesso.</response>
    /// <response code="404">Imóvel não encontrado.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        Result deleted = await _service.DeleteAsync(id);
        
        if (!deleted.Sucesso)
            return Conflict(new
            {
                sucesso = false,
                erro = deleted.Mensagem ?? "Erro ao excluir o imóvel."
            });

        return Ok(new { sucesso = true });
    }
}
