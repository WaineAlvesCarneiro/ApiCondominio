using ApiCondominio.Application.DTOs;
using ApiCondominio.Application.Interfaces;
using ApiCondominio.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace ApiCondominio.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MoradorController(IMoradorService service) : ControllerBase
{
    private readonly IMoradorService _service = service;

    /// <summary>
    /// Lista todos os morador.
    /// </summary>
    /// <response code="200">Lista retornada com sucesso.</response>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Result<IEnumerable<MoradorDto>> result = await _service.GetAllAsync();
        if (!result.Sucesso)
            return Conflict(new { sucesso = false, erro = result.Mensagem });

        return Ok(new { sucesso = true, dados = result.Dados });
    }

    /// <summary>
    /// Lista os morador com paginação.
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
        [FromQuery] string orderBy = "nome",
        [FromQuery] string direction = "ASC")
    {
        Result<PagedResultDto<MoradorDto>> result = await _service.GetAllPagedAsync(page, pageSize, orderBy, direction);
        if (!result.Sucesso)
            return Conflict(new { sucesso = false, erro = result.Mensagem });

        return Ok(new { sucesso = true, dados = result.Dados });
    }

    /// <summary>
    /// Retorna um morador por ID.
    /// </summary>
    /// <response code="200">Morador encontrado.</response>
    /// <response code="404">Morador não encontrado.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        Result<MoradorDto> result = await _service.GetByIdAsync(id);
        if (!result.Sucesso)
            return NotFound(new { sucesso = false, erro = result.Mensagem });

        return Ok(new { sucesso = true, dados = result.Dados });
    }

    /// <summary>
    /// Cadastra um novo morador.
    /// </summary>
    /// <response code="201">Morador criado com sucesso.</response>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MoradorDto dto)
    {
        Result<MoradorDto> result = await _service.AddAsync(dto);

        if (!result.Sucesso)
            return Conflict(new { sucesso = false, erro = result.Mensagem });

        return CreatedAtAction(nameof(GetById), new { id = result.Dados!.Id }, new 
        {
            sucesso = true,
            dados = result.Dados 
        });
    }

    /// <summary>
    /// Atualiza os dados de um morador existente.
    /// </summary>
    /// <response code="204">Morador atualizado com sucesso.</response>
    /// <response code="404">Morador não encontrado.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] MoradorDto dto)
    {
        Result result = await _service.UpdateAsync(id, dto);

        if (!result.Sucesso)
            return NotFound(new { sucesso = false, erro = result.Mensagem });

        return NoContent();
    }

    /// <summary>
    /// Deleta um morador por ID.
    /// </summary>
    /// <response code="204">Morador deletado com sucesso.</response>
    /// <response code="404">Morador não encontrado.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        Result result = await _service.DeleteAsync(id);
        if (!result.Sucesso)
            return Conflict(new
            {
                sucesso = false,
                erro = result.Mensagem ?? "Erro ao excluir o morador."
            });

        return Ok(new { sucesso = true });
    }
}
