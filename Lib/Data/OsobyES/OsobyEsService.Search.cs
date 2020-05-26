using Devmasters.Core;
using HlidacStatu.Lib.Searching;
using Nest;
using System;

namespace HlidacStatu.Lib.Data.OsobyES
{
    public static partial class OsobyEsService
    {
        
        public static OsobaEsSearchResult FulltextSearch(string query, int page, int pageSize)
        {

            page = page - 1 < 0 ? 0 : page - 1;

            var sw = new StopWatchEx();
            sw.Start();
            

            ISearchResponse<OsobaES> res = null;
            try
            {

                res = _esClient //.MultiSearch<OsobaES>(s => s
                        .Search<OsobaES>(s => s
                        .Size(pageSize)
                        .From(page * pageSize)
                        .Query(q => q.MultiMatch(c => c
                            .Fields(f=> f
                                .Field(p=>p.FullName)
                                .Field("fullName.*")
                                )
                            .Type(TextQueryType.MostFields)
                            .Fuzziness(Fuzziness.EditDistance(2))
                            .Query(query)
                        ))
                        .TrackTotalHits(true)
                );

            }
            catch (Exception e)
            {
                Audit.Add(Audit.Operations.Search, "", "", "OsobaES", "error", query, null);
                if (res != null && res.ServerError != null)
                {
                    ES.Manager.LogQueryError<OsobaES>(res, "Exception, Orig query:"
                        + query + "   query:"
                        + query
                        , ex: e);
                }
                else
                {
                    HlidacStatu.Util.Consts.Logger.Error("", e);
                }
                throw;
            }
            sw.Stop();

            Audit.Add(Audit.Operations.Search, "", "", "OsobaES", res.IsValid ? "valid" : "invalid", query, null);

            if (res.IsValid == false)
            {
                ES.Manager.LogQueryError<OsobaES>(res, "Exception, Orig query:"
                    + query + "   query:"
                    + query
                    );
            }

            var search = new OsobaEsSearchResult
            {
                OrigQuery = query,
                Total = res?.Total ?? 0,
                IsValid = res?.IsValid ?? false,
                ElasticResults = res,
                ElapsedTime = sw.Elapsed
            };
            return search;
        }

    }
}
