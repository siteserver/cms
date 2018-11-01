using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateMatch : BasePageCms
    {
		public ListBox LbChannelId;
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

            VerifySitePermissions(ConfigManager.WebSitePermissions.Template);

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
			var selectedChannelIdList = new List<string>();
			foreach (ListItem listitem in LbChannelId.Items)
			{
				if (listitem.Selected) selectedChannelIdList.Add(listitem.Value);
			}
			var selectedChannelTemplateId = LbChannelTemplateId.SelectedValue;
			var selectedContentTemplateId = LbContentTemplateId.SelectedValue;

            LbChannelId.Items.Clear();
            LbChannelTemplateId.Items.Clear();
            LbContentTemplateId.Items.Clear();
			var channelIdList = ChannelManager.GetChannelIdList(SiteId);
            var nodeCount = channelIdList.Count;
			_isLastNodeArray = new bool[nodeCount];
            foreach (var theChannelId in channelIdList)
			{
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, theChannelId);
                var listitem = new ListItem(GetTitle(nodeInfo), nodeInfo.Id.ToString());
                LbChannelId.Items.Add(listitem);
			}

            LbChannelTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(SiteId, TemplateType.ChannelTemplate);
            LbContentTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(SiteId, TemplateType.ContentTemplate);
			DataBind();

			ControlUtils.SelectMultiItems(LbChannelId, selectedChannelIdList);
			ControlUtils.SelectSingleItem(LbChannelTemplateId, selectedChannelTemplateId);
			ControlUtils.SelectSingleItem(LbContentTemplateId, selectedContentTemplateId);
		}

		public void MatchChannelTemplateButton_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(true, true)) return;

		    var channelIdList = new List<int>();
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (item.Selected)
		        {
		            var channelId = int.Parse(item.Value);
                    channelIdList.Add(channelId);
		        }
		    }
		    var channelTemplateId = int.Parse(LbChannelTemplateId.SelectedValue);
		    Process(channelIdList, channelTemplateId, true);
		}

		public void RemoveChannelTemplateButton_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, true)) return;

		    var channelIdList = new List<int>();
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (item.Selected)
		        {
		            var channelId = int.Parse(item.Value);
		            channelIdList.Add(channelId);
		        }
		    }
		    Process(channelIdList, 0, true);
		}

		public void MatchContentTemplateButton_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(true, false)) return;

		    var channelIdList = new List<int>();
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (item.Selected)
		        {
		            var channelId = int.Parse(item.Value);
		            channelIdList.Add(channelId);
		        }
		    }
		    var contentTemplateId = int.Parse(LbContentTemplateId.SelectedValue);
		    Process(channelIdList, contentTemplateId, false);
		}

		public void RemoveContentTemplateButton_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var channelIdList = new List<int>();
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (item.Selected)
		        {
		            var channelId = int.Parse(item.Value);
		            channelIdList.Add(channelId);
		        }
		    }
		    Process(channelIdList, 0, false);
		}

		private bool Validate(bool isMatch, bool isChannelTemplate)
		{
			if (LbChannelId.SelectedIndex < 0)
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

		private void Process(List<int> channelIdList, int templateId, bool isChannelTemplate)
		{
			if (channelIdList != null && channelIdList.Count > 0)
			{
                if (isChannelTemplate)
                {
                    foreach (var channelId in channelIdList)
                    {
                        var channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
                        channelInfo.ChannelTemplateId = templateId;
                        DataProvider.ChannelDao.UpdateChannelTemplateId(channelInfo);
                    }
                }
                else
                {
                    foreach (var channelId in channelIdList)
                    {
                        var channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
                        channelInfo.ContentTemplateId = templateId;
                        DataProvider.ChannelDao.UpdateContentTemplateId(channelInfo);
                    }
                }
			}
			
			if (templateId == 0)
			{
                AuthRequest.AddSiteLog(SiteId, "取消模板匹配", $"栏目:{GetNodeNames()}");
				SuccessMessage("取消匹配成功！");
			}
			else
			{
                AuthRequest.AddSiteLog(SiteId, "模板匹配", $"栏目:{GetNodeNames()}");
				SuccessMessage("模板匹配成功！");
			}
            
            BindListBox();
		}

        private string GetNodeNames()
        {
            var builder = new StringBuilder();
            foreach (ListItem listItem in LbChannelId.Items)
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
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (!item.Selected) continue;

		        var channelId = int.Parse(item.Value);
		        var channelTemplateId = -1;

		        var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
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
		            var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, AuthRequest.AdminName);
		            if (nodeInfo.ParentId > 0)
		            {
		                nodeInfo.ChannelTemplateId = insertedTemplateId;
		                DataProvider.ChannelDao.UpdateChannelTemplateId(nodeInfo);

                        //TemplateManager.UpdateChannelTemplateId(SiteId, channelId, insertedTemplateId);
		                //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(channelId, insertedTemplateID);
		            }
								
		        }
		    }

		    AuthRequest.AddSiteLog(SiteId, "生成并匹配栏目模版", $"栏目:{GetNodeNames()}");

		    SuccessMessage("生成栏目模版并匹配成功！");

		    BindListBox();
		}

		public void CreateSubChannelTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(SiteId, TemplateType.ChannelTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(SiteId, TemplateType.ChannelTemplate);
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (!item.Selected) continue;

		        var channelId = int.Parse(item.Value);
		        var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);

		        var templateInfo = new TemplateInfo(0, SiteId, nodeInfo.ChannelName + "_下级", TemplateType.ChannelTemplate, "T_" + nodeInfo.ChannelName + "_下级.html", "index.html", ".html", ECharsetUtils.GetEnumType(SiteInfo.Additional.Charset), false);
								
		        if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
		        {
		            continue;
		        }
		        if (templateNameList.Contains(templateInfo.TemplateName))
		        {
		            continue;
		        }
		        var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, AuthRequest.AdminName);
		        var childChannelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(SiteId, channelId), EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
		        foreach (var childChannelId in childChannelIdList)
		        {
		            var childChannelInfo = ChannelManager.GetChannelInfo(SiteId, childChannelId);
		            childChannelInfo.ChannelTemplateId = insertedTemplateId;
		            DataProvider.ChannelDao.UpdateChannelTemplateId(childChannelInfo);

                    //TemplateManager.UpdateChannelTemplateId(SiteId, childChannelId, insertedTemplateId);
		            //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(childChannelId, insertedTemplateID);
		        }
		    }

		    AuthRequest.AddSiteLog(SiteId, "生成并匹配下级栏目模版", $"栏目:{GetNodeNames()}");

		    SuccessMessage("生成下级栏目模版并匹配成功！");

		    BindListBox();
		}

		public void CreateContentTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var defaultContentTemplateId = TemplateManager.GetDefaultTemplateId(SiteId, TemplateType.ContentTemplate);
		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(SiteId, TemplateType.ContentTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(SiteId, TemplateType.ContentTemplate);
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (!item.Selected) continue;

		        var channelId = TranslateUtils.ToInt(item.Value);

		        var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);

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
		            var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, AuthRequest.AdminName);

		            var channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
		            channelInfo.ContentTemplateId = insertedTemplateId;
		            DataProvider.ChannelDao.UpdateContentTemplateId(channelInfo);

                    //TemplateManager.UpdateContentTemplateId(SiteId, channelId, insertedTemplateId);
		            //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(channelId, insertedTemplateID);
		        }
		    }

		    AuthRequest.AddSiteLog(SiteId, "生成并匹配内容模版", $"栏目:{GetNodeNames()}");
					
		    SuccessMessage("生成内容模版并匹配成功！");
                    
		    BindListBox();
		}

		public void CreateSubContentTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(SiteId, TemplateType.ContentTemplate);
		    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(SiteId, TemplateType.ContentTemplate);
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (!item.Selected) continue;

		        var channelId = int.Parse(item.Value);
		        var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);

		        var templateInfo = new TemplateInfo(0, SiteId, nodeInfo.ChannelName + "_下级", TemplateType.ContentTemplate, "T_" + nodeInfo.ChannelName + "_下级.html", "index.html", ".html", ECharsetUtils.GetEnumType(SiteInfo.Additional.Charset), false);
								
		        if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
		        {
		            continue;
		        }
		        if (templateNameList.Contains(templateInfo.TemplateName))
		        {
		            continue;
		        }
		        var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, AuthRequest.AdminName);
                var childChannelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(SiteId, channelId), EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
                foreach (var childChannelId in childChannelIdList)
		        {
		            var childChannelInfo = ChannelManager.GetChannelInfo(SiteId, childChannelId);
		            childChannelInfo.ContentTemplateId = insertedTemplateId;
		            DataProvider.ChannelDao.UpdateContentTemplateId(childChannelInfo);

                    //TemplateManager.UpdateContentTemplateId(SiteId, childChannelId, insertedTemplateId);
		            //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(childChannelId, insertedTemplateID);
		        }
		    }

		    AuthRequest.AddSiteLog(SiteId, "生成并匹配下级内容模版", $"栏目:{GetNodeNames()}");
					
		    SuccessMessage("生成下级内容模版并匹配成功！");
                    
		    BindListBox();
		}
	}
}
