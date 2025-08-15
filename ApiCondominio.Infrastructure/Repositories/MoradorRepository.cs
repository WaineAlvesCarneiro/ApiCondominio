using ApiCondominio.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiCondominio.Infrastructure.Repositories;

public class MoradorRepository(ApplicationDbContext context) : IMoradorRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Morador>> GetAllAsync()
    {
        string sql = @"SELECT m.id, m.nome, m.celular, m.email, m.is_proprietario, m.data_entrada, m.data_saida, m.data_inclusao, m.data_alteracao, m.imovel_id,
                i.id AS CodImovel, i.bloco, i.apartamento, i.box_garagem
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id
            ORDER BY m.id";

        List<Morador> moradores = await _context.Moradors.FromSqlRaw(sql).ToListAsync();

        return moradores;
    }

    public async Task<(IEnumerable<Morador> Items, int TotalCount)> GetAllPagedAsync(int page, int linesPerPage, string orderBy, string direction)
    {
        string order = string.IsNullOrEmpty(orderBy) ? "m.id" : orderBy;
        string dir = direction?.ToUpper() == "DESC" ? "DESC" : "ASC";

        string[] allowedColumns =
        {
            "m.id", "m.nome", "m.celular", "m.email", "m.is_proprietario",
            "m.data_entrada", "m.data_saida", "m.data_inclusao", "m.data_alteracao",
            "i.bloco", "i.apartamento", "i.box_garagem"
        };

        if (!allowedColumns.Contains(order.ToLower()))
            order = "m.id";

        int offset = page * linesPerPage;

        string sqlPaged = $@"SELECT m.id, m.nome, m.celular, m.email, m.is_proprietario, m.data_entrada, m.data_saida, m.data_inclusao, m.data_alteracao, m.imovel_id,
                i.id AS CodImovel, i.bloco, i.apartamento, i.box_garagem
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id
            ORDER BY {order} {dir}
            LIMIT {linesPerPage} OFFSET {offset}";

        string sqlCount = @" SELECT COUNT(*) FROM public.morador m INNER JOIN public.imovel i ON i.id = m.imovel_id";

        List<Morador> items = await _context.Moradors.FromSqlRaw(sqlPaged, linesPerPage, offset).ToListAsync();

        int totalCount = await _context.Database.ExecuteSqlRawAsync(sqlCount);

        totalCount = await _context.Moradors.CountAsync();

        return (items, totalCount);
    }

    public async Task<Morador?> GetByIdAsync(int id)
    {
        string sql = @"SELECT m.id, m.nome, m.celular, m.email, m.is_proprietario, m.data_entrada, m.data_saida, m.data_inclusao, m.data_alteracao, m.imovel_id,
                i.id AS CodImovel, i.bloco, i.apartamento, i.box_garagem
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id
            WHERE m.id = {0}";

        Morador? retorno = await _context.Set<Morador>().FromSqlRaw(sql, id).AsNoTracking().FirstOrDefaultAsync();

        return retorno;
    }

    public async Task AddAsync(Morador morador)
    {
        var horaAgora = DateTime.Now.TimeOfDay;

        var dataEntradaHoje = morador.DataEntrada.Date;
        var dataEntradaComHora = dataEntradaHoje + horaAgora;
        morador.DataEntrada = DateTime.SpecifyKind(dataEntradaComHora, DateTimeKind.Local).ToUniversalTime();

        var dataInclusaoHoje = morador.DataInclusao.Date;
        var dataInclusaoComHora = dataInclusaoHoje + horaAgora;
        morador.DataInclusao = DateTime.SpecifyKind(dataInclusaoComHora, DateTimeKind.Local).ToUniversalTime();

        string sql = @"INSERT INTO public.morador(nome, celular, email, is_proprietario, data_entrada, data_saida, data_inclusao, data_alteracao, imovel_id)
            VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";

        int newId = await _context.Database.ExecuteSqlRawAsync(sql, morador.Nome, morador.Celular, morador.Email, morador.IsProprietario, morador.DataEntrada,
                morador.DataSaida, morador.DataInclusao, morador.DataAlteracao, morador.ImovelId);

        morador.Id = newId;
    }

    public async Task UpdateAsync(Morador morador)
    {
        var horaAgora = DateTime.Now.TimeOfDay;

        var dataEntradaHoje = morador.DataEntrada.Date;
        var dataEntradaComHora = dataEntradaHoje + horaAgora;
        morador.DataEntrada = DateTime.SpecifyKind(dataEntradaComHora, DateTimeKind.Local).ToUniversalTime();

        var dataInclusaoHoje = morador.DataInclusao.Date;
        var dataInclusaoComHora = dataInclusaoHoje + horaAgora;
        morador.DataInclusao = DateTime.SpecifyKind(dataInclusaoComHora, DateTimeKind.Local).ToUniversalTime();

        if (morador.DataSaida.HasValue)
        {
            var dataSaidaHoje = morador.DataSaida?.Date;
            var dataSaidaComHora = dataSaidaHoje + horaAgora;

            morador.DataSaida = DateTime.SpecifyKind((DateTime)dataSaidaComHora, DateTimeKind.Local).ToUniversalTime();
        }

        if (morador.DataAlteracao.HasValue)
        {
            var dataAlteracaoHoje = morador.DataAlteracao?.Date;
            var dataAlteracaoComHora = dataAlteracaoHoje + horaAgora;

            morador.DataAlteracao = DateTime.SpecifyKind((DateTime)dataAlteracaoComHora, DateTimeKind.Local).ToUniversalTime();
        }

        string sql = @"UPDATE public.morador SET nome = {0}, celular = {1}, email = {2}, is_proprietario = {3}, data_entrada = {4}, data_saida = {5}, 
                data_inclusao = {6}, data_alteracao = {7}, imovel_id = {8}
            WHERE id = {9}";

        await _context.Database.ExecuteSqlRawAsync(sql, morador.Nome, morador.Celular, morador.Email, morador.IsProprietario, morador.DataEntrada, morador.DataSaida,
            morador.DataInclusao, morador.DataAlteracao, morador.ImovelId, morador.Id);
    }

    public async Task DeleteAsync(int id)
    {
        string sql = "DELETE FROM public.morador WHERE id = {0}";
        await _context.Database.ExecuteSqlRawAsync(sql, id);
    }

    public async Task<bool> ExistsByMoradorIdAsync(int imovelId)
    {
        string sql = @"SELECT m.id, m.nome, m.celular, m.email, m.is_proprietario, m.data_entrada, m.data_saida, m.data_inclusao, m.data_alteracao, m.imovel_id,
                i.id AS CodImovel, i.bloco, i.apartamento, i.box_garagem
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id
            WHERE m.imovel_id = {0}
            ORDER BY m.id";

        List<Morador> moradores = await _context.Moradors.FromSqlRaw(sql, imovelId).ToListAsync();

        return moradores.Count > 0;
    }
}
