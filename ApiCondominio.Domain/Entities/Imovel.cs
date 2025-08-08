using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCondominio.Domain.Entities;

[Table("imovel", Schema = "public")]
public class Imovel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("bloco")]
    public required string Bloco { get; set; }

    [Column("apartamento")]
    public required string Apartamento { get; set; }

    [Column("box_garagem")]
    public required string BoxGaragem { get; set; }
}
