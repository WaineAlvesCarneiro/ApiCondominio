using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCondominio.Domain.Entities;

[Table("morador")]
public class Morador
{
    public int id { get; set; }
    public required string nome { get; set; }
    public required string celular { get; set; }
    public required string email { get; set; }

    [Column("is_proprietario")]
    public bool isProprietario { get; set; }

    [Column("data_entrada")]
    public DateTime dataEntrada { get; set; }

    [Column("data_saida")]
    public DateTime? dataSaida { get; set; }

    [Column("data_inclusao")]
    public DateTime dataInclusao { get; set; }

    [Column("data_alteracao")]
    public DateTime? dataAlteracao { get; set; }

    [Column("imovel_id")]
    public required int imovelId { get; set; }

    public Imovel? Imovel { get; set; } = null!;
}
