using ApiCondominio.Application.DTOs;
using FluentValidation;

namespace ApiCondominio.Application.Validators;

public class MoradorValidator : AbstractValidator<MoradorDto>
{
    public MoradorValidator()
    {
        RuleFor(p => p.nome)
            .NotEmpty().WithMessage("Nome do Morador é obrigatório")
            .Length(3, 50).WithMessage("O campo Nome do Morador precisa ter entre 3 e 50 caracteres");

        RuleFor(p => p.celular)
            .NotEmpty().WithMessage("Celular é obrigatório")
            .Length(11, 16).WithMessage("O campo Celular precisa ter entre 11 e 16 caracteres");

        RuleFor(p => p.email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Formato de e-mail inválido")
            .MaximumLength(50).WithMessage("O campo Email pode ter no máximo 50 caracteres");

        RuleFor(p => p.dataEntrada)
            .NotEmpty().WithMessage("Data de entrada é obrigatória")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Data de entrada não pode ser futura");

        RuleFor(p => p.dataSaida)
            .GreaterThan(p => p.dataEntrada)
            .When(p => p.dataSaida.HasValue)
            .WithMessage("Data de saída deve ser maior que a data de entrada");

        RuleFor(p => p.imovelId)
            .GreaterThan(0).WithMessage("Imóvel é obrigatório");
    }
}
