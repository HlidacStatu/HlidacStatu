using System.Collections.Generic;
using System.Linq;

namespace FullTextSearch
{
    public class Sentence<T>
    {
        public string Text
        {
            get => string.Join(" ", Tokens);
        }

        //public Guid Guid { get; } = Guid.NewGuid();
        public T Original { get; private set; }
        public List<Token<T>> Tokens { get; private set; } = new List<Token<T>>();

        private readonly ITokenizer _tokenizer;

        public Sentence(T original, ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            Original = original;
            CreateTokens();
        }

        private void CreateTokens()
        {
            //todo: nefunguje, když T je typu string
            var objectValues = Original
                    .GetType()
                    .GetProperties()
                    .Where(p => p.CustomAttributes.Any(ca => ca.AttributeType == typeof(SearchAttribute)))
                    .Select(p => p.GetValue(Original)?.ToString());

            
            var words = objectValues.SelectMany(x => _tokenizer.Tokenize(x)); //.Distinct();

            foreach (string word in words)
            {
                var token = new Token<T>(this, word);
                Tokens.Add(token);
            }
        }

        public override string ToString()
        {
            return Text;
        }

    }
}
