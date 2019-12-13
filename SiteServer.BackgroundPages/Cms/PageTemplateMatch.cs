using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;
using SiteServer.Abstractions;

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

        public string GetTitle(Channel node)
		{
			var str = string.Empty;
			if (node.Id == SiteId)
			{
                node.LastNode = true;
			}
            if (node.LastNode == false)
			{
                _isLastNodeArray[node.ParentsCount] = false;
			}
			else
			{
                _isLastNodeArray[node.ParentsCount] = true;
			}
            for (var i = 0; i < node.ParentsCount; i++)
            {
                str = string.Concat(str, _isLastNodeArray[i] ? "　" : "│");
            }
		    str = string.Concat(str, node.LastNode ? "└" : "├");
		    str = string.Concat(str, WebUtils.MaxLengthText(node.ChannelName, 8));

            if (node.ParentId == 0)
			{
                var indexTemplateId = TemplateManager.GetDefaultTemplateIdAsync(SiteId, TemplateType.IndexPageTemplate).GetAwaiter().GetResult();
                var indexTemplateName = TemplateManager.GetTemplateNameAsync(SiteId, indexTemplateId).GetAwaiter().GetResult();
				str = string.Concat(str, $" ({indexTemplateName})");
            }
            else
            {
                var channelTemplateId = node.ChannelTemplateId;
                var contentTemplateId = node.ContentTemplateId;

                var channelTemplateName = string.Empty;
                if (channelTemplateId != 0)
                {
                    channelTemplateName = TemplateManager.GetTemplateNameAsync(SiteId, channelTemplateId).GetAwaiter().GetResult();
                }
                if (string.IsNullOrEmpty(channelTemplateName))
                {
                    channelTemplateName = _defaultChannelTemplateName;
                }
                str = string.Concat(str, $" ({channelTemplateName})");

                var contentTemplateName = string.Empty;
                if (contentTemplateId != 0)
                {
                    contentTemplateName = TemplateManager.GetTemplateNameAsync(SiteId, contentTemplateId).GetAwaiter().GetResult();
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

            var defaultChannelTemplateId = TemplateManager.GetDefaultTemplateIdAsync(SiteId, TemplateType.ChannelTemplate).GetAwaiter().GetResult();
            _defaultChannelTemplateName = TemplateManager.GetTemplateNameAsync(SiteId, defaultChannelTemplateId).GetAwaiter().GetResult();

            var defaultContentTemplateId = TemplateManager.GetDefaultTemplateIdAsync(SiteId, TemplateType.ContentTemplate).GetAwaiter().GetResult();
            _defaultContentTemplateName = TemplateManager.GetTemplateNameAsync(SiteId, defaultContentTemplateId).GetAwaiter().GetResult();

            if (IsPostBack) return;

            VerifySitePermissions(Constants.SitePermissions.TemplatesMatch);

            LbChannelTemplateId.Attributes.Add("onFocus", "$('#LbContentTemplateId option:selected').removeAttr('selected')");
            LbContentTemplateId.Attributes.Add("onFocus", "$('#LbChannelTemplateId option:selected').removeAttr('selected')");

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
			var channelIdList = ChannelManager.GetChannelIdListAsync(SiteId).GetAwaiter().GetResult();
            var nodeCount = channelIdList.Count;
			_isLastNodeArray = new bool[nodeCount];
            foreach (var theChannelId in channelIdList)
			{
                var nodeInfo = ChannelManager.GetChannelAsync(SiteId, theChannelId).GetAwaiter().GetResult();
                var listitem = new ListItem(GetTitle(nodeInfo), nodeInfo.Id.ToString());
                LbChannelId.Items.Add(listitem);
			}

            LbChannelTemplateId.DataSource = DataProvider.TemplateRepository.GetTemplateListByTypeAsync(SiteId, TemplateType.ChannelTemplate).GetAwaiter().GetResult();
            LbContentTemplateId.DataSource = DataProvider.TemplateRepository.GetTemplateListByTypeAsync(SiteId, TemplateType.ContentTemplate).GetAwaiter().GetResult();
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
                        var channelInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
                        channelInfo.ChannelTemplateId = templateId;
                        DataProvider.ChannelRepository.UpdateChannelTemplateIdAsync(channelInfo).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    foreach (var channelId in channelIdList)
                    {
                        var channelInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
                        channelInfo.ContentTemplateId = templateId;
                        DataProvider.ChannelRepository.UpdateContentTemplateIdAsync(channelInfo).GetAwaiter().GetResult();
                    }
                }
			}
			
			if (templateId == 0)
			{
                AuthRequest.AddSiteLogAsync(SiteId, "取消模板匹配", $"栏目:{GetNodeNames()}").GetAwaiter().GetResult();
				SuccessMessage("取消匹配成功！");
			}
			else
			{
                AuthRequest.AddSiteLogAsync(SiteId, "模板匹配", $"栏目:{GetNodeNames()}").GetAwaiter().GetResult();
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

		    var defaultChannelTemplateId = TemplateManager.GetDefaultTemplateIdAsync(SiteId, TemplateType.ChannelTemplate).GetAwaiter().GetResult();
		    var relatedFileNameList = DataProvider.TemplateRepository.GetRelatedFileNameListAsync(SiteId, TemplateType.ChannelTemplate).GetAwaiter().GetResult();
		    var templateNameList = DataProvider.TemplateRepository.GetTemplateNameListAsync(SiteId, TemplateType.ChannelTemplate).GetAwaiter().GetResult();
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (!item.Selected) continue;

		        var channelId = int.Parse(item.Value);
		        var channelTemplateId = -1;

		        var nodeInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
                if (nodeInfo.ParentId > 0)
		        {
		            channelTemplateId = nodeInfo.ChannelTemplateId;
		        }

		        if (channelTemplateId != -1 && channelTemplateId != 0 && channelTemplateId != defaultChannelTemplateId)
		        {
		            if (TemplateManager.GetTemplateAsync(SiteId, channelTemplateId).GetAwaiter().GetResult() == null)
		            {
		                channelTemplateId = -1;
		            }
		        }

		        if (channelTemplateId != -1)
		        {
		            var templateInfo = new Template
                    {
                        Id = 0,
                        SiteId = SiteId,
                        TemplateName = nodeInfo.ChannelName,
                        Type = TemplateType.ChannelTemplate,
                        RelatedFileName = "T_" + nodeInfo.ChannelName + ".html",
                        CreatedFileFullName = "index.html",
                        CreatedFileExtName = ".html",
                        CharsetType = ECharsetUtils.GetEnumType(Site.Charset),
                        Default = false
                    };
								
		            if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
		            {
		                continue;
		            }
		            if (templateNameList.Contains(templateInfo.TemplateName))
		            {
		                continue;
		            }
		            var insertedTemplateId = DataProvider.TemplateRepository.InsertAsync(templateInfo, string.Empty, AuthRequest.AdminName).GetAwaiter().GetResult();
		            if (nodeInfo.ParentId > 0)
		            {
		                nodeInfo.ChannelTemplateId = insertedTemplateId;
		                DataProvider.ChannelRepository.UpdateChannelTemplateIdAsync(nodeInfo).GetAwaiter().GetResult();

                        //TemplateManager.UpdateChannelTemplateId(SiteId, channelId, insertedTemplateId);
                        //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(channelId, insertedTemplateID);
                    }
								
		        }
		    }

		    AuthRequest.AddSiteLogAsync(SiteId, "生成并匹配栏目模版", $"栏目:{GetNodeNames()}").GetAwaiter().GetResult();

		    SuccessMessage("生成栏目模版并匹配成功！");

		    BindListBox();
		}

		public void CreateSubChannelTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var relatedFileNameList = DataProvider.TemplateRepository.GetRelatedFileNameListAsync(SiteId, TemplateType.ChannelTemplate).GetAwaiter().GetResult();
		    var templateNameList = DataProvider.TemplateRepository.GetTemplateNameListAsync(SiteId, TemplateType.ChannelTemplate).GetAwaiter().GetResult();
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (!item.Selected) continue;

		        var channelId = int.Parse(item.Value);
		        var nodeInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = SiteId,
                    TemplateName = nodeInfo.ChannelName + "_下级",
                    Type = TemplateType.ChannelTemplate,
                    RelatedFileName = "T_" + nodeInfo.ChannelName + "_下级.html",
                    CreatedFileFullName = "index.html",
                    CreatedFileExtName = ".html",
                    CharsetType = ECharsetUtils.GetEnumType(Site.Charset),
                    Default = false
                };
								
		        if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
		        {
		            continue;
		        }
		        if (templateNameList.Contains(templateInfo.TemplateName))
		        {
		            continue;
		        }
		        var insertedTemplateId = DataProvider.TemplateRepository.InsertAsync(templateInfo, string.Empty, AuthRequest.AdminName).GetAwaiter().GetResult();
		        var childChannelIdList = ChannelManager.GetChannelIdListAsync(ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult(), EScopeType.Descendant, string.Empty, string.Empty, string.Empty).GetAwaiter().GetResult();
                foreach (var childChannelId in childChannelIdList)
		        {
		            var childChannelInfo = ChannelManager.GetChannelAsync(SiteId, childChannelId).GetAwaiter().GetResult();
                    childChannelInfo.ChannelTemplateId = insertedTemplateId;
		            DataProvider.ChannelRepository.UpdateChannelTemplateIdAsync(childChannelInfo).GetAwaiter().GetResult();

                    //TemplateManager.UpdateChannelTemplateId(SiteId, childChannelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(childChannelId, insertedTemplateID);
                }
		    }

		    AuthRequest.AddSiteLogAsync(SiteId, "生成并匹配下级栏目模版", $"栏目:{GetNodeNames()}").GetAwaiter().GetResult();

		    SuccessMessage("生成下级栏目模版并匹配成功！");

		    BindListBox();
		}

		public void CreateContentTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var defaultContentTemplateId = TemplateManager.GetDefaultTemplateIdAsync(SiteId, TemplateType.ContentTemplate).GetAwaiter().GetResult();
            var relatedFileNameList = DataProvider.TemplateRepository.GetRelatedFileNameListAsync(SiteId, TemplateType.ContentTemplate).GetAwaiter().GetResult();
		    var templateNameList = DataProvider.TemplateRepository.GetTemplateNameListAsync(SiteId, TemplateType.ContentTemplate).GetAwaiter().GetResult();
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (!item.Selected) continue;

		        var channelId = TranslateUtils.ToInt(item.Value);

		        var nodeInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();

                var contentTemplateId = nodeInfo.ContentTemplateId;                            

		        if (contentTemplateId != 0 && contentTemplateId != defaultContentTemplateId)
		        {
		            if (TemplateManager.GetTemplateAsync(SiteId, contentTemplateId).GetAwaiter().GetResult() == null)
		            {
		                contentTemplateId = -1;
		            }
		        }

		        if (contentTemplateId != -1)
		        {
		            var templateInfo = new Template
                    {
                        Id = 0,
                        SiteId = SiteId,
                        TemplateName = nodeInfo.ChannelName,
                        Type = TemplateType.ContentTemplate,
                        RelatedFileName = "T_" + nodeInfo.ChannelName + ".html",
                        CreatedFileFullName = "index.html",
                        CreatedFileExtName = ".html",
                        CharsetType = ECharsetUtils.GetEnumType(Site.Charset),
                        Default = false
                    };
		            if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
		            {
		                continue;
		            }
		            if (templateNameList.Contains(templateInfo.TemplateName))
		            {
		                continue;
		            }
		            var insertedTemplateId = DataProvider.TemplateRepository.InsertAsync(templateInfo, string.Empty, AuthRequest.AdminName).GetAwaiter().GetResult();

		            var channelInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
                    channelInfo.ContentTemplateId = insertedTemplateId;
		            DataProvider.ChannelRepository.UpdateContentTemplateIdAsync(channelInfo).GetAwaiter().GetResult();

                    //TemplateManager.UpdateContentTemplateId(SiteId, channelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(channelId, insertedTemplateID);
                }
		    }

		    AuthRequest.AddSiteLogAsync(SiteId, "生成并匹配内容模版", $"栏目:{GetNodeNames()}").GetAwaiter().GetResult();
					
		    SuccessMessage("生成内容模版并匹配成功！");
                    
		    BindListBox();
		}

		public void CreateSubContentTemplate_Click(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid || !Validate(false, false)) return;

		    var relatedFileNameList = DataProvider.TemplateRepository.GetRelatedFileNameListAsync(SiteId, TemplateType.ContentTemplate).GetAwaiter().GetResult();
		    var templateNameList = DataProvider.TemplateRepository.GetTemplateNameListAsync(SiteId, TemplateType.ContentTemplate).GetAwaiter().GetResult();
		    foreach (ListItem item in LbChannelId.Items)
		    {
		        if (!item.Selected) continue;

		        var channelId = int.Parse(item.Value);
		        var nodeInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = SiteId,
                    TemplateName = nodeInfo.ChannelName + "_下级",
                    Type = TemplateType.ContentTemplate,
                    RelatedFileName = "T_" + nodeInfo.ChannelName + "_下级.html",
                    CreatedFileFullName = "index.html",
                    CreatedFileExtName = ".html",
                    CharsetType = ECharsetUtils.GetEnumType(Site.Charset),
                    Default = false
                };
								
		        if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
		        {
		            continue;
		        }
		        if (templateNameList.Contains(templateInfo.TemplateName))
		        {
		            continue;
		        }
		        var insertedTemplateId = DataProvider.TemplateRepository.InsertAsync(templateInfo, string.Empty, AuthRequest.AdminName).GetAwaiter().GetResult();
                var childChannelIdList = ChannelManager.GetChannelIdListAsync(ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult(), EScopeType.Descendant, string.Empty, string.Empty, string.Empty).GetAwaiter().GetResult();
                foreach (var childChannelId in childChannelIdList)
		        {
		            var childChannelInfo = ChannelManager.GetChannelAsync(SiteId, childChannelId).GetAwaiter().GetResult();
                    childChannelInfo.ContentTemplateId = insertedTemplateId;
		            DataProvider.ChannelRepository.UpdateContentTemplateIdAsync(childChannelInfo).GetAwaiter().GetResult();

                    //TemplateManager.UpdateContentTemplateId(SiteId, childChannelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(childChannelId, insertedTemplateID);
                }
		    }

		    AuthRequest.AddSiteLogAsync(SiteId, "生成并匹配下级内容模版", $"栏目:{GetNodeNames()}").GetAwaiter().GetResult();
					
		    SuccessMessage("生成下级内容模版并匹配成功！");
                    
		    BindListBox();
		}
	}
}
