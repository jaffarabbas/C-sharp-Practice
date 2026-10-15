using FluentValidation;
using ObservabilityApi.Domain.Products;

namespace ObservabilityApi.Application.Products;

// Validators reuse the domain's constants and rules so there is a single source of truth.
// Messages are explicit so they never echo the attempted value (FluentValidation's defaults can),
// which keeps user-supplied text out of responses and logs.

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .Must(sku => Product.IsValidSku(Product.NormalizeSku(sku)))
            .WithMessage($"SKU must be {Product.SkuMinLength}-{Product.SkuMaxLength} characters: letters, digits and '-', starting with a letter or digit.");

        RuleFor(x => x.Name).ValidName();
        RuleFor(x => x.Description).ValidDescription();
        RuleFor(x => x.Price).ValidPrice();
    }
}

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name).ValidName();
        RuleFor(x => x.Description).ValidDescription();
        RuleFor(x => x.Price).ValidPrice();
        RuleFor(x => x.Version)
            .NotEmpty()
            .Must(v => RowVersion.TryDecode(v, out _))
            .WithMessage("Version must be the value returned by the API when the product was last read.");
    }
}

internal static class ProductRuleExtensions
{
    public static IRuleBuilderOptions<T, string?> ValidName<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("Name is required.")
               .Must(n => n is null || n.Trim().Length <= Product.NameMaxLength)
               .WithMessage($"Name must be at most {Product.NameMaxLength} characters.");

    public static IRuleBuilderOptions<T, string?> ValidDescription<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(d => d is null || d.Trim().Length <= Product.DescriptionMaxLength)
               .WithMessage($"Description must be at most {Product.DescriptionMaxLength} characters.");

    public static IRuleBuilderOptions<T, decimal?> ValidPrice<T>(this IRuleBuilder<T, decimal?> rule)
        => rule.NotNull().WithMessage("Price is required.")
               .InclusiveBetween(0m, Product.MaxPrice).WithMessage($"Price must be between 0 and {Product.MaxPrice}.")
               .Must(p => p is null || Product.HasValidPrecision(p.Value))
               .WithMessage($"Price must have at most {Product.PriceDecimalPlaces} decimal places.");
}

internal static class RowVersion
{
    private const int SqlServerRowVersionLength = 8;

    public static bool TryDecode(string? value, out byte[] bytes)
    {
        bytes = [];
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        var buffer = new byte[SqlServerRowVersionLength];
        if (!Convert.TryFromBase64String(value, buffer, out var written) || written != SqlServerRowVersionLength)
        {
            return false;
        }

        bytes = buffer;
        return true;
    }
}
