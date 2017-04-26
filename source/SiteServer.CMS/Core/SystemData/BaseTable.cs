using System;
using System.Collections;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core.SystemData
{
	/// <summary>
	/// AuxiliaryTable 的摘要说明。
	/// </summary>
	public class BaseTable
	{
		private BaseTable()
		{
		}

		public static ArrayList GetDefaultMenuDisplayArrayList(int publishmentSystemID)
		{
			var arraylist = new ArrayList();

			var menuDisplayInfo = new MenuDisplayInfo(0, publishmentSystemID, "系统菜单显示方式", "true", "", 12, "plain", "", "center", "middle", "#000000", "#F2F2F2", "#FFFFFF", "#CCCCCC", "-10", "20", "true", 115, 24, 0, 0, 0, 500, "true", 1, "#A8A8A8", "", "#A8A8A8", string.Empty, DateTime.Now, true, "系统菜单显示方式");
			arraylist.Add(menuDisplayInfo);

			return arraylist;
		}

        public static PublishmentSystemInfo GetDefaultPublishmentSystemInfo(string publishmentSystemName, EPublishmentSystemType publishmentSystemType, string auxiliaryTableForContent, string auxiliaryTableForGovPublic, string auxiliaryTableForGovInteract, string auxiliaryTableForVote, string auxiliaryTableForJob, string publishmentSystemDir, string publishmentSystemUrl, int parentPublishmentSystemID)
		{
            var psInfo = new PublishmentSystemInfo(0, publishmentSystemName, publishmentSystemType, auxiliaryTableForContent, auxiliaryTableForGovPublic, auxiliaryTableForGovInteract, auxiliaryTableForVote, auxiliaryTableForJob, false, 0, publishmentSystemDir, publishmentSystemUrl, false, parentPublishmentSystemID, 0, string.Empty);
			return psInfo;
		}


		/// <summary>
		/// 默认的发布系统内容审核次数
		/// </summary>
		public static int DefaultContentCheckNum => 1;


	    /// <summary>
		/// 默认的发布系统栏目审核次数
		/// </summary>
		public static int DefaultChannelCheckNum => 1;
	}
}
