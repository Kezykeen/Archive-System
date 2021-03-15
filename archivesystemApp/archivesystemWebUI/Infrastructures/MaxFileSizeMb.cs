using System.ComponentModel.DataAnnotations;
using System.Web;

namespace archivesystemWebUI.Infrastructures
{
    public class MaxFileSizeMb : ValidationAttribute
    {
        private readonly int _maxSize;


        public MaxFileSizeMb(int maxSize )
        :base("Maximum File Size Exceeded ("+ maxSize + "Mb)")
        {
            _maxSize = maxSize;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
                HttpPostedFileBase file = value as HttpPostedFileBase;
                bool isValid = true;

                // Verification.  
                if (file == null) return ValidationResult.Success;
                // Initialization.  
                var fileSize = file.ContentLength;
                // Settings.  
                isValid = fileSize <= _maxSize * 1024 * 1024;

                if (isValid) return ValidationResult.Success;
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);

        }
      
    }
}