using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

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

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTemplateMatch), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public string GetTitle(NodeInfo nodeInfo)
		{
			var str = string.Empty;
			if (nodeInfo.NodeId == PublishmentSystemId)
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
		    str = string.Concat(str, StringUtils.MaxLengthText(nodeInfo.NodeName, 8));

            if (nodeInfo.ParentId == 0)
			{
                var indexTemplateId = TemplateManager.GetDefaultTemplateId(PublishmentSystemId, ETemplateType.IndexPageTemplate);
                var indexTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, indexTemplateId);
				str = string.Concat(str, $" ({indexTemplateName})");
            }
            else
            {
                var channelTemplateId = nodeInfo.ChannelTemplateId;
                var contentTemplateId = nodeInfo.ContentTemplateId;

                var channelTemplateName = string.Empty;
                if (channelTemplateId != 0)
                {
                    channelTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, channelTemplateId);
                }
                if (string.IsNullOrEmpty(channelTemplateName))
                {
                    channelTemplateName = _defaultChannelTemplateName;
                }
                str = string.Concat(str, $" ({channelTemplateName})");

                var contentTemplateName = string.Empty;
                if (contentTemplateId != 0)
                {
                    contentTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, contentTemplateId);
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

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            var defaultChannelTemplateId = TemplateManager.GetDefaultTemplateId(PublishmentSystemId, ETemplateType.ChannelTemplate);
            _defaultChannelTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, defaultChannelTemplateId);

            var defaultContentTemplateId = TemplateManager.GetDefaultTemplateId(PublishmentSystemId, ETemplateType.ContentTemplate);
            _defaultContentTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, defaultContentTemplateId);

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
			var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
            var nodeCount = nodeIdList.Count;
			_isLastNodeArray = new bool[nodeCount];
            foreach (var theNodeId in nodeIdList)
			{
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theNodeId);
                var listitem = new ListItem(GetTitle(nodeInfo), nodeInfo.NodeId.ToString());
                LbNodeId.Items.Add(listitem);
			}

            LbChannelTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);
            LbContentTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);
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
                        TemplateManager.UpdateChannelTemplateId(PublishmentSystemId, nodeId, templateId);
                    }
                }
                else
                {
                    foreach (var nodeId in nodeIdList)
                    {
                        TemplateManager.UpdateContentTemplateId(PublishmentSystemId, nodeId, templateId);
                    }
                }
			}
			
			if (templateId == 0)
			{
                Body.AddSiteLog(PublishmentSystemId, "取消模板匹配", $"栏目:{GetNodeNames()}");
				SuccessMessage("取消匹配成功！");
			}
			else
			{
                Body.AddSiteLog(PublishmentSystemId, "模板匹配", $"栏目:{GetNodeNames()}");
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

		    var defaultChannelTemplateId = TemplateManager.GetDefaultTemplateId(PublishmentSystemId, ETemplateType.ChannelTemplate);
		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(PublishmentSystemId, ETemplateType.ChannelTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(PublishmentSystemId, ETemplateType.ChannelTemplate);
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (!item.Selected) continue;

		        var nodeId = int.Parse(item.Value);
		        var channelTemplateId = -1;

		        var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
		        if (nodeInfo.ParentId > 0)
		        {
		            channelTemplateId = nodeInfo.ChannelTemplateId;
		        }

		        if (channelTemplateId != -1 && channelTemplateId != 0 && channelTemplateId != defaultChannelTemplateId)
		        {
		            if (TemplateManager.GetTemplateInfo(PublishmentSystemId, channelTemplateId) == null)
		            {
		                channelTemplateId = -1;
		            }
		        }

		        if (channelTemplateId != -1)
		        {
		            var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName, ETemplateType.ChannelTemplate, "T_" + nodeInfo.NodeName + ".html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								
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
		                TemplateManager.UpdateChannelTemplateId(PublishmentSystemId, nodeId, insertedTemplateId);
		                //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(nodeID, insertedTemplateID);
		            }
								
		        }
		    }

		    Body.AddSiteLog(PublishmentSystemId, "生成并匹配栏目模版", $"栏目:{GetNodeNames()}");

		    SuccessMessage("生成栏目模版并匹配成功！");

		    BindListBox();
		}

		public void CreateSubChannelTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(PublishmentSystemId, ETemplateType.ChannelTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(PublishmentSystemId, ETemplateType.ChannelTemplate);
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (!item.Selected) continue;

		        var nodeId = int.Parse(item.Value);
		        var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

		        var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName + "_下级", ETemplateType.ChannelTemplate, "T_" + nodeInfo.NodeName + "_下级.html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								
		        if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
		        {
		            continue;
		        }
		        if (templateNameList.Contains(templateInfo.TemplateName))
		        {
		            continue;
		        }
		        var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdminName);
		        var childNodeIdArrayList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeId);
		        foreach (var childNodeId in childNodeIdArrayList)
		        {
		            TemplateManager.UpdateChannelTemplateId(PublishmentSystemId, childNodeId, insertedTemplateId);
		            //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(childNodeID, insertedTemplateID);
		        }
		    }

		    Body.AddSiteLog(PublishmentSystemId, "生成并匹配下级栏目模版", $"栏目:{GetNodeNames()}");

		    SuccessMessage("生成下级栏目模版并匹配成功！");

		    BindListBox();
		}

		public void CreateContentTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var defaultContentTemplateId = TemplateManager.GetDefaultTemplateId(PublishmentSystemId, ETemplateType.ContentTemplate);
		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(PublishmentSystemId, ETemplateType.ContentTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(PublishmentSystemId, ETemplateType.ContentTemplate);
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (!item.Selected) continue;

		        var nodeId = TranslateUtils.ToInt(item.Value);

		        var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

		        var contentTemplateId = nodeInfo.ContentTemplateId;                            

		        if (contentTemplateId != 0 && contentTemplateId != defaultContentTemplateId)
		        {
		            if (TemplateManager.GetTemplateInfo(PublishmentSystemId, contentTemplateId) == null)
		            {
		                contentTemplateId = -1;
		            }
		        }

		        if (contentTemplateId != -1)
		        {
		            var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName, ETemplateType.ContentTemplate, "T_" + nodeInfo.NodeName + ".html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
		            if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
		            {
		                continue;
		            }
		            if (templateNameList.Contains(templateInfo.TemplateName))
		            {
		                continue;
		            }
		            var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdminName);
		            TemplateManager.UpdateContentTemplateId(PublishmentSystemId, nodeId, insertedTemplateId);
		            //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(nodeID, insertedTemplateID);
		        }
		    }

		    Body.AddSiteLog(PublishmentSystemId, "生成并匹配内容模版", $"栏目:{GetNodeNames()}");
					
		    SuccessMessage("生成内容模版并匹配成功！");
                    
		    BindListBox();
		}

		public void CreateSubContentTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(PublishmentSystemId, ETemplateType.ContentTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(PublishmentSystemId, ETemplateType.ContentTemplate);
		    foreach (ListItem item in LbNodeId.Items)
		    {
		        if (!item.Selected) continue;

		        var nodeId = int.Parse(item.Value);
		        var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

		        var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName + "_下级", ETemplateType.ContentTemplate, "T_" + nodeInfo.NodeName + "_下级.html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								
		        if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
		        {
		            continue;
		        }
		        if (templateNameList.Contains(templateInfo.TemplateName))
		        {
		            continue;
		        }
		        var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdminName);
		        var childNodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeId);
		        foreach (var childNodeId in childNodeIdList)
		        {
		            TemplateManager.UpdateContentTemplateId(PublishmentSystemId, childNodeId, insertedTemplateId);
		            //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(childNodeID, insertedTemplateID);
		        }
		    }

		    Body.AddSiteLog(PublishmentSystemId, "生成并匹配下级内容模版", $"栏目:{GetNodeNames()}");
					
		    SuccessMessage("生成下级内容模版并匹配成功！");
                    
		    BindListBox();
		}
	}
}
