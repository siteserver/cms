using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentTranslate : BasePageCms
    {
        public Literal LtlContents;
        public Button BtnTranslateAdd;
        public RadioButtonList RblTranslateType;

        private Dictionary<int, List<int>> _idsDictionary = new Dictionary<int, List<int>>();
        private string _returnUrl;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentTranslate), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public static string GetRedirectClickStringForMultiChannels(int publishmentSystemId, string returnUrl)
        {
            var redirectUrl = PageUtils.GetCmsUrl(nameof(PageContentTranslate), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
            return PageUtils.GetRedirectStringWithCheckBoxValue(redirectUrl, "IDsCollection", "IDsCollection", "请选择需要转移的内容！");
        }

        public static string GetRedirectClickString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            var redirectUrl = PageUtils.GetCmsUrl(nameof(PageContentTranslate), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
            return PageUtils.GetRedirectStringWithCheckBoxValue(redirectUrl, "ContentIDCollection", "ContentIDCollection", "请选择需要转移的内容！");
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("PublishmentSystemID", "ReturnUrl");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            //if (!base.HasChannelPermissions(this.nodeID, AppManager.CMS.Permission.Channel.ContentTranslate))
            //{
            //    PageUtils.RedirectToErrorPage("您没有此栏目的内容转移权限！");
            //    return;
            //}

            //bool isCut = base.HasChannelPermissions(this.nodeID, AppManager.CMS.Permission.Channel.ContentDelete);
            const bool isCut = true;
            _idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);

            if (IsPostBack) return;

            var builder = new StringBuilder();
            foreach (var nodeId in _idsDictionary.Keys)
            {
                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);
                var contentIdArrayList = _idsDictionary[nodeId];
                if (contentIdArrayList != null)
                {
                    foreach (int contentId in contentIdArrayList)
                    {
                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                        if (contentInfo != null)
                        {
                            builder.Append(
                                $@"{WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, _returnUrl)}<br />");
                        }
                    }
                }
            }
            LtlContents.Text = builder.ToString();

            BtnTranslateAdd.Attributes.Add("onclick", ModalChannelMultipleSelect.GetOpenWindowString(PublishmentSystemId, true));

            ETranslateContentTypeUtils.AddListItems(RblTranslateType, isCut);
            ControlUtils.SelectSingleItem(RblTranslateType, ETranslateContentTypeUtils.GetValue(ETranslateContentType.Copy));
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (!string.IsNullOrEmpty(Request.Form["translateCollection"]))
            {
                try
                {
                    var translateType = ETranslateContentTypeUtils.GetEnumType(RblTranslateType.SelectedValue);

                    foreach (var nodeId in _idsDictionary.Keys)
                    {
                        var contentIdArrayList = _idsDictionary[nodeId];
                        if (contentIdArrayList != null)
                        {
                            contentIdArrayList.Reverse();
                            if (contentIdArrayList.Count > 0)
                            {
                                foreach (var contentId in contentIdArrayList)
                                {
                                    ContentUtility.Translate(PublishmentSystemInfo, nodeId, contentId, Request.Form["translateCollection"], translateType, Body.AdminName);

                                    Body.AddSiteLog(PublishmentSystemInfo.PublishmentSystemId, nodeId, contentId, "转移内容", string.Empty);
                                }
                            }
                        }
                    }

                    SuccessMessage("内容转移成功！");
                    AddWaitAndRedirectScript(_returnUrl);
                }
                catch (Exception ex)
                {
                    LogUtils.AddSystemErrorLog(ex);
                    FailMessage(ex, "内容转移失败！");
                }
            }
            else
            {
                FailMessage("请选择需要转移到的栏目！");
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(_returnUrl);
        }
	}
}
