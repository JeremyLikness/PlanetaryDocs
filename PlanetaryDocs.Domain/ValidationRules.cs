using System.Linq;

namespace PlanetaryDocs.Domain
{
    public static class ValidationRules
    {
        const char uidSeparator = '/';
        const string punctuation = "(),?!'\".";
        const string lowRange = "az";
        const string highRange = "AZ";
        const string numbers = "09";
        
        public static ValidationResult ValidResult() =>
            new () { IsValid = true };

        public static ValidationResult InvalidResult(string message) =>
            new ()
            {
                IsValid = false,
                Message = message,
            };

        public static ValidationResult IsRequired(
            string fieldName, 
            string val) => string.IsNullOrWhiteSpace(val) ?
                InvalidResult($"{fieldName} is required.")
                : ValidResult();

        public static ValidationResult IsAlphaOnly(
            string fieldName,
            string val)
        {
            var required = IsRequired(fieldName, val);

            if (!required.IsValid)
            {
                return required;
            }

            return val.All(c => (c >= lowRange[0] && c <= lowRange[1])
            || (c >= highRange[0] && c <= highRange[1])) ?
                ValidResult()
                : InvalidResult($"Field '{fieldName}' contains non-alpha characters.");
        }
       
        public static ValidationResult IsSimpleText(
            string fieldName,
            string val,
            bool uidCheck = false)
        {
            var required = IsRequired(fieldName, val);

            if (!required.IsValid)
            {
                return required;
            }
            var valid = true;
            var limits = new[] { lowRange, highRange, numbers };

            foreach (var c in val)
            {
                if (!valid)
                {
                    break;
                }

                if (c == ' ')
                {
                    continue;
                }

                if (punctuation.Contains(c))
                {
                    continue;
                }

                if (uidCheck && c == uidSeparator)
                {
                    continue;
                }

                bool rangeCheck = false;
                foreach (var range in limits)
                {
                    if (rangeCheck)
                    {
                        break;
                    }

                    if (c >= range[0] && c <= range[1])
                    {
                        rangeCheck = true;
                        continue;
                    }
                }

                valid = rangeCheck;
            }

            return valid ? ValidResult() :
                InvalidResult($"Field '{fieldName}' contains invalid characters.");
        }
    }
}
