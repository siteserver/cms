using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentTaxis : BasePageCms
    {
        protected RadioButtonList TaxisType;
        protected TextBox TaxisNum;

        private int _nodeId;
        private string _returnUrl;
        private List<int> _contentIdList;
        private string _tableName;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("内容排序", PageUtils.GetCmsUrl(nameof(ModalContentTaxis), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "ContentIDCollection", "请选择需要排序的内容！", 300, 220);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ReturnUrl", "ContentIDCollection");

            _nodeId = Body.GetQueryInt("NodeID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            _contentIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeId);

            if (!IsPostBack)
            {
                TaxisType.Items.Add(new ListItem("上升", "Up"));
                TaxisType.Items.Add(new ListItem("下降", "Down"));
                ControlUtils.SelectListItems(TaxisType, "Up");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isUp = TaxisType.SelectedValue == "Up";
            var taxisNum = int.Parse(TaxisNum.Text);

            if (isUp == false)
            {
                _contentIdList.Reverse();
            }

            foreach (var contentId in _contentIdList)
            {
                var isTop = TranslateUtils.ToBool(BaiRongDataProvider.ContentDao.GetValue(_tableName, contentId, ContentAttribute.IsTop));
                for (var i = 1; i <= taxisNum; i++)
                {
                    if (isUp)
                    {
                        if (BaiRongDataProvider.ContentDao.UpdateTaxisToUp(_tableName, _nodeId, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (BaiRongDataProvider.ContentDao.UpdateTaxisToDown(_tableName, _nodeId, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                }
            }

            CreateManager.CreateContentTrigger(PublishmentSystemId, _nodeId);

            Body.AddSiteLog(PublishmentSystemId, _nodeId, 0, "对内容排序", string.Empty);

            PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
        }

    }
}
