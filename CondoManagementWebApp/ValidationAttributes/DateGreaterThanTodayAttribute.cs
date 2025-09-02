using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.ValidationAttributes
{
    public class DateGreaterThanTodayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date <= DateTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "The date must be a future date.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
