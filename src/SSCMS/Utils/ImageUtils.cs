using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SSCMS.Utils
{
    public static class ImageUtils
    {
        public static (int width, int height) GetSize(string filePath)
        {
            using var image = Image.Load(filePath);
            return (image.Width, image.Height);
        }

        public static void Resize(string originalFilePath, string resizeFilePath, int width, int height)
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
    }
}
