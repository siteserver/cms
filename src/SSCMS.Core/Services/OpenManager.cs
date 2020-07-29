using System;
using Senparc.Weixin.MP;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class OpenManager : IOpenManager
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IOpenAccountRepository _openAccountRepository;
        private readonly IOpenMenuRepository _openMenuRepository;
        private readonly IMaterialMessageRepository _materialMessageRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;

        public OpenManager(ISettingsManager settingsManager, IOpenAccountRepository openAccountRepository, IOpenMenuRepository openMenuRepository, IMaterialMessageRepository materialMessageRepository, IMaterialImageRepository materialImageRepository, IMaterialAudioRepository materialAudioRepository, IMaterialVideoRepository materialVideoRepository)
        {
            _settingsManager = settingsManager;
            _openAccountRepository = openAccountRepository;
            _openMenuRepository = openMenuRepository;
            _materialMessageRepository = materialMessageRepository;
            _materialImageRepository = materialImageRepository;
            _materialAudioRepository = materialAudioRepository;
            _materialVideoRepository = materialVideoRepository;
        }

        private GroupMessageType GetGroupMessageType(MaterialType materialType)
        {
            if (materialType == MaterialType.Message) return GroupMessageType.mpnews;
            if (materialType == MaterialType.Text) return GroupMessageType.text;
            if (materialType == MaterialType.Image) return GroupMessageType.image;
            if (materialType == MaterialType.Audio) return GroupMessageType.voice;
            if (materialType == MaterialType.Video) return GroupMessageType.video;
            return GroupMessageType.mpnews;
        }

        private string SaveImages(string content)
        {
            var originalImageSrcs = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var originalImageSrc in originalImageSrcs)
            {
                if (!PageUtils.IsProtocolUrl(originalImageSrc)) continue;

                var extName = "png";
                if (StringUtils.Contains(originalImageSrc, "wx_fmt="))
                {
                    extName = originalImageSrc.Substring(originalImageSrc.LastIndexOf("=", StringComparison.Ordinal) + 1);
                }

                var materialFileName = PathUtils.GetMaterialFileNameByExtName(extName);
                var virtualDirectoryPath = PathUtils.GetMaterialVirtualDirectoryPath(UploadType.Image);

                var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
                var filePath = PathUtils.Combine(directoryPath, materialFileName);

                WebClientUtils.Download(originalImageSrc, filePath);

                var imageUrl = PageUtils.Combine(virtualDirectoryPath, materialFileName);

                content = content.Replace(" data-src=", "src=");
                content = content.Replace(originalImageSrc, imageUrl);
            }
            return content;
        }
    }
}
