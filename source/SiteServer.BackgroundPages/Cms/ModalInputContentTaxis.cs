using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalInputContentTaxis : BasePageCms
    {
        protected RadioButtonList TaxisType;
        protected TextBox TaxisNum;

        private int _inputId;
        private string _returnUrl;
        private List<int> _contentIdArrayList;

        public static string GetOpenWindowString(int publishmentSystemId,   int inputId, string returnUrl)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("表单内容排序", PageUtils.GetCmsUrl(nameof(ModalInputContentTaxis), new NameValueCollection
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
            _contentIdArrayList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));

            if (!IsPostBack)
            {
                TaxisType.Items.Add(new ListItem("上升", "Up"));
                TaxisType.Items.Add(new ListItem("下降", "Down"));
                ControlUtils.SelectListItems(TaxisType, "Up");


            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isUp = (TaxisType.SelectedValue == "Up");
            var taxisNum = int.Parse(TaxisNum.Text);

            if (isUp == false)
            {
                _contentIdArrayList.Reverse();
            }

            foreach (int contentID in _contentIdArrayList)
            {
                for (var i = 1; i <= taxisNum; i++)
                {
                    if (isUp)
                    {
                        if (DataProvider.InputContentDao.UpdateTaxisToUp(_inputId, contentID))
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (DataProvider.InputContentDao.UpdateTaxisToDown(_inputId, contentID))
                        {
                            break;
                        }
                    }
                }
            }

            PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
        }

    }
}
