using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateMatch : BasePageCms
    {
		public ListBox LbNodeId;
		public ListBox LbChannelTemplateId;
		public ListBox LbContentTemplateId;
        public Button BtnCreateChannelTemplate;
        public Button BtnCreateSubChannelTemplate;
        public Button BtnCreateContentTemplate;
        public Button BtnCreateSubContentTemplate;

        private bool[] _isLastNodeArray;
        private string _defaultChannelTemplateName;
		private string _defaultContentTemplateName;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTemplateMatch), null);
        }

        public string GetTitle(ChannelInfo nodeInfo)
		{
			var str = string.Empty;
			if (nodeInfo.Id == SiteId)
			{
                nodeInfo.IsLastNode = true;
			}
            if (nodeInfo.IsLastNode == false)
			{
                _isLastNodeArray[nodeInfo.ParentsCount] = false;
			}
			else
			{
                _isLastNodeArray[nodeInfo.ParentsCount] = true;
			}
            for (var i = 0; i < nodeInfo.ParentsCount; i++)
            {
                str = string.Concat(str, _isLastNodeArray[i] ? "　" : "│");
            }
		    str = string.Concat(str, nodeInfo.IsLastNode ? "└" : "├");
		    str = string.Concat(str, StringUtils.MaxLengthText(nodeInfo.ChannelName, 8));

            if (nodeInfo.ParentId == 0)
			{
                var indexTemplateId = TemplateManager.GetDefaultTemplateId(SiteId, TemplateType.IndexPageTemplate);
                var indexTemplateName = TemplateManager.GetTemplateName(SiteId, indexTemplateId);
				str = string.Concat(str, $" ({indexTemplateName})");
            }
            else
            {
                var channelTemplateId = nodeInfo.ChannelTemplateId;
                var contentTemplateId = nodeInfo.ContentTemplateId;

                var channelTemplateName = string.Empty;
                if (channelTemplateId != 0)
                {
                    channelTemplateName = TemplateManager.GetTemplateName(SiteId, channelTemplateId);
                }
                if (string.IsNullOrEmpty(channelTemplateName))
                {
                    channelTemplateName = _defaultChannelTemplateName;
                }
                str = string.Concat(str, $" ({channelTemplateName})");

                var contentTemplateName = string.Empty;
                if (contentTemplateId != 0)
                {
                    contentTemplateName = TemplateManager.GetTemplateName(SiteId, contentTemplateId);
                }
                if (string.IsNullOrEmpty(contentTemplateName))
                {
                    contentTemplateName = _defaultContentTemplateName;
                }
                str = string.Concat(str, $" ({contentTemplateName})");
            }
			
			return str;
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            var defaultChannelTemplateId = TemplateManager.GetDefaultTemplateId(SiteId, TemplateType.ChannelTemplate);
            _defaultChannelTemplateName = TemplateManager.GetTemplateName(SiteId, defaultChannelTemplateId);

            var defaultContentTemplateId = TemplateManager.GetDefaultTemplateId(SiteId, TemplateType.ContentTemplate);
            _defaultContentTemplateName = TemplateManager.GetTemplateName(SiteId, defaultContentTemplateId);

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Template);

            LbChannelTemplateId.Attributes.Add("onfocus", "$('#LbContentTemplateId option:selected').removeAttr('selected')");
            LbContentTemplateId.Attributes.Add("onfocus", "$('#LbChannelTemplateId option:selected').removeAttr('selected')");

            BindListBox();

            BtnCreateChannelTemplate.OnClientClick = $"{AlertUtils.Confirm("创建栏目模板", "此操作将创建空的栏目模板并匹配选中栏目，确认吗？", "创 建", "$('#BtnCreateChannelTemplateReal').click()")}";
            BtnCreateSubChannelTemplate.OnClientClick = $"{AlertUtils.Confirm("创建下级栏目模版", "此操作将创建空的栏目模板并匹配选中栏目的下级栏目，确认吗？", "创 建", "$('#BtnCreateSubChannelTemplateReal').click()")}";
            BtnCreateContentTemplate.OnClientClick = $"{AlertUtils.Confirm("创建内容模版", "此操作将创建空的内容模板并匹配选中栏目，确认吗？", "创 建", "$('#BtnCreateContentTemplateReal').click()")}";
            BtnCreateSubContentTemplate.OnClientClick = $"{AlertUtils.Confirm("创建下级内容模版", "此操作将创建空的内容模板并匹配选中栏目的下级栏目，确认吗？", "创 建", "$('#BtnCreateSubContentTemplateReal').click()")}";
        }


		public void BindListBox()
		{
			var selectedNodeIdList = new List<string>();
			foreach (ListItem listitem in LbNodeId.Items)
			{
				if (listitem.Selected) selectedNodeIdList.Add(listitem.Value);
			}
			var selectedChannelTemplateId = LbChannelTemplateId.SelectedValue;
			var selectedContentTemplateId = LbContentTemplateId.SelectedValue;

            LbNodeId.Items.Clear();
            LbChannelTemplateId.Items.Clear();
            LbContentTemplateId.Items.Clear();
			var nodeIdList = DataProvider.ChannelDao.GetIdListBySiteId(SiteId);
            var nodeCount = nodeIdList.Count;
			_isLastNodeArray = new bool[nodeCount];
            foreach (var theNodeId in nodeIdList)
			{
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, theNodeId);
                var listitem = new ListItem(GetTitle(nodeInfo), nodeInfo.Id.ToString());
                LbNodeId.Items.Add(listitem);
			}

            LbChannelTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(SiteId, TemplateType.ChannelTemplate);
            LbContentTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(SiteId, TemplateType.ContentTemplate);
			DataBind();

			ControlUtils.SelectMultiItems(LbNodeId, selectedNodeIdList);
			ControlUtils.SelectSingleItem(LbChannelTemplateId, selectedChannelTemplateId);
			ControlUtils.SelectSingleItem(LbContentTemplateId, selectedContentTemplateId);
		}

		public void MatchChannelTemplateButton_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(true, true)) return;

		    var nodeIdList = new List<int>();
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (item.Selected)
		        {
		            var nodeId = int.Parse(item.Value);
                    nodeIdList.Add(nodeId);
		        }
		    }
		    var channelTemplateId = int.Parse(LbChannelTemplateId.SelectedValue);
		    Process(nodeIdList, channelTemplateId, true);
		}

		public void RemoveChannelTemplateButton_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, true)) return;

		    var nodeIdList = new List<int>();
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (item.Selected)
		        {
		            var nodeId = int.Parse(item.Value);
		            nodeIdList.Add(nodeId);
		        }
		    }
		    Process(nodeIdList, 0, true);
		}

		public void MatchContentTemplateButton_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(true, false)) return;

		    var nodeIdList = new List<int>();
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (item.Selected)
		        {
		            var nodeId = int.Parse(item.Value);
		            nodeIdList.Add(nodeId);
		        }
		    }
		    var contentTemplateId = int.Parse(LbContentTemplateId.SelectedValue);
		    Process(nodeIdList, contentTemplateId, false);
		}

		public void RemoveContentTemplateButton_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var nodeIdList = new List<int>();
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (item.Selected)
		        {
		            var nodeId = int.Parse(item.Value);
		            nodeIdList.Add(nodeId);
		        }
		    }
		    Process(nodeIdList, 0, false);
		}

		private bool Validate(bool isMatch, bool isChannelTemplate)
		{
			if (LbNodeId.SelectedIndex < 0)
			{
                FailMessage("请选择栏目！");
				return false;
			}
			if (isMatch)
			{
				if (isChannelTemplate)
				{
					if (LbChannelTemplateId.SelectedIndex < 0)
					{
                        FailMessage("请选择栏目模板！");
						return false;
					}
				}
				else
				{
					if (LbContentTemplateId.SelectedIndex < 0)
					{
                        FailMessage("请选择内容模板！");
						return false;
					}
				}
			}
			return true;
		}

		private void Process(List<int> nodeIdList, int templateId, bool isChannelTemplate)
		{
			if (nodeIdList != null && nodeIdList.Count > 0)
			{
                if (isChannelTemplate)
                {
                    foreach (var nodeId in nodeIdList)
                    {
                        TemplateManager.UpdateChannelTemplateId(SiteId, nodeId, templateId);
                    }
                }
                else
                {
                    foreach (var nodeId in nodeIdList)
                    {
                        TemplateManager.UpdateContentTemplateId(SiteId, nodeId, templateId);
                    }
                }
			}
			
			if (templateId == 0)
			{
                Body.AddSiteLog(SiteId, "取消模板匹配", $"栏目:{GetNodeNames()}");
				SuccessMessage("取消匹配成功！");
			}
			else
			{
                Body.AddSiteLog(SiteId, "模板匹配", $"栏目:{GetNodeNames()}");
				SuccessMessage("模板匹配成功！");
			}
            
            BindListBox();
		}

        private string GetNodeNames()
        {
            var builder = new StringBuilder();
            foreach (ListItem listItem in LbNodeId.Items)
            {
                if (listItem.Selected)
                {
                    builder.Append(listItem.Text).Append(",");
                }
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
            }

            return RegexUtils.Replace("\\(.*?\\)|│|├|　|└", builder.ToString(), string.Empty);
        }

		public void CreateChannelTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var defaultChannelTemplateId = TemplateManager.GetDefaultTemplateId(SiteId, TemplateType.ChannelTemplate);
		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(SiteId, TemplateType.ChannelTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(SiteId, TemplateType.ChannelTemplate);
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (!item.Selected) continue;

		        var nodeId = int.Parse(item.Value);
		        var channelTemplateId = -1;

		        var nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);
		        if (nodeInfo.ParentId > 0)
		        {
		            channelTemplateId = nodeInfo.ChannelTemplateId;
		        }

		        if (channelTemplateId != -1 && channelTemplateId != 0 && channelTemplateId != defaultChannelTemplateId)
		        {
		            if (TemplateManager.GetTemplateInfo(SiteId, channelTemplateId) == null)
		            {
		                channelTemplateId = -1;
		            }
		        }

		        if (channelTemplateId != -1)
		        {
		            var templateInfo = new TemplateInfo(0, SiteId, nodeInfo.ChannelName, TemplateType.ChannelTemplate, "T_" + nodeInfo.ChannelName + ".html", "index.html", ".html", ECharsetUtils.GetEnumType(SiteInfo.Additional.Charset), false);
								
		            if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
		            {
		                continue;
		            }
		            if (templateNameList.Contains(templateInfo.TemplateName))
		            {
		                continue;
		            }
		            var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdminName);
		            if (nodeInfo.ParentId > 0)
		            {
		                TemplateManager.UpdateChannelTemplateId(SiteId, nodeId, insertedTemplateId);
		                //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(nodeID, insertedTemplateID);
		            }
								
		        }
		    }

		    Body.AddSiteLog(SiteId, "生成并匹配栏目模版", $"栏目:{GetNodeNames()}");

		    SuccessMessage("生成栏目模版并匹配成功！");

		    BindListBox();
		}

		public void CreateSubChannelTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(SiteId, TemplateType.ChannelTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(SiteId, TemplateType.ChannelTemplate);
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (!item.Selected) continue;

		        var nodeId = int.Parse(item.Value);
		        var nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);

		        var templateInfo = new TemplateInfo(0, SiteId, nodeInfo.ChannelName + "_下级", TemplateType.ChannelTemplate, "T_" + nodeInfo.ChannelName + "_下级.html", "index.html", ".html", ECharsetUtils.GetEnumType(SiteInfo.Additional.Charset), false);
								
		        if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
		        {
		            continue;
		        }
		        if (templateNameList.Contains(templateInfo.TemplateName))
		        {
		            continue;
		        }
		        var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdminName);
		        var childNodeIdArrayList = DataProvider.ChannelDao.GetIdListForDescendant(nodeId);
		        foreach (var childNodeId in childNodeIdArrayList)
		        {
		            TemplateManager.UpdateChannelTemplateId(SiteId, childNodeId, insertedTemplateId);
		            //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(childNodeID, insertedTemplateID);
		        }
		    }

		    Body.AddSiteLog(SiteId, "生成并匹配下级栏目模版", $"栏目:{GetNodeNames()}");

		    SuccessMessage("生成下级栏目模版并匹配成功！");

		    BindListBox();
		}

		public void CreateContentTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var defaultContentTemplateId = TemplateManager.GetDefaultTemplateId(SiteId, TemplateType.ContentTemplate);
		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(SiteId, TemplateType.ContentTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(SiteId, TemplateType.ContentTemplate);
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (!item.Selected) continue;

		        var nodeId = TranslateUtils.ToInt(item.Value);

		        var nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);

		        var contentTemplateId = nodeInfo.ContentTemplateId;                            

		        if (contentTemplateId != 0 && contentTemplateId != defaultContentTemplateId)
		        {
		            if (TemplateManager.GetTemplateInfo(SiteId, contentTemplateId) == null)
		            {
		                contentTemplateId = -1;
		            }
		        }

		        if (contentTemplateId != -1)
		        {
		            var templateInfo = new TemplateInfo(0, SiteId, nodeInfo.ChannelName, TemplateType.ContentTemplate, "T_" + nodeInfo.ChannelName + ".html", "index.html", ".html", ECharsetUtils.GetEnumType(SiteInfo.Additional.Charset), false);
		            if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
		            {
		                continue;
		            }
		            if (templateNameList.Contains(templateInfo.TemplateName))
		            {
		                continue;
		            }
		            var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdminName);
		            TemplateManager.UpdateContentTemplateId(SiteId, nodeId, insertedTemplateId);
		            //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(nodeID, insertedTemplateID);
		        }
		    }

		    Body.AddSiteLog(SiteId, "生成并匹配内容模版", $"栏目:{GetNodeNames()}");
					
		    SuccessMessage("生成内容模版并匹配成功！");
                    
		    BindListBox();
		}

		public void CreateSubContentTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(SiteId, TemplateType.ContentTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(SiteId, TemplateType.ContentTemplate);
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (!item.Selected) continue;

		        var nodeId = int.Parse(item.Value);
		        var nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);

		        var templateInfo = new TemplateInfo(0, SiteId, nodeInfo.ChannelName + "_下级", TemplateType.ContentTemplate, "T_" + nodeInfo.ChannelName + "_下级.html", "index.html", ".html", ECharsetUtils.GetEnumType(SiteInfo.Additional.Charset), false);
								
		        if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
		        {
		            continue;
		        }
		        if (templateNameList.Contains(templateInfo.TemplateName))
		        {
		            continue;
		        }
		        var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdminName);
		        var childNodeIdList = DataProvider.ChannelDao.GetIdListForDescendant(nodeId);
		        foreach (var childNodeId in childNodeIdList)
		        {
		            TemplateManager.UpdateContentTemplateId(SiteId, childNodeId, insertedTemplateId);
		            //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(childNodeID, insertedTemplateID);
		        }
		    }

		    Body.AddSiteLog(SiteId, "生成并匹配下级内容模版", $"栏目:{GetNodeNames()}");
					
		    SuccessMessage("生成下级内容模版并匹配成功！");
                    
		    BindListBox();
		}
	}
}
