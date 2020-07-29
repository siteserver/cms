using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public class MaterialMessageRepository : IMaterialMessageRepository
    {
        private readonly Repository<MaterialMessage> _repository;
        private readonly IMaterialArticleRepository _materialArticleRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;
        private readonly IMaterialFileRepository _materialFileRepository;
        private readonly IMaterialMessageItemRepository _materialMessageItemRepository;

        public MaterialMessageRepository(ISettingsManager settingsManager, IMaterialArticleRepository materialArticleRepository, IMaterialImageRepository materialImageRepository, IMaterialVideoRepository materialVideoRepository, IMaterialAudioRepository materialAudioRepository, IMaterialFileRepository materialFileRepository, IMaterialMessageItemRepository materialMessageItemRepository)
        {
            _repository = new Repository<MaterialMessage>(settingsManager.Database, settingsManager.Redis);
            CacheKey = CacheUtils.GetListKey(_repository.TableName);
            _materialArticleRepository = materialArticleRepository;
            _materialImageRepository = materialImageRepository;
            _materialVideoRepository = materialVideoRepository;
            _materialAudioRepository = materialAudioRepository;
            _materialFileRepository = materialFileRepository;
            _materialMessageItemRepository = materialMessageItemRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey { get; }

        public async Task InsertAsync(int groupId, string title, string imageUrl, string summary, string body)
        {
            var article = new MaterialArticle
            {
                GroupId = groupId,
                ThumbMediaId = string.Empty,
                Author = string.Empty,
                Title = title,
                ContentSourceUrl = string.Empty,
                Content = body,
                Digest = summary,
                ShowCoverPic = !string.IsNullOrEmpty(imageUrl),
                ThumbUrl = imageUrl,
                CommentType = CommentType.Block
            };

            var materialId = await _materialArticleRepository.InsertAsync(article);

            var messageId = await _repository.InsertAsync(
                new MaterialMessage
                {
                    GroupId = groupId
                },
                Q.CachingRemove(CacheKey)
            );

            var item = new MaterialMessageItem
            {
                MessageId = messageId,
                MaterialType = MaterialType.Article,
                MaterialId = materialId,
                Taxis = 1
            };
            await _materialMessageItemRepository.InsertAsync(item);
        }

        public async Task<int> InsertAsync(int groupId, string mediaId, List<MaterialMessageItem> items)
        {
            var messageId = await _repository.InsertAsync(
                new MaterialMessage
                {
                    GroupId = groupId,
                    MediaId = mediaId
                },
                Q.CachingRemove(CacheKey)
            );

            var taxis = 1;
            foreach (var item in items)
            {
                var materialId = item.MaterialId;
                if (materialId > 0)
                {
                    var article = await _materialArticleRepository.GetAsync(materialId);

                    article.ThumbMediaId = item.ThumbMediaId;
                    article.Author = item.Author;
                    article.Title = item.Title;
                    article.ContentSourceUrl = item.ContentSourceUrl;
                    article.Content = item.Content;
                    article.Digest = item.Digest;
                    article.ShowCoverPic = item.ShowCoverPic;
                    article.ThumbUrl = item.ThumbUrl;
                    article.CommentType = item.CommentType;

                    await _materialArticleRepository.UpdateAsync(article);
                }
                else
                {
                    materialId = await _materialArticleRepository.InsertAsync(new MaterialArticle
                    {
                        GroupId = groupId,
                        ThumbMediaId = item.ThumbMediaId,
                        Author = item.Author,
                        Title = item.Title,
                        ContentSourceUrl = item.ContentSourceUrl,
                        Content = item.Content,
                        Digest = item.Digest,
                        ShowCoverPic = item.ShowCoverPic,
                        ThumbUrl = item.ThumbUrl,
                        CommentType = item.CommentType
                    });
                }

                await _materialMessageItemRepository.InsertAsync(new MaterialMessageItem
                {
                    MessageId = messageId,
                    MaterialType = MaterialType.Article,
                    MaterialId = materialId,
                    Taxis = taxis++
                });
            }

            await _repository.RemoveCacheAsync(CacheKey);

            return messageId;
        }

        public async Task UpdateAsync(int messageId, int groupId, List<MaterialMessageItem> items)
        {
            await _materialMessageItemRepository.DeleteAllAsync(messageId);

            var taxis = 1;
            foreach (var item in items)
            {
                var materialId = item.MaterialId;
                if (materialId > 0)
                {
                    var article = await _materialArticleRepository.GetAsync(materialId);

                    article.ThumbMediaId = item.ThumbMediaId;
                    article.Author = item.Author;
                    article.Title = item.Title;
                    article.ContentSourceUrl = item.ContentSourceUrl;
                    article.Content = item.Content;
                    article.Digest = item.Digest;
                    article.ShowCoverPic = item.ShowCoverPic;
                    article.ThumbUrl = item.ThumbUrl;
                    article.CommentType = item.CommentType;
                    await _materialArticleRepository.UpdateAsync(article);
                }
                else
                {
                    materialId = await _materialArticleRepository.InsertAsync(new MaterialArticle
                    {
                        GroupId = groupId,
                        ThumbMediaId = item.ThumbMediaId,
                        Author = item.Author,
                        Title = item.Title,
                        ContentSourceUrl = item.ContentSourceUrl,
                        Content = item.Content,
                        Digest = item.Digest,
                        ShowCoverPic = item.ShowCoverPic,
                        ThumbUrl = item.ThumbUrl,
                        CommentType = item.CommentType
                    });
                }

                await _materialMessageItemRepository.InsertAsync(new MaterialMessageItem
                {
                    MessageId = messageId,
                    MaterialType = MaterialType.Article,
                    MaterialId = materialId,
                    Taxis = taxis++
                });
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(MaterialMessage.GroupId), groupId)
                .Where(nameof(MaterialMessage.Id), messageId)
                .CachingRemove(CacheKey)
            );
        }

        public async Task UpdateAsync(int messageId, int groupId)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(MaterialMessage.GroupId), groupId)
                .Where(nameof(MaterialMessage.Id), messageId)
                .CachingRemove(CacheKey)
            );
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var message = await GetAsync(id);
            foreach (var item in message.Items)
            {
                if (!await _materialMessageItemRepository.IsDeletable(item.MaterialType, item.MaterialId)) continue;

                if (item.MaterialType == MaterialType.Article)
                {
                    await _materialArticleRepository.DeleteAsync(item.MaterialId);
                }
            }

            await _materialMessageItemRepository.DeleteAllAsync(message.Id);

            return await _repository.DeleteAsync(id, Q
                .CachingRemove(CacheKey)
            );
        }

        public async Task<int> GetCountAsync(int groupId, string keyword)
        {
            var messages = await GetAllAsync();

            if (groupId != 0)
            {
                messages = messages.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                messages = messages.Where(x =>
                    {
                        return x.Items.Exists(item =>
                            StringUtils.ContainsIgnoreCase(item.Title, keyword) ||
                            StringUtils.ContainsIgnoreCase(item.Digest, keyword) ||
                            StringUtils.ContainsIgnoreCase(item.Content, keyword));
                    }).ToList();
            }

            return messages.Count;
        }

        private async Task<List<MaterialMessage>> GetAllAsync()
        {
            var messages = await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(MaterialArticle.LastModifiedDate))
                .CachingGet(CacheKey)
            );

            var itemMessages = new List<MaterialMessage>();
            foreach (var message in messages)
            {
                message.Items = await _materialMessageItemRepository.GetAllAsync(message.Id);
                if (message.Items == null || message.Items.Count == 0) continue;
                itemMessages.Add(message);
            }

            return itemMessages;
        }

        public async Task<List<MaterialMessage>> GetAllAsync(int groupId, string keyword, int page, int perPage)
        {
            var messages = await GetAllAsync();

            if (groupId != 0)
            {
                messages = messages.Where(x => x.GroupId == groupId).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                messages = messages.Where(x =>
                {
                    return x.Items.Exists(item =>
                        StringUtils.ContainsIgnoreCase(item.Title, keyword) ||
                        StringUtils.ContainsIgnoreCase(item.Digest, keyword) ||
                        StringUtils.ContainsIgnoreCase(item.Content, keyword));
                }).ToList();
            }

            return messages.Skip((page - 1) * perPage).Take(perPage).ToList();
        }

        public async Task<MaterialMessage> GetAsync(int id)
        {
            if (id == 0) return null;
            var messages = await GetAllAsync();
            return messages.FirstOrDefault(x => x.Id == id);
        }

        public async Task<bool> IsExistsAsync(string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId)) return false;

            var messages = await GetAllAsync();
            return messages.Exists(x => x.MediaId == mediaId);
        }
    }
}
