using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<Dotace> YieldAllDotace(string scrollTimeout = "2m", int scrollSize = 1000)
        {
            ISearchResponse<Dotace> initialResponse = _esClient.Search<Dotace>
                (scr => scr.From(0)
                     .Take(scrollSize)
                     .MatchAll()
                     .Scroll(scrollTimeout));

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
