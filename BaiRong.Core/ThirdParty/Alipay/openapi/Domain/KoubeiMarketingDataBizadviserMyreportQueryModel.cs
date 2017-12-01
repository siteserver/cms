using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingDataBizadviserMyreportQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingDataBizadviserMyreportQueryModel : AopObject
    {
        /// <summary>
        /// 非必须参数，uniq_key不同，参数也不同：  参数格式如下：  [  {"paramKey": " campStatus ","paramValue": " campStatusValue"}  ]  参数组织方式根据不同的uniqkey进行，规则如下  1、activityList：campStatus ，campStatus值为PROCESSING  2、activityDetail：campId，campId值需要传要查询的活动ID  3、activityTop11ShopList：campId，campId值需要传要查询的活动ID  4、activityTradeTrend：campId，campId值需要传要查询的活动ID  5、activityShopList：campId，campId值需要传要查询的活动ID  6、codeSingleShopInfo：shopId，shopId店铺ID值  7、codeSingleShopTrend：shopId，shopId店铺ID值  7、cardMemberBigData：dimension  Dimension 对应关系  1- 性别；2-年龄；3-是否学生；4-是否有小孩；5-消费频率；6-消费金额；7-笔单价；  没有在此列举的uniq_key表示无需传入此参数；
        /// </summary>
        [XmlElement("req_parameters")]
        public string ReqParameters { get; set; }

        /// <summary>
        /// uniq_key 是每次请求不同数据需要传不同的value，具体区别如下：  当uniq_key 为cardOperateSum时询经营分析卡片总数据；  cardActivitySum时查营销活动总数据；  cardCodeSum时查营销 商户昨日扫码数据；  cardOperateTrend时查经营分析趋势信息；  cardOperateCodeTrend时查营销 商户扫码近期趋势 数据；  activityList时查 活动交易明细列表 数据；  activityDetail时查 活动交易明细查询 数据；  activityTop11ShopList时查 近30天门店交易额排名Top11 数据；  activityTradeTrend时查单个活动金额趋势图 数据；  tradeShopRank时查 近30天门店交易额排名 数据；  activityShopList时查 门店活动交易明细列表 数据；  codeAllShopInfo时查 全部门店 活跃门店码数，活跃桌码数，到店非会员数数据；  codeAllShopTrend时查 全部门店 扫码近期趋势 扫码次数、优惠领券、买单次数数据；  codeSingleShopInfo时查单个门店 活跃门店码数，活跃桌码数，到店非会员数数据；  codeSingleShopTrend时查单个门店 扫码近期趋势 扫码次数、优惠领券、买单次数数据；  tradeTop11ShopList时查 近30天门店交易额排名Top11 数据；  tradeUnitPrice时查 消费笔单价分布 数据；  tradeDayTrend时查 按日趋势 数据；  tradeWeekTrend时查按周趋势 数据；  tradeMonthTrend时查【交易】按月趋势 数据；  memberPreTradeCnt时查【会员】上月交易笔数 数据；  cardMemberBigData时查会员大数据 数据；  cardMemberSum时查会员总；  cardMemberTotalMember时查会员数据汇总；  cardMemberClassify时查会员分层数据；  memberCurTradeCnt时查当月交易笔数 数据；  memberHierarchical时查商户会员分层查询；  memberDefaultShop时查商户门店数  loginCount时查询登录次数；
        /// </summary>
        [XmlElement("uniq_key")]
        public string UniqKey { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
