using ApiCondominio.Domain.Entities;
using ApiCondominio.Domain.Interfaces;
using ApiCondominio.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ApiCondominio.Infrastructure.Repositories;

public class MoradorRepository(ApplicationDbContext context) : IMoradorRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Morador>> GetAllAsync()
    {
        string sql = @"SELECT m.id, m.nome, m.celular, m.email, m.is_proprietario, m.data_entrada, m.data_saida, m.data_inclusao, m.data_alteracao, m.imovel_id,
               i.id AS Imovel_Id, i.bloco, i.apartamento, i.box_garagem
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id
            ORDER BY m.id";

        using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        List<Morador> moradores = new List<Morador>();

        using System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Morador morador = new Morador
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Celular = reader.GetString(reader.GetOrdinal("celular")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                IsProprietario = reader.GetBoolean(reader.GetOrdinal("is_proprietario")),
                DataEntrada = reader.GetDateTime(reader.GetOrdinal("data_entrada")),
                DataSaida = reader.IsDBNull(reader.GetOrdinal("data_saida")) ? null : reader.GetDateTime(reader.GetOrdinal("data_saida")),
                DataInclusao = reader.GetDateTime(reader.GetOrdinal("data_inclusao")),
                DataAlteracao = reader.IsDBNull(reader.GetOrdinal("data_alteracao")) ? null : reader.GetDateTime(reader.GetOrdinal("data_alteracao")),
                ImovelId = reader.GetInt32(reader.GetOrdinal("imovel_id")),
                Imovel = new Imovel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Imovel_Id")),
                    Bloco = reader.GetString(reader.GetOrdinal("bloco")),
                    Apartamento = reader.GetString(reader.GetOrdinal("apartamento")),
                    BoxGaragem = reader.GetString(reader.GetOrdinal("box_garagem"))
                }
            };

            moradores.Add(morador);
        }

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

        string sqlPaged = $@"SELECT m.id, m.nome, m.celular, m.email, m.is_proprietario,
                   m.data_entrada, m.data_saida, m.data_inclusao, m.data_alteracao, m.imovel_id,
                   i.id AS Imovel_Id, i.bloco, i.apartamento, i.box_garagem
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id
            ORDER BY {order} {dir}
            LIMIT {linesPerPage} OFFSET {offset}";

        string sqlCount = @" SELECT COUNT(*) FROM public.morador m INNER JOIN public.imovel i ON i.id = m.imovel_id";

        var items = new List<Morador>();

        using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = sqlPaged;
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var morador = new Morador
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nome = reader.GetString(reader.GetOrdinal("nome")),
                    Celular = reader.GetString(reader.GetOrdinal("celular")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    IsProprietario = reader.GetBoolean(reader.GetOrdinal("is_proprietario")),
                    DataEntrada = reader.GetDateTime(reader.GetOrdinal("data_entrada")),
                    DataSaida = reader.IsDBNull(reader.GetOrdinal("data_saida")) ? null : reader.GetDateTime(reader.GetOrdinal("data_saida")),
                    DataInclusao = reader.GetDateTime(reader.GetOrdinal("data_inclusao")),
                    DataAlteracao = reader.IsDBNull(reader.GetOrdinal("data_alteracao")) ? null : reader.GetDateTime(reader.GetOrdinal("data_alteracao")),
                    ImovelId = reader.GetInt32(reader.GetOrdinal("imovel_id")),
                    Imovel = new Imovel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Imovel_Id")),
                        Bloco = reader.GetString(reader.GetOrdinal("bloco")),
                        Apartamento = reader.GetString(reader.GetOrdinal("apartamento")),
                        BoxGaragem = reader.GetString(reader.GetOrdinal("box_garagem"))
                    }
                };
                items.Add(morador);
            }
        }

        int totalCount;
        using (var commandCount = connection.CreateCommand())
        {
            commandCount.CommandText = sqlCount;
            totalCount = Convert.ToInt32(await commandCount.ExecuteScalarAsync());
        }

        return (items, totalCount);
    }

    public async Task<Morador?> GetByIdAsync(int id)
    {
        string sql = @"SELECT m.id, m.nome, m.celular, m.email, m.is_proprietario,
                   m.data_entrada, m.data_saida, m.data_inclusao, m.data_alteracao,
                   m.imovel_id,
                   i.id AS Imovel_Id, i.bloco, i.apartamento, i.box_garagem
            FROM public.morador m
            INNER JOIN public.imovel i ON i.id = m.imovel_id
            WHERE m.id = @id";

        using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@id";
        parameter.Value = id;
        command.Parameters.Add(parameter);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Morador
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Celular = reader.GetString(reader.GetOrdinal("celular")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                IsProprietario = reader.GetBoolean(reader.GetOrdinal("is_proprietario")),
                DataEntrada = reader.GetDateTime(reader.GetOrdinal("data_entrada")),
                DataSaida = reader.IsDBNull(reader.GetOrdinal("data_saida")) ? null : reader.GetDateTime(reader.GetOrdinal("data_saida")),
                DataInclusao = reader.GetDateTime(reader.GetOrdinal("data_inclusao")),
                DataAlteracao = reader.IsDBNull(reader.GetOrdinal("data_alteracao")) ? null : reader.GetDateTime(reader.GetOrdinal("data_alteracao")),
                ImovelId = reader.GetInt32(reader.GetOrdinal("imovel_id")),
                Imovel = new Imovel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Imovel_Id")),
                    Bloco = reader.GetString(reader.GetOrdinal("bloco")),
                    Apartamento = reader.GetString(reader.GetOrdinal("apartamento")),
                    BoxGaragem = reader.GetString(reader.GetOrdinal("box_garagem"))
                }
            };
        }

        return null;
    }

    public async Task AddAsync(Morador morador)
    {
        morador.DataEntrada = DateTime.SpecifyKind(morador.DataEntrada, DateTimeKind.Local).ToUniversalTime();
        morador.DataInclusao = DateTime.SpecifyKind(morador.DataInclusao, DateTimeKind.Local).ToUniversalTime();
        if (morador.DataSaida.HasValue)
            morador.DataSaida = DateTime.SpecifyKind(morador.DataSaida.Value, DateTimeKind.Local).ToUniversalTime();
        if (morador.DataAlteracao.HasValue)
            morador.DataAlteracao = DateTime.SpecifyKind(morador.DataAlteracao.Value, DateTimeKind.Local).ToUniversalTime();

        string sql = @"INSERT INTO public.morador(nome, celular, email, is_proprietario, data_entrada, data_saida, data_inclusao, data_alteracao, imovel_id)
            VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";

        int newId = await _context.Database.ExecuteSqlRawAsync(sql, morador.Nome, morador.Celular, morador.Email, morador.IsProprietario, morador.DataEntrada,
                morador.DataSaida, morador.DataInclusao, morador.DataAlteracao, morador.ImovelId);

        morador.Id = newId;
    }

    public async Task UpdateAsync(Morador morador)
    {
        morador.DataEntrada = DateTime.SpecifyKind(morador.DataEntrada, DateTimeKind.Local).ToUniversalTime();
        morador.DataInclusao = DateTime.SpecifyKind(morador.DataInclusao, DateTimeKind.Local).ToUniversalTime();
        if (morador.DataSaida.HasValue)
            morador.DataSaida = DateTime.SpecifyKind(morador.DataSaida.Value, DateTimeKind.Local).ToUniversalTime();
        if (morador.DataAlteracao.HasValue)
            morador.DataAlteracao = DateTime.SpecifyKind(morador.DataAlteracao.Value, DateTimeKind.Local).ToUniversalTime();

        string sql = @"UPDATE public.morador 
                SET nome = @nome,
                    celular = @celular,
                    email = @email,
                    is_proprietario = @is_proprietario,
                    data_entrada = @data_entrada,
                    data_saida = @data_saida,
                    data_inclusao = @data_inclusao,
                    data_alteracao = @data_alteracao,
                    imovel_id = @imovel_id
                WHERE id = @id";

        var parameters = new[]
        {
            new NpgsqlParameter("nome", morador.Nome),
            new NpgsqlParameter("celular", morador.Celular),
            new NpgsqlParameter("email", morador.Email),
            new NpgsqlParameter("is_proprietario", morador.IsProprietario),
            new NpgsqlParameter("data_entrada", morador.DataEntrada),
            new NpgsqlParameter("data_saida", morador.DataSaida),
            new NpgsqlParameter("data_inclusao", morador.DataInclusao),
            new NpgsqlParameter("data_alteracao", morador.DataAlteracao),
            new NpgsqlParameter("imovel_id", morador.ImovelId),
            new NpgsqlParameter("id", morador.Id)
        };

        await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public async Task DeleteAsync(int id)
    {
        string sql = "DELETE FROM public.morador WHERE id = {0}";
        await _context.Database.ExecuteSqlRawAsync(sql, id);
    }

    public async Task<bool> ExistsByMoradorIdAsync(int id)
    {
        string sql = "SELECT COUNT(*) FROM public.morador WHERE imovel_id = @id";

        using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        int totalCount;
        using (var command = connection.CreateCommand())
        {
            var parameter = command.CreateParameter();

            parameter.ParameterName = "@id";
            parameter.Value = id;
            command.Parameters.Add(parameter);

            command.CommandText = sql;
            totalCount = Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        return totalCount > 0;
    }
}
