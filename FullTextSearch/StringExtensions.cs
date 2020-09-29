using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FullTextSearch
{
    public static class StringExtensions
    {
        /// <summary>
        /// Remove all accents (diacritics) from text
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveAccents(this string input)
        {
            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Changes string to letters, numbers + allowed characters
        /// </summary>
        /// <param name="allowedCharacters">List of characters which are allowed to stay in string</param>
        /// <returns>Returns string that contains only alphaNumeric characters + allowed characters</returns>
        public static string ToAlphaNumeric(this string self, params char[] allowedCharacters)
        {
            return new string(Array.FindAll(
                self.ToCharArray(),
                c => char.IsLetterOrDigit(c) || allowedCharacters.Contains(c)));
        }

       
    }
}
