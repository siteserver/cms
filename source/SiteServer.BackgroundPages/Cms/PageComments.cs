using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Sys.Stl.Comments;
using SiteServer.CMS.StlControls;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageComments : BasePageCms
    {
        public CommentInput StlCommentInput;
        public Button BtnExport;

        private int _nodeId;
        private int _contentId;
        private string _returnUrl;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, int contentId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageComments), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ContentID", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        protected override bool IsSinglePage => true;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ContentID");
            _nodeId = Body.GetQueryInt("NodeID");
            _contentId = Body.GetQueryInt("ContentID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("returnUrl"));

            if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "评论管理", string.Empty);

			    StlCommentInput.ApiUrl = PageUtils.OuterApiUrl;
			    StlCommentInput.IsAnonymous = true;
			    StlCommentInput.PageNum = 20;
			    StlCommentInput.ApiActionsAddUrl = ActionsAdd.GetUrl(StlCommentInput.ApiUrl, PublishmentSystemId, _nodeId, _contentId);
			    StlCommentInput.ApiActionsDeleteUrl = ActionsDelete.GetUrl(StlCommentInput.ApiUrl, PublishmentSystemId, _nodeId, _contentId);
			    StlCommentInput.ApiActionsGoodUrl = ActionsGood.GetUrl(StlCommentInput.ApiUrl, PublishmentSystemId, _nodeId, _contentId);
			    StlCommentInput.ApiGetUrl = Get.GetUrl(StlCommentInput.ApiUrl, PublishmentSystemId, _nodeId, _contentId);
			    StlCommentInput.ApiActionsLogoutUrl = ActionsLogout.GetUrl(StlCommentInput.ApiUrl);
			    StlCommentInput.IsDelete = true;

                BtnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToComment(PublishmentSystemId, _nodeId, _contentId));
            }
		}

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(_returnUrl);
        }
    }
}
