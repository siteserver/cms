using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalRelatedFieldAdd : BasePageCms
    {
        protected TextBox RelatedFieldName;
        protected DropDownList TotalLevel;

        protected TextBox Prefix1;
        protected TextBox Suffix1;
        protected PlaceHolder phFix2;
        protected TextBox Prefix2;
        protected TextBox Suffix2;
        protected PlaceHolder phFix3;
        protected TextBox Prefix3;
        protected TextBox Suffix3;
        protected PlaceHolder phFix4;
        protected TextBox Prefix4;
        protected TextBox Suffix4;
        protected PlaceHolder phFix5;
        protected TextBox Prefix5;
        protected TextBox Suffix5;

        public static string GetOpenWindowString(int publishmentSystemId, int relatedFieldId)
        {
            return PageUtils.GetOpenWindowString("修改联动字段", PageUtils.GetCmsUrl(nameof(ModalRelatedFieldAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedFieldID", relatedFieldId.ToString()}
            }), 420, 450);
        }

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加联动字段", PageUtils.GetCmsUrl(nameof(ModalRelatedFieldAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 420, 450);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                if (Body.IsQueryExists("RelatedFieldID"))
				{
                    var relatedFieldId = Body.GetQueryInt("RelatedFieldID");
                    var relatedFieldInfo = DataProvider.RelatedFieldDao.GetRelatedFieldInfo(relatedFieldId);
                    if (relatedFieldInfo != null)
					{
                        RelatedFieldName.Text = relatedFieldInfo.RelatedFieldName;
                        ControlUtils.SelectListItemsIgnoreCase(TotalLevel, relatedFieldInfo.TotalLevel.ToString());

                        if (!string.IsNullOrEmpty(relatedFieldInfo.Prefixes))
                        {
                            var collection = TranslateUtils.StringCollectionToStringCollection(relatedFieldInfo.Prefixes);
                            Prefix1.Text = collection[0];
                            Prefix2.Text = collection[1];
                            Prefix3.Text = collection[2];
                            Prefix4.Text = collection[3];
                            Prefix5.Text = collection[4];
                        }
                        if (!string.IsNullOrEmpty(relatedFieldInfo.Suffixes))
                        {
                            var collection = TranslateUtils.StringCollectionToStringCollection(relatedFieldInfo.Suffixes);
                            Suffix1.Text = collection[0];
                            Suffix2.Text = collection[1];
                            Suffix3.Text = collection[2];
                            Suffix4.Text = collection[3];
                            Suffix5.Text = collection[4];
                        }
					}
				}
                TotalLevel_SelectedIndexChanged(null, EventArgs.Empty);
				
			}
		}

        public void TotalLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            phFix2.Visible = phFix3.Visible = phFix4.Visible = phFix5.Visible = false;

            var totalLevel = TranslateUtils.ToInt(TotalLevel.SelectedValue);
            if (totalLevel >= 2)
            {
                phFix2.Visible = true;
            }
            if (totalLevel >= 3)
            {
                phFix3.Visible = true;
            }
            if (totalLevel >= 4)
            {
                phFix4.Visible = true;
            }
            if (totalLevel >= 5)
            {
                phFix5.Visible = true;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            var relatedFieldInfo = new RelatedFieldInfo
            {
                RelatedFieldName = RelatedFieldName.Text,
                PublishmentSystemID = PublishmentSystemId,
                TotalLevel = TranslateUtils.ToInt(TotalLevel.SelectedValue)
            };
            var prefix = new ArrayList
            {
                Prefix1.Text,
                Prefix2.Text,
                Prefix3.Text,
                Prefix4.Text,
                Prefix5.Text
            };
            relatedFieldInfo.Prefixes = TranslateUtils.ObjectCollectionToString(prefix);
            var suffix = new ArrayList
            {
                Suffix1.Text,
                Suffix2.Text,
                Suffix3.Text,
                Suffix4.Text,
                Suffix5.Text
            };
            relatedFieldInfo.Suffixes = TranslateUtils.ObjectCollectionToString(suffix);
				
			if (Body.IsQueryExists("RelatedFieldID"))
			{
				try
				{
                    relatedFieldInfo.RelatedFieldID = Body.GetQueryInt("RelatedFieldID");
                    DataProvider.RelatedFieldDao.Update(relatedFieldInfo);
                    Body.AddSiteLog(PublishmentSystemId, "修改联动字段", $"联动字段:{relatedFieldInfo.RelatedFieldName}");
					isChanged = true;
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "联动字段修改失败！");
				}
			}
			else
			{
                var relatedFieldNameArrayList = DataProvider.RelatedFieldDao.GetRelatedFieldNameArrayList(PublishmentSystemId);
                if (relatedFieldNameArrayList.IndexOf(RelatedFieldName.Text) != -1)
				{
                    FailMessage("联动字段添加失败，联动字段名称已存在！");
				}
				else
				{
					try
					{
                        DataProvider.RelatedFieldDao.Insert(relatedFieldInfo);
                        Body.AddSiteLog(PublishmentSystemId, "添加联动字段", $"联动字段:{relatedFieldInfo.RelatedFieldName}");
						isChanged = true;
					}
					catch(Exception ex)
					{
						FailMessage(ex, "联动字段添加失败！");
					}
				}
			}

			if (isChanged)
			{
				PageUtils.CloseModalPage(Page);
			}
		}
	}
}
