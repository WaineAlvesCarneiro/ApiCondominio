using ApiCondominio.Application.DTOs;
using FluentValidation;

namespace ApiCondominio.Application.Validators;

public class ImovelValidator : AbstractValidator<ImovelDto>
{
    public ImovelValidator()
    {
        RuleFor(p => p.Bloco)
            .NotEmpty().WithMessage("Bloco é obrigatório")
            .Length(1, 10).WithMessage("O campo Bloco precisa ter entre 1 e 10 caracteres");

        RuleFor(p => p.Apartamento)
            .NotEmpty().WithMessage("Apartamento é obrigatório")
            .Length(1, 10).WithMessage("O campo Apartamento precisa ter entre 1 e 10 caracteres");

        RuleFor(p => p.BoxGaragem)
            .NotEmpty().WithMessage("Box Garagem é obrigatório")
            .Length(1, 10).WithMessage("O campo Box Garagem precisa ter entre 1 e 10 caracteres");
    }
}
