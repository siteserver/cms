using System;
using System.Collections.Generic;
using System.Web;

namespace WxPayAPI
{
    public class OrderQuery
    {
        /***
        * 订单查询完整业务流程逻辑
        * @param transaction_id 微信订单号（优先使用）
        * @param out_trade_no 商户订单号
        * @return 订单查询结果（xml格式）
        */
        public static string Run(string transaction_id, string out_trade_no)
        {
            Log.Info("OrderQuery", "OrderQuery is processing...");

            WxPayData data = new WxPayData();
            if(!string.IsNullOrEmpty(transaction_id))//如果微信订单号存在，则以微信订单号为准
            {
                data.SetValue("transaction_id", transaction_id);
            }
            else//微信订单号不存在，才根据商户订单号去查单
            {
                data.SetValue("out_trade_no", out_trade_no);
            }

            WxPayData result = WxPayApi.OrderQuery(data);//提交订单查询请求给API，接收返回数据

            Log.Info("OrderQuery", "OrderQuery process complete, result : " + result.ToXml());
            return result.ToPrintStr();
        }
    }
}