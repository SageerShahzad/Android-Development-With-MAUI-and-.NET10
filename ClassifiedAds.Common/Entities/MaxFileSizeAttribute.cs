using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Common.Entities
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxBytes;
        public bool Each { get; set; }

        public MaxFileSizeAttribute(int maxBytes)
        {
            _maxBytes = maxBytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null) return ValidationResult.Success;

            if (value is IFormFile singleFile)
            {
                return ValidateFile(singleFile);
            }

            if (value is IEnumerable<IFormFile> files)
            {
                if (Each)
                {
                    foreach (var file in files)
                    {
                        var result = ValidateFile(file);
                        if (result != ValidationResult.Success)
                            return result;
                    }
                }
                else
                {
                    var totalSize = files.Sum(f => f.Length);
                    return totalSize > _maxBytes
                        ? new ValidationResult(ErrorMessage)
                        : ValidationResult.Success;
                }
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateFile(IFormFile file)
        {
            return file.Length > _maxBytes
                ? new ValidationResult(ErrorMessage)
                : ValidationResult.Success;
        }
    }
}
