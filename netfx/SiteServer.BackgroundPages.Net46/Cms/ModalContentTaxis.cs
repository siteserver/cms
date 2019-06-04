using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentTaxis : BasePageCms
    {
        protected DropDownList DdlTaxisType;
        protected TextBox TbTaxisNum;

        private int _channelId;
        private string _returnUrl;
        private List<int> _contentIdList;
        private ChannelInfo _channelInfo;

        public static string GetOpenWindowString(int siteId, int channelId, string returnUrl)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("内容排序", PageUtilsEx.GetCmsUrl(siteId, nameof(ModalContentTaxis), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "contentIdCollection", "请选择需要排序的内容！", 400, 280);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            FxUtils.CheckRequestParameter("siteId", "channelId", "ReturnUrl", "contentIdCollection");

            _channelId = AuthRequest.GetQueryInt("channelId");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));
            _contentIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("contentIdCollection"));
            _channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);

            if (IsPostBack) return;

            DdlTaxisType.Items.Add(new ListItem("上升", "Up"));
            DdlTaxisType.Items.Add(new ListItem("下降", "Down"));
            ControlUtils.SelectSingleItem(DdlTaxisType, "Up");
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isUp = DdlTaxisType.SelectedValue == "Up";
            var taxisNum = TranslateUtils.ToInt(TbTaxisNum.Text);

            var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            if (ETaxisTypeUtils.Equals(channelInfo.DefaultTaxisType, ETaxisType.OrderByTaxis))
            {
                isUp = !isUp;
            }

            if (isUp == false)
            {
                _contentIdList.Reverse();
            }

            foreach (var contentId in _contentIdList)
            {
                var tuple = _channelInfo.ContentDao.GetValueWithChannelId<string>(contentId, ContentAttribute.IsTop);
                if (tuple == null) continue;

                var isTop = TranslateUtils.ToBool(tuple.Item2);
                for (var i = 1; i <= taxisNum; i++)
                {
                    if (isUp)
                    {
                        if (_channelInfo.ContentDao.SetTaxisToUp(_channelId, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_channelInfo.ContentDao.SetTaxisToDown(_channelId, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                }
            }

            CreateManager.TriggerContentChangedEvent(SiteId, _channelId);
            AuthRequest.AddChannelLog(SiteId, _channelId, "对内容排序");

            LayerUtils.CloseAndRedirect(Page, _returnUrl);
        }

    }
}
