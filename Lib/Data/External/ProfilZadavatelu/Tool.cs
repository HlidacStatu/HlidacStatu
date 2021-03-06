﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Batch;

namespace HlidacStatu.Lib.Data.External.ProfilZadavatelu
{
    public static class Tool
    {
        public static void ProcessProfilyZadavatelu(bool onlyWithErr, DateTime from, Action<string> outputWriter = null, Action<ActionProgressData> progressWriter = null)
        {
            var profily2 = new List<VZ.ProfilZadavatele>();

            //if (false)
            //{
            //    //_processProfileZadavatelu("https://mfcr.ezak.cz/profile_display_58.html", new DateTime(2017, 6, 1), DateTime.Now);
            //    var profil = ProfilZadavatele.GetByUrl("https://mfcr.ezak.cz/profile_display_58.html");
            //    _processProfileZadavatelu(profil, new DateTime(2016, 1, 9));
            //}

            Parser parser = Parser.Instance();

            if (onlyWithErr == false) //vsechny profily
            {
                Console.WriteLine("Reading profily zadavatelu");
                Lib.Searching.Tools.DoActionForAll<VZ.ProfilZadavatele>(
                    (pz, obj) =>
                    {
                        profily2.Add(pz.Source);

                        return new Devmasters.Batch.ActionOutputData();
                    }, null,
                    outputWriter ?? Devmasters.Batch.Manager.DefaultOutputWriter, progressWriter ?? Devmasters.Batch.Manager.DefaultProgressWriter,
                    false, elasticClient: Lib.ES.Manager.GetESClient_VZ());

                Console.WriteLine("Let's go mining");



            }
            else //jen ty s http chybami
            {
                Console.WriteLine("Reading profily zadavatelu with HTTP error");
                Func<int, int, Nest.ISearchResponse<Lib.Data.Logs.ProfilZadavateleDownload>> searchFunc = (size, page) =>
                {
                    return HlidacStatu.Lib.ES.Manager.GetESClient_Logs()
                            .Search<Lib.Data.Logs.ProfilZadavateleDownload>(a => a
                                .Size(size)
                                .Source(ss => ss.Excludes(f => f.Fields("xmlError", "xmlInvalidContent", "httpError")))
                                .From(page * size)
                                .Query(q => q.Term(t => t.Field(f => f.HttpValid).Value(false)))
                                .TrackTotalHits(page * size == 0 ? true : (bool?)null)
                                .Scroll("2m")
                                );
                };
                Searching.Tools.DoActionForQuery<Lib.Data.Logs.ProfilZadavateleDownload>(Lib.ES.Manager.GetESClient_Logs(), searchFunc,
                    (pzd, obj) =>
                    {
                        var profileId = pzd.Source.ProfileId;
                        if (!profily2.Any(m => m.Id == profileId))
                        {
                            var pz = VZ.ProfilZadavatele.GetById(profileId);
                            if (pz != null)
                                profily2.Add(pz);
                        }
                        return new Devmasters.Batch.ActionOutputData();
                    }, null, 
                    outputWriter ?? Devmasters.Batch.Manager.DefaultOutputWriter, progressWriter ?? Devmasters.Batch.Manager.DefaultProgressWriter
                    , false);


            }

            Console.WriteLine("Let's go mining");
            HlidacStatu.Util.Consts.Logger.Debug("ProfilyZadavatelu: Let's go mining num." + profily2.Count);
            Devmasters.Batch.Manager.DoActionForAll<VZ.ProfilZadavatele>(Devmasters.Collections.Algorithms.RandomShuffle(profily2),
                (p) =>
                {
                    parser.ProcessProfileZadavatelu(p, from);
                    return new Devmasters.Batch.ActionOutputData();
                }, outputWriter ?? Devmasters.Batch.Manager.DefaultOutputWriter, progressWriter ?? Devmasters.Batch.Manager.DefaultProgressWriter, true);


        }

    }


    
}
