using System.Web.UI;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Controls
{
	public class Code : LiteralControl 
	{
		public virtual string Type 
		{
			get 
			{
				var type = ViewState["Type"] as string;
				return !string.IsNullOrEmpty(type) ? type : string.Empty;
			}
			set 
			{
				ViewState["Type"] = value;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			var code = string.Empty;
			if (!string.IsNullOrEmpty(Type))
			{
                if (Type.Trim().ToLower() == "jquery")
                {
                    code =
                        $@"<script language=""javascript"" src=""{SiteServerAssets.GetUrl(SiteServerAssets.JQuery.Js)}""></script>";
                }
                else if (Type.Trim().ToLower() == "ajaxupload")
                {
                    code =
                        $@"<script language=""javascript"" src=""{SiteServerAssets.GetUrl(SiteServerAssets.AjaxUpload.Js)}""></script>";
                }
                else if (Type.Trim().ToLower() == "bootstrap")
                {
                    var cssUrl = SiteServerAssets.GetUrl(SiteServerAssets.Bootstrap.Css);
                    var jsUrl = SiteServerAssets.GetUrl(SiteServerAssets.Bootstrap.Js);

                    code = $@"
<link rel=""stylesheet"" type=""text/css"" href=""{cssUrl}"">
<script language=""javascript"" src=""{jsUrl}""></script>
";
                }
                else if (Type.Trim().ToLower() == "calendar")
                {
                    code =
                        $@"<script language=""javascript"" src=""{SiteServerAssets.GetUrl(SiteServerAssets.DatePicker.Js)}""></script>";
                }
                else if (Type.Trim().ToLower() == "toastr")
                {
                    code =
                        $@"<link rel=""stylesheet"" type=""text/css"" href=""{SiteServerAssets.GetUrl(SiteServerAssets.Toastr.Css)}""><script language=""javascript"" src=""{SiteServerAssets.GetUrl(SiteServerAssets.Toastr.Js)}""></script>";
                }
                else if (Type.Trim().ToLower() == "layer")
                {
                    code =
                        $@"<script language=""javascript"" src=""{SiteServerAssets.GetUrl(SiteServerAssets.Layer.Js)}""></script>";
                }
			}

			writer.Write(code);
		}


	}
}
