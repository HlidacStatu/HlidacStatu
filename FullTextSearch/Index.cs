using System;
using System.Collections.Generic;
using System.Linq;

namespace FullTextSearch
{
    public class Index<T>
    {
        public SortedList<string, Token<T>> SortedTokens { get; private set; } = new SortedList<string, Token<T>>();
        public List<Sentence<T>> Sentences { get; private set; } = new List<Sentence<T>>();

        private readonly ITokenizer _tokenizer;
        

        public Index(IEnumerable<T> inputObjects)
        {
            _tokenizer = Tokenizer.DefaultTokenizer();
            BuildIndex(inputObjects);
        }

        public Index(IEnumerable<T> inputObjects, ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            BuildIndex(inputObjects);
        }

        private void BuildIndex(IEnumerable<T> inputObjects)
        {
            foreach (T inputObject in inputObjects)
            {
                var sentence = new Sentence<T>(inputObject, _tokenizer);
                Sentences.Add(sentence);

                AddTokens(sentence.Tokens);
            }
        }

        private void AddTokens(List<Token<T>> tokens)
        {
            for(int i = 0; i < tokens.Count(); i++)
            {
                if(SortedTokens.TryGetValue(tokens[i].Word, out Token<T> olderToken))
                {
                    olderToken.MergeWith(tokens[i]);
                }
                else
                {
                    SortedTokens.Add(tokens[i].Word, tokens[i]);
                }
            }
        }

        public IEnumerable<Result<T>> Search(string query, int count)
        {
            var tokenizedQuery = _tokenizer.Tokenize(query);

            List<ScoredSentence<T>> results = new List<ScoredSentence<T>>();
            foreach(string queryToken in tokenizedQuery)
            {
                
                // tokeny, které odpovídají query
                var filteredTokens = SortedTokens
                    .Where(t => t.Key.StartsWith(queryToken))
                    .Select(x => x.Value);


                foreach(var token in filteredTokens)
                {
                    var scoredSentences = ScoreToken(token, queryToken);
                    results.AddRange(scoredSentences);
                }
            }

            var summedResults = results
                .GroupBy(r => r.Sentence,
                    (sentence, result) => new ScoredSentence<T>(sentence, result.Sum(x => x.Score)))
                .ToList();

            // přidat score za nejdelší řetězec
            foreach(ScoredSentence<T> result in summedResults)
            {
                result.Score += ScoreSentence(result.Sentence, tokenizedQuery); 
            }

            return summedResults
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => new Result<T>()
                {
                    Original = x.Sentence.Original,
                    Score = x.Score
                });
                
        }


        // Neověřuju na začátku, jestli jsou stejné
        // předpokládám, že sem už stejné texty lezou - asi chybně
        private List<ScoredSentence<T>> ScoreToken(Token<T> token, string queryToken)
        {
            double basicScore = queryToken.Length;
            
            // bonus for whole word
            if (queryToken.Length == token.Word.Length)
            {
                basicScore *= 1.2;
            }

            //bonus for first three words

            //var results = new List<ScoredSentence<T>>();
            //foreach (var sentence in token.Sentences)
            //{
            //    double score = basicScore;
            //    for (int wordPosition = 0; wordPosition < 3; wordPosition++)
            //    {
            //        if (sentence.Tokens.Count > wordPosition)
            //            if (sentence.Tokens[wordPosition].Word.StartsWith(queryToken))
            //            {
            //                score = score * (1.3 - (0.1 * wordPosition));
            //                break;
            //            }
            //    }
            //    results.Add(new ScoredSentence<T>(sentence, score));
            //}

            return token.Sentences.Select(s => new ScoredSentence<T>(s, basicScore)).ToList();
        }

        private Double ScoreSentence(Sentence<T> sentence, string[] tokenizedQuery)
        {
            double score = 0;
            string firstQueryToken = tokenizedQuery.FirstOrDefault();
            // bonus for first three words
            if (!string.IsNullOrWhiteSpace(firstQueryToken))
            {
                for (int wordPosition = 0; wordPosition < 3; wordPosition++)
                {
                    if (sentence.Tokens.Count > wordPosition)
                        if (sentence.Tokens[wordPosition].Word.StartsWith(firstQueryToken))
                        {
                            score += firstQueryToken.Length * (0.3 - (0.1 * wordPosition));
                            break;
                        }
                }

            }

            // Query == sentence
            if (sentence.Text == string.Join(" ", tokenizedQuery))
            {
                return 20 + score;
            }

            // sentence starts with query without its last word
            if (tokenizedQuery.Length > 2) // 3+ words
            {
                string shorterQuery = string.Join(" ", tokenizedQuery.Take(tokenizedQuery.Length - 1));
                if (sentence.Text.StartsWith(shorterQuery) )
                {
                    return 10 + score;
                }

            }

            // Má smysl scorovat nejdelší shodný substring? Zatím si myslím, že asi ne,
            // protože by to mohlo zamíchat pořadím. Navíc takový výpočet není levný
            // a pro velký počet dokumentů by to mohlo znamenat pomalé hledání.

            return score;
        }

    }
}
