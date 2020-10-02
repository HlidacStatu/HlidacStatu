using System;
using System.Collections.Generic;

namespace FullTextSearch
{
    public interface ITokenizer
    {
        string[] Tokenize(string text);
    } 

    public class Tokenizer : ITokenizer
    {
        private readonly char[] _splitChars;
        private readonly IEnumerable<Func<string, string>> _tokenizePipeline;

        public Tokenizer(char[] splitChars, IEnumerable<Func<string, string>> tokenizePipeline)
        {
            _splitChars = splitChars;
            _tokenizePipeline = tokenizePipeline;
        }

        public static Tokenizer DefaultTokenizer()
        {
            char[] defaultSplitChars = " ,.?!:;\t-_".ToCharArray();
            var defaultPipeline = new List<Func<string, string>>()
            {
                x => x.ToLowerInvariant(),
                StringExtensions.RemoveAccents,
                x => x.ToAlphaNumeric(defaultSplitChars)
            };

            return new Tokenizer(defaultSplitChars, defaultPipeline);
        }

        public string[] Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Array.Empty<string>();

            foreach (var fn in _tokenizePipeline)
            {
                text = fn(text);
            }

            return text.Split(_splitChars, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
