using System.Text.RegularExpressions;

namespace ClassifiedAds.Common.Helpers
{
    public static class PostalCodeValidator
    {
        // Full postcode (e.g. "SW1A 1AA")
        private static readonly Regex FullRegex = new Regex(
            @"^([A-Z]{1,2}\d{1,2}[A-Z]?\s?\d[A-Z]{2})$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Outward code only (e.g. "SW1A")
        private static readonly Regex OutwardRegex = new Regex(
            @"^[A-Z]{1,2}\d{1,2}[A-Z]?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool IsValid(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return false;
            postalCode = postalCode.Trim().ToUpper();
            // Validate either full postcode or just outward code
            return FullRegex.IsMatch(postalCode) || OutwardRegex.IsMatch(postalCode);
        }
    }
}
