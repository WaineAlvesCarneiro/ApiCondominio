using System.ComponentModel.DataAnnotations;

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

    private DateTime _dataEntrada;
    [Display(Name = "Data de entrada")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "{0} é obrigatória")]
    public DateTime DataEntrada
    {
        get => _dataEntrada;
        set => _dataEntrada = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private DateTime? _dataSaida;
    [Display(Name = "Data de saída")]
    [DataType(DataType.Date)]
    public DateTime? DataSaida
    {
        get => _dataSaida;
        set => _dataSaida = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    private DateTime _dataInclusao;
    public DateTime DataInclusao
    {
        get => _dataInclusao;
        set => _dataInclusao = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private DateTime? _dataAlteracao;
    public DateTime? DataAlteracao
    {
        get => _dataAlteracao;
        set => _dataAlteracao = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    [Display(Name = "Imóvel")]
    [Required(ErrorMessage = "{0} é obrigatório")]
    public int ImovelId { get; set; }

    public ImovelDto? ImovelDto { get; set; }
}
