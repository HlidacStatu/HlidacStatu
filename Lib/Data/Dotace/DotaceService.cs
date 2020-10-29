using HlidacStatu.Lib.Searching;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HlidacStatu.Lib.Data.Dotace
{
    public partial class DotaceService
    {
        private readonly Nest.ElasticClient _esClient = ES.Manager.GetESClient_Dotace();

        public Dotace Get(string idDotace)
        {
            try
            {
                // použít get místo search.
                // var searchResponse = _esClient.Search<Dotace>(s => s.From(0).Size(1).Query(q => q.Term(t => t.Field(p => p.IdObdobi).Value(idRozhodnuti))));
                var response = _esClient.Get<Dotace>(idDotace);

                return response.IsValid
                    ? response.Source
                    : null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns true if some error ocurred
        /// </summary>
        /// <param name="dotace"></param>
        /// <returns></returns>
        public bool BulkSave(List<Dotace> dotace)
        {
            foreach (var d in dotace)
            {
                // fill in sums
                d.CalculateTotals();
            }

            var result = _esClient.IndexMany<Dotace>(dotace);

            if (result.Errors)
            {
                var a = result.DebugInformation;
                Util.Consts.Logger.Error($"Error when bulkSaving dotace to ES: {a}");
            }

            return result.Errors;
        }

        public IEnumerable<Dotace> YieldAllDotace(string scrollTimeout = "2m", int scrollSize = 300)
        {
            return YieldAll(null, scrollTimeout, scrollSize);

        }

        public (decimal Sum, int Count) GetStatisticsForIco(string ico)
        {
            var dotaceAggs = new AggregationContainerDescriptor<Dotace>()
                .Sum("souhrn", s => s
                    .Field(f => f.DotaceCelkem)
                );

            var dotaceSearch = this.SimpleSearch($"ico:{ico}", 1, 1,
                Searching.DotaceSearchResult.DotaceOrderResult.FastestForScroll, false,
                dotaceAggs, exactNumOfResults: true);

            decimal sum = (decimal)dotaceSearch.ElasticResults.Aggregations.Sum("souhrn").Value;
            int count = (int)dotaceSearch.Total;

            return (sum, count);
        }

        public Dictionary<string, (decimal Sum, int Count)> GetStatisticsForHolding(string ico)
        {
            var dotaceAggsH = new AggregationContainerDescriptor<Dotace>()
                .Terms("icos", s => s
                    .Field(f => f.Prijemce.Ico)
                    .Size(5000)
                    .Aggregations(a => a
                        .Sum("sum", ss => ss.Field(ff => ff.DotaceCelkem))
                    )
                );
            var dotaceSearchH = new DotaceService().SimpleSearch($"holding:{ico}", 1, 1,
                Searching.DotaceSearchResult.DotaceOrderResult.FastestForScroll, false,
                dotaceAggsH, exactNumOfResults: true);

            var items = ((BucketAggregate)dotaceSearchH.ElasticResults.Aggregations["icos"]).Items;

            Dictionary<string, (decimal Sum, int Count)> dict = items.ToDictionary(
                i => ((KeyedBucket<object>)i).Key.ToString(),
                i => ((decimal)((KeyedBucket<object>)i).Sum("sum").Value,
                    (int)((KeyedBucket<object>)i).DocCount)
                );

            return dict;
        }

        public IEnumerable<Dotace> GetDotaceForIco(string ico)
        {
            QueryContainer qc = new QueryContainerDescriptor<Dotace>()
                .Term(f => f.Prijemce.Ico, ico);

            return YieldAll(qc);
        }

        public IEnumerable<Dotace> GetDotaceForHolding(string holdingIco)
        {
            string query = Lib.Searching.Tools.FixInvalidQuery($"holding:{holdingIco}", queryShorcuts(), queryOperators);
            var qc = SimpleQueryCreator.GetSimpleQuery<Dotace>(query, irules);

            //QueryContainer qc = new QueryContainerDescriptor<Dotace>()
            //    .Term(f => f.Prijemce.Ico, holdingIco);

            return YieldAll(qc);
        }

        private IEnumerable<Dotace> YieldAll(QueryContainer query, 
            string scrollTimeout = "2m", 
            int scrollSize = 300)
        {
            ISearchResponse<Dotace> initialResponse = null;
            if (query is null)
            {
                initialResponse = _esClient.Search<Dotace>(scr => scr
                    .From(0)
                    .Take(scrollSize)
                    .MatchAll()
                    .Scroll(scrollTimeout));
            }
            else
            {
                initialResponse = _esClient.Search<Dotace>(scr => scr
                    .From(0)
                    .Take(scrollSize)
                    .Query(q => query)
                    .Scroll(scrollTimeout));
            }

            if (!initialResponse.IsValid || string.IsNullOrEmpty(initialResponse.ScrollId))
                throw new Exception(initialResponse.ServerError.Error.Reason);

            if (initialResponse.Documents.Any())
                foreach (var dotace in initialResponse.Documents)
                {
                    yield return dotace;
                }

            string scrollid = initialResponse.ScrollId;
            bool isScrollSetHasData = true;
            while (isScrollSetHasData)
            {
                ISearchResponse<Dotace> loopingResponse = _esClient.Scroll<Dotace>(scrollTimeout, scrollid);
                if (loopingResponse.IsValid)
                {
                    foreach (var dotace in loopingResponse.Documents)
                    {
                        yield return dotace;
                    }
                    scrollid = loopingResponse.ScrollId;
                }
                isScrollSetHasData = loopingResponse.Documents.Any();
            }

            _esClient.ClearScroll(new ClearScrollRequest(scrollid));
        }
    }

}
