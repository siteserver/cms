using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Net;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Sys
{
	public class PageSiteTemplateOnline : BasePageCms
    {
        protected override bool IsSinglePage => true;

	    public DataGrid dgContents;
        private List<string> _directoryNameLowerList;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Body.IsQueryExists("Delete"))
			{
                var siteTemplateDir = Body.GetQueryString("SiteTemplateDir");
			
				try
				{
					SiteTemplateManager.Instance.DeleteSiteTemplate(siteTemplateDir);

                    Body.AddAdminLog("删除站点模板", $"站点模板:{siteTemplateDir}");

					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}

            _directoryNameLowerList = SiteTemplateManager.Instance.GetDirectoryNameLowerList();
			
			if (!Page.IsPostBack)
            {
                BreadCrumbSys(AppManager.Sys.LeftMenu.Site, "在线模板下载", AppManager.Sys.Permission.SysSite);

                try
                {
                    var list = new List<Dictionary<string, string>> ();

                    var content = WebClientUtils.GetRemoteFileSource(StringUtils.Constants.UrlMoban, ECharset.utf_8);

                    var document = XmlUtils.GetXmlDocument(content);
                    var rootNode = XmlUtils.GetXmlNode(document, "//siteTemplates");
                    if (rootNode.ChildNodes.Count > 0)
                    {
                        foreach (XmlNode node in rootNode.ChildNodes)
                        {
                            var ie = node.ChildNodes.GetEnumerator();
                            var templateName = string.Empty;
                            var templateType = string.Empty;
                            var size = string.Empty;
                            var author = string.Empty;
                            var uploadDate = string.Empty;
                            var pageUrl = string.Empty;
                            var demoUrl = string.Empty;
                            var downloadUrl = string.Empty;

                            while (ie.MoveNext())
                            {
                                var childNode = (XmlNode)ie.Current;
                                var nodeName = childNode.Name;
                                if (StringUtils.EqualsIgnoreCase(nodeName, "templateName"))
                                {
                                    templateName = childNode.InnerText;
                                }
                                else if (StringUtils.EqualsIgnoreCase(nodeName, "templateType"))
                                {
                                    templateType = childNode.InnerText;
                                }
                                else if (StringUtils.EqualsIgnoreCase(nodeName, "size"))
                                {
                                    size = childNode.InnerText;
                                }
                                else if (StringUtils.EqualsIgnoreCase(nodeName, "author"))
                                {
                                    author = childNode.InnerText;
                                }
                                else if (StringUtils.EqualsIgnoreCase(nodeName, "uploadDate"))
                                {
                                    uploadDate = childNode.InnerText;
                                }
                                else if (StringUtils.EqualsIgnoreCase(nodeName, "pageUrl"))
                                {
                                    pageUrl = childNode.InnerText;
                                }
                                else if (StringUtils.EqualsIgnoreCase(nodeName, "demoUrl"))
                                {
                                    demoUrl = childNode.InnerText;
                                }
                                else if (StringUtils.EqualsIgnoreCase(nodeName, "downloadUrl"))
                                {
                                    downloadUrl = childNode.InnerText;
                                }
                            }

                            var directorName = PageUtils.GetFileNameFromUrl(downloadUrl);
                            directorName = directorName.Replace(".zip", string.Empty);

                            list.Add(new Dictionary<string, string>
                            {
                                ["templateName"] = templateName,
                                ["templateType"] = templateType,
                                ["size"] = size,
                                ["author"] = author,
                                ["uploadDate"] = uploadDate,
                                ["pageUrl"] = pageUrl,
                                ["demoUrl"] = demoUrl,
                                ["downloadUrl"] = downloadUrl,
                                ["directoryName"] = directorName
                            });
                        }
                    }

                    dgContents.DataSource = list;
                    dgContents.ItemDataBound += dgContents_ItemDataBound;
                    dgContents.DataBind();
                }
                catch(Exception ex)
                {
                    FailMessage(ex, "在线模板获取失败：页面地址无法访问！");
                }
			}
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var dict = (Dictionary<string, string>)e.Item.DataItem;

                var ltlTemplateName = (Literal)e.Item.FindControl("ltlTemplateName");
                var ltlTemplateType = (Literal)e.Item.FindControl("ltlTemplateType");
                var ltlDirectoryName = (Literal)e.Item.FindControl("ltlDirectoryName"); 
                var ltlSize = (Literal)e.Item.FindControl("ltlSize");
                var ltlAuthor = (Literal)e.Item.FindControl("ltlAuthor");
                var ltlUploadDate = (Literal)e.Item.FindControl("ltlUploadDate");
                var ltlPageUrl = (Literal)e.Item.FindControl("ltlPageUrl");
                var ltlDemoUrl = (Literal)e.Item.FindControl("ltlDemoUrl");
                var ltlDownloadUrl = (Literal)e.Item.FindControl("ltlDownloadUrl");

                ltlTemplateName.Text = dict["templateName"];
                var templateType = dict["templateType"];
                var publishmentSystemType = EPublishmentSystemTypeUtils.GetEnumType(templateType);
                ltlTemplateType.Text = EPublishmentSystemTypeUtils.GetHtml(publishmentSystemType);
                var directoryName = dict["directoryName"];
                ltlDirectoryName.Text = directoryName;
                ltlSize.Text = dict["size"];
                ltlAuthor.Text = dict["author"];
                ltlUploadDate.Text = dict["uploadDate"];
                var pageUrl = dict["pageUrl"];
                ltlPageUrl.Text = $@"<a href=""{PageUtils.AddProtocolToUrl(pageUrl)}"" target=""_blank"">简介</a>";
                var demoUrl = dict["demoUrl"];
                ltlDemoUrl.Text = $@"<a href=""{PageUtils.AddProtocolToUrl(demoUrl)}"" target=""_blank"">演示</a>";
                if (_directoryNameLowerList.Contains(directoryName.ToLower().Trim()))
                {
                    ltlDownloadUrl.Text = "已下载";
                }
                else
                {
                    var downloadUrl = dict["downloadUrl"];
                    downloadUrl = TranslateUtils.EncryptStringBySecretKey(downloadUrl);
                    ltlDownloadUrl.Text =
                        $@"<a href=""javascript:;"" onclick=""{Cms.ModalProgressBar.GetOpenWindowStringWithSiteTemplateDownload(
                            downloadUrl, directoryName)}"">下载并导入</a>";
                }
            }
        }
	}
}
