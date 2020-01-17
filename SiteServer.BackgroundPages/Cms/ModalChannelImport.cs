using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalChannelImport : BasePageCms
    {
        protected DropDownList DdlParentChannelId;
		public HtmlInputFile HifFile;
		public DropDownList DdlIsOverride;

        private bool[] _isLastNodeArray;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScript("导入栏目",
                PageUtils.GetCmsUrl(siteId, nameof(ModalChannelImport), new NameValueCollection
                {
                    {"channelId", channelId.ToString()}
                }), 600, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            var channelId = AuthRequest.GetQueryInt("channelId", SiteId);
            var channelIdList = ChannelManager.GetChannelIdListAsync(SiteId).GetAwaiter().GetResult();
            var nodeCount = channelIdList.Count;
            _isLastNodeArray = new bool[nodeCount];
            foreach (var theChannelId in channelIdList)
            {
                var nodeInfo = ChannelManager.GetChannelAsync(SiteId, theChannelId).GetAwaiter().GetResult();
                var itemChannelId = nodeInfo.Id;
                var nodeName = nodeInfo.ChannelName;
                var parentsCount = nodeInfo.ParentsCount;
                var value = IsOwningChannelId(itemChannelId) ? itemChannelId.ToString() : string.Empty;
                if (!string.IsNullOrEmpty(value))
                {
                    if (!HasChannelPermissions(theChannelId, Constants.ChannelPermissions.ChannelAdd))
                    {
                        value = string.Empty;
                    }
                }
                var listitem = new ListItem(GetTitle(itemChannelId, nodeName, parentsCount), value);
                if (itemChannelId == channelId)
                {
                    listitem.Selected = true;
                }
                DdlParentChannelId.Items.Add(listitem);
            }
        }

        public string GetTitle(int channelId, string nodeName, int parentsCount)
        {
            var str = "";

            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, _isLastNodeArray[i] ? "　" : "│");
            }
            str = string.Concat(str, nodeName);
            return str;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (HifFile.PostedFile != null && "" != HifFile.PostedFile.FileName)
			{
				var filePath = HifFile.PostedFile.FileName;
                if (!EFileSystemTypeUtils.IsZip(PathUtils.GetExtension(filePath)))
				{
                    FailMessage("必须上传Zip压缩文件");
					return;
				}

				try
				{
                    var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                    HifFile.PostedFile.SaveAs(localFilePath);

					var importObject = new ImportObject(SiteId, AuthRequest.AdminName);
                    importObject.ImportChannelsAndContentsByZipFileAsync(TranslateUtils.ToInt(DdlParentChannelId.SelectedValue), localFilePath, TranslateUtils.ToBool(DdlIsOverride.SelectedValue)).GetAwaiter().GetResult();

                    AuthRequest.AddSiteLogAsync(SiteId, "导入栏目").GetAwaiter().GetResult();

                    LayerUtils.Close(Page);
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "导入栏目失败！");
				}
			}
		}
	}
}
