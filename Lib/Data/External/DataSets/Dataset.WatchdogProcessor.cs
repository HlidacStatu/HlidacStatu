using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Watchdogs;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public partial class DataSet
    {
        public class WatchdogProcessor : Lib.Watchdogs.IWatchdogProcessor
        {
            //public WatchDog OrigWD { get; private set; }

            DataSet _dataset = null;

            private WatchDog originalWD = null;
            public WatchDog OrigWD
            {
                get => originalWD;
                set
                {
                    if (value != null)
                    {
                        if (value.dataType != WatchDog.AllDbDataType)
                        {
                            var dataSetId = value.dataType.Replace("DataSet.", "");
                            try
                            {
                                this.DataSet = DataSet.CachedDatasets.Get(dataSetId);
                            }
                            catch
                            {
                                //removed dataset, disable wd
                                this.DataSet = null;
                                originalWD = value;
                                originalWD.DisableWDBySystem(WatchDog.DisabledBySystemReasons.NoDataset);
                            }
                        }
                    }
                    originalWD = value;
                }

            }
            protected DataSet DataSet
            {
                get
                {
                    if (_dataset == null)
                    {
                        var dataSetId = OrigWD.dataType.Replace("DataSet.", "");
                        _dataset = DataSet.CachedDatasets.Get(dataSetId);
                    }
                    return _dataset;
                }

                set
                {
                    _dataset = value;
                }
            }
            public WatchdogProcessor(WatchDog wd)
                : this(wd,null)
            {
            }

            public WatchdogProcessor(WatchDog wd, DataSet ds)
            {
                this.OrigWD = wd;
                this.DataSet = ds;
            }

            public DateTime GetLatestRec(DateTime toDate)
            {
                var query = "DbCreated:" + string.Format("[* TO {0}]", Lib.Searching.Tools.ToElasticDate(toDate));
                DataSearchResult res = this.DataSet.SearchData(query, 1, 1, "DbCreated desc");

                if (res.IsValid == false)
                    return DateTime.Now.Date.AddYears(-10);
                if (res.Total == 0)
                    return DateTime.Now;
                return (DateTime)res.Result.First().DbCreated;
            }


            public Results GetResults(DateTime? fromDate = null, DateTime? toDate = null, int? maxItems = null, string order = null)
            {
                maxItems = maxItems ?? 30;
                string query = this.OrigWD.SearchTerm;
                if (fromDate.HasValue || toDate.HasValue)
                {
                    query += " AND DbCreated:";
                    query += string.Format("[{0} TO {1}]", Lib.Searching.Tools.ToElasticDate(fromDate, "*"), Lib.Searching.Tools.ToElasticDate(toDate, "*"));
                }
                DataSearchResult res = this.DataSet.SearchData(query, 1, 50, order);

                return new Watchdogs.Results(res.Result, res.Total,
                    query, fromDate, toDate, res.IsValid, WatchDog.AllDbDataType);
            }

            public RenderedContent RenderResults(Results data, long numOfListed = 5)
            {
                RenderedContent ret = new RenderedContent();
                List<RenderedContent> items = new List<RenderedContent>();
                if (data.Total <= (numOfListed + 2))
                    numOfListed = data.Total;

                IEnumerable<dynamic> dataToRender = null;
                if (data.Items.Count() > numOfListed)
                    dataToRender = data.Items.Take((int)numOfListed);
                else
                    dataToRender = data.Items.ToArray();

                HlidacStatu.Lib.Data.External.DataSets.DataSearchResult resultToRender = new DataSearchResult();
                resultToRender.Result = dataToRender;

                //var renderH = new Lib.Render.ScribanT(HtmlTemplate.Replace("#LIMIT#", numOfListed.ToString()));
                ret.ContentHtml = this.DataSet.Registration().searchResultTemplate.Render(this.DataSet,resultToRender, data.SearchQuery );
                if (data.Total > dataToRender.Count())
                {
                    var s = "<table><tr><td height='30' style='line-height: 50px; min-height: 50px;'></td></tr></table>"
                        + @"<table border='0' cellpadding='4' width='100%'><tr><td>" 
                        + @"<a href='" + this.DataSet.DatasetSearchUrl(data.SearchQuery, false) + "'>" 
                        + Devmasters.Lang.Plural.Get(data.Total - dataToRender.Count(), 
                                "Další nalezená záznam",
                                "Další {0} nalezené záznamy",
                                "Dalších {0} nalezených záznamů"
                            )
                       + @"</a>.</td></tr></table>"
                       + "<table><tr><td height='30' style='line-height: 50px; min-height: 50px;'></td></tr></table>";
                    ret.ContentHtml += s;
                }

                //var renderT = new Lib.Render.ScribanT(TextTemplate.Replace("#LIMIT#", numOfListed.ToString()));
                ret.ContentText = " ";
                ret.ContentTitle = this.DataSet.Registration().name;

                return ret;
            }

//            static string HtmlTemplate = @"
//";
//            static string TextTemplate = @"";
        }
    }
}
