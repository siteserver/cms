using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Net;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteTemplateOnline : BasePageCms
    {
        protected override bool IsSinglePage => true;

        public const string UrlTemplates = "http://templates.siteserver.cn/templates.xml";

        public Repeater RptContents;
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

            if (Page.IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Site);

            InfoMessage(@"本页面只显示部分免费模板，更多站点模板请访问官网：<a href=""http://templates.siteserver.cn"" target=""_blank"">http://templates.siteserver.cn</a>");

            try
            {
                var list = new List<Dictionary<string, string>>();

                var content = WebClientUtils.GetRemoteFileSource(UrlTemplates, ECharset.utf_8);

                var document = XmlUtils.GetXmlDocument(content);
                var rootNode = XmlUtils.GetXmlNode(document, "//siteTemplates");
                if (rootNode.ChildNodes.Count > 0)
                {
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        var ie = node.ChildNodes.GetEnumerator();
                        var title = string.Empty;
                        var description = string.Empty;
                        var author = string.Empty;
                        var source = string.Empty;
                        var lastEditDate = string.Empty;

                        while (ie.MoveNext())
                        {
                            var childNode = (XmlNode)ie.Current;
                            if (childNode == null) continue;

                            var nodeName = childNode.Name;
                            if (StringUtils.EqualsIgnoreCase(nodeName, "title"))
                            {
                                title = childNode.InnerText;
                            }
                            else if (StringUtils.EqualsIgnoreCase(nodeName, "description"))
                            {
                                description = childNode.InnerText;
                            }
                            else if (StringUtils.EqualsIgnoreCase(nodeName, "author"))
                            {
                                author = childNode.InnerText;
                            }
                            else if (StringUtils.EqualsIgnoreCase(nodeName, "source"))
                            {
                                source = childNode.InnerText;
                            }
                            else if (StringUtils.EqualsIgnoreCase(nodeName, "lastEditDate"))
                            {
                                lastEditDate = childNode.InnerText;
                            }
                        }

                        if (!string.IsNullOrEmpty(title))
                        {
                            list.Add(new Dictionary<string, string>
                            {
                                ["title"] = title,
                                ["description"] = description,
                                ["author"] = author,
                                ["source"] = source,
                                ["lastEditDate"] = lastEditDate
                            });
                        }
                    }
                }

                RptContents.DataSource = list;
                RptContents.ItemDataBound += RptContents_ItemDataBound;
                RptContents.DataBind();
            }
            catch (Exception ex)
            {
                FailMessage(ex, "在线模板获取失败：页面地址无法访问！");
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;

            var dict = (Dictionary<string, string>)e.Item.DataItem;
            var title = dict["title"];
            var description = dict["description"];
            var author = dict["author"];
            var source = dict["source"];
            var lastEditDate = dict["lastEditDate"];

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription"); 
            var ltlAuthor = (Literal)e.Item.FindControl("ltlAuthor");
            var ltlLastEditDate = (Literal)e.Item.FindControl("ltlLastEditDate");
            var ltlPreviewUrl = (Literal)e.Item.FindControl("ltlPreviewUrl");
            var ltlDownloadUrl = (Literal)e.Item.FindControl("ltlDownloadUrl");

            ltlTitle.Text = $@"<a href=""http://templates.siteserver.cn/t-{title.ToLower()}/index.html"" target=""_blank"">{title}</a>";
            
            ltlDescription.Text = description;
            ltlAuthor.Text = author;
            if (!string.IsNullOrEmpty(source) && PageUtils.IsProtocolUrl(source))
            {
                ltlAuthor.Text = $@"<a href=""{source}"" target=""_blank"">{ltlAuthor.Text}</a>";
            }
            ltlLastEditDate.Text = lastEditDate;

            ltlPreviewUrl.Text = $@"<a href=""http://templates.siteserver.cn/t-{title.ToLower()}/index.html"" target=""_blank"">预览</a>";
            if (_directoryNameLowerList.Contains($"T_{title}".ToLower().Trim()))
            {
                ltlDownloadUrl.Text = "已下载";
            }
            else
            {
                ltlDownloadUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{Cms.ModalProgressBar
                        .GetOpenWindowStringWithSiteTemplateDownload(
                            $"http://download.siteserver.cn/templates/T_{title}.zip")}"">下载并导入</a>";
            }
        }
	}
}
