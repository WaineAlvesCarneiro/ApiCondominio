using ApiCondominio.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("morador", Schema = "public")]
public class Morador
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = null!;

    [Column("celular")]
    public string Celular { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;

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
    public int ImovelId { get; set; }

    public Imovel? Imovel { get; set; } = null!;

    [NotMapped]
    public int? CodImovel { get; set; }

    [Column("bloco")]
    public string? Bloco { get; set; }

    [Column("apartamento")]
    public string? Apartamento { get; set; }

    [Column("box_garagem")]
    public string? BoxGaragem { get; set; }
}
