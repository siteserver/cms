namespace SiteServer.CMS.Core
{
	public class SourceManager
	{
        public const int CaiJi = -2;        //采集
        public const int Preview = -99;     //预览
        public const int Default = 0;       //正常录入

        public static string GetSourceName(int sourceId)
        {
            if (sourceId == Default)
            {
                return "正常录入";
            }
            if (sourceId == CaiJi)
            {
                return "系统采集";
            }
            if (sourceId == Preview)
            {
                return "预览插入";
            }
            if (sourceId <= 0) return string.Empty;

            var publishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(sourceId);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo == null) return "内容转移";

            var nodeNames = NodeManager.GetNodeNameNavigation(publishmentSystemId, sourceId);
            if (!string.IsNullOrEmpty(nodeNames))
            {
                return publishmentSystemInfo.PublishmentSystemName + "：" + nodeNames;
            }
            return publishmentSystemInfo.PublishmentSystemName;
        }
	}
}
