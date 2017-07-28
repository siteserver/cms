using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalRestrictionAdd : BasePage
    {
        protected TextBox Restriction;
        private string _type;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, string type)
        {
            return PageUtils.GetOpenWindowString("添加IP访问规则", PageUtils.GetSettingsUrl(nameof(ModalRestrictionAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"Type", type}
            }), 450, 300);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, string type, string restriction)
        {
            return PageUtils.GetOpenWindowString("修改IP访问规则", PageUtils.GetSettingsUrl(nameof(ModalRestrictionAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"Type", type},
                {"Restriction", restriction}
            }), 450, 300);
        }

		public void Page_Load(object sender, EventArgs e)
		{
            if (IsForbidden) return;

            _type = Body.GetQueryString("Type");
			if (!IsPostBack)
			{
                if (Body.IsQueryExists("Restriction"))
				{
                    Restriction.Text = Body.GetQueryString("Restriction");
				}
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            StringCollection restrictionList;

            if (_type == "Black")
            {
                restrictionList = ConfigManager.Instance.RestrictionBlackList;
            }
            else
            {
                restrictionList = ConfigManager.Instance.RestrictionWhiteList;
            }

            if (Body.IsQueryExists("Restriction"))
			{
				try
				{
                    var stringColl = new StringCollection();
                    foreach (var restriction in restrictionList)
                    {
                        if (restriction == Body.GetQueryString("Restriction"))
                        {
                            stringColl.Add(Restriction.Text);
                        }
                        else
                        {
                            stringColl.Add(restriction);
                        }
                    }

                    if (_type == "Black")
                    {
                        ConfigManager.Instance.RestrictionBlackList = stringColl;
                    }
                    else
                    {
                        ConfigManager.Instance.RestrictionWhiteList = stringColl;
                    }

                    BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

					isChanged = true;
				}
				catch
				{
                    FailMessage("IP访问规则修改失败！");
				}
			}
			else
			{
                if (restrictionList.IndexOf(Restriction.Text) != -1)
				{
                    FailMessage("IP访问规则添加失败，IP访问规则已存在！");
				}
				else
				{
					try
					{
                        restrictionList.Add(Restriction.Text);

                        if (_type == "Black")
                        {
                            ConfigManager.Instance.RestrictionBlackList = restrictionList;
                        }
                        else
                        {
                            ConfigManager.Instance.RestrictionWhiteList = restrictionList;
                        }

                        BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

						isChanged = true;
					}
					catch(Exception ex)
					{
                        FailMessage(ex, "IP访问规则添加失败！");
					}
				}
			}

			if (isChanged)
			{
                Body.AddAdminLog("设置后台IP访问规则");
				PageUtils.CloseModalPage(Page);
			}
		}
	}
}
