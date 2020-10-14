using System;
using System.Linq;
using HlidacStatu.Lib.Data;
using Nest;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class KindexFeedback
    {
        [Keyword]
        public int Year { get; set; }
        [Date]
        public DateTime? SignDate { get; set; }
        [Keyword]
        public string Ico { get; set; }
        [Text]
        public string Text { get; set; }
        [Text]
        public string Author { get; set; }

        public void Save()
        {
            var res = ES.Manager.GetESClient_KindexFeedback().IndexDocument(this); //druhy parametr musi byt pole, ktere je unikatni
            if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
        }

        public static bool TryGetKindexFeedback(string ico, int year, out KindexFeedback kindexFeedback)
        {
            
            ElasticClient _esClient = ES.Manager.GetESClient_KindexFeedback();

            ISearchResponse<KindexFeedback> searchResults = null;
            try
            {
                searchResults = _esClient.Search<KindexFeedback>(s =>
                        s.Query(q =>
                            q.Term(f => f.Ico, ico)
                            && q.Term(f => f.Year, year)
                            )
                        );
                
                if (searchResults.IsValid && searchResults.Hits.Count > 0)
                {
                    var firstHit = searchResults.Hits.FirstOrDefault();
                    kindexFeedback = firstHit.Source;
                    return true;
                }
            }
            catch (Exception e)
            {
                string origQuery = $"ico:{ico}; year:{year};";
                Audit.Add(Audit.Operations.Search, "", "", "KindexFeedback", "error", origQuery, null);
                if (searchResults != null && searchResults.ServerError != null)
                {
                    ES.Manager.LogQueryError<KindexFeedback>(searchResults, 
                        $"Exception for {origQuery}",
                        ex: e);
                }
                else
                {
                    Util.Consts.Logger.Error(origQuery, e);
                }
            }

            kindexFeedback = null;
            return false;
        }
    }
}
