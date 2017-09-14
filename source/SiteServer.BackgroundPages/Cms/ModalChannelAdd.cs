using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalChannelAdd : BasePageCms
    {
        public HyperLink HlSelectChannel;
        public Literal LtlSelectChannelScript;
        public DropDownList DdlContentModelId;
        public CheckBoxList CblPlugins;
        public PlaceHolder PhPlugins;
        public TextBox TbNodeNames;

        public CheckBox CbIsNameToIndex;
        public DropDownList DdlChannelTemplateId;
        public DropDownList DdlContentTemplateId;

        private string _returnUrl;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("添加栏目",
                PageUtils.GetCmsUrl(nameof(ModalChannelAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()},
                    {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
                }));
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(ModalChannelAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ReturnUrl");

            var nodeId = Body.GetQueryInt("NodeID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            if (IsPostBack) return;

            DdlContentModelId.Items.Add(new ListItem("<与父栏目相同>", string.Empty));
            var contentTables = PluginCache.GetEnabledPluginMetadatas<IContentTable>();
            foreach (var contentTable in contentTables)
            {
                DdlContentModelId.Items.Add(new ListItem($"插件：{contentTable.DisplayName}", contentTable.Id));
            }

            var pluginChannels = PluginCache.GetAllChannels(false);
            if (pluginChannels.Count > 0)
            {
                foreach (var pluginMetadata in pluginChannels)
                {
                    CblPlugins.Items.Add(new ListItem(pluginMetadata.DisplayName, pluginMetadata.Id));
                }
            }
            else
            {
                PhPlugins.Visible = false;
            }

            DdlChannelTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);
            DdlContentTemplateId.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);

            DdlChannelTemplateId.DataBind();
            DdlChannelTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
            DdlChannelTemplateId.Items[0].Selected = true;
            DdlContentTemplateId.DataBind();
            DdlContentTemplateId.Items.Insert(0, new ListItem("<默认>", "0"));
            DdlContentTemplateId.Items[0].Selected = true;

            HlSelectChannel.Attributes.Add("onclick", ModalChannelSelect.GetOpenWindowString(PublishmentSystemId));
            LtlSelectChannelScript.Text =
                $@"<script>selectChannel('{NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeId)}', '{nodeId}');</script>";
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            bool isChanged;
            var parentNodeId = TranslateUtils.ToInt(Request.Form["nodeID"]);
            if (parentNodeId == 0)
            {
                parentNodeId = PublishmentSystemId;
            }

            try
            {
                if (string.IsNullOrEmpty(TbNodeNames.Text))
                {
                    FailMessage("请填写需要添加的栏目名称");
                    return;
                }

                var insertedNodeIdHashtable = new Hashtable {[1] = parentNodeId}; //key为栏目的级别，1为第一级栏目

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

                    if (!string.IsNullOrEmpty(nodeName) && insertedNodeIdHashtable.Contains(count))
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
                                nodeIndexNameList = DataProvider.NodeDao.GetNodeIndexNameList(PublishmentSystemId);
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

                        var parentId = (int)insertedNodeIdHashtable[count];
                        var contentModelId = DdlContentModelId.SelectedValue;
                        if (string.IsNullOrEmpty(contentModelId))
                        {
                            var parentNodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, parentId);
                            contentModelId = parentNodeInfo.ContentModelId;
                        }

                        var channelTemplateId = TranslateUtils.ToInt(DdlChannelTemplateId.SelectedValue);
                        var contentTemplateId = TranslateUtils.ToInt(DdlContentTemplateId.SelectedValue);

                        var insertedNodeId = DataProvider.NodeDao.InsertNodeInfo(PublishmentSystemId, parentId, nodeName, nodeIndex, contentModelId, channelTemplateId, contentTemplateId, ControlUtils.GetSelectedListControlValueCollection(CblPlugins));
                        insertedNodeIdHashtable[count + 1] = insertedNodeId;

                        CreateManager.CreateChannel(PublishmentSystemId, insertedNodeId);
                    }
                }

                Body.AddSiteLog(PublishmentSystemId, parentNodeId, 0, "快速添加栏目", $"父栏目:{NodeManager.GetNodeName(PublishmentSystemId, parentNodeId)},栏目:{TbNodeNames.Text.Replace('\n', ',')}");

                isChanged = true;
            }
            catch (Exception ex)
            {
                isChanged = false;
                FailMessage(ex, ex.Message);
            }

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
            }
        }
    }
}
