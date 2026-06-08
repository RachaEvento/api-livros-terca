using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MeuAcervo.Shared.Text;

public static partial class TextNormalizationHelper
{
    private static readonly Lazy<IReadOnlyDictionary<string, string>> LanguageMap = new(CreateLanguageMap);

    public static string NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = value.Trim();
        var decomposed = trimmed.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        var withoutDiacritics = builder.ToString().Normalize(NormalizationForm.FormC);
        var collapsedWhitespace = WhitespaceRegex().Replace(withoutDiacritics, " ");

        return collapsedWhitespace.ToUpperInvariant();
    }

    public static string NormalizeLanguageCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().ToLowerInvariant();
        if (normalized.Length == 2)
        {
            return normalized;
        }

        return LanguageMap.Value.TryGetValue(normalized, out var twoLetterCode)
            ? twoLetterCode
            : normalized;
    }

    public static string NormalizeIsbn(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return NonIsbnCharacterRegex().Replace(value.Trim(), string.Empty).ToUpperInvariant();
    }

    public static DateTime? CreatePublishedAtUtc(int? year)
    {
        if (!year.HasValue || year.Value < 1 || year.Value > 9999)
        {
            return null;
        }

        return new DateTime(year.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    private static IReadOnlyDictionary<string, string> CreateLanguageMap()
    {
        return CultureInfo.GetCultures(CultureTypes.NeutralCultures | CultureTypes.SpecificCultures)
            .Select(culture => new
            {
                ThreeLetter = culture.ThreeLetterISOLanguageName.ToLowerInvariant(),
                TwoLetter = culture.TwoLetterISOLanguageName.ToLowerInvariant()
            })
            .Where(item => item.ThreeLetter.Length == 3 && item.TwoLetter.Length == 2 && item.TwoLetter != "iv")
            .GroupBy(item => item.ThreeLetter)
            .ToDictionary(group => group.Key, group => group.First().TwoLetter, StringComparer.OrdinalIgnoreCase);
    }

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"[^0-9Xx]", RegexOptions.Compiled)]
    private static partial Regex NonIsbnCharacterRegex();
}
