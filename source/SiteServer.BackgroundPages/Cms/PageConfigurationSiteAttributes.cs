using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using BaiRong.Core.AuxiliaryTable;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationSiteAttributes : BasePageCms
    {
		public TextBox TbPublishmentSystemName;
        public AuxiliaryControl AcAttributes;
        public Literal LtlSettings;
        public Button BtnSubmit;

        private List<int> _relatedIdentities;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageConfigurationSiteAttributes), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.Site, PublishmentSystemId, PublishmentSystemId);

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "站点属性设置", AppManager.Cms.Permission.WebSite.Configration);

                TbPublishmentSystemName.Text = PublishmentSystemInfo.PublishmentSystemName;

                LtlSettings.Text =
                    $@"<a class=""btn btn-success"" href=""{PageTableStyle.GetRedirectUrl(PublishmentSystemId,
                        ETableStyle.Site, DataProvider.PublishmentSystemDao.TableName, PublishmentSystemId)}"">设置站点属性</a>";

                AcAttributes.SetParameters(PublishmentSystemInfo.Additional.Attributes, PublishmentSystemInfo, 0, _relatedIdentities, ETableStyle.Site, DataProvider.PublishmentSystemDao.TableName, true, IsPostBack);

                BtnSubmit.Attributes.Add("onclick", InputParserUtils.GetValidateSubmitOnClickScript("myForm"));
            }
            else
            {
                AcAttributes.SetParameters(Request.Form, PublishmentSystemInfo, 0, _relatedIdentities, ETableStyle.Site, DataProvider.PublishmentSystemDao.TableName, true, IsPostBack);
            }
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				PublishmentSystemInfo.PublishmentSystemName = TbPublishmentSystemName.Text;
                
				try
				{
                    InputTypeParser.AddValuesToAttributes(ETableStyle.Site, DataProvider.PublishmentSystemDao.TableName, PublishmentSystemInfo, _relatedIdentities, Page.Request.Form, PublishmentSystemInfo.Additional.Attributes);

                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改站点设置");

					SuccessMessage("站点设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "站点设置修改失败！");
				}
			}
		}
	}
}
