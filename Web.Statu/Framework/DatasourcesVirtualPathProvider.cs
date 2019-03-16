using HlidacSmluv.Lib.Data.External.DataSets;
using System;
using System.Collections;
using System.Web.Caching;
using System.Web.Hosting;

namespace HlidacStatu.Web.Framework
{
    public class DataSetsVirtualPathProvider : VirtualPathProvider
    {
        public static string CustomDataSearchTemplatePostfixBody = "_data_hledatbody_customdatatemplate_";


        public static string CustomDataDetailTemplatePostfix = "_data_detail_customdatatemplate_";

        //Data/(_data_hledat_customdatatemplate_|_data_detail_customdatatemplate_)(?<datasetId>.*)\.cshtml$ 
        public static string regexDatasetId = "Data/("
            + CustomDataDetailTemplatePostfix + "|" + CustomDataSearchTemplatePostfixBody
            + @")(?<datasetId>[0-9a-zA-Z-]*)(\.mobile){0,1}\.cshtml$";
        
        public override bool FileExists(string virtualPath)
        {
            if (IsCustomDataTemplate(virtualPath))
            {
                return true;
            }
            else
            {
                return base.FileExists(virtualPath);
            }
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsCustomDataTemplate(virtualPath))
            {
                //return new VirtualFileFromString(virtualPath);
                return new VirtualFileFromString(virtualPath);
            }
            else
            {
                return base.GetFile(virtualPath);
            }
        }

        private bool IsCustomDataTemplate(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
                return false;
            if (!virtualPath.EndsWith("cshtml"))
                return false;

            return
                virtualPath.ToLower().Contains(CustomDataSearchTemplatePostfixBody)
                || virtualPath.ToLower().Contains(CustomDataDetailTemplatePostfix)
                ;
        }


        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (IsCustomDataTemplate(virtualPath))
            {
                return TemplateVirtualFileCacheManager.SetPageCache(virtualPath);
            }
            else
                return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public class VirtualFileFromString : VirtualFile
        {
            static string noTemplateMsg = "<h2>Tento dataset nemá definovaný template (err VirtualFileFromString)<h2>";

            static string initTemplateCode = @"
@{
dynamic item = Newtonsoft.Json.Linq.JObject.Parse(Model.Data);
}
" + HlidacSmluv.Lib.Init.DatasetTemplateFunctions;


            private string content = null;

            public VirtualFileFromString(string virtualPath) : base(virtualPath)
            {
                virtualPath = virtualPath.ToLower();
                var datasetId = HlidacStatu.Util.ParseTools.RegexGroupValue(virtualPath, regexDatasetId, "datasetId");
                if (!string.IsNullOrEmpty(datasetId))
                {
                    var dataset = DataSetDB.Instance.GetRegistration(datasetId);
                    if (dataset != null)
                    {
                        if (virtualPath.Contains(CustomDataSearchTemplatePostfixBody))
                            this.content = initTemplateCode + dataset.searchResultTemplate?.body ?? "";
                        
                        else if (virtualPath.Contains(CustomDataDetailTemplatePostfix))
                            this.content = initTemplateCode + dataset.detailTemplate?.ToPageContent();
                    }
                }

            }
            public VirtualFileFromString(string virtualPath, string content) : base(virtualPath)
            {
                this.content = content;
            }


            public override System.IO.Stream Open()
            {

                var ms = new System.IO.MemoryStream();
                var sw = new System.IO.StreamWriter(ms, System.Text.Encoding.UTF8);
                //sw.Write(System.Text.Encoding.UTF8.GetPreamble());
                sw.Write(this.content ?? (noTemplateMsg + this.VirtualPath));
                sw.Flush();
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return ms;

            }
        }
    }





    public class TemplateVirtualFileCacheDependency : System.Web.Caching.CacheDependency
    {
        public TemplateVirtualFileCacheDependency()
        {
            base.SetUtcLastModified(DateTime.UtcNow);
        }
        public void Invalidate()
        {
            base.SetUtcLastModified(DateTime.UtcNow);
            NotifyDependencyChanged(this, EventArgs.Empty);
        }
    }

    public class TemplateVirtualFileCacheManager
    {
        static string prefix = "ccd!";
        // Page Cash
        public static TemplateVirtualFileCacheDependency SetPageCache(string virtualPath)
        {
            TemplateVirtualFileCacheDependency customeCacheDependency = new TemplateVirtualFileCacheDependency();
            System.Web.HttpContext.Current.Application[prefix+virtualPath] = customeCacheDependency;


            return customeCacheDependency;
        }
        public static void InvalidateTemplateCache(string datasetId)
        {
            string[] allPaths = new string[] {
                $"~/Views/Data/_data_hledatbody_customdatatemplate_{datasetId}.cshtml",
                $"~/Views/Data/_data_hledatbody_customdatatemplate_{datasetId}.Mobile.cshtml",
                $"/Views/Data/_data_hledatbody_customdatatemplate_{datasetId}.cshtml",
                $"/Views/Data/_data_hledatbody_customdatatemplate_{datasetId}.Mobile.cshtml",

                $"~/Views/Data/_data_detail_customdatatemplate_{datasetId}.cshtml",
                $"~/Views/Data/_data_detail_customdatatemplate_{datasetId}.Mobile.cshtml",
                $"/Views/Data/_data_detail_customdatatemplate_{datasetId}.cshtml",
                $"/Views/Data/_data_detail_customdatatemplate_{datasetId}.Mobile.cshtml",
            };

            foreach (var vp in allPaths)
            {
                TemplateVirtualFileCacheDependency customeCacheDependency = System.Web.HttpContext.Current.Application[prefix+vp] as TemplateVirtualFileCacheDependency;
                if (customeCacheDependency != null)
                    customeCacheDependency.Invalidate();
            }
        }
    }
}