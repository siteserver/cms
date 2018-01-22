using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentTagAdd : BasePageCms
    {
		protected TextBox TbTags;

        private string _tagName;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return LayerUtils.GetOpenScript("添加标签", PageUtils.GetCmsUrl(nameof(ModalContentTagAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 600, 360);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, string tagName)
        {
            return LayerUtils.GetOpenScript("修改标签", PageUtils.GetCmsUrl(nameof(ModalContentTagAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TagName", tagName}
            }), 600, 360);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tagName = Body.GetQueryString("TagName");

            if (IsPostBack) return;

            if (!string.IsNullOrEmpty(_tagName))
            {
                TbTags.Text = _tagName;

                var count = DataProvider.TagDao.GetTagCount(_tagName, PublishmentSystemId);

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
                        var contentIdList = DataProvider.TagDao.GetContentIdListByTag(_tagName, PublishmentSystemId);
                        if (contentIdList.Count > 0)
                        {
                            foreach (int contentId in contentIdList)
                            {
                                if (!tagCollection.Contains(_tagName))//删除
                                {
                                    var tagInfo = DataProvider.TagDao.GetTagInfo(PublishmentSystemId, _tagName);
                                    if (tagInfo != null)
                                    {
                                        var idArrayList = TranslateUtils.StringCollectionToIntList(tagInfo.ContentIdCollection);
                                        idArrayList.Remove(contentId);
                                        tagInfo.ContentIdCollection = TranslateUtils.ObjectCollectionToString(idArrayList);
                                        tagInfo.UseNum = idArrayList.Count;
                                        DataProvider.TagDao.Update(tagInfo);
                                    }
                                }

                                TagUtils.AddTags(tagCollection, PublishmentSystemId, contentId);

                                var contentTags = DataProvider.ContentDao.GetValue(PublishmentSystemInfo.AuxiliaryTableForContent, contentId, ContentAttribute.Tags);
                                var contentTagArrayList = TranslateUtils.StringCollectionToStringList(contentTags);
                                contentTagArrayList.Remove(_tagName);
                                foreach (var theTag in tagCollection)
                                {
                                    if (!contentTagArrayList.Contains(theTag))
                                    {
                                        contentTagArrayList.Add(theTag);
                                    }
                                }
                                DataProvider.ContentDao.SetValue(PublishmentSystemInfo.AuxiliaryTableForContent, contentId, ContentAttribute.Tags, TranslateUtils.ObjectCollectionToString(contentTagArrayList));
                            }
                        }
                        else
                        {
                            DataProvider.TagDao.DeleteTag(_tagName, PublishmentSystemId);
                        }
                    }

                    Body.AddSiteLog(PublishmentSystemId, "修改内容标签", $"内容标签:{TbTags.Text}");

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
                    TagUtils.AddTags(tagCollection, PublishmentSystemId, 0);
                    Body.AddSiteLog(PublishmentSystemId, "添加内容标签", $"内容标签:{TbTags.Text}");
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
