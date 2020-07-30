using System;
using System.Threading.Tasks;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task SendPreviewAsync(string token, MaterialType materialType, string value, string wxName);

        Task SendAsync(string token, MaterialType materialType, string value, bool isToAll, string tagId,
            bool isTiming, DateTime runOnceAt);
    }
}
