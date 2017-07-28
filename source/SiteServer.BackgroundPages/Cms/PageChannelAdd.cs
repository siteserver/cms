using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageChannelAdd : BasePageCms
    {
        public DropDownList ParentNodeID;
        public TextBox NodeName;
        public TextBox NodeIndexName;
        public DropDownList ContentModelID;
        public TextBox LinkURL;
        public CheckBoxList NodeGroupNameCollection;
        public DropDownList LinkType;
        public DropDownList ChannelTemplateID;
        public DropDownList ContentTemplateID;
        public RadioButtonList IsChannelAddable;
        public RadioButtonList IsContentAddable;
        public TextBox NavigationPicPath;
        public TextBox FilePath;
        public TextBox ChannelFilePathRule;
        public TextBox ContentFilePathRule;

        public TextEditorControl Content;
        public TextBox Keywords;
        public TextBox Description;

        public ChannelAuxiliaryControl channelControl;

        public Button CreateChannelRule;
        public Button CreateContentRule;
        public Button SelectImage;
        public Button UploadImage;

        private int _nodeId;
        private string _returnUrl;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageChannelAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString() },
                {"NodeID", nodeId.ToString() },
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl) }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ReturnUrl");
            _nodeId = Body.GetQueryInt("NodeID");
            _returnUrl = StringUtils.ValueFromUrl(PageUtils.FilterSqlAndXss(Body.GetQueryString("ReturnUrl")));
            //if (!base.HasChannelPermissions(this.nodeID, AppManager.CMS.Permission.Channel.ChannelAdd))
            //{
            //    PageUtils.RedirectToErrorPage("您没有添加栏目的权限！");
            //    return;
            //}

            var parentNodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            if (parentNodeInfo.Additional.IsChannelAddable == false)
            {
                PageUtils.RedirectToErrorPage("此栏目不能添加子栏目！");
                return;
            }

            channelControl = (ChannelAuxiliaryControl)FindControl("ControlForAuxiliary");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "添加栏目", string.Empty);

                NodeManager.AddListItems(ParentNodeID.Items, PublishmentSystemInfo, true, true, true, Body.AdministratorName);
                ControlUtils.SelectListItems(ParentNodeID, _nodeId.ToString());

                var contentModelInfoList = ContentModelManager.GetContentModelInfoList(PublishmentSystemInfo);
                foreach (var modelInfo in contentModelInfoList)
                {
                    ContentModelID.Items.Add(new ListItem(modelInfo.ModelName, modelInfo.ModelId));
                }
                ControlUtils.SelectListItems(ContentModelID, parentNodeInfo.ContentModelId);

                channelControl.SetParameters(null, false, IsPostBack);

                NavigationPicPath.Attributes.Add("onchange", GetShowImageScript("preview_NavigationPicPath", PublishmentSystemInfo.PublishmentSystemUrl));

                var showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, true, ChannelFilePathRule.ClientID);
                CreateChannelRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, false, ContentFilePathRule.ClientID);
                CreateContentRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, NavigationPicPath.ClientID);
                SelectImage.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalUploadImage.GetOpenWindowString(PublishmentSystemId, NavigationPicPath.ClientID);
                UploadImage.Attributes.Add("onclick", showPopWinString);

                IsChannelAddable.Items[0].Value = true.ToString();
                IsChannelAddable.Items[1].Value = false.ToString();
                IsContentAddable.Items[0].Value = true.ToString();
                IsContentAddable.Items[1].Value = false.ToString();

                ELinkTypeUtils.AddListItems(LinkType);

                NodeGroupNameCollection.DataSource = DataProvider.NodeGroupDao.GetDataSource(PublishmentSystemId);
                ChannelTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);
                ContentTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);

                DataBind();

                ChannelTemplateID.Items.Insert(0, new ListItem("<未设置>", "0"));
                ChannelTemplateID.Items[0].Selected = true;

                ContentTemplateID.Items.Insert(0, new ListItem("<未设置>", "0"));
                ContentTemplateID.Items[0].Selected = true;
                Content.SetParameters(PublishmentSystemInfo, NodeAttribute.Content, null, false, IsPostBack);
            }
            else
            {
                channelControl.SetParameters(Request.Form, false, IsPostBack);
            }
        }

        public void ParentNodeID_SelectedIndexChanged(object sender, EventArgs e)
        {
            var theNodeId = TranslateUtils.ToInt(ParentNodeID.SelectedValue);
            if (theNodeId == 0)
            {
                theNodeId = _nodeId;
            }
            PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId, theNodeId, Body.GetQueryString("ReturnUrl")));
        }

        public string PreviewImageSrc
        {
            get
            {
                if (!string.IsNullOrEmpty(NavigationPicPath.Text))
                {
                    var extension = PathUtils.GetExtension(NavigationPicPath.Text);
                    if (EFileSystemTypeUtils.IsImage(extension))
                    {
                        return PageUtility.ParseNavigationUrl(PublishmentSystemInfo, NavigationPicPath.Text);
                    }
                    else if (EFileSystemTypeUtils.IsFlash(extension))
                    {
                        return SiteServerAssets.GetIconUrl("flash.jpg");
                    }
                    else if (EFileSystemTypeUtils.IsPlayer(extension))
                    {
                        return SiteServerAssets.GetIconUrl("player.gif");
                    }
                }
                return SiteServerAssets.GetIconUrl("empty.gif");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                int insertNodeId;
                try
                {
                    var nodeId = Body.GetQueryInt("NodeID");
                    var nodeInfo = new NodeInfo
                    {
                        ParentId = nodeId,
                        ContentModelId = ContentModelID.SelectedValue
                    };

                    if (NodeIndexName.Text.Length != 0)
                    {
                        var nodeIndexNameArrayList = DataProvider.NodeDao.GetNodeIndexNameList(PublishmentSystemId);
                        if (nodeIndexNameArrayList.IndexOf(NodeIndexName.Text) != -1)
                        {
                            FailMessage("栏目添加失败，栏目索引已存在！");
                            return;
                        }
                    }

                    if (FilePath.Text.Length != 0)
                    {
                        if (!DirectoryUtils.IsDirectoryNameCompliant(FilePath.Text))
                        {
                            FailMessage("栏目页面路径不符合系统要求！");
                            return;
                        }

                        if (PathUtils.IsDirectoryPath(FilePath.Text))
                        {
                            FilePath.Text = PageUtils.Combine(FilePath.Text, "index.html");
                        }

                        var filePathArrayList = DataProvider.NodeDao.GetAllFilePathByPublishmentSystemId(PublishmentSystemId);
                        if (filePathArrayList.IndexOf(FilePath.Text) != -1)
                        {
                            FailMessage("栏目添加失败，栏目页面路径已存在！");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(ChannelFilePathRule.Text))
                    {
                        if (!DirectoryUtils.IsDirectoryNameCompliant(ChannelFilePathRule.Text))
                        {
                            FailMessage("栏目页面命名规则不符合系统要求！");
                            return;
                        }
                        if (PathUtils.IsDirectoryPath(ChannelFilePathRule.Text))
                        {
                            FailMessage("栏目页面命名规则必须包含生成文件的后缀！");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(ContentFilePathRule.Text))
                    {
                        if (!DirectoryUtils.IsDirectoryNameCompliant(ContentFilePathRule.Text))
                        {
                            FailMessage("内容页面命名规则不符合系统要求！");
                            return;
                        }
                        if (PathUtils.IsDirectoryPath(ContentFilePathRule.Text))
                        {
                            FailMessage("内容页面命名规则必须包含生成文件的后缀！");
                            return;
                        }
                    }

                    var extendedAttributes = new ExtendedAttributes();
                    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeId);
                    InputTypeParser.AddValuesToAttributes(ETableStyle.Channel, DataProvider.NodeDao.TableName, PublishmentSystemInfo, relatedIdentities, Request.Form, extendedAttributes.Attributes);
                    var attributes = extendedAttributes.Attributes;
                    foreach (string key in attributes)
                    {
                        nodeInfo.Additional.SetExtendedAttribute(key, attributes[key]);
                    }

                    nodeInfo.NodeName = NodeName.Text;
                    nodeInfo.NodeIndexName = NodeIndexName.Text;
                    nodeInfo.FilePath = FilePath.Text;
                    nodeInfo.ChannelFilePathRule = ChannelFilePathRule.Text;
                    nodeInfo.ContentFilePathRule = ContentFilePathRule.Text;

                    var list = new ArrayList();
                    foreach (ListItem item in NodeGroupNameCollection.Items)
                    {
                        if (item.Selected)
                        {
                            list.Add(item.Value);
                        }
                    }
                    nodeInfo.NodeGroupNameCollection = TranslateUtils.ObjectCollectionToString(list);
                    nodeInfo.ImageUrl = NavigationPicPath.Text;
                    nodeInfo.Content = StringUtility.TextEditorContentEncode(Request.Form[NodeAttribute.Content], PublishmentSystemInfo, PublishmentSystemInfo.Additional.IsSaveImageInTextEditor);
                    nodeInfo.Keywords = Keywords.Text;
                    nodeInfo.Description = Description.Text;
                    nodeInfo.Additional.IsChannelAddable = TranslateUtils.ToBool(IsChannelAddable.SelectedValue);
                    nodeInfo.Additional.IsContentAddable = TranslateUtils.ToBool(IsContentAddable.SelectedValue);

                    nodeInfo.LinkUrl = LinkURL.Text;
                    nodeInfo.LinkType = ELinkTypeUtils.GetEnumType(LinkType.SelectedValue);
                    nodeInfo.ChannelTemplateId = (ChannelTemplateID.Items.Count > 0) ? int.Parse(ChannelTemplateID.SelectedValue) : 0;
                    nodeInfo.ContentTemplateId = (ContentTemplateID.Items.Count > 0) ? int.Parse(ContentTemplateID.SelectedValue) : 0;

                    nodeInfo.AddDate = DateTime.Now;
                    insertNodeId = DataProvider.NodeDao.InsertNodeInfo(nodeInfo);
                    //栏目选择投票样式后，内容
                }
                catch (Exception ex)
                {
                    FailMessage(ex, $"栏目添加失败：{ex.Message}");
                    LogUtils.AddErrorLog(ex);
                    return;
                }

                CreateManager.CreateChannel(PublishmentSystemId, insertNodeId);

                Body.AddSiteLog(PublishmentSystemId, "添加栏目", $"栏目:{NodeName.Text}");

                SuccessMessage("栏目添加成功！");
                AddWaitAndRedirectScript(_returnUrl);
            }
        }

        public string ReturnUrl => _returnUrl;
    }
}
