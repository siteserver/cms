using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovInteractChannelAdd : BasePageCms
	{
        public TextBox ChannelName;
        public PlaceHolder phParentID;
        public DropDownList ParentID;
        public TextBox Summary;

        private int _channelId;
        private string _returnUrl = string.Empty;
        private bool[] _isLastNodeArray;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("添加节点", PageUtils.GetWcmUrl(nameof(ModalGovInteractChannelAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), 460, 300);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int channelId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("修改节点", PageUtils.GetWcmUrl(nameof(ModalGovInteractChannelAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ChannelID", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), 460, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _channelId = TranslateUtils.ToInt(Request.QueryString["ChannelID"]);
            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["ReturnUrl"]);
            if (string.IsNullOrEmpty(_returnUrl))
            {
                _returnUrl = PageGovInteractChannel.GetRedirectUrl(PublishmentSystemId);
            }

			if (!IsPostBack)
			{
                if (_channelId == 0)
                {
                    ParentID.Items.Add(new ListItem("<无上级节点>", "0"));

                    var nodeIdList = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, PublishmentSystemInfo.Additional.GovInteractNodeId);
                    var count = nodeIdList.Count;
                    _isLastNodeArray = new bool[count + 100];
                    foreach (var theChannelId in nodeIdList)
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theChannelId);
                        var listitem = new ListItem(GetTitle(nodeInfo), theChannelId.ToString());
                        ParentID.Items.Add(listitem);
                    }
                }
                else
                {
                    phParentID.Visible = false;
                }

                if (_channelId != 0)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _channelId);

                    ChannelName.Text = nodeInfo.NodeName;
                    ParentID.SelectedValue = nodeInfo.ParentId.ToString();
                    var channelInfo = DataProvider.GovInteractChannelDao.GetChannelInfo(PublishmentSystemId, _channelId);
                    if (channelInfo != null)
                    {
                        Summary.Text = channelInfo.Summary;
                    }
                }
			}
		}

        public string GetTitle(NodeInfo nodeInfo)
        {
            var str = "";
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
            str = string.Concat(str, nodeInfo.NodeName);
            return str;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                if (_channelId == 0)
                {
                    var parentId = TranslateUtils.ToInt(ParentID.SelectedValue);
                    if (parentId == 0)
                    {
                        parentId = PublishmentSystemInfo.Additional.GovInteractNodeId;
                    }
                    var nodeId = DataProvider.NodeDao.InsertNodeInfo(PublishmentSystemId, parentId, ChannelName.Text, string.Empty, EContentModelTypeUtils.GetValue(EContentModelType.GovInteract));

                    var channelInfo = new GovInteractChannelInfo(nodeId, PublishmentSystemId, 0, 0, string.Empty, Summary.Text);

                    DataProvider.GovInteractChannelDao.Insert(channelInfo);
                }
                else
                {
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _channelId);
                    nodeInfo.NodeName = ChannelName.Text;
                    DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                    var channelInfo = DataProvider.GovInteractChannelDao.GetChannelInfo(PublishmentSystemId, _channelId);
                    if (channelInfo == null)
                    {
                        channelInfo = new GovInteractChannelInfo(_channelId, PublishmentSystemId, 0, 0, string.Empty, Summary.Text);
                        DataProvider.GovInteractChannelDao.Insert(channelInfo);
                    }
                    else
                    {
                        channelInfo.Summary = Summary.Text;
                        DataProvider.GovInteractChannelDao.Update(channelInfo);
                    }
                }

                Body.AddAdminLog("维护分类信息");

                SuccessMessage("分类设置成功！");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "分类设置失败！");
            }

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
            }
        }
	}
}
