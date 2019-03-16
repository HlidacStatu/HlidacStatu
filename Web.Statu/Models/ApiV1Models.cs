using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Web.Models
{
    public class ApiV1Models
    {

        public class DumpInfoModel
        {
            public string url { get; set; }
            public DateTime date { get; set; }
            public long size { get; set; }
            public bool fulldump { get; set; }
            public DateTime created { get; set; }
        }

        public class ClassificatioListItemModel
        {
            public Contract[] contracts { get; set; }

            public class Contract
            {
                public string contractId { get; set; }
                public string url { get; set; }
                //public DateTime processingDate { get; set; }
            }

        }



        //public class ClassificationItemModel
        //{
        //    public ProcessingInfo processinginfo { get; set; }
        //    public ProcessingResult processingresult { get; set; }
        //    public class ProcessingInfo
        //    {
        //        public int contractId { get; set; }
        //        public string url { get; set; }
        //        public string processingDate { get; set; }
        //    }

        //    public class ProcessingResult
        //    {
        //        public string attachment1url { get; set; }
        //        public string attachment2url { get; set; }
        //        public string attachmentNurl { get; set; }
        //    }

        //}

        public class PolitikTypeAhead : IEqualityComparer<PolitikTypeAhead>
        {
            public  string name; public string nameId;

            public bool Equals(PolitikTypeAhead x, PolitikTypeAhead y)
            {
                if (x == null && y == null)
                    return true;
                if ((x == null && y != null) || (x != null && y == null))
                    return false;
                return (x.nameId == y.nameId && x.name == y.name);
            }

            public int GetHashCode(PolitikTypeAhead obj)
            {
                return obj.GetHashCode();
            }
        }

    }
}