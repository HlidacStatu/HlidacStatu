using System;
using OfficeOpenXml;

namespace HlidacStatu.ExportData
{
    public class Excel : IExport
    {
        public byte[] ExportData(Data data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add("Data");
                //Header
                for (int c = 0; c < data.Columns.Count; c++)
                {
                    ws.Cells[1, c+1].Value = data.Columns[c].Name;
                    ws.Cells[1, c+1].Style.Font.Bold = true;
                }
                for (int r = 0; r < data.Rows.Count; r++)
                {
                    for (int c = 0; c < data.Columns.Count; c++)
                    {
                        if (string.IsNullOrWhiteSpace(data.Columns[c].FormatString))
                            ws.Cells[r+2, c+1].Value = data.Rows[r].Values[c];
                        else
                            ws.Cells[r + 2, c + 1].Value = string.Format("{0:" + data.Columns[c].FormatString + "}",data.Rows[r].Values[c]);
                    }
                }

                return p.GetAsByteArray();
            }

        }
    }
}
