using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AppCore.Models;

namespace AppCore.Attributes;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class MinYearAttribute : ValidationAttribute
{
    private int MinYear { get; }

    public MinYearAttribute(int minYear)
    {
        MinYear = minYear;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        try
        {
            var now = DateTime.Now;
            var datetime = (DateTime)value;
            if (datetime.AddYears(MinYear).Date >= now.Date)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
        catch (RegexMatchTimeoutException)
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
        catch (Exception)
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}