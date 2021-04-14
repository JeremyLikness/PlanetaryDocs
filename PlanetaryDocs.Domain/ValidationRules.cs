using System;
using System.Linq;

namespace PlanetaryDocs.Domain
{
    public static class ValidationRules
    {
        const string punctuation = "(),?!'\".";
        const string uidAllowed = "_.-";
        const string lowRange = "az";
        const string highRange = "AZ";
        const string numbers = "09";
        
        public static ValidationResult ValidResult() =>
            new() { IsValid = true };

        public static ValidationResult InvalidResult(string message) =>
            new()
            {
                IsValid = false,
                Message = message,
            };

        public static ValidationResult CompoundResult(
            string fieldName,
            string fieldValue,
            params Func<string, string, ValidationResult>[] validations)
        {
            var result = ValidResult();
            foreach (var validation in validations)
            {
                result = validation(fieldName, fieldValue);
                if (!result.IsValid)
                {
                    return result;
                }
            }
            return result;
        }

        public static ValidationResult IsRequired(
            string fieldName,
            string val) => string.IsNullOrWhiteSpace(val) ?
                InvalidResult($"{fieldName} is required.")
                : ValidResult();

        public static ValidationResult IsAlphaOnly(
            string fieldName,
            string val)
        {
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

                if (!uidCheck && punctuation.Contains(c))
                {
                    continue;
                }

                if (uidCheck && uidAllowed.Contains(c))
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

        public static ValidationResult ValidateProperty(
            string name,
            string value)
        {
            switch (name)
            {
                case nameof(Document.AuthorAlias):
                    return CompoundResult(
                        name,
                        value,
                        IsRequired,
                        IsAlphaOnly);

                case nameof(Document.Description):
                    return IsRequired(
                        name,
                        value);

                case nameof(Document.Markdown):
                    return IsRequired(
                        name,
                        value);

                case nameof(Document.Title):
                    return CompoundResult(
                        name,
                        value,
                        IsRequired,
                        (n, v) => IsSimpleText(n, v, false));

                case nameof(Document.Uid):
                    return CompoundResult(
                        name,
                        value,
                        IsRequired,
                        (n, v) => IsSimpleText(n, v, true));
            };

            return InvalidResult("Unknown property.");
        }

        public static ValidationResult[] ValidateDocument(Document doc)
        => doc == null ?
            new[] { InvalidResult("Document cannot be null") }
            : new[]
            {
                ValidateProperty(nameof(Document.Uid), doc.Uid),
                ValidateProperty(nameof(Document.AuthorAlias), doc.AuthorAlias),
                ValidateProperty(nameof(Document.Description), doc.Description),
                ValidateProperty(nameof(Document.Markdown), doc.Markdown),
                ValidateProperty(nameof(Document.Title), doc.Title)
            };
    }
}
