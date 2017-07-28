using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovPublicCategoryClassAdd : BasePageCms
	{
        protected TextBox tbClassName;
        protected TextBox tbClassCode;
        protected RadioButtonList rblIsEnabled;
        protected TextBox tbDescription;

        private string _classCode;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加分类法",
                PageUtils.GetWcmUrl(nameof(ModalGovPublicCategoryClassAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()}
                }), 400, 360);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, string classCode)
        {
            return PageUtils.GetOpenWindowString("修改分类法",
                PageUtils.GetWcmUrl(nameof(ModalGovPublicCategoryClassAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"ClassCode", classCode}
                }), 400, 360);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _classCode = Request.QueryString["ClassCode"];

			if (!IsPostBack)
			{
                if (!string.IsNullOrEmpty(_classCode))
                {
                    var categoryClassInfo = DataProvider.GovPublicCategoryClassDao.GetCategoryClassInfo(_classCode, PublishmentSystemId);
                    if (categoryClassInfo != null)
                    {
                        tbClassName.Text = categoryClassInfo.ClassName;
                        tbClassCode.Text = categoryClassInfo.ClassCode;
                        tbClassCode.Enabled = false;
                        ControlUtils.SelectListItems(rblIsEnabled, categoryClassInfo.IsEnabled.ToString());
                        if (categoryClassInfo.IsSystem)
                        {
                            rblIsEnabled.Enabled = false;
                        }
                        tbDescription.Text = categoryClassInfo.Description;
                    }
                }
                
				
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;
            GovPublicCategoryClassInfo categoryClassInfo = null;
				
			if (!string.IsNullOrEmpty(_classCode))
			{
				try
				{
                    categoryClassInfo = DataProvider.GovPublicCategoryClassDao.GetCategoryClassInfo(_classCode, PublishmentSystemId);
                    if (categoryClassInfo != null)
                    {
                        categoryClassInfo.ClassName = tbClassName.Text;
                        categoryClassInfo.ClassCode = tbClassCode.Text;
                        categoryClassInfo.IsEnabled = TranslateUtils.ToBool(rblIsEnabled.SelectedValue);
                        categoryClassInfo.Description = tbDescription.Text;
                    }
                    DataProvider.GovPublicCategoryClassDao.Update(categoryClassInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改分类法", $"分类法:{categoryClassInfo.ClassName}");

					isChanged = true;
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "分类法修改失败！");
				}
			}
			else
			{
                var classNameArrayList = DataProvider.GovPublicCategoryClassDao.GetClassNameArrayList(PublishmentSystemId);
                var classCodeArrayList = DataProvider.GovPublicCategoryClassDao.GetClassCodeArrayList(PublishmentSystemId);
                if (classNameArrayList.IndexOf(tbClassName.Text) != -1)
				{
                    FailMessage("分类法添加失败，分类法名称已存在！");
                }
                else if (classCodeArrayList.IndexOf(tbClassCode.Text) != -1)
                {
                    FailMessage("分类法添加失败，分类代码已存在！");
                }
				else
				{
					try
					{
                        categoryClassInfo = new GovPublicCategoryClassInfo(tbClassCode.Text, PublishmentSystemId, tbClassName.Text, false, TranslateUtils.ToBool(rblIsEnabled.SelectedValue), string.Empty, 0, tbDescription.Text);

                        DataProvider.GovPublicCategoryClassDao.Insert(categoryClassInfo);

                        Body.AddSiteLog(PublishmentSystemId, "添加分类法", $"分类法:{categoryClassInfo.ClassName}");

						isChanged = true;
					}
					catch(Exception ex)
					{
                        FailMessage(ex, "分类法添加失败！");
					}
				}
			}

			if (isChanged)
			{
                PageUtils.CloseModalPageAndRedirect(Page, PageGovPublicCategoryClass.GetRedirectUrl(PublishmentSystemId));
			}
		}
	}
}
