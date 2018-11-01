using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalChannelsAdd : BasePageCms
    {
        public HyperLink HlSelectChannel;
        public Literal LtlSelectChannelScript;
        public DropDownList DdlContentModelPluginId;
        public PlaceHolder PhContentRelatedPluginIds;
        public CheckBoxList CblContentRelatedPluginIds;
        public TextBox TbNodeNames;

        public CheckBox CbIsNameToIndex;
        public DropDownList DdlChannelTemplateId;
        public DropDownList DdlContentTemplateId;

        private string _returnUrl;

        public static string GetOpenWindowString(int siteId, int channelId, string returnUrl)
        {
            return LayerUtils.GetOpenScript("添加栏目",
                PageUtils.GetCmsUrl(siteId, nameof(ModalChannelsAdd), new NameValueCollection
                {
                    {"channelId", channelId.ToString()},
                    {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
                }));
        }

        public static string GetRedirectUrl(int siteId, int channelId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalChannelsAdd), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId", "ReturnUrl");

            var channelId = AuthRequest.GetQueryInt("channelId");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));

            if (IsPostBack) return;

            DdlContentModelPluginId.Items.Add(new ListItem("<与父栏目相同>", string.Empty));
            var contentTables = PluginContentManager.GetContentModelPlugins();
            foreach (var contentTable in contentTables)
            {
                DdlContentModelPluginId.Items.Add(new ListItem(contentTable.Title, contentTable.Id));
            }

            var plugins = PluginContentManager.GetAllContentRelatedPlugins(false);
            if (plugins.Count > 0)
            {
                foreach (var pluginMetadata in plugins)
                {
                    CblContentRelatedPluginIds.Items.Add(new ListItem(pluginMetadata.Title, pluginMetadata.Id));
                }
            }
            else
            {
                PhContentRelatedPluginIds.Visible = false;
            }

            DdlChannelTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(SiteId, TemplateType.ChannelTemplate);
            DdlContentTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(SiteId, TemplateType.ContentTemplate);

            DdlChannelTemplateId.DataBind();
            DdlChannelTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
            DdlChannelTemplateId.Items[0].Selected = true;
            DdlContentTemplateId.DataBind();
            DdlContentTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
            DdlContentTemplateId.Items[0].Selected = true;

            HlSelectChannel.Attributes.Add("onclick", ModalChannelSelect.GetOpenWindowString(SiteId));
            LtlSelectChannelScript.Text =
                $@"<script>selectChannel('{ChannelManager.GetChannelNameNavigation(SiteId, channelId)}', '{channelId}');</script>";
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            bool isChanged;
            var parentChannelId = TranslateUtils.ToInt(Request.Form["channelId"]);
            if (parentChannelId == 0)
            {
                parentChannelId = SiteId;
            }

            try
            {
                if (string.IsNullOrEmpty(TbNodeNames.Text))
                {
                    FailMessage("请填写需要添加的栏目名称");
                    return;
                }

                var insertedChannelIdHashtable = new Hashtable {[1] = parentChannelId}; //key为栏目的级别，1为第一级栏目

                var nodeNameArray = TbNodeNames.Text.Split('\n');
                List<string> nodeIndexNameList = null;
                foreach (var item in nodeNameArray)
                {
                    if (string.IsNullOrEmpty(item)) continue;

                    //count为栏目的级别
                    var count = (StringUtils.GetStartCount('－', item) == 0) ? StringUtils.GetStartCount('-', item) : StringUtils.GetStartCount('－', item);
                    var nodeName = item.Substring(count, item.Length - count);
                    var nodeIndex = string.Empty;
                    count++;

                    if (!string.IsNullOrEmpty(nodeName) && insertedChannelIdHashtable.Contains(count))
                    {
                        if (CbIsNameToIndex.Checked)
                        {
                            nodeIndex = nodeName.Trim();
                        }

                        if (StringUtils.Contains(nodeName, "(") && StringUtils.Contains(nodeName, ")"))
                        {
                            var length = nodeName.IndexOf(')') - nodeName.IndexOf('(');
                            if (length > 0)
                            {
                                nodeIndex = nodeName.Substring(nodeName.IndexOf('(') + 1, length);
                                nodeName = nodeName.Substring(0, nodeName.IndexOf('('));
                            }
                        }
                        nodeName = nodeName.Trim();
                        nodeIndex = nodeIndex.Trim(' ', '(', ')');
                        if (!string.IsNullOrEmpty(nodeIndex))
                        {
                            if (nodeIndexNameList == null)
                            {
                                nodeIndexNameList = DataProvider.ChannelDao.GetIndexNameList(SiteId);
                            }
                            if (nodeIndexNameList.IndexOf(nodeIndex) != -1)
                            {
                                nodeIndex = string.Empty;
                            }
                            else
                            {
                                nodeIndexNameList.Add(nodeIndex);
                            }
                        }

                        var parentId = (int)insertedChannelIdHashtable[count];
                        var contentModelPluginId = DdlContentModelPluginId.SelectedValue;
                        if (string.IsNullOrEmpty(contentModelPluginId))
                        {
                            var parentNodeInfo = ChannelManager.GetChannelInfo(SiteId, parentId);
                            contentModelPluginId = parentNodeInfo.ContentModelPluginId;
                        }

                        var channelTemplateId = TranslateUtils.ToInt(DdlChannelTemplateId.SelectedValue);
                        var contentTemplateId = TranslateUtils.ToInt(DdlContentTemplateId.SelectedValue);

                        var insertedChannelId = DataProvider.ChannelDao.Insert(SiteId, parentId, nodeName, nodeIndex, contentModelPluginId, ControlUtils.GetSelectedListControlValueCollection(CblContentRelatedPluginIds), channelTemplateId, contentTemplateId);
                        insertedChannelIdHashtable[count + 1] = insertedChannelId;

                        CreateManager.CreateChannel(SiteId, insertedChannelId);
                    }
                }

                AuthRequest.AddSiteLog(SiteId, parentChannelId, 0, "快速添加栏目", $"父栏目:{ChannelManager.GetChannelName(SiteId, parentChannelId)},栏目:{TbNodeNames.Text.Replace('\n', ',')}");

                isChanged = true;
            }
            catch (Exception ex)
            {
                isChanged = false;
                FailMessage(ex, ex.Message);
            }

            if (isChanged)
            {
                LayerUtils.CloseAndRedirect(Page, _returnUrl);
            }
        }
    }
}
