using ApiCondominio.Application.DTOs;
using FluentValidation;

namespace ApiCondominio.Application.Validators;

public class MoradorValidator : AbstractValidator<MoradorDto>
{
    public MoradorValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("Nome do Morador é obrigatório")
            .Length(3, 50).WithMessage("O campo Nome do Morador precisa ter entre 3 e 50 caracteres");

        RuleFor(p => p.Celular)
            .NotEmpty().WithMessage("Celular é obrigatório")
            .Length(11, 16).WithMessage("O campo Celular precisa ter entre 11 e 16 caracteres");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Formato de e-mail inválido")
            .MaximumLength(50).WithMessage("O campo Email pode ter no máximo 50 caracteres");

        RuleFor(p => p.DataEntrada)
            .NotEmpty().WithMessage("Data de entrada é obrigatória")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Data de entrada não pode ser futura");

        RuleFor(p => p.DataSaida)
            .GreaterThan(p => p.DataEntrada)
            .When(p => p.DataSaida.HasValue)
            .WithMessage("Data de saída deve ser maior que a data de entrada");

        RuleFor(p => p.ImovelId)
            .GreaterThan(0).WithMessage("Imóvel é obrigatório");
    }
}
