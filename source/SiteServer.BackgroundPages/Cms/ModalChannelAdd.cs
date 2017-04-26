using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalChannelAdd : BasePageCms
    {
        public HtmlControl divSelectChannel;
        public Literal ltlSelectChannelScript;
        public DropDownList ContentModelID;
        public TextBox NodeNames;

        public CheckBox ckNameToIndex;
        public DropDownList ChannelTemplateID;
        public DropDownList ContentTemplateID;

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

            if (!IsPostBack)
            {
                ContentModelID.Items.Add(new ListItem("<<与父栏目相同>>", string.Empty));
                var contentModelInfoList = ContentModelManager.GetContentModelInfoList(PublishmentSystemInfo);
                foreach (var modelInfo in contentModelInfoList)
                {
                    ContentModelID.Items.Add(new ListItem(modelInfo.ModelName, modelInfo.ModelId));
                }

                ChannelTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);
                ContentTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);

                ChannelTemplateID.DataBind();
                ChannelTemplateID.Items.Insert(0, new ListItem("<未设置>", "0"));
                ChannelTemplateID.Items[0].Selected = true;
                ContentTemplateID.DataBind();
                ContentTemplateID.Items.Insert(0, new ListItem("<未设置>", "0"));
                ContentTemplateID.Items[0].Selected = true;

                divSelectChannel.Attributes.Add("onclick", ModalChannelSelect.GetOpenWindowString(PublishmentSystemId));
                ltlSelectChannelScript.Text =
                    $@"<script>selectChannel('{NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeId)}', '{nodeId}');</script>";
            }
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
                if (string.IsNullOrEmpty(NodeNames.Text))
                {
                    FailMessage("请填写需要添加的栏目名称");
                    return;
                }

                var insertedNodeIdHashtable = new Hashtable();//key为栏目的级别，1为第一级栏目
                insertedNodeIdHashtable[1] = parentNodeId;

                var nodeNameArray = NodeNames.Text.Split('\n');
                List<string> nodeIndexNameList = null;
                foreach (var item in nodeNameArray)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        //count为栏目的级别
                        var count = (StringUtils.GetStartCount('－', item) == 0) ? StringUtils.GetStartCount('-', item) : StringUtils.GetStartCount('－', item);
                        var nodeName = item.Substring(count, item.Length - count);
                        var nodeIndex = string.Empty;
                        count++;

                        if (!string.IsNullOrEmpty(nodeName) && insertedNodeIdHashtable.Contains(count))
                        {
                            if (ckNameToIndex.Checked)
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
                            var contentModelId = ContentModelID.SelectedValue;
                            if (string.IsNullOrEmpty(contentModelId))
                            {
                                var parentNodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, parentId);
                                contentModelId = parentNodeInfo.ContentModelId;
                            }

                            var channelTemplateId = TranslateUtils.ToInt(ChannelTemplateID.SelectedValue);
                            var contentTemplateId = TranslateUtils.ToInt(ContentTemplateID.SelectedValue);

                            var insertedNodeId = DataProvider.NodeDao.InsertNodeInfo(PublishmentSystemId, parentId, nodeName, nodeIndex, contentModelId, channelTemplateId, contentTemplateId);
                            insertedNodeIdHashtable[count + 1] = insertedNodeId;

                            CreateManager.CreateChannel(PublishmentSystemId, insertedNodeId);
                        }
                    }
                }

                Body.AddSiteLog(PublishmentSystemId, parentNodeId, 0, "快速添加栏目", $"父栏目:{NodeManager.GetNodeName(PublishmentSystemId, parentNodeId)},栏目:{NodeNames.Text.Replace('\n', ',')}");

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
