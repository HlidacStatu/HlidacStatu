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

        public bool BulkSave(List<Dotace> dotace)
        {
            try
            {
                // pro debug toho, co se bude posílat na server slouží zakomentovaný řádek níže
                //var response = System.Text.Encoding.UTF8.GetString(_esClient.IndexMany(dotace).ApiCall.RequestBodyInBytes);
                var response = _esClient.IndexMany(dotace);
                if( response.IsValid && response.Errors)
                {
                    var errors = response.ItemsWithErrors.Select(it => $"DotaceService.BulkSave error - failedId: {it.Id}, reason: {it.Error.Reason}" ).ToArray();
                    
                    foreach(string error in errors)
                    {
                        HlidacStatu.Util.Consts.Logger.Warning(error);
                    }
                }

                return response.IsValid;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

    }
}
