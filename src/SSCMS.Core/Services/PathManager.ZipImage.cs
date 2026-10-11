using ICSharpCode.SharpZipLib.Zip;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        public void CreateZip(string zipFilePath, string directoryPath, string fileFilter = null)
        {
            var fz = new FastZip { CreateEmptyDirectories = true };
            fz.CreateZip(zipFilePath, directoryPath, true, fileFilter);
        }

        public void ExtractZip(string zipFilePath, string directoryPath, string fileFilter = null)
        {
            var fz = new FastZip();
            fz.ExtractZip(zipFilePath, directoryPath, fileFilter);
        }

        public (int width, int height) GetImageSize(string filePath)
        {
            return ImageUtils.GetSize(filePath);
        }

        public void ResizeImage(string originalFilePath, string resizeFilePath, int width, int height)
        {
            ImageUtils.ResizeTo(originalFilePath, resizeFilePath, width, height);
        }

        public void ResizeImageByMax(string originalFilePath, string resizeFilePath, int maxWidth, int maxHeight)
        {
            var (width, height) = ImageUtils.GetSize(originalFilePath);

            if (maxWidth > 0 && width > maxWidth && maxHeight > 0 && height > maxHeight)
            {
                int resizeWidth;
                int resizeHeight;
                if (width > height)
                {
                    resizeWidth = maxWidth;
                    resizeHeight = (int)(height * (maxWidth / (double)width));
                }
                else
                {
                    resizeHeight = maxHeight;
                    resizeWidth = (int)(width * (maxHeight / (double)height));
                }

                ImageUtils.ResizeTo(originalFilePath, resizeFilePath, resizeWidth, resizeHeight);
            }
            else if (maxWidth > 0 && width > maxWidth)
            {
                var resizeHeight = (int)(height * (maxWidth / (double)width));
                ImageUtils.ResizeTo(originalFilePath, resizeFilePath, maxWidth, resizeHeight);
            }
            else if (maxHeight > 0 && height > maxHeight)
            {
                var resizeWidth = (int)(width * (maxHeight / (double)height));
                ImageUtils.ResizeTo(originalFilePath, resizeFilePath, resizeWidth, maxHeight);
            }
            else
            {
                FileUtils.CopyFile(originalFilePath, resizeFilePath);
            }
        }
    }
}
