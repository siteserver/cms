using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.BackgroundPages.Core;
using SiteServer.BackgroundPages.Utils;
using SiteServer.CMS.Caches;
using SiteServer.Utils;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Fx;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentTaxis : BasePageCms
    {
        protected DropDownList DdlTaxisType;
        protected TextBox TbTaxisNum;

        private int _channelId;
        private string _returnUrl;
        private List<int> _contentIdList;
        private string _tableName;
        private ChannelInfo _channelInfo;

        public static string GetOpenWindowString(int siteId, int channelId, string returnUrl)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("内容排序", FxUtils.GetCmsUrl(siteId, nameof(ModalContentTaxis), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "contentIdCollection", "请选择需要排序的内容！", 400, 280);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            WebPageUtils.CheckRequestParameter("siteId", "channelId", "ReturnUrl", "contentIdCollection");

            _channelId = AuthRequest.GetQueryInt("channelId");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));
            _contentIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("contentIdCollection"));
            _tableName = ChannelManager.GetTableName(SiteInfo, _channelId);
            _channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);

            if (IsPostBack) return;

            DdlTaxisType.Items.Add(new ListItem("上升", "Up"));
            DdlTaxisType.Items.Add(new ListItem("下降", "Down"));
            SystemWebUtils.SelectSingleItem(DdlTaxisType, "Up");
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
                if (!_channelInfo.ContentRepository.GetChanelIdAndValue(contentId, ContentAttribute.IsTop,
                    out var channelId, out string value)) continue;

                var isTop = TranslateUtils.ToBool(value);
                for (var i = 1; i <= taxisNum; i++)
                {
                    if (isUp)
                    {
                        if (channelInfo.ContentRepository.SetTaxisToUp(channelId, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (channelInfo.ContentRepository.SetTaxisToDown(channelId, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                }
            }

            CreateManager.TriggerContentChangedEvent(SiteId, _channelId);
            AuthRequest.AddSiteLog(SiteId, _channelId, 0, "对内容排序", string.Empty);

            LayerUtils.CloseAndRedirect(Page, _returnUrl);
        }

    }
}
