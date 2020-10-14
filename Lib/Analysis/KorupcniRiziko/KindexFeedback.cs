using System;
using System.Collections.Generic;
using System.Linq;
using HlidacStatu.Lib.Data;
using Nest;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class KindexFeedback
    {
        [Keyword]
        public string Id { get; set; }
        [Keyword]
        public int Year { get; set; }
        [Date]
        public DateTime? SignDate { get; set; }
        [Keyword]
        public string Ico { get; set; }
        [Text]
        public string Company { get; set; }
        [Text]
        public string Text { get; set; }
        [Text]
        public string Author { get; set; }

        public void Save()
        {
            if(string.IsNullOrWhiteSpace(Id))
            {
                this.Id = Guid.NewGuid().ToString();
            }
            if(string.IsNullOrWhiteSpace(Company))
            {
                var firma = Firma.FromIco(Ico);
                Company = firma.Jmeno;
            }
            var res = ES.Manager.GetESClient_KindexFeedback().IndexDocument(this); //druhy parametr musi byt pole, ktere je unikatni
            if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
        }

        public static IEnumerable<KindexFeedback> GetKindexFeedbacks(string ico, int year)
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
                    var hits = searchResults.Hits.Select(h => h.Source).OrderByDescending(s=>s.SignDate);
                    return hits;
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

            return Enumerable.Empty<KindexFeedback>();
        }

        public static KindexFeedback GetById(string id)
        {

            ElasticClient _esClient = ES.Manager.GetESClient_KindexFeedback();

            ISearchResponse<KindexFeedback> searchResults = null;
            try
            {
                searchResults = _esClient.Search<KindexFeedback>(s =>
                        s.Query(q =>
                            q.Term(f => f.Id, id)
                            )
                        );

                if (searchResults.IsValid && searchResults.Hits.Count > 0)
                {
                    var hits = searchResults.Hits.Select(h => h.Source).OrderByDescending(s => s.SignDate);
                    return hits.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                string origQuery = $"id:{id};";
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

            return null;
        }
    }
}
