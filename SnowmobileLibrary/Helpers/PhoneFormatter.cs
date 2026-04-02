namespace SnowmobileLibrary.Helpers
{
    public static class PhoneFormatter
    {
        public static string Format(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            bool hasPlus = raw.TrimStart().StartsWith("+");
            string digits = new string(raw.Where(char.IsDigit).ToArray());

            // 10-digit NANP (US/Canada): (123) 456-7890
            if (!hasPlus && digits.Length == 10)
                return $"({digits[..3]}) {digits[3..6]}-{digits[6..]}";

            // 11-digit starting with 1 (NANP + country code): +1 (123) 456-7890
            if (digits.Length == 11 && digits[0] == '1')
                return $"+1 ({digits[1..4]}) {digits[4..7]}-{digits[7..]}";

            // International: preserve + and digits
            if (hasPlus)
                return "+" + digits;

            // Fallback: just the digits
            return digits;
        }
    }
}