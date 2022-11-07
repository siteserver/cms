using System.Threading.Tasks;
using SSCMS.Dto;

namespace SSCMS.Services
{
    public interface IVodManager
    {
        Task<bool> IsVodAsync();

        Task<VodPlay> UploadVodAsync(string filePath);
    }
}
