using ApiCondominio.Domain.Entities;
using ApiCondominio.Domain.Interfaces;
using ApiCondominio.Persistence;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ApiCondominio.Infrastructure.Repositories;

public class MoradorRepository(ApplicationDbContext context) : IMoradorRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Morador>> GetAllAsync()
    {
        string sql = @"
            SELECT m.id, m.nome, m.celular, m.email, m.is_proprietario,
                   m.data_entrada, m.data_saida, m.data_inclusao, m.data_alteracao,
                   m.imovel_id
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id
            ORDER BY m.id;";

        return await _context.Moradors
            .FromSqlRaw(sql)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Morador> Items, int TotalCount)> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction)
    {
        // Normaliza ordenação
        string order = string.IsNullOrEmpty(orderBy) ? "m.id" : orderBy;
        string dir = direction?.ToUpper() == "DESC" ? "DESC" : "ASC";

        // Evita SQL injection validando colunas
        var allowedColumns = new[]
        {
            "m.id", "m.nome", "m.celular", "m.email", "m.is_proprietario",
            "m.data_entrada", "m.data_saida", "m.data_inclusao", "m.data_alteracao",
            "i.apartamento", "i.bloco", "i.box_garagem"
        };
        if (!allowedColumns.Contains(order.ToLower()))
            order = "m.id";

        int offset = page * linesPerPage;

        string sqlPaged = $@"
            SELECT m.id, m.nome, m.celular, m.email, m.is_proprietario,
                   m.data_entrada, m.data_saida, m.data_inclusao, m.data_alteracao,
                   m.imovel_id
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id
            ORDER BY {order} {dir}
            OFFSET @p0 LIMIT @p1;";

        string sqlCount = @"
            SELECT COUNT(*) 
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id;";

        var items = await _context.Moradors
            .FromSqlRaw(sqlPaged, offset, linesPerPage)
            .ToListAsync();

        int totalCount = await _context.Database
            .SqlQueryRaw<int>(sqlCount)
            .SingleAsync();

        return (items, totalCount);
    }

    public async Task<Morador?> GetByIdAsync(int id)
    {
        string sql = @"
            SELECT m.id, m.nome, m.celular, m.email, m.is_proprietario,
                   m.data_entrada, m.data_saida, m.data_inclusao, m.data_alteracao,
                   m.imovel_id
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id
            WHERE m.id = @p0;";

        return await _context.Moradors
            .FromSqlRaw(sql, id)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Morador morador)
    {
        string sql = @"
            INSERT INTO public.morador
                (nome, celular, email, is_proprietario, data_entrada, data_saida, data_inclusao, data_alteracao, imovel_id)
            VALUES
                (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)
            RETURNING id;";

        var newId = await _context.Database
            .SqlQueryRaw<int>(sql,
                morador.Nome,
                morador.Celular,
                morador.Email,
                morador.IsProprietario,
                morador.DataEntrada,
                morador.DataSaida,
                morador.DataInclusao,
                morador.DataAlteracao,
                morador.ImovelId)
            .SingleAsync();

        morador.Id = newId;
    }

    public async Task UpdateAsync(Morador morador)
    {
        string sql = @"
            UPDATE public.morador
            SET nome = @p0,
                celular = @p1,
                email = @p2,
                is_proprietario = @p3,
                data_entrada = @p4,
                data_saida = @p5,
                data_inclusao = @p6,
                data_alteracao = @p7,
                imovel_id = @p8
            WHERE id = @p9;";

        await _context.Database.ExecuteSqlRawAsync(sql,
            morador.Nome,
            morador.Celular,
            morador.Email,
            morador.IsProprietario,
            morador.DataEntrada,
            morador.DataSaida,
            morador.DataInclusao,
            morador.DataAlteracao,
            morador.ImovelId,
            morador.Id);
    }

    public async Task DeleteAsync(int id)
    {
        string sql = "DELETE FROM public.morador WHERE id = {0}";
        await _context.Database.ExecuteSqlRawAsync(sql, id);
    }

    public async Task<bool> ExistsByMoradorIdAsync(int id)
    {
        string sql = "SELECT COUNT(*) FROM public.morador WHERE imovel_id = {0}";
        int count = await _context.Database.ExecuteSqlRawAsync(sql, id);

        return count > 0;
    }
}
