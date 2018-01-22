using System.Web.UI;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlControls
{
	public class CommentInput : LiteralControl
	{
        public bool IsAnonymous
        {
            get { return ViewState["IsAnonymous"] == null || TranslateUtils.ToBool(ViewState["IsAnonymous"].ToString()); }
            set { ViewState["IsAnonymous"] = value; }
        }

        public int PageNum
        {
            get { return ViewState["PageNum"] != null ? TranslateUtils.ToInt(ViewState["PageNum"].ToString()) : 20; }
            set { ViewState["PageNum"] = value; }
        }

        public string ApiUrl
        {
            get { return ViewState["ApiUrl"] as string; }
            set { ViewState["ApiUrl"] = value; }
        }

        public string ApiGetUrl
        {
            get { return ViewState["ApiGetUrl"] as string; }
            set { ViewState["ApiGetUrl"] = value; }
        }

        public string ApiActionsAddUrl
        {
            get { return ViewState["ApiActionsAddUrl"] as string; }
            set { ViewState["ApiActionsAddUrl"] = value; }
        }

        public string ApiActionsGoodUrl
        {
            get { return ViewState["ApiActionsGoodUrl"] as string; }
            set { ViewState["ApiActionsGoodUrl"] = value; }
        }

        public string ApiActionsDeleteUrl
        {
            get { return ViewState["ApiActionsDeleteUrl"] as string; }
            set { ViewState["ApiActionsDeleteUrl"] = value; }
        }

        public string ApiActionsLogoutUrl
        {
            get { return ViewState["ApiActionsLogoutUrl"] as string; }
            set { ViewState["ApiActionsLogoutUrl"] = value; }
        }

        public bool IsDelete
        {
            get { return (bool)ViewState["IsDelete"]; }
            set { ViewState["IsDelete"] = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write($@"
<link href=""{SiteFilesAssets.CommentInput.GetStyleUrl(ApiUrl)}"" rel=""stylesheet"" type=""text/css"" />
<script src=""{SiteFilesAssets.CommentInput.GetScriptUrl(ApiUrl)}"" language=""javascript""></script>
<div id=""stlCommentContainer"" class=""stlCommentContainer"">
    {TemplateManager.GetContentByFilePath(SiteFilesAssets.CommentInput.CommentsTemplatePath)}
</div>
<script>
    loadComments('{ApiGetUrl}', '{ApiActionsAddUrl}', '{ApiActionsGoodUrl}', '{ApiActionsDeleteUrl}', '{ApiActionsLogoutUrl}', {PageNum}, {IsDelete.ToString().ToLower()});
</script>
");
		}
	}
}
