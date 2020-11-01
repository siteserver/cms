namespace SSCMS.Services
{
    public partial interface IPathManager
    {
        void CreateZip(string zipFilePath, string directoryPath, string fileFilter = null);

        void ExtractZip(string zipFilePath, string directoryPath, string fileFilter = null);

        (int width, int height) GetImageSize(string filePath);

        void ResizeImage(string originalFilePath, string resizeFilePath, int width, int height);

        void ResizeImageByMax(string originalFilePath, string resizeFilePath, int maxWidth,
            int maxHeight);
    }
}
