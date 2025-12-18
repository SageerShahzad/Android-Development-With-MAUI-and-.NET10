using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Common.Extentions
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions) =>
            _extensions = extensions;

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_extensions.Contains(ext))
                    return new ValidationResult($"Only files with these extensions are allowed: {string.Join(", ", _extensions)}");
            }
            return ValidationResult.Success;
        }
    }

}
