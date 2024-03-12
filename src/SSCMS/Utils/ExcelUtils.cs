using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace SSCMS.Utils
{
    public static class ExcelUtils
    {
        public static DataTable Read(string filePath)
        {
            DataTable table;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
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

        public static (List<string> columns, int rowIndex) GetColumns(DataTable sheet)
        {
            var columns = new List<string>();
            var rowIndex = 0;

            var row = sheet.Rows[rowIndex];
            var isSuccess = true;
            for (var i = 0; i < sheet.Columns.Count; i++)
            {
                var value = StringUtils.Trim(row[i].ToString());
                if (string.IsNullOrEmpty(value))
                {
                    isSuccess = false;
                    columns = new List<string>();
                    break;
                }
                columns.Add(value);
            }

            if (!isSuccess)
            {
                rowIndex = 1;
                row = sheet.Rows[rowIndex];
                for (var i = 0; i < sheet.Columns.Count; i++)
                {
                    var value = StringUtils.Trim(row[i].ToString());
                    if (!string.IsNullOrEmpty(value))
                    {
                        columns.Add(value);
                    }
                }
            }

            return (columns, rowIndex + 1);
        }

        public static void Write(string filePath, List<string> head, List<List<string>> rows)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Sheet1");

                List<string> columns = new List<string>();
                IRow row = excelSheet.CreateRow(0);
                int columnIndex = 0;

                foreach (string column in head)
                {
                    columns.Add(column);
                    row.CreateCell(columnIndex).SetCellValue(column);
                    columnIndex++;
                }

                int rowIndex = 1;
                foreach (List<string> dsrow in rows)
                {
                    row = excelSheet.CreateRow(rowIndex);
                    int cellIndex = 0;
                    for (int i = 0; i < head.Count; i++)
                    {
                        var val = dsrow[i];
                        row.CreateCell(cellIndex).SetCellValue(val);
                        cellIndex++;
                    }

                    rowIndex++;
                }
                workbook.Write(fs, false);
            }
        }


        public class TypeValue
        {
            public Type TypeOf { get; set; }
            public object Value { get; set; }
        }

        public static void Write(string filePath, List<string> head, List<List<TypeValue>> rows)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Sheet1");

                List<string> columns = new List<string>();
                IRow row = excelSheet.CreateRow(0);
                int columnIndex = 0;

                foreach (string column in head)
                {
                    columns.Add(column);
                    row.CreateCell(columnIndex).SetCellValue(column);
                    columnIndex++;
                }

                int rowIndex = 1;
                foreach (List<TypeValue> dsrow in rows)
                {
                    row = excelSheet.CreateRow(rowIndex);
                    int cellIndex = 0;
                    for (int i = 0; i < head.Count; i++)
                    {
                        var typeValue = dsrow[i];
                        var value = typeValue.Value;
                        if (value == null)
                        {
                            row.CreateCell(cellIndex).SetCellValue(string.Empty);
                        }
                        else if (typeValue.TypeOf == typeof(double))
                        {
                            row.CreateCell(cellIndex).SetCellValue(TranslateUtils.ToDouble(value.ToString()));
                        }
                        else if (typeValue.TypeOf == typeof(bool))
                        {
                            row.CreateCell(cellIndex).SetCellValue(TranslateUtils.ToBool(value.ToString()));
                        }
                        else if (typeValue.TypeOf == typeof(DateTime))
                        {
                            row.CreateCell(cellIndex).SetCellValue(TranslateUtils.ToDateTime(value.ToString()));
                        }
                        else
                        {
                            row.CreateCell(cellIndex).SetCellValue(value.ToString());
                        }
                        cellIndex++;
                    }

                    rowIndex++;
                }
                workbook.Write(fs, false);
            }
        }
    }
}