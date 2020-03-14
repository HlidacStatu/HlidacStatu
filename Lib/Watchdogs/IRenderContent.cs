using System;
using System.Linq;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Watchdogs
{
    public interface IRenderContent
    {
        RenderedContent Item();
        RenderedContent Header();
        RenderedContent Footer();

    }
}
