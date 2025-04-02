using System.Globalization;
using System.Text.RegularExpressions;

namespace Shared.Core.CommonUtils
{
    public static class StringHelper
    {
        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public static bool IsNullOrWhiteSpace(string input) => string.IsNullOrWhiteSpace(input);

        public static string GenerateSlug(string text)
        {
            text = text.ToLower().Replace(" ", "-");
            return Regex.Replace(text, "[^a-z0-9-]", "");
        }
    }

}
