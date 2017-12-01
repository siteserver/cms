using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Plugins
{
    public class ModalInputContentTaxis : BasePageCms
    {
        protected RadioButtonList RblTaxisType;
        protected TextBox TbTaxisNum;

        private int _inputId;
        private string _returnUrl;
        private List<int> _contentIdList;

        public static string GetOpenWindowString(int publishmentSystemId,   int inputId, string returnUrl)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("表单内容排序", PageUtils.GetPluginsUrl(nameof(ModalInputContentTaxis), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "ContentIDCollection", "请选择需要排序的内容！", 300, 220);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID",  "ReturnUrl");
            _inputId = Body.GetQueryInt("InputID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            _contentIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));

            if (!IsPostBack)
            {
                RblTaxisType.Items.Add(new ListItem("上升", "Up"));
                RblTaxisType.Items.Add(new ListItem("下降", "Down"));
                ControlUtils.SelectListItems(RblTaxisType, "Up");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isUp = RblTaxisType.SelectedValue == "Up";
            var taxisNum = TranslateUtils.ToInt(TbTaxisNum.Text);

            if (isUp == false)
            {
                _contentIdList.Reverse();
            }

            foreach (var contentId in _contentIdList)
            {
                for (var i = 0; i < taxisNum; i++)
                {
                    if (isUp)
                    {
                        DataProvider.InputContentDao.UpdateTaxisToUp(_inputId, contentId);
                    }
                    else
                    {
                        DataProvider.InputContentDao.UpdateTaxisToDown(_inputId, contentId);
                    }
                }
            }

            PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
        }

    }
}
