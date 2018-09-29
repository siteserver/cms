using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;

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

                var count = DataProvider.TagDao.GetTagCount(_tagName, SiteId);

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
                        var tagCollection = TagUtils.ParseTagsString(TbTags.Text);
                        var contentIdList = DataProvider.TagDao.GetContentIdListByTag(_tagName, SiteId);
                        if (contentIdList.Count > 0)
                        {
                            foreach (var contentId in contentIdList)
                            {
                                if (!tagCollection.Contains(_tagName))//删除
                                {
                                    var tagInfo = DataProvider.TagDao.GetTagInfo(SiteId, _tagName);
                                    if (tagInfo != null)
                                    {
                                        var idArrayList = TranslateUtils.StringCollectionToIntList(tagInfo.ContentIdCollection);
                                        idArrayList.Remove(contentId);
                                        tagInfo.ContentIdCollection = TranslateUtils.ObjectCollectionToString(idArrayList);
                                        tagInfo.UseNum = idArrayList.Count;
                                        DataProvider.TagDao.Update(tagInfo);
                                    }
                                }

                                TagUtils.AddTags(tagCollection, SiteId, contentId);

                                var tuple = DataProvider.ContentDao.GetValue(SiteInfo.TableName, contentId, ContentAttribute.Tags);

                                if (tuple != null)
                                {
                                    var contentTagList = TranslateUtils.StringCollectionToStringList(tuple.Item2);
                                    contentTagList.Remove(_tagName);
                                    foreach (var theTag in tagCollection)
                                    {
                                        if (!contentTagList.Contains(theTag))
                                        {
                                            contentTagList.Add(theTag);
                                        }
                                    }
                                    DataProvider.ContentDao.Update(SiteInfo.TableName, tuple.Item1, contentId, ContentAttribute.Tags, TranslateUtils.ObjectCollectionToString(contentTagList));
                                }
                            }
                        }
                        else
                        {
                            DataProvider.TagDao.DeleteTag(_tagName, SiteId);
                        }
                    }

                    AuthRequest.AddSiteLog(SiteId, "修改内容标签", $"内容标签:{TbTags.Text}");

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
                    var tagCollection = TagUtils.ParseTagsString(TbTags.Text);
                    TagUtils.AddTags(tagCollection, SiteId, 0);
                    AuthRequest.AddSiteLog(SiteId, "添加内容标签", $"内容标签:{TbTags.Text}");
                    isChanged = true;
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
