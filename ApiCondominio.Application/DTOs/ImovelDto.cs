using System.ComponentModel.DataAnnotations;

namespace ApiCondominio.Application.DTOs;

public class ImovelDto
{
    public int id { get; set; }

    [Display(Name = "Bloco")]
    [Required(ErrorMessage = "{0} é obrigatório")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
    public string bloco { get; set; }

    [Display(Name = "Apartamento")]
    [Required(ErrorMessage = "{0} é obrigatório")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
    public string apartamento { get; set; }

    [Display(Name = "Box Garagem")]
    [Required(ErrorMessage = "{0} é obrigatório")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
    public string boxGaragem { get; set; }
}
