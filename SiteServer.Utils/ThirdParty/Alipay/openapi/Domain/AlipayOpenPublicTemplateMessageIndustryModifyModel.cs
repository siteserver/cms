using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicTemplateMessageIndustryModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicTemplateMessageIndustryModifyModel : AopObject
    {
        /// <summary>
        /// 服务窗消息模板所属主行业一/二级编码
        /// </summary>
        [XmlElement("primary_industry_code")]
        public string PrimaryIndustryCode { get; set; }

        /// <summary>
        /// 服务窗消息模板所属主行业一/二级名称(参数说明如下：)  |主行业| 副行业 |代码|  IT科技/互联网|电子商务 10001/20101  IT科技/IT软件与服务 10001/20102  IT科技/IT软件与设备 10001/20103  IT科技/电子技术 10001/20104  IT科技/通信与运营商 10001/20105  IT科技/网络游戏 10001/20106  金融业/银行 10002/20201  金融业/证券|基金|理财|信托 10002/20202  金融业/保险 10002/20203  餐饮/餐饮 10003/20301  酒店旅游/酒店 10004/20401  酒店旅游/旅游 10004/20401  运输与仓储/快递 10005/20501  运输与仓储/物流 10005/20501  运输与仓储/仓储 10005/20501  教育/培训 10006/20601  教育/院校 10006/20602  政府与公共事业/学术科研 10007/20701  政府与公共事业/交警 10007/20702  政府与公共事业/博物馆 10007/20703  政府与公共事业/政府公共事业非盈利机构 10007/20704  医药护理/医药医疗 10008/20801  医药护理/护理美容 10008/20802  医药护理/保健与卫生 10008/20803  交通工具/汽车相关 10009/20901  交通工具/摩托车相关 10009/20902  交通工具/火车相关 10009/20903  交通工具/飞机相关 10009/20904  房地产/房地产|建筑 10010/21001  房地产/物业 10010/21002  消费品/消费品 10011/21101  商业服务/法律 10012/21201  商业服务/广告会展 10012/21201  商业服务/中介服务 10012/21202  商业服务/检测|认证 10012/21203  商业服务/会计|审计 10012/21204  文体娱乐/文化|传媒 10013/21301  文体娱乐/体育 10013/21302  文体娱乐/娱乐休闲 10013/21303  印刷/打印|印刷 10014/21401  其它/其它 10015/21501
        /// </summary>
        [XmlElement("primary_industry_name")]
        public string PrimaryIndustryName { get; set; }

        /// <summary>
        /// 服务窗消息模板所属副行业一/二级编码
        /// </summary>
        [XmlElement("secondary_industry_code")]
        public string SecondaryIndustryCode { get; set; }

        /// <summary>
        /// 服务窗消息模板所属副行业一/二级名称
        /// </summary>
        [XmlElement("secondary_industry_name")]
        public string SecondaryIndustryName { get; set; }
    }
}
