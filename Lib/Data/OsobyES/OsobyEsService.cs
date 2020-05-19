using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.OsobyES
{
    public static partial class OsobyEsService
    {
        private static readonly Nest.ElasticClient _esClient = ES.Manager.GetESClient_Osoby();

        public static OsobaES Get(string idOsoby)
        {
            try
            {
                var response = _esClient.Get<OsobaES>(idOsoby);

                return response.IsValid
                    ? response.Source
                    : null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void BulkSave(List<OsobaES> osoby)
        {
            var result = _esClient.IndexMany<OsobaES>(osoby);

            if (result.Errors)
            {
                var a = result.DebugInformation;
                Util.Consts.Logger.Error($"Error when bulkSaving osoby to ES: {a}");
            }
        }
    }
}
