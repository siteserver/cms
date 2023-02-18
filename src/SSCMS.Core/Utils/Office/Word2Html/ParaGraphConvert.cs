using System.Collections.Generic;
using System.Text;
using NPOI.XWPF.UserModel;

namespace SSCMS.Core.Utils.Office.Word2Html
{
    public class ParaGraphConvert
    {
        /// <summary>
        ///     word文档对应行内容处理
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="picInfoList"></param>
        /// <returns></returns>
        public StringBuilder ParaGraphHandle(XWPFParagraph paragraph, List<PicInfo> picInfoList)
        {
            var tagPConvert = new TagPConvert();
            var drawingConvert = new DrawingConvert();
            var fontConvert = new FontConvert();


            var sb = new StringBuilder();

            #region P标签

            sb.Append(tagPConvert.TagPHandle(paragraph));

            #endregion


            var runs = paragraph.Runs;
            foreach (var run in runs)
            {
                var ctr = run.GetCTR();

                #region 图片格式

                sb.Append(drawingConvert.DrawingHandle(ctr, picInfoList));

                #endregion

                #region 文本格式

                sb.Append(fontConvert.FontHandle(ctr));

                #endregion
            }

            sb.Append("</p>");
            return sb;
        }
    }
}