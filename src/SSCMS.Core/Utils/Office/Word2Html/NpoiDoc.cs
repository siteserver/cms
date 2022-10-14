using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NPOI.XWPF.UserModel;

namespace SSCMS.Core.Utils.Office.Word2Html
{
    public class NpoiDoc
    {
        /// <summary>
        ///     Npoi处理Docx
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="uploadImgUrlyDelegate">上传图片事件 入参byte[] 为图片数据 string为图片类型 </param>
        /// <returns></returns>
        public async Task<string> NpoiDocx(Stream stream, Func<byte[], string, string> uploadImgUrlyDelegate = null)
        {
            var myDocx = new XWPFDocument(stream); //打开07（.docx）以上的版本的文档

            var picturesConvert = new PicturesConvert();
            picturesConvert.uploadImgUrlyDelegate += uploadImgUrlyDelegate;

            var paraGraphConvert = new ParaGraphConvert();


            var tableConvert = new TableConvert();
            var picInfoList = await picturesConvert.PicturesHandleAsync(myDocx);

            var sb = new StringBuilder();

            foreach (var para in myDocx.BodyElements)
                switch (para.ElementType)
                {
                    case BodyElementType.PARAGRAPH:
                    {
                        var paragraph = (XWPFParagraph) para;
                        sb.Append(paraGraphConvert.ParaGraphHandle(paragraph, picInfoList));

                        break;
                    }

                    case BodyElementType.TABLE:
                        var paraTable = (XWPFTable) para;
                        sb.Append(tableConvert.TableHandle(paraTable, picInfoList));
                        break;
                }


            return sb.Replace(" style=''", "").ToString();
        }
    }


    public class NpoiDocTest
    {
        public async Task<string> TestNpoiDocx(Stream stream)
        {
            var npoiDoc = new NpoiDoc();
            return await npoiDoc.NpoiDocx(stream, NpoiDoc_uploadImgUrlyDelegate);
        }

        private string NpoiDoc_uploadImgUrlyDelegate(byte[] imgByte, string picType)
        {
            return $"data:{picType};base64,{Convert.ToBase64String(imgByte)}";
        }
    }
}