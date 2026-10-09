namespace Market.Application.Modules.Catalog.Categories.Commands.Update;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");

        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Category name is required.")
            .MaximumLength(CategoryEntity.Constraints.CategoryNameMaxLength)
            .WithMessage($"Category name can be at most {CategoryEntity.Constraints.CategoryNameMaxLength} characters long.");
    }
}
