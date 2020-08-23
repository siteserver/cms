using ICSharpCode.SharpZipLib.Zip;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
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
            using var image = Image.Load(filePath);
            return (image.Width, image.Height);
        }

        public void ResizeImage(string originalFilePath, string resizeFilePath, int width, int height)
        {
            // Open the file automatically detecting the file type to decode it.
            // Our image is now in an uncompressed, file format agnostic, structure in-memory as
            // a series of pixels.
            using var image = Image.Load(originalFilePath);
            // Resize the image in place and return it for chaining.
            // 'x' signifies the current image processing context.
            image.Mutate(x => x.Resize(width, height));

            // The library automatically picks an encoder based on the file extension then
            // encodes and write the data to disk.
            // You can optionally set the encoder to choose.
            image.Save(resizeFilePath);
        }

        public void ResizeImageByMax(string originalFilePath, string resizeFilePath, int maxWidth, int maxHeight)
        {
            using var image = Image.Load(originalFilePath);
            var (width, height) = (image.Width, image.Height);

            if (width > maxWidth && height > maxHeight)
            {
                int resizeWidth;
                int resizeHeight;
                if (width > height)
                {
                    resizeWidth = maxWidth;
                    resizeHeight = (int)(height * (maxWidth / (double)width));

                    image.Mutate(x => x.Resize(resizeWidth, resizeHeight));
                    image.Save(resizeFilePath);
                }
                else
                {
                    resizeHeight = maxHeight;
                    resizeWidth = (int)(width * (maxHeight / (double)height));

                    image.Mutate(x => x.Resize(resizeWidth, resizeHeight));
                    image.Save(resizeFilePath);
                }

                image.Mutate(x => x.Resize(resizeWidth, resizeHeight));
                image.Save(resizeFilePath);
            }
            else if (width > maxWidth)
            {
                var resizeWidth = maxWidth;
                var resizeHeight = (int)(height * (maxWidth / (double)width));

                image.Mutate(x => x.Resize(resizeWidth, resizeHeight));
                image.Save(resizeFilePath);
            }
            else if (height > maxHeight)
            {
                var resizeHeight = maxHeight;
                var resizeWidth = (int)(width * (maxHeight / (double)height));

                image.Mutate(x => x.Resize(resizeWidth, resizeHeight));
                image.Save(resizeFilePath);
            }
            else
            {
                FileUtils.CopyFile(originalFilePath, resizeFilePath);
            }
        }
    }
}
