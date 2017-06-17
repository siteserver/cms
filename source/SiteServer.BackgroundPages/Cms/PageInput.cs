using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageInput : BasePageCms
    {
		public DataGrid DgContents;
        public Button BtnAddInput;
        public Button BtnImport;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageInput), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("InputID") && (Body.IsQueryExists("Up") || Body.IsQueryExists("Down")))
            {
                var inputId = Body.GetQueryInt("InputID");
                var isDown = Body.IsQueryExists("Down");
                if (isDown)
                {
                    DataProvider.InputDao.UpdateTaxisToDown(PublishmentSystemId, inputId);
                }
                else
                {
                    DataProvider.InputDao.UpdateTaxisToUp(PublishmentSystemId, inputId);
                }

                Body.AddSiteLog(PublishmentSystemId, "提交表单排序" + (isDown ? "下降" : "上升"));

                PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId));
                return;
            }
            else if (Body.IsQueryExists("Delete"))
            {
                var inputId = Body.GetQueryInt("InputID");
                try
                {
                    var inputInfo = DataProvider.InputDao.GetInputInfo(inputId);
                    if (inputInfo != null)
                    {
                        DataProvider.InputDao.Delete(inputId);
                        Body.AddSiteLog(PublishmentSystemId, "删除提交表单", $"提交表单:{inputInfo.InputName}");
                    }

                    SuccessMessage("删除成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "删除失败！");
                }
            }

			if(!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, String.Empty, "提交表单管理", AppManager.Cms.Permission.WebSite.Input);

                DgContents.DataSource = DataProvider.InputDao.GetDataSource(PublishmentSystemId);
                DgContents.ItemDataBound += DgContents_ItemDataBound;
                DgContents.DataBind();

                BtnAddInput.Attributes.Add("onclick", ModalInputAdd.GetOpenWindowStringToAdd(PublishmentSystemId));
                BtnImport.Attributes.Add("onclick", ModalImport.GetOpenWindowString(PublishmentSystemId, ModalImport.TypeInput));

                if (Body.IsQueryExists("RefreshLeft") || Body.IsQueryExists("Delete"))
                {
                    ClientScriptRegisterStartupScript("RefreshLeft", @"
<script language=""javascript"">
top.frames[""left""].location.reload( false );
</script>
");
                }
			}
		}

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var inputId = SqlUtils.EvalInt(e.Item.DataItem, "InputID");
            var inputName = SqlUtils.EvalString(e.Item.DataItem, "InputName");
            var isChecked = SqlUtils.EvalBool(e.Item.DataItem, "IsChecked");
            var isIsReply = SqlUtils.EvalBool(e.Item.DataItem, "IsReply");

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlIsCheck = (Literal) e.Item.FindControl("ltlIsCheck");
            var ltlIsReply = (Literal)e.Item.FindControl("ltlIsReply");
            var ltlUpLink = (Literal)e.Item.FindControl("ltlUpLink");
            var ltlDownLink = (Literal)e.Item.FindControl("ltlDownLink");
            var ltlStyleUrl = (Literal)e.Item.FindControl("ltlStyleUrl");
            var ltlPreviewUrl = (Literal)e.Item.FindControl("ltlPreviewUrl");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
            var ltlExportUrl = (Literal)e.Item.FindControl("ltlExportUrl");
            var ltlDeleteUrl = (Literal) e.Item.FindControl("ltlDeleteUrl");

            ltlTitle.Text = $@"<a href=""{PageInputContent.GetRedirectUrl(PublishmentSystemId, inputName)}"">{inputName}</a>";
            ltlIsCheck.Text = StringUtils.GetTrueImageHtml(!isChecked);
            ltlIsReply.Text = StringUtils.GetTrueImageHtml(isIsReply);

            var urlUp = PageUtils.GetCmsUrl(nameof(PageInput), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"Up", true.ToString()}
            });
            ltlUpLink.Text = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

            var urlDown = PageUtils.GetCmsUrl(nameof(PageInput), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"Down", true.ToString()}
            });
            ltlDownLink.Text =
                $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

            ltlStyleUrl.Text =
                $@"<a href=""{PageTableStyle.GetRedirectUrl(PublishmentSystemId, ETableStyle.InputContent,
                    DataProvider.InputContentDao.TableName, inputId)}"">表单字段</a>";

            ltlPreviewUrl.Text = $@"<a href=""{PageInputPreview.GetRedirectUrl(PublishmentSystemId, inputId, string.Empty)}"">预览</a>";

            ltlEditUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{ModalInputAdd.GetOpenWindowStringToEdit(
                    PublishmentSystemId, inputId, false)}"">编辑</a>";

            ltlExportUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{ModalExportMessage.GetOpenWindowStringToInput(
                    PublishmentSystemId, inputId)}"">导出</a>";

            var urlDelete = PageUtils.GetCmsUrl(nameof(PageInput), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"Delete", true.ToString()}
            });
            ltlDeleteUrl.Text = $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将删除提交表单“{inputName}”及相关数据，确认吗？');"">删除</a> </ItemTemplate>";
        }
	}
}
