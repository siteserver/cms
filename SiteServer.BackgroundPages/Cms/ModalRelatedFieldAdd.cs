using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalRelatedFieldAdd : BasePageCms
    {
        public TextBox TbRelatedFieldName;
        public DropDownList DdlTotalLevel;

        public TextBox TbPrefix1;
        public TextBox TbSuffix1;
        public PlaceHolder PhFix2;
        public TextBox TbPrefix2;
        public TextBox TbSuffix2;
        public PlaceHolder PhFix3;
        public TextBox TbPrefix3;
        public TextBox TbSuffix3;
        public PlaceHolder PhFix4;
        public TextBox TbPrefix4;
        public TextBox TbSuffix4;
        public PlaceHolder PhFix5;
        public TextBox TbPrefix5;
        public TextBox TbSuffix5;

        public static string GetOpenWindowString(int siteId, int relatedFieldId)
        {
            return LayerUtils.GetOpenScript("修改联动字段", PageUtils.GetCmsUrl(siteId, nameof(ModalRelatedFieldAdd), new NameValueCollection
            {
                {"RelatedFieldID", relatedFieldId.ToString()}
            }), 550, 550);
        }

        public static string GetOpenWindowString(int siteId)
        {
            return LayerUtils.GetOpenScript("添加联动字段", PageUtils.GetCmsUrl(siteId, nameof(ModalRelatedFieldAdd), null), 550, 550);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            if (AuthRequest.IsQueryExists("RelatedFieldID"))
            {
                var relatedFieldId = AuthRequest.GetQueryInt("RelatedFieldID");
                var relatedFieldInfo = DataProvider.RelatedFieldDao.GetRelatedFieldInfo(relatedFieldId);
                if (relatedFieldInfo != null)
                {
                    TbRelatedFieldName.Text = relatedFieldInfo.Title;
                    ControlUtils.SelectSingleItemIgnoreCase(DdlTotalLevel, relatedFieldInfo.TotalLevel.ToString());

                    if (!string.IsNullOrEmpty(relatedFieldInfo.Prefixes))
                    {
                        var collection = TranslateUtils.StringCollectionToStringCollection(relatedFieldInfo.Prefixes);
                        TbPrefix1.Text = collection[0];
                        TbPrefix2.Text = collection[1];
                        TbPrefix3.Text = collection[2];
                        TbPrefix4.Text = collection[3];
                        TbPrefix5.Text = collection[4];
                    }
                    if (!string.IsNullOrEmpty(relatedFieldInfo.Suffixes))
                    {
                        var collection = TranslateUtils.StringCollectionToStringCollection(relatedFieldInfo.Suffixes);
                        TbSuffix1.Text = collection[0];
                        TbSuffix2.Text = collection[1];
                        TbSuffix3.Text = collection[2];
                        TbSuffix4.Text = collection[3];
                        TbSuffix5.Text = collection[4];
                    }
                }
            }
            DdlTotalLevel_SelectedIndexChanged(null, EventArgs.Empty);
        }

        public void DdlTotalLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhFix2.Visible = PhFix3.Visible = PhFix4.Visible = PhFix5.Visible = false;

            var totalLevel = TranslateUtils.ToInt(DdlTotalLevel.SelectedValue);
            if (totalLevel >= 2)
            {
                PhFix2.Visible = true;
            }
            if (totalLevel >= 3)
            {
                PhFix3.Visible = true;
            }
            if (totalLevel >= 4)
            {
                PhFix4.Visible = true;
            }
            if (totalLevel >= 5)
            {
                PhFix5.Visible = true;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            var relatedFieldInfo = new RelatedFieldInfo
            {
                Title = TbRelatedFieldName.Text,
                SiteId = SiteId,
                TotalLevel = TranslateUtils.ToInt(DdlTotalLevel.SelectedValue)
            };
            var prefix = new ArrayList
            {
                TbPrefix1.Text,
                TbPrefix2.Text,
                TbPrefix3.Text,
                TbPrefix4.Text,
                TbPrefix5.Text
            };
            relatedFieldInfo.Prefixes = TranslateUtils.ObjectCollectionToString(prefix);
            var suffix = new ArrayList
            {
                TbSuffix1.Text,
                TbSuffix2.Text,
                TbSuffix3.Text,
                TbSuffix4.Text,
                TbSuffix5.Text
            };
            relatedFieldInfo.Suffixes = TranslateUtils.ObjectCollectionToString(suffix);
				
			if (AuthRequest.IsQueryExists("RelatedFieldID"))
			{
				try
				{
                    relatedFieldInfo.Id = AuthRequest.GetQueryInt("RelatedFieldID");
                    DataProvider.RelatedFieldDao.Update(relatedFieldInfo);
                    AuthRequest.AddSiteLog(SiteId, "修改联动字段", $"联动字段:{relatedFieldInfo.Title}");
					isChanged = true;
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "联动字段修改失败！");
				}
			}
			else
			{
                var relatedFieldNameList = DataProvider.RelatedFieldDao.GetTitleList(SiteId);
                if (relatedFieldNameList.IndexOf(TbRelatedFieldName.Text) != -1)
				{
                    FailMessage("联动字段添加失败，联动字段名称已存在！");
				}
				else
				{
					try
					{
                        DataProvider.RelatedFieldDao.Insert(relatedFieldInfo);
                        AuthRequest.AddSiteLog(SiteId, "添加联动字段", $"联动字段:{relatedFieldInfo.Title}");
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
                LayerUtils.Close(Page);
			}
		}
	}
}
