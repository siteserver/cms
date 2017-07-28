using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalChannelImport : BasePageCms
    {
        protected DropDownList ParentNodeID;
		public HtmlInputFile myFile;
		public RadioButtonList IsOverride;

        bool[] _isLastNodeArray;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenLayerString("导入栏目",
                PageUtils.GetCmsUrl(nameof(ModalChannelImport), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()}
                }), 560, 260);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                var nodeId = Body.GetQueryInt("NodeID", PublishmentSystemId);
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
                var nodeCount = nodeIdList.Count;
                _isLastNodeArray = new bool[nodeCount];
                foreach (var theNodeId in nodeIdList)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theNodeId);
                    var itemNodeId = nodeInfo.NodeId;
                    var nodeName = nodeInfo.NodeName;
                    var parentsCount = nodeInfo.ParentsCount;
                    var isLastNode = nodeInfo.IsLastNode;
                    var value = IsOwningNodeId(itemNodeId) ? itemNodeId.ToString() : string.Empty;
                    value = (nodeInfo.Additional.IsChannelAddable) ? value : string.Empty;
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!HasChannelPermissions(theNodeId, AppManager.Cms.Permission.Channel.ChannelAdd))
                        {
                            value = string.Empty;
                        }
                    }
                    var listitem = new ListItem(GetTitle(itemNodeId, nodeName, parentsCount, isLastNode), value);
                    if (itemNodeId == nodeId)
                    {
                        listitem.Selected = true;
                    }
                    ParentNodeID.Items.Add(listitem);
                }
			}
		}

        public string GetTitle(int nodeId, string nodeName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (nodeId == PublishmentSystemId)
            {
                isLastNode = true;
            }
            if (isLastNode == false)
            {
                _isLastNodeArray[parentsCount] = false;
            }
            else
            {
                _isLastNodeArray[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, _isLastNodeArray[i] ? "　" : "│");
            }
            str = string.Concat(str, isLastNode ? "└" : "├");
            str = string.Concat(str, nodeName);
            return str;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
			{
				var filePath = myFile.PostedFile.FileName;
                if (!EFileSystemTypeUtils.IsCompressionFile(PathUtils.GetExtension(filePath)))
				{
                    FailMessage("必须上传压缩文件");
					return;
				}

				try
				{
                    var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

					myFile.PostedFile.SaveAs(localFilePath);

					var importObject = new ImportObject(PublishmentSystemId);
                    importObject.ImportChannelsAndContentsByZipFile(TranslateUtils.ToInt(ParentNodeID.SelectedValue), localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue));

                    Body.AddSiteLog(PublishmentSystemId, "导入栏目");

                    PageUtils.CloseModalPage(Page);
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "导入栏目失败！");
				}
			}
		}
	}
}
