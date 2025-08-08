using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCondominio.Domain.Entities;

[Table("morador", Schema = "public")]
public class Morador
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nome")]
    public required string Nome { get; set; }

    [Column("celular")]
    public required string Celular { get; set; }

    [Column("email")]
    public required string Email { get; set; }

    [Column("is_proprietario")]
    public bool IsProprietario { get; set; }

    [Column("data_entrada")]
    public DateTime DataEntrada { get; set; }

    [Column("data_saida")]
    public DateTime? DataSaida { get; set; }

    [Column("data_inclusao")]
    public DateTime DataInclusao { get; set; }

    [Column("data_alteracao")]
    public DateTime? DataAlteracao { get; set; }

    [Column("imovel_id")]
    public required int ImovelId { get; set; }

    public Imovel? Imovel { get; set; } = null!;
}
