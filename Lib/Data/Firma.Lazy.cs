using Devmasters.Core;
using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
        : Bookmark.IBookmarkable, ISocialInfo
    {
        public class Lazy
            : Bookmark.IBookmarkable, ISocialInfo
        {
            private Firma firma = null;
            private string ico = null;
            public Lazy(string ico)
            {
                this.ico = ico;
            }

            object lo = new object();
            private Firma gf()
            {
                if (firma == null)
                {
                    lock (lo)
                    {
                        if (firma == null)
                            firma = Firmy.Get(this.ico);
                    }
                }
                return firma;
            }

            public string Ico { get { return this.ico; } }

            public string BookmarkName()
            {
                return gf().BookmarkName();
            }

            public string GetUrl(bool local)
            {
                return gf().GetUrl(local);
            }

            public string GetUrl(bool local, string foundWithQuery)
            {
                return gf().GetUrl(local, foundWithQuery);
            }

            public InfoFact[] InfoFacts()
            {
                return gf().InfoFacts();
            }
            public bool NotInterestingToShow() { return gf().NotInterestingToShow(); }

            public string SocialInfoBody()
            {
                return gf().SocialInfoBody();
            }

            public string SocialInfoFooter()
            {
                return gf().SocialInfoFooter();
            }

            public string SocialInfoImageUrl()
            {
                return gf().SocialInfoImageUrl();
            }

            public string SocialInfoSubTitle()
            {
                return gf().SocialInfoSubTitle();
            }

            public string SocialInfoTitle()
            {
                return gf().SocialInfoTitle();
            }

            public string ToAuditJson()
            {
                return gf().ToAuditJson();
            }

            public string ToAuditObjectId()
            {
                return gf().ToAuditObjectId();
            }

            public string ToAuditObjectTypeName()
            {
                return gf().ToAuditObjectTypeName();
            }
        }
    }
}
