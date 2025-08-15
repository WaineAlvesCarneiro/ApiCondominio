using System.ComponentModel.DataAnnotations;

namespace ApiCondominio.Application.DTOs;

public class ImovelDto
{
    public int Id { get; set; }

    [Display(Name = "Bloco")]
    [Required(ErrorMessage = "{0} é obrigatório")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
    public required string Bloco { get; set; }

    [Display(Name = "Apartamento")]
    [Required(ErrorMessage = "{0} é obrigatório")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
    public required string Apartamento { get; set; }

    [Display(Name = "Box Garagem")]
    [Required(ErrorMessage = "{0} é obrigatório")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
    public required string BoxGaragem { get; set; }
}
