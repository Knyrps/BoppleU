using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bopple.Core.Extensions
{
    public static class StringExtensions
    {
        public static string PascalToReadable(this string pascalCase)
        {
            return Regex.Replace(pascalCase, "(?<!^)([A-Z])", " $1");
        }
    }
}