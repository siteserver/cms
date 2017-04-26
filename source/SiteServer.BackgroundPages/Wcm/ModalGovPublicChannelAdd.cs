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
	public class ModalGovPublicChannelAdd : BasePageCms
	{
        public TextBox ChannelName;
        public TextBox ChannelCode;
        public PlaceHolder phParentID;
        public DropDownList ParentID;
        public TextBox Summary;

        private int _channelId;
        private string _returnUrl = string.Empty;
        private bool[] _isLastNodeArray;

	    public static string GetOpenWindowStringToAdd(int publishmentSystemId, string returnUrl)
	    {
	        return PageUtils.GetOpenWindowString("添加节点",
	            PageUtils.GetWcmUrl(nameof(ModalGovPublicChannelAdd), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()},
	                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
	            }), 460, 360);
	    }

	    public static string GetOpenWindowStringToEdit(int publishmentSystemId, int channelId, string returnUrl)
	    {
	        return PageUtils.GetOpenWindowString("修改节点",
	            PageUtils.GetWcmUrl(nameof(ModalGovPublicChannelAdd), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()},
	                {"ChannelID", channelId.ToString()},
	                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
	            }), 460, 360);
	    }

	    public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _channelId = TranslateUtils.ToInt(Request.QueryString["ChannelID"]);
            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["ReturnUrl"]);
            if (string.IsNullOrEmpty(_returnUrl))
            {
                _returnUrl = PageGovPublicChannel.GetRedirectUrl(PublishmentSystemId);
            }

			if (!IsPostBack)
			{
                if (_channelId == 0)
                {
                    ParentID.Items.Add(new ListItem("<无上级节点>", "0"));

                    var nodeIdList = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, PublishmentSystemInfo.Additional.GovPublicNodeId);
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
                    var channelInfo = DataProvider.GovPublicChannelDao.GetChannelInfo(_channelId);
                    if (channelInfo != null)
                    {
                        ChannelCode.Text = channelInfo.Code;
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
                        parentId = PublishmentSystemInfo.Additional.GovPublicNodeId;
                    }
                    var nodeId = DataProvider.NodeDao.InsertNodeInfo(PublishmentSystemId, parentId, ChannelName.Text, string.Empty, EContentModelTypeUtils.GetValue(EContentModelType.GovPublic));

                    var channelInfo = new GovPublicChannelInfo(nodeId, PublishmentSystemId, ChannelCode.Text, Summary.Text);

                    DataProvider.GovPublicChannelDao.Insert(channelInfo);
                }
                else
                {
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _channelId);
                    nodeInfo.NodeName = ChannelName.Text;
                    DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                    var channelInfo = DataProvider.GovPublicChannelDao.GetChannelInfo(_channelId);
                    if (channelInfo == null)
                    {
                        channelInfo = new GovPublicChannelInfo(_channelId, PublishmentSystemId, ChannelCode.Text, Summary.Text);
                        DataProvider.GovPublicChannelDao.Insert(channelInfo);
                    }
                    else
                    {
                        channelInfo.Code = ChannelCode.Text;
                        channelInfo.Summary = Summary.Text;
                        DataProvider.GovPublicChannelDao.Update(channelInfo);
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
