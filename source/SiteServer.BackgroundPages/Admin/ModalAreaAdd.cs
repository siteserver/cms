using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Admin
{
	public class ModalAreaAdd : BasePage
    {
        public TextBox AreaName;
        public PlaceHolder phParentID;
        public DropDownList ParentID;

        private int _areaId;
        private string _returnUrl = string.Empty;
        private bool[] _isLastNodeArray;

        public static string GetOpenWindowStringToAdd(string returnUrl)
        {
            return PageUtils.GetOpenWindowString("添加区域", PageUtils.GetAdminUrl(nameof(ModalAreaAdd), new NameValueCollection
            {
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), 460, 360);
        }

        public static string GetOpenWindowStringToEdit(int areaId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("修改区域", PageUtils.GetAdminUrl(nameof(ModalAreaAdd), new NameValueCollection
            {
                {"AreaID", areaId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), 460, 360);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _areaId = Body.GetQueryInt("AreaID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            if (string.IsNullOrEmpty(_returnUrl))
            {
                _returnUrl = PageArea.GetRedirectUrl(0);
            }

			if (!IsPostBack)
			{
                if (_areaId == 0)
                {
                    ParentID.Items.Add(new ListItem("<无上级区域>", "0"));

                    var areaIdList = AreaManager.GetAreaIdList();
                    var count = areaIdList.Count;
                    _isLastNodeArray = new bool[count];
                    foreach (var theAreaId in areaIdList)
                    {
                        var areaInfo = AreaManager.GetAreaInfo(theAreaId);
                        var listitem = new ListItem(GetTitle(areaInfo.AreaId, areaInfo.AreaName, areaInfo.ParentsCount, areaInfo.IsLastNode), theAreaId.ToString());
                        ParentID.Items.Add(listitem);
                    }
                }
                else
                {
                    phParentID.Visible = false;
                }

                if (_areaId != 0)
                {
                    var areaInfo = AreaManager.GetAreaInfo(_areaId);

                    AreaName.Text = areaInfo.AreaName;
                    ParentID.SelectedValue = areaInfo.ParentId.ToString();
                }
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
                if (_isLastNodeArray[i])
                {
                    str = string.Concat(str, "　");
                }
                else
                {
                    str = string.Concat(str, "│");
                }
            }
            if (isLastNode)
            {
                str = string.Concat(str, "└");
            }
            else
            {
                str = string.Concat(str, "├");
            }
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
                        AreaName = AreaName.Text,
                        ParentId = TranslateUtils.ToInt(ParentID.SelectedValue)
                    };

                    BaiRongDataProvider.AreaDao.Insert(areaInfo);
                }
                else
                {
                    var areaInfo = AreaManager.GetAreaInfo(_areaId);

                    areaInfo.AreaName = AreaName.Text;
                    areaInfo.ParentId = TranslateUtils.ToInt(ParentID.SelectedValue);

                    BaiRongDataProvider.AreaDao.Update(areaInfo);
                }

                Body.AddAdminLog("维护区域信息");

                SuccessMessage("区域设置成功！");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "区域设置失败！");
            }

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
            }
        }
	}
}
