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
		public DataGrid dgContents;
        public Button AddInput;
        public Button Import;

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

                dgContents.DataSource = DataProvider.InputDao.GetDataSource(PublishmentSystemId);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();

                AddInput.Attributes.Add("onclick", ModalInputAdd.GetOpenWindowStringToAdd(PublishmentSystemId));
                Import.Attributes.Add("onclick", ModalImport.GetOpenWindowString(PublishmentSystemId, ModalImport.TypeInput));

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

        public string GetIsCheckedHtml(string isCheckedString)
        {
            var val = !TranslateUtils.ToBool(isCheckedString);
            return StringUtils.GetTrueImageHtml(val.ToString());
        }

        public string GetIsCodeValidateHtml(string isCodeValidateString)
        {
            return StringUtils.GetTrueImageHtml(isCodeValidateString);
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var inputID = SqlUtils.EvalInt(e.Item.DataItem, "InputID");
                var inputName = SqlUtils.EvalString(e.Item.DataItem, "InputName");

                var LtlTitle = (Literal)e.Item.FindControl("LtlTitle");
                var upLink = (Literal)e.Item.FindControl("UpLink");
                var downLink = (Literal)e.Item.FindControl("DownLink");
                var styleUrl = (Literal)e.Item.FindControl("StyleUrl");
                var previewUrl = (Literal)e.Item.FindControl("PreviewUrl");
                var editUrl = (Literal)e.Item.FindControl("EditUrl");
                var exportUrl = (Literal)e.Item.FindControl("ExportUrl");

                LtlTitle.Text = $@"<a href=""{PageInputContent.GetRedirectUrl(PublishmentSystemId, inputName)}"">{inputName}</a>";

                var urlUp = PageUtils.GetCmsUrl(nameof(PageInput), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"InputID", inputID.ToString()},
                    {"Up", true.ToString()}
                });
                upLink.Text = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                var urlDown = PageUtils.GetCmsUrl(nameof(PageInput), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"InputID", inputID.ToString()},
                    {"Down", true.ToString()}
                });
                downLink.Text =
                    $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                styleUrl.Text =
                    $@"<a href=""{PageTableStyle.GetRedirectUrl(PublishmentSystemId, ETableStyle.InputContent,
                        DataProvider.InputContentDao.TableName, inputID)}"">表单字段</a>";

                previewUrl.Text = $@"<a href=""{PageInputPreview.GetRedirectUrl(PublishmentSystemId, inputID, string.Empty)}"">预览</a>";

                editUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalInputAdd.GetOpenWindowStringToEdit(
                        PublishmentSystemId, inputID, false)}"">编辑</a>";

                exportUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalExportMessage.GetOpenWindowStringToInput(
                        PublishmentSystemId, inputID)}"">导出</a>";
            }
        }
	}
}
