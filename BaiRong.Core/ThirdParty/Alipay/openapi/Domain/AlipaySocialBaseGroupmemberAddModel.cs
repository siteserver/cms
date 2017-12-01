using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySocialBaseGroupmemberAddModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySocialBaseGroupmemberAddModel : AopObject
    {
        /// <summary>
        /// 加人的时候，是否需要校验加人者和被加的人的好友关系
        /// </summary>
        [XmlElement("friend_validate")]
        public bool FriendValidate { get; set; }

        /// <summary>
        /// 群的id
        /// </summary>
        [XmlElement("group_id")]
        public string GroupId { get; set; }

        /// <summary>
        /// 增加群成员的时候，选择的用户userid,每次不能超过50个，每个群人数上限500人，user_ids的值为错误的uid时，多个uid的情况下会添加成功正确的uid，如果所有添加的uid全部错误，则会报错
        /// </summary>
        [XmlArray("user_ids")]
        [XmlArrayItem("string")]
        public List<string> UserIds { get; set; }
    }
}
