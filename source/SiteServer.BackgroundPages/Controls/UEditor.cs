using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Web.Controls;

namespace SiteServer.BackgroundPages.Controls
{
    public class UEditor : TextEditorBase, IPostBackDataHandler
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var controllerUrl = CMS.Controllers.Files.UEditor.GetUrl(PageUtils.GetApiUrl(), 0);
            var editorUrl = SiteFilesAssets.GetUrl(PageUtils.GetApiUrl(), "ueditor");

            if (string.IsNullOrEmpty(Height) || Height == "0")
            {
                Height = "280";
            }
            if (string.IsNullOrEmpty(Width) || Width == "0")
            {
                Width = "100%";
            }

            var builder = new StringBuilder();
            builder.Append(
                $@"<script type=""text/javascript"">window.UEDITOR_HOME_URL = ""{editorUrl}/"";window.UEDITOR_CONTROLLER_URL = ""{controllerUrl}"";</script><script type=""text/javascript"" src=""{editorUrl}/editor_config.js""></script><script type=""text/javascript"" src=""{editorUrl}/ueditor_all_min.js""></script>");

            builder.Append($@"
<textarea id=""{ClientID}"" name=""{ClientID}"" style=""display:none"">{HttpUtility.HtmlEncode(Text)}</textarea>
<script type=""text/javascript"">
$(function(){{
  UE.getEditor('{ClientID}', {{allowDivTransToP: false}});
  $('#{ClientID}').show();
}});
</script>");

            writer.Write(builder);
        }

        public event EventHandler TextChanged;

        public bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            var presentValue = Text;
            var postedValue = postCollection[postDataKey];
            if (!presentValue.Equals(postedValue))
            {
                Text = postedValue;
                return true;
            }
            return false;
        }

        public void RaisePostDataChangedEvent()
        {
            OnTextChanged(EventArgs.Empty);
        }

        protected virtual void OnTextChanged(EventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }
    }
}