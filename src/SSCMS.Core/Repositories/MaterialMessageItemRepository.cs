using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class MaterialMessageItemRepository : IMaterialMessageItemRepository
    {
        private readonly Repository<MaterialMessageItem> _repository;
        private readonly IMaterialArticleRepository _materialArticleRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;
        private readonly IMaterialFileRepository _materialFileRepository;

        public MaterialMessageItemRepository(ISettingsManager settingsManager, IMaterialArticleRepository materialArticleRepository, IMaterialImageRepository materialImageRepository, IMaterialVideoRepository materialVideoRepository, IMaterialAudioRepository materialAudioRepository, IMaterialFileRepository materialFileRepository)
        {
            _repository = new Repository<MaterialMessageItem>(settingsManager.Database, settingsManager.Redis);
            _materialArticleRepository = materialArticleRepository;
            _materialImageRepository = materialImageRepository;
            _materialVideoRepository = materialVideoRepository;
            _materialAudioRepository = materialAudioRepository;
            _materialFileRepository = materialFileRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(int messageId)
        {
            return CacheUtils.GetListKey(_repository.TableName, messageId);
        }

        public async Task<int> InsertAsync(MaterialMessageItem item)
        {
            return await _repository.InsertAsync(item, Q
                .CachingRemove(GetCacheKey(item.MessageId))
            );
        }

        public async Task DeleteAllAsync(int messageId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(MaterialMessageItem.MessageId), messageId)
                .CachingRemove(GetCacheKey(messageId))
            );
        }

        public async Task<List<MaterialMessageItem>> GetAllAsync(int messageId)
        {
            var items = await _repository.GetAllAsync(Q
                .Where(nameof(MaterialMessageItem.MessageId), messageId)
                .OrderBy(nameof(MaterialMessageItem.Taxis))
                .CachingGet(GetCacheKey(messageId))
            );

            foreach (var item in items)
            {
                if (item.MaterialType == MaterialType.Article)
                {
                    var article = await _materialArticleRepository.GetAsync(item.MaterialId);

                    item.ThumbMediaId = article.ThumbMediaId;
                    item.Author = article.Author;
                    item.Title = article.Title;
                    item.ContentSourceUrl = article.ContentSourceUrl;
                    item.Content = article.Content;
                    item.Digest = article.Digest;
                    item.ShowCoverPic = article.ShowCoverPic;
                    item.ThumbUrl = article.ThumbUrl;
                    item.CommentType = article.CommentType;
                }
                //else if (item.MaterialType == MaterialType.Image)
                //{
                //    var image = await _materialImageRepository.GetAsync(item.MaterialId);
                //    item.Title = image.Title;
                //    item.ImageUrl = image.Url;
                //}
                //else if (item.MaterialType == MaterialType.Video)
                //{
                //    var video = await _materialVideoRepository.GetAsync(item.MaterialId);
                //    item.Title = video.Title;
                //}
                //else if (item.MaterialType == MaterialType.Audio)
                //{
                //    var audio = await _materialAudioRepository.GetAsync(item.MaterialId);
                //    item.Title = audio.Title;
                //}
                //else if (item.MaterialType == MaterialType.File)
                //{
                //    var file = await _materialFileRepository.GetAsync(item.MaterialId);
                //    item.Title = file.Title;
                //}
            }

            return items;
        }

        public async Task<bool> IsDeletable(MaterialType materialType, int materialId)
        {
            var count = await _repository.CountAsync(Q
                .Where(nameof(MaterialMessageItem.MaterialType), materialType.GetValue())
                .Where(nameof(MaterialMessageItem.MaterialId), materialId)
            );
            return count <= 1;
        }
    }
}
