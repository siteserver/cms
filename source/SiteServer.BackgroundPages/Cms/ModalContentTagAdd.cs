using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentTagAdd : BasePageCms
    {
		protected TextBox tbTags;

        private string _tagName;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加标签", PageUtils.GetCmsUrl(nameof(ModalContentTagAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 380, 360);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, string tagName)
        {
            return PageUtils.GetOpenWindowString("修改标签", PageUtils.GetCmsUrl(nameof(ModalContentTagAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TagName", tagName}
            }), 380, 360);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tagName = Body.GetQueryString("TagName");

			if (!IsPostBack)
			{
                if (!string.IsNullOrEmpty(_tagName))
                {
                    tbTags.Text = _tagName;

                    var count = BaiRongDataProvider.TagDao.GetTagCount(_tagName, PublishmentSystemId);

                    InfoMessage($@"标签“<strong>{_tagName}</strong>”被使用 {count} 次，编辑此标签将更新所有使用此标签的内容。");
                }
                else
                {
                    InfoMessage("在此可添加新标签");
                }
				
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;
				
			if (!string.IsNullOrEmpty(_tagName))
			{
				try
				{
                    if (!string.Equals(_tagName, tbTags.Text))
                    {
                        var tagCollection = TagUtils.ParseTagsString(tbTags.Text);
                        var contentIDArrayList = BaiRongDataProvider.TagDao.GetContentIdListByTag(_tagName, PublishmentSystemId);
                        if (contentIDArrayList.Count > 0)
                        {
                            foreach (int contentID in contentIDArrayList)
                            {
                                if (!tagCollection.Contains(_tagName))//删除
                                {
                                    var tagInfo = BaiRongDataProvider.TagDao.GetTagInfo(PublishmentSystemId, _tagName);
                                    if (tagInfo != null)
                                    {
                                        var idArrayList = TranslateUtils.StringCollectionToIntList(tagInfo.ContentIdCollection);
                                        idArrayList.Remove(contentID);
                                        tagInfo.ContentIdCollection = TranslateUtils.ObjectCollectionToString(idArrayList);
                                        tagInfo.UseNum = idArrayList.Count;
                                        BaiRongDataProvider.TagDao.Update(tagInfo);
                                    }
                                }

                                TagUtils.AddTags(tagCollection, PublishmentSystemId, contentID);

                                var contentTags = BaiRongDataProvider.ContentDao.GetValue(PublishmentSystemInfo.AuxiliaryTableForContent, contentID, ContentAttribute.Tags);
                                var contentTagArrayList = TranslateUtils.StringCollectionToStringList(contentTags);
                                contentTagArrayList.Remove(_tagName);
                                foreach (var theTag in tagCollection)
                                {
                                    if (!contentTagArrayList.Contains(theTag))
                                    {
                                        contentTagArrayList.Add(theTag);
                                    }
                                }
                                BaiRongDataProvider.ContentDao.SetValue(PublishmentSystemInfo.AuxiliaryTableForContent, contentID, ContentAttribute.Tags, TranslateUtils.ObjectCollectionToString(contentTagArrayList));
                            }
                        }
                        else
                        {
                            BaiRongDataProvider.TagDao.DeleteTag(_tagName, PublishmentSystemId);
                        }
                    }

                    Body.AddSiteLog(PublishmentSystemId, "修改内容标签", $"内容标签:{tbTags.Text}");

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
                    var tagCollection = TagUtils.ParseTagsString(tbTags.Text);
                    TagUtils.AddTags(tagCollection, PublishmentSystemId, 0);
                    Body.AddSiteLog(PublishmentSystemId, "添加内容标签", $"内容标签:{tbTags.Text}");
                    isChanged = true;
                }
                catch(Exception ex)
                {
                    FailMessage(ex, "标签添加失败！");
                }
			}

			if (isChanged)
			{
				PageUtils.CloseModalPage(Page);
			}
		}
	}
}
