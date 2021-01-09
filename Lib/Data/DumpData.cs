using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public class DumpData
    {
        public enum ShouldDownloadStatus
        {
            Yes,
            No,
            WaitForData
        }

        public static ShouldDownloadStatus ShouldDownload(XML.indexDump dump)
        {


            string cnnStr = Devmasters.Config.GetWebConfigValue("CnnString");
            string sql = @"select top 1 * from [DumpData] where mesic = @mesic and rok = @rok and den = @den order by created desc";
            using (var p = new Devmasters.PersistLib())
            {
                var ds = p.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sql, new IDataParameter[] {
                            new System.Data.SqlClient.SqlParameter("den", (int)dump.den),
                            new System.Data.SqlClient.SqlParameter("mesic", (int)dump.mesic),
                            new System.Data.SqlClient.SqlParameter("rok", (int)dump.rok),
                });
                if (ds.Tables[0].Rows.Count == 0)
                    return ShouldDownloadStatus.Yes;
                DataRow dr = ds.Tables[0].Rows[0];
                string oldHash = (string)dr["hash"];

                if (string.IsNullOrEmpty(oldHash))
                    return ShouldDownloadStatus.Yes;

                if (oldHash != dump.hashDumpu.Value)
                {
                    //if (dump.casGenerovani < DateTime.Now.Date)
                    //    return ShouldDownloadStatus.WaitForData;
                    //else
                        return ShouldDownloadStatus.Yes;
                }

                return ShouldDownloadStatus.No;
            }
        }

        public static void SaveDumpProcessed(XML.indexDump dump, DateTime? processed, Exception ex = null)
        {
            string cnnStr = Devmasters.Config.GetWebConfigValue("CnnString");
            string sql = @"INSERT INTO [dbo].[DumpData]
           ([Created]
           ,[Processed]
           ,[mesic]
           ,[rok]
           ,[hash]
           ,[velikost]
           ,[casGenerovani], exception)
     VALUES
           (@Created
           ,@Processed
           ,@mesic
           ,@rok
           ,@hash
           ,@velikost
           ,@casGenerovani
            ,@exception)
";

            try
            {

                using (var p = new Devmasters.PersistLib())
                {
                    p.ExecuteNonQuery(cnnStr, System.Data.CommandType.Text, sql, new IDataParameter[] {
                        new System.Data.SqlClient.SqlParameter("created", DateTime.Now),
                        new System.Data.SqlClient.SqlParameter("processed", processed),
                        new System.Data.SqlClient.SqlParameter("mesic", (int)dump.mesic),
                        new System.Data.SqlClient.SqlParameter("rok", (int)dump.rok),
                        new System.Data.SqlClient.SqlParameter("hash", dump.hashDumpu.Value),
                        new System.Data.SqlClient.SqlParameter("velikost", (long)dump.velikostDumpu),
                        new System.Data.SqlClient.SqlParameter("casGenerovani", dump.casGenerovani),
                        new System.Data.SqlClient.SqlParameter("exception", ex==null ? (string)null : ex.ToString()),
                        });

                }
            }
            catch (Exception e)
            {

                HlidacStatu.Util.Consts.Logger.Error("SaveDumpProcessed error", e);
            }

        }


        public class XML
        {




            /// <remarks/>
            [System.SerializableAttribute()]
            [System.ComponentModel.DesignerCategoryAttribute("code")]
            [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://portal.gov.cz/rejstriky/ISRS/1.2/")]
            [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://portal.gov.cz/rejstriky/ISRS/1.2/", IsNullable = false)]
            public partial class index
            {

                private indexDump[] dumpField;

                /// <remarks/>
                [System.Xml.Serialization.XmlElementAttribute("dump")]
                public indexDump[] dump
                {
                    get
                    {
                        return this.dumpField;
                    }
                    set
                    {
                        this.dumpField = value;
                    }
                }
            }

            /// <remarks/>
            [System.SerializableAttribute()]
            [System.ComponentModel.DesignerCategoryAttribute("code")]
            [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://portal.gov.cz/rejstriky/ISRS/1.2/")]
            public partial class indexDump
            {
                private byte denField;

                private byte mesicField;

                private ushort rokField;

                private indexDumpHashDumpu hashDumpuField;

                private uint velikostDumpuField;

                private System.DateTime casGenerovaniField;

                private byte dokoncenyMesicField;

                private string odkazField;

                public byte den
                {
                    get
                    {
                        return this.denField;
                    }
                    set
                    {
                        this.denField = value;
                    }
                }

                /// <remarks/>
                public byte mesic
                {
                    get
                    {
                        return this.mesicField;
                    }
                    set
                    {
                        this.mesicField = value;
                    }
                }

                /// <remarks/>
                public ushort rok
                {
                    get
                    {
                        return this.rokField;
                    }
                    set
                    {
                        this.rokField = value;
                    }
                }

                /// <remarks/>
                public indexDumpHashDumpu hashDumpu
                {
                    get
                    {
                        return this.hashDumpuField;
                    }
                    set
                    {
                        this.hashDumpuField = value;
                    }
                }

                /// <remarks/>
                public uint velikostDumpu
                {
                    get
                    {
                        return this.velikostDumpuField;
                    }
                    set
                    {
                        this.velikostDumpuField = value;
                    }
                }

                /// <remarks/>
                public System.DateTime casGenerovani
                {
                    get
                    {
                        return this.casGenerovaniField;
                    }
                    set
                    {
                        this.casGenerovaniField = value;
                    }
                }

                /// <remarks/>
                public byte dokoncenyMesic
                {
                    get
                    {
                        return this.dokoncenyMesicField;
                    }
                    set
                    {
                        this.dokoncenyMesicField = value;
                    }
                }

                /// <remarks/>
                public string odkaz
                {
                    get
                    {
                        return this.odkazField;
                    }
                    set
                    {
                        this.odkazField = value;
                    }
                }
            }

            /// <remarks/>
            [System.SerializableAttribute()]
            [System.ComponentModel.DesignerCategoryAttribute("code")]
            [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://portal.gov.cz/rejstriky/ISRS/1.2/")]
            public partial class indexDumpHashDumpu
            {

                private string algoritmusField;

                private string valueField;

                /// <remarks/>
                [System.Xml.Serialization.XmlAttributeAttribute()]
                public string algoritmus
                {
                    get
                    {
                        return this.algoritmusField;
                    }
                    set
                    {
                        this.algoritmusField = value;
                    }
                }

                /// <remarks/>
                [System.Xml.Serialization.XmlTextAttribute()]
                public string Value
                {
                    get
                    {
                        return this.valueField;
                    }
                    set
                    {
                        this.valueField = value;
                    }
                }
            }


        }

    }
}
