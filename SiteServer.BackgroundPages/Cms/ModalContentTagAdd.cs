using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentTagAdd : BasePageCms
    {
		protected TextBox TbTags;

        private string _tagName;

        public static string GetOpenWindowStringToAdd(int siteId)
        {
            return LayerUtils.GetOpenScript("添加标签", PageUtils.GetCmsUrl(siteId, nameof(ModalContentTagAdd), null), 600, 360);
        }

        public static string GetOpenWindowStringToEdit(int siteId, string tagName)
        {
            return LayerUtils.GetOpenScript("修改标签", PageUtils.GetCmsUrl(siteId, nameof(ModalContentTagAdd), new NameValueCollection
            {
                {"TagName", tagName}
            }), 600, 360);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tagName = AuthRequest.GetQueryString("TagName");

            if (IsPostBack) return;

            if (!string.IsNullOrEmpty(_tagName))
            {
                TbTags.Text = _tagName;

                var count = DataProvider.ContentTagRepository.GetTagCountAsync(_tagName, SiteId).GetAwaiter().GetResult();

                InfoMessage($@"标签“<strong>{_tagName}</strong>”被使用 {count} 次，编辑此标签将更新所有使用此标签的内容。");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;
				
			if (!string.IsNullOrEmpty(_tagName))
			{
				try
				{
                    if (!string.Equals(_tagName, TbTags.Text))
                    {
                        var tagCollection = ContentTagUtils.ParseTagsString(TbTags.Text);
                        var contentIdList = DataProvider.ContentTagRepository.GetContentIdListByTagAsync(_tagName, SiteId).GetAwaiter().GetResult();
                        if (contentIdList.Count > 0)
                        {
                            foreach (var contentId in contentIdList)
                            {
                                if (!tagCollection.Contains(_tagName))//删除
                                {
                                    var tagInfo = DataProvider.ContentTagRepository.GetTagAsync(SiteId, _tagName).GetAwaiter().GetResult();
                                    if (tagInfo != null)
                                    {
                                        tagInfo.ContentIds.Remove(contentId);
                                        tagInfo.UseNum = tagInfo.ContentIds.Count;
                                        DataProvider.ContentTagRepository.UpdateAsync(tagInfo).GetAwaiter().GetResult();
                                    }
                                }

                                ContentTagUtils.UpdateTagsAsync(string.Empty, TbTags.Text, SiteId, contentId).GetAwaiter().GetResult();

                                var tags = DataProvider.ContentRepository.GetValueAsync(Site.TableName, contentId, ContentAttribute.Tags).GetAwaiter().GetResult();

                                if (!string.IsNullOrEmpty(tags))
                                {
                                    var contentTagList = StringUtils.GetStringList(tags);
                                    contentTagList.Remove(_tagName);
                                    foreach (var theTag in tagCollection)
                                    {
                                        if (!contentTagList.Contains(theTag))
                                        {
                                            contentTagList.Add(theTag);
                                        }
                                    }
                                    DataProvider.ContentRepository.UpdateAsync(Site.TableName, contentId, ContentAttribute.Tags, TranslateUtils.ObjectCollectionToString(contentTagList)).GetAwaiter().GetResult();
                                }
                            }
                        }
                        else
                        {
                            DataProvider.ContentTagRepository.DeleteTagAsync(_tagName, SiteId).GetAwaiter().GetResult();
                        }
                    }

                    AuthRequest.AddSiteLogAsync(SiteId, "修改内容标签", $"内容标签:{TbTags.Text}").GetAwaiter().GetResult();

					isChanged = true;
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "标签修改失败！");
				}
			}
			else
			{
                try
                {
                    ContentTagUtils.UpdateTagsAsync(string.Empty, TbTags.Text, SiteId, 0).GetAwaiter().GetResult();
                    AuthRequest.AddSiteLogAsync(SiteId, "添加内容标签", $"内容标签:{TbTags.Text}").GetAwaiter().GetResult();
                }
                catch(Exception ex)
                {
                    FailMessage(ex, "标签添加失败！");
                }
			}

			if (isChanged)
			{
                LayerUtils.Close(Page);
			}
		}
	}
}
