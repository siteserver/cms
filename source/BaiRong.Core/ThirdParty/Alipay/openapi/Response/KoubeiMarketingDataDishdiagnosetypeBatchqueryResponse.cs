using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingDataDishdiagnosetypeBatchqueryResponse.
    /// </summary>
    public class KoubeiMarketingDataDishdiagnosetypeBatchqueryResponse : AopResponse
    {
        /// <summary>
        /// 菜品类型list
        /// </summary>
        [XmlArray("item_diagnose_type_list")]
        [XmlArrayItem("item_diagnose_type")]
        public List<ItemDiagnoseType> ItemDiagnoseTypeList { get; set; }
    }
}
