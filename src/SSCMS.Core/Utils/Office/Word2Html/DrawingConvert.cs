using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace SSCMS.Core.Utils.Office.Word2Html
{
    public class DrawingConvert
    {
        /// <summary>
        ///     word图片对应处理
        /// </summary>
        /// <param name="ctr"></param>
        /// <param name="picInfoList"></param>
        /// <returns></returns>
        public StringBuilder DrawingHandle(CT_R ctr, List<PicInfo> picInfoList)
        {
            var sb = new StringBuilder();
            var drawingList = ctr.GetDrawingList();
            foreach (var drawing in drawingList)
            {
                var a = drawing.GetInlineList();
                foreach (var a1 in a)
                {
                    var anyList = a1.graphic.graphicData.Any;

                    foreach (var any1 in anyList)
                    {
                        var pictures = picInfoList
                            .FirstOrDefault(x =>
                                any1.IndexOf("a:blip r:embed=\"" + x.Id + "\"", StringComparison.Ordinal) > -1);
                        if (pictures != null && !string.IsNullOrWhiteSpace(pictures.Url))
                            sb.Append($@"<img src='{pictures.Url}' />");
                    }
                }
            }

            return sb;
        }
    }
}