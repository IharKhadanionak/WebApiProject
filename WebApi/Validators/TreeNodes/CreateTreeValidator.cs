using FluentValidation;

namespace WebApi.Validators.TreeNodes
{
    public sealed class CreateTreeValidator : AbstractValidator<string>
    {
        public CreateTreeValidator()
        {
            RuleFor(x => x).NotEmpty();
        }
    }
}
