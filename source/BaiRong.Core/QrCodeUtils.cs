using System.Collections.Specialized;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ThoughtWorks.QRCode.Codec;

namespace BaiRong.Core
{
    public class QrCodeUtils
    {
        private QrCodeUtils()
        {
        }

        //输出二维码图片
        public static byte[] GetBuffer(string str)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeScale = 4;

            //将字符串生成二维码图片
            Bitmap image = qrCodeEncoder.Encode(str, Encoding.Default);

            //保存为PNG到内存流  
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);

            return ms.GetBuffer();
        }
    }
}
