using ApiCondominio.Application.Uteis;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiCondominio.Application.DTOs;

public class MoradorDto
{
    public int Id { get; set; }

    [Display(Name = "Nome do Morador")]
    [Required(ErrorMessage = "{0} é obrigatório")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
    public required string Nome { get; set; }

    [Required(ErrorMessage = "{0} é obrigatório")]
    [StringLength(16, MinimumLength = 11, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
    public required string Celular { get; set; }

    [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
    [Required(ErrorMessage = "{0} é obrigatório")]
    [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
    public required string Email { get; set; }

    [Display(Name = "É proprietário?")]
    public bool IsProprietario { get; set; }

    [Display(Name = "Data de entrada")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "{0} é obrigatória")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime DataEntrada { get; set; }

    [Display(Name = "Data de saída")]
    [DataType(DataType.Date)]
    [JsonConverter(typeof(DateOnlyNullableConverter))]
    public DateTime? DataSaida { get; set; }

    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime DataInclusao { get; set; }

    [JsonConverter(typeof(DateOnlyNullableConverter))]
    public DateTime? DataAlteracao { get; set; }

    [Display(Name = "Imóvel")]
    [Required(ErrorMessage = "{0} é obrigatório")]
    public int ImovelId { get; set; }

    public ImovelDto? ImovelDto { get; set; }
}
