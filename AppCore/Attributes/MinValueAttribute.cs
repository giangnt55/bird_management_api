using System.ComponentModel.DataAnnotations;
using AppCore.Models;

namespace AppCore.Attributes;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class MinValueAttribute : ValidationAttribute
{
    public MinValueAttribute(double minValue)
    {
        MinValue = minValue;
    }

    private double MinValue { get; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if ((double)value < MinValue)
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        return ValidationResult.Success;
    }
}