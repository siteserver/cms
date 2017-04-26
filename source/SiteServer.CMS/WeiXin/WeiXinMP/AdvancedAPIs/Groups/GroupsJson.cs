using System.Collections.Generic;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult;

namespace SiteServer.CMS.WeiXin.WeiXinMP.AdvancedAPIs.Groups
{
    public class GroupsJson : WxJsonResult
    {
        public List<GroupsJson_Group> groups { get; set; }
    }

    public class GroupsJson_Group
    {
        public int id { get; set; }
        public string name { get; set; }
        /// <summary>
        /// 此属性在CreateGroupResult的Json数据中，创建结果中始终为0
        /// </summary>
        public int count { get; set; }
    }
}
