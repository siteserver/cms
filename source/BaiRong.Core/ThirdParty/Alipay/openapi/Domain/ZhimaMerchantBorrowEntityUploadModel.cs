using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ZhimaMerchantBorrowEntityUploadModel Data Structure.
    /// </summary>
    [Serializable]
    public class ZhimaMerchantBorrowEntityUploadModel : AopObject
    {
        /// <summary>
        /// 地址描述
        /// </summary>
        [XmlElement("address_desc")]
        public string AddressDesc { get; set; }

        /// <summary>
        /// 是否可借用，Y:可借，N:不可借。如果不可借用，则不在芝麻借还频道地图展示
        /// </summary>
        [XmlElement("can_borrow")]
        public string CanBorrow { get; set; }

        /// <summary>
        /// 可借用数量，如果是借用实物，如自行车，传1即可。如果是借用门店或借还机柜，则传入可借用的物品数量
        /// </summary>
        [XmlElement("can_borrow_cnt")]
        public string CanBorrowCnt { get; set; }

        /// <summary>
        /// 类目Code，传入芝麻借还规定的类目Code，其他值会认为非法参数，参数值如下：  雨伞：umbrella   充电宝：power_bank
        /// </summary>
        [XmlElement("category_code")]
        public string CategoryCode { get; set; }

        /// <summary>
        /// 是否收租金，Y:收租金，N:不收租金
        /// </summary>
        [XmlElement("collect_rent")]
        public string CollectRent { get; set; }

        /// <summary>
        /// 联系电话，手机11位数字，座机：区号－数字
        /// </summary>
        [XmlElement("contact_number")]
        public string ContactNumber { get; set; }

        /// <summary>
        /// 外部实体编号，唯一标识一个实体，如自行车编号，机柜编号  注：商户维度下，类目Code（categoryCode）+实体编号（entity_code）唯一，一个商户下相同类目code+实体编号多次调用，将按照上传时间（upload_time）更新，更新规则取最新的upload_time快照数据
        /// </summary>
        [XmlElement("entity_code")]
        public string EntityCode { get; set; }

        /// <summary>
        /// 实体名称，借用实体的描述，如XX雨伞，XX充电宝，XX自行车
        /// </summary>
        [XmlElement("entity_name")]
        public string EntityName { get; set; }

        /// <summary>
        /// 地址位置纬度，取值范围：纬度-90~90，中国地区经度范围：纬度3.86~53.55
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 地址位置经度，取值范围：经度-180~180，中国地区经度范围：73.66~135.05
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 营业时间，格式：xx:xx-xx:xx，24小时制，如果是昼夜00:00—24:00
        /// </summary>
        [XmlElement("office_hours_desc")]
        public string OfficeHoursDesc { get; set; }

        /// <summary>
        /// 信用借还的签约产品码,传入固定值:w1010100000000002858
        /// </summary>
        [XmlElement("product_code")]
        public string ProductCode { get; set; }

        /// <summary>
        /// 租金描述，该借还点的租金描述，例如：5元/小时，5-10元／小时
        /// </summary>
        [XmlElement("rent_desc")]
        public string RentDesc { get; set; }

        /// <summary>
        /// 借用总数，如果是借用实物，如自行车，传1即可。如果是借用门店或借还机柜，则传入提供借还物品的总量
        /// </summary>
        [XmlElement("total_borrow_cnt")]
        public string TotalBorrowCnt { get; set; }

        /// <summary>
        /// 实体上传时间，某一借还实体信息多次上传，以最新上传时间数据为当前最新快照，格式：yyyy-mm-dd hh:MM:ss
        /// </summary>
        [XmlElement("upload_time")]
        public string UploadTime { get; set; }
    }
}
