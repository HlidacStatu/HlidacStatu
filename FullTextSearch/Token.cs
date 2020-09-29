using System;
using System.Collections.Generic;

namespace FullTextSearch
{
    public class Token<T> : IEquatable<Token<T>>
    {
        public string Word { get; set; }
        public List<Sentence<T>> Sentences { get; set; } = new List<Sentence<T>>();

        public Token(Sentence<T> sentence, string word)
        {
            Word = word;
            Sentences.Add(sentence);
        }

        public void MergeWith(Token<T> merged)
        {
            if (this == merged) return; //fuse to not merge myself with myself

            foreach(var sentence in merged.Sentences)
            {
                this.Sentences.Add(sentence);
                for(int i = 0; i < sentence.Tokens.Count; i++)
                {
                    if(sentence.Tokens[i].Equals(merged))
                    {
                        sentence.Tokens[i] = this;
                    }
                }
            }
        }

        public override string ToString()
        {
            return Word;
        }

        public override int GetHashCode()
        {
            return Word.GetHashCode();
        }

        public bool Equals(Token<T> other)
        {
            return this.Word.Equals(other.Word, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
