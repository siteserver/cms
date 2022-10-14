using System;
using System.Text;
using NPOI.XWPF.UserModel;

namespace SSCMS.Core.Utils.Office.Word2Html
{
    public class TagPConvert
    {
        /// <summary>
        ///     word行处理为P标签
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        public StringBuilder TagPHandle(XWPFParagraph paragraph)
        {
            var sb = new StringBuilder();
            sb.Append("<p style='");

            try
            {
                //左右对齐

                var fontAlignment = paragraph.FontAlignment;
                string fontAlignmentName;
                switch (fontAlignment)
                {
                    case 0:
                        fontAlignmentName = "auto";
                        break;
                    case 1:
                        fontAlignmentName = "left";
                        break;
                    case 2:
                        fontAlignmentName = "center";
                        break;
                    case 3:
                        fontAlignmentName = "right";
                        break;
                    default:
                        fontAlignmentName = "auto";
                        break;
                }

                //自动和左对齐不需样式
                if (fontAlignment > 1) sb.Append($"text-align:{fontAlignmentName};");


                var em = paragraph.IndentationFirstLine / 240;

                if (em > 0) sb.Append($"text-indent:{em}em;");
            }
            catch (Exception)
            {
                // ignored
            }

            sb.Append("'>");
            return sb;
        }
    }
}