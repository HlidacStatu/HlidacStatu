using System;

namespace HlidacStatu.ExportData
{
    public interface IExport
    {
        byte[] ExportData(Data data);

    }
}
