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

    }
}
