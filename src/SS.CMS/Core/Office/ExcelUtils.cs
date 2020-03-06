using System.Data;
using System.IO;
using ExcelDataReader;

namespace SS.CMS.Core.Office
{
    public static class ExcelUtils
    {
        public static DataTable GetDataTable(string filePath)
        {
            DataTable table;
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    table = result.Tables[0];
                    // The result of each spreadsheet is in result.Tables
                }
            }

            return table;
        }
    }
}