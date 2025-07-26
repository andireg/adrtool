using System.Text.RegularExpressions;

namespace AdrTool.Core;

public static class StringFormatter
{
    private static readonly Regex Regex = new ("\\{%([^\\}:]*)[:]{0,1}([^\\}:]*){0,1}\\}");

    public static string FormatWithObject(this string text, object argument)
        => Regex.Replace(text, m => GetPropertyValue(argument, m.Groups[1].Value, m.Groups[2].Value) ?? m.Groups[0].Value);

    private static string? GetPropertyValue(object argument, string propertyName, string formatter)
    {
        object? value = argument.GetType().GetProperty(propertyName)?.GetValue(argument, null);
        if (value == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(formatter))
        {
            return value.ToString();
        }

        return string.Format($"{{0:{formatter}}}", value);
    }
}