using System.Text;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace SSCMS.Core.Utils.Office.Word2Html
{
    public class FontConvert
    {
        /// <summary>
        ///     word文本对应处理
        /// </summary>
        /// <param name="ctr"></param>
        /// <returns></returns>
        public StringBuilder FontHandle(CT_R ctr)
        {
            var sb = new StringBuilder();

            #region 文本格式

            var textList = ctr.GetTList();
            foreach (var text in textList)
            {
                sb.Append(
                    "<span style='");
                if (!string.IsNullOrWhiteSpace(ctr.rPr?.color?.val))
                    sb.Append(
                        $"color:#{ctr.rPr.color.val};");
                if (!string.IsNullOrWhiteSpace(ctr.rPr?.highlight?.val.ToString()))
                    sb.Append(
                        $"background-color: {ctr.rPr.highlight.val};");
                if (ctr.rPr?.i?.val == true)
                    sb.Append(
                        "font-style:italic;");
                if (ctr.rPr?.b?.val == true)
                    sb.Append(
                        "font-weight:bold;");
                if (ctr.rPr?.sz != null)
                    sb.Append(
                        $"font-size:{ctr.rPr.sz.val}px;");
                if (!string.IsNullOrWhiteSpace(ctr.rPr?.rFonts?.ascii))
                    sb.Append(
                        $"font-family:{ctr.rPr.rFonts.ascii};");

                sb.Append(
                    "'>");

                sb.Append(text.Value);
                sb.Append("</span>");
            }

            #endregion

            return sb;
        }
    }
}