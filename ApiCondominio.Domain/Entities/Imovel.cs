using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCondominio.Domain.Entities;

[Table("imovel")]
public class Imovel
{
    public int id { get; set; }
    public required string bloco { get; set; }
    public required string apartamento { get; set; }

    [Column("box_garagem")]
    public required string boxGaragem { get; set; }
}
