using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public interface ISocialInfo
    {
        bool NotInterestingToShow();

        string SocialInfoTitle();
        string SocialInfoSubTitle();
        string SocialInfoBody();
        string SocialInfoFooter();
        string SocialInfoImageUrl();
        InfoFact[] InfoFacts();

    }
}
