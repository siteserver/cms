using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IChannelRepository : IRepository
    {
        Task InsertChannelAsync(Channel parentChannel, Channel channel);

        Task<int> InsertAsync(int siteId, int parentId, string channelName, string indexName, string contentModelPluginId, int channelTemplateId, int contentTemplateId);

        Task<int> InsertAsync(Channel channel);

        Task UpdateAsync(Channel channel);

        Task UpdateChannelTemplateIdAsync(Channel channel);

        Task UpdateContentTemplateIdAsync(Channel channel);

        Task DeleteAsync(Site site, int channelId, int adminId);

        Task DeleteAllAsync(int siteId);

        Task<bool> IsFilePathExistsAsync(int siteId, string filePath);

        string GetWhereString(string group, string groupNot, bool isImageExists, bool isImage);

        Task<List<int>> GetChannelIdsByTotalNumAsync(List<int> channelIds, int totalNum, string orderByString,
            string whereString);

        Task<List<string>> GetAllFilePathBySiteIdAsync(int siteId);

        int GetTemplateUseCount(int siteId, int templateId, TemplateType templateType, bool isDefault,
            List<Channel> channels);

        Task<List<int>> GetChannelIdsAsync(Template template);

        List<int> GetChannelIdsByTemplateId(bool isChannelTemplate, int templateId, List<Channel> channels);
    }
}