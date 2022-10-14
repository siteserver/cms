using System.Collections.Generic;
using System.Text;
using NPOI.XWPF.UserModel;

namespace SSCMS.Core.Utils.Office.Word2Html
{
    public class TableConvert
    {
        /// <summary>
        ///     word中的表格处理
        /// </summary>
        /// <param name="paraTable"></param>
        /// <param name="picInfoList"></param>
        /// <returns></returns>
        public StringBuilder TableHandle(XWPFTable paraTable, List<PicInfo> picInfoList)
        {
            var paraGraphConvert = new ParaGraphConvert();

            var sb = new StringBuilder();

            var rows = paraTable.Rows;
            sb.Append("<table border='1' cellspacing='0'>");
            foreach (var row in rows)
            {
                var cells = row.GetTableCells();

                sb.Append(
                    "<tr style='");
                //var firstRowCell = cells[0];


                sb.Append(
                    "'>");


                foreach (var cell in cells)
                {
                    var cellCtTc = cell.GetCTTc();
                    var tcPr = cellCtTc.tcPr;


                    sb.Append("<td style='");

                    if (!string.IsNullOrWhiteSpace(tcPr.tcW?.w))
                        sb.Append($"width:{tcPr.tcW.w}px;");
                    if (!string.IsNullOrWhiteSpace(tcPr.shd?.fill))
                        sb.Append($"background-color: #{tcPr.shd.fill};");

                    sb.Append("'>");
                    var cellParagraphs = cell.Paragraphs;
                    foreach (var cellParagraph in cellParagraphs)
                        sb.Append(paraGraphConvert.ParaGraphHandle(cellParagraph, picInfoList));

                    //sb.Append(cell.GetText());
                    sb.Append("</td>");
                }


                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return sb;
        }
    }
}