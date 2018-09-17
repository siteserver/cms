using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalAreaAdd : BasePage
    {
        public TextBox TbAreaName;
        public PlaceHolder PhParentId;
        public DropDownList DdlParentId;

        private int _areaId;
        private string _returnUrl = string.Empty;
        private bool[] _isLastNodeArray;

        public static string GetOpenWindowStringToAdd(string returnUrl)
        {
            return LayerUtils.GetOpenScript("添加区域", PageUtils.GetSettingsUrl(nameof(ModalAreaAdd), new NameValueCollection
            {
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), 460, 360);
        }

        public static string GetOpenWindowStringToEdit(int areaId, string returnUrl)
        {
            return LayerUtils.GetOpenScript("修改区域", PageUtils.GetSettingsUrl(nameof(ModalAreaAdd), new NameValueCollection
            {
                {"AreaID", areaId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), 460, 360);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _areaId = AuthRequest.GetQueryInt("AreaID");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));
            if (string.IsNullOrEmpty(_returnUrl))
            {
                _returnUrl = PageAdminArea.GetRedirectUrl(0);
            }

            if (IsPostBack) return;

            if (_areaId == 0)
            {
                DdlParentId.Items.Add(new ListItem("<无上级区域>", "0"));

                var areaIdList = AreaManager.GetAreaIdList();
                var count = areaIdList.Count;
                _isLastNodeArray = new bool[count];
                foreach (var theAreaId in areaIdList)
                {
                    var areaInfo = AreaManager.GetAreaInfo(theAreaId);
                    var listitem = new ListItem(GetTitle(areaInfo.Id, areaInfo.AreaName, areaInfo.ParentsCount, areaInfo.IsLastNode), theAreaId.ToString());
                    DdlParentId.Items.Add(listitem);
                }
            }
            else
            {
                PhParentId.Visible = false;
            }

            if (_areaId != 0)
            {
                var areaInfo = AreaManager.GetAreaInfo(_areaId);

                TbAreaName.Text = areaInfo.AreaName;
                DdlParentId.SelectedValue = areaInfo.ParentId.ToString();
            }
        }

        public string GetTitle(int areaId, string areaName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (isLastNode == false)
            {
                _isLastNodeArray[parentsCount] = false;
            }
            else
            {
                _isLastNodeArray[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, _isLastNodeArray[i] ? "　" : "│");
            }
            str = string.Concat(str, isLastNode ? "└" : "├");
            str = string.Concat(str, areaName);
            return str;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                if (_areaId == 0)
                {
                    var areaInfo = new AreaInfo
                    {
                        AreaName = TbAreaName.Text,
                        ParentId = TranslateUtils.ToInt(DdlParentId.SelectedValue)
                    };

                    DataProvider.AreaDao.Insert(areaInfo);
                }
                else
                {
                    var areaInfo = AreaManager.GetAreaInfo(_areaId);

                    areaInfo.AreaName = TbAreaName.Text;
                    areaInfo.ParentId = TranslateUtils.ToInt(DdlParentId.SelectedValue);

                    DataProvider.AreaDao.Update(areaInfo);
                }

                AuthRequest.AddAdminLog("维护区域信息");

                SuccessMessage("区域设置成功！");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "区域设置失败！");
            }

            if (isChanged)
            {
                LayerUtils.CloseAndRedirect(Page, _returnUrl);
            }
        }
	}
}
