using System.Collections;
using BaiRong.Core;

namespace SiteServer.CMS.Core
{
	public class ChannelUtility
	{
        private ChannelUtility()
		{
		}

		public static bool IsAncestorOrSelf(int publishmentSystemID, int parentID, int childID)
		{
			if (parentID == childID)
			{
				return true;
			}
			var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemID, childID);
			if (nodeInfo == null)
			{
				return false;
			}
			if (CompareUtils.Contains(nodeInfo.ParentsPath, parentID.ToString()))
			{
				return true;
			}
			return false;
		}

		public static ArrayList GetParentsNodeIDArrayList(string parentsPath)
		{
			var arraylist = new ArrayList();
			if (!string.IsNullOrEmpty(parentsPath))
			{
				var nodeIDStrArray = parentsPath.Split(',');
				if (nodeIDStrArray != null && nodeIDStrArray.Length > 0)
				{
					foreach (var nodeIDStr in nodeIDStrArray)
					{
						var nodeID = TranslateUtils.ToInt(nodeIDStr.Trim());
						if (nodeID != 0)
						{
							arraylist.Add(nodeID);
						}
					}
				}
			}
			return arraylist;
		}
	}
}
