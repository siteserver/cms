namespace SiteServer.Plugin.Models
{
    public enum ValidateType
    {
        None,               //无
        Chinese,			//中文
        English,	        //英文
        Email,				//Email格式
        Url,				//网址格式
        Phone,				//电话号码 
        Mobile,				//手机号码
        Integer,			//整数
        Currency,			//货币格式
        Zip,				//邮政编码
        IdCard,				//身份证号码
        RegExp,             //正则表达式验证
    }
}
