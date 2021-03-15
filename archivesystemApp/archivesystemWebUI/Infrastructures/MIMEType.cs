using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using MimeTypes;
using Ninject.Infrastructure.Language;

namespace archivesystemWebUI.Infrastructures
{
    public class MIMEType : ValidationAttribute
    {
        private readonly string _mimeTypes;

        public MIMEType(string mimeTypes = "")
            : base($@"Unsupported Media Type (Supported: Audio , Video, Text { (!mimeTypes.Split(',').Any() ? string.Join(", ", mimeTypes.Split(',')) :"," + mimeTypes )})")
        {
            _mimeTypes = mimeTypes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Verification.  
            HttpPostedFileBase file = value as HttpPostedFileBase;

            if (file == null) return ValidationResult.Success;
            // Initialization.  
            var fileType = file.ContentType;
            var cleanMimeTypes = new List<string>();
            var errorMessage = FormatErrorMessage(validationContext.DisplayName);

            fileType = fileType.Split('/').FirstOrDefault()?.ToLower();

            switch (fileType)
            {
                case  "audio":
                    return ValidationResult.Success;

                case "video":
                    return ValidationResult.Success;
                case "text":
                    return ValidationResult.Success;
                case "image":
                    return ValidationResult.Success;
                default:

                    return CheckExt(cleanMimeTypes, file, validationContext, ErrorMessage);
            }

        }

        private ValidationResult CheckExt(List<string> exts, HttpPostedFileBase file, ValidationContext validationContext, string errMsg)
        {
            if (string.IsNullOrEmpty(_mimeTypes)) return new ValidationResult(errMsg);
            if (_mimeTypes.Contains(','))
            {
                _mimeTypes.Split(',').ForEach(m => exts.Add(m.Trim().ToLower()));
            }
            else
            {
                exts.Add(_mimeTypes);
            }
            var fileExt = file.FileName.Split('.').LastOrDefault()?.ToLower();
            if (fileExt == null) return new ValidationResult(errMsg);
            if (!exts.Contains(fileExt)) return new ValidationResult(errMsg);
            if (fileExt == "zip")
            {
                return file.ContentType.Contains(fileExt) ? ValidationResult.Success : new ValidationResult(errMsg);
            }
            var fileMimeType = MimeTypeMap.GetMimeType(fileExt);
            return file.ContentType.ToLower() != fileMimeType.ToLower() ? new ValidationResult(errMsg) : ValidationResult.Success;
        }
    }
}