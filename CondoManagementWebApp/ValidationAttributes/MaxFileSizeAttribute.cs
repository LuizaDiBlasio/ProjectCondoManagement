using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.ValidationAttributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(AtributeErrorMessage());
                }

            }
            return ValidationResult.Success; //só avalia tamanho do file
        }

        public string AtributeErrorMessage()
        {
            return $"Maximum allowed file size is {_maxFileSize / 1024 / 1024} MB.";
        }
    }
}
