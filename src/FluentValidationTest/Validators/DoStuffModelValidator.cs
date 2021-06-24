using FluentValidation;
using FluentValidationTest.Models;

namespace FluentValidationTest.Validators
{
    // ReSharper disable once UnusedType.Global
    public class DoStuffModelValidator : AbstractValidator<DoStuffModel>
    {
        public DoStuffModelValidator()
        {
            RuleFor(m => m.Password)
                .NotEmpty()
                .WithMessage($"Invalid {nameof(DoStuffModel.Password)}");

            RuleFor(m => m.Username)
                .NotEmpty()
                .WithMessage($"Invalid {nameof(DoStuffModel.Username)}");
        }
    }
}