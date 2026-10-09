namespace Market.Application.Modules.Catalog.Categories.Commands.Create;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Category name is required.")
            .MaximumLength(CategoryEntity.Constraints.CategoryNameMaxLength)
            .WithMessage($"Category name can be at most {CategoryEntity.Constraints.CategoryNameMaxLength} characters long.");
    }
}
