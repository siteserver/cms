using System;
using System.Text;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EInputValidateType
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
        QQ,				    //QQ号码
        Custom,				//自定义正则表达式验证
	}

	public class EInputValidateTypeUtils
	{
		public static string GetValue(EInputValidateType type)
		{
            if (type == EInputValidateType.None)
			{
                return "None";
            }
            else if (type == EInputValidateType.Chinese)
            {
                return "Chinese";
            }
            else if (type == EInputValidateType.English)
			{
                return "English";
			}
            else if (type == EInputValidateType.Email)
			{
                return "Email";
            }
            else if (type == EInputValidateType.Url)
            {
                return "Url";
            }
            else if (type == EInputValidateType.Phone)
            {
                return "Phone";
            }
            else if (type == EInputValidateType.Mobile)
            {
                return "Mobile";
            }
            else if (type == EInputValidateType.Integer)
            {
                return "Integer";
            }
            else if (type == EInputValidateType.Currency)
            {
                return "Currency";
            }
            else if (type == EInputValidateType.Zip)
            {
                return "Zip";
            }
            else if (type == EInputValidateType.IdCard)
            {
                return "IdCard";
            }
            else if (type == EInputValidateType.QQ)
            {
                return "QQ";
            }
            else if (type == EInputValidateType.Custom)
            {
                return "Custom";
            }
			else
			{
				throw new Exception();
			}
		}

        public static string GetText(EInputValidateType type)
		{
            if (type == EInputValidateType.None)
            {
                return "无";
            }
            else if (type == EInputValidateType.Chinese)
            {
                return "中文";
            }
            else if (type == EInputValidateType.English)
            {
                return "英文";
            }
            else if (type == EInputValidateType.Email)
            {
                return "Email格式";
            }
            else if (type == EInputValidateType.Url)
            {
                return "网址格式";
            }
            else if (type == EInputValidateType.Phone)
            {
                return "电话号码";
            }
            else if (type == EInputValidateType.Mobile)
            {
                return "手机号码";
            }
            else if (type == EInputValidateType.Integer)
            {
                return "整数";
            }
            else if (type == EInputValidateType.Currency)
            {
                return "货币格式";
            }
            else if (type == EInputValidateType.Zip)
            {
                return "邮政编码";
            }
            else if (type == EInputValidateType.IdCard)
            {
                return "身份证号码";
            }
            else if (type == EInputValidateType.QQ)
            {
                return "QQ号码";
            }
            else if (type == EInputValidateType.Custom)
            {
                return "自定义正则表达式验证";
            }
            else
            {
                throw new Exception();
            }
		}

        public static string GetValidateInfo(TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();
            if (styleInfo.Additional.IsRequired)
            {
                builder.Append("必填项;");
            }
            if (styleInfo.Additional.MinNum > 0)
            {
                builder.Append($"最少{styleInfo.Additional.MinNum}个字符;");
            }
            if (styleInfo.Additional.MaxNum > 0)
            {
                builder.Append($"最多{styleInfo.Additional.MaxNum}个字符;");
            }
            if (styleInfo.Additional.ValidateType != EInputValidateType.None)
            {
                builder.Append($"验证:{GetText(styleInfo.Additional.ValidateType)};");
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
            }
            else
            {
                builder.Append("无验证");
            }
            return builder.ToString();
        }

		public static EInputValidateType GetEnumType(string typeStr)
		{
			var retval = EInputValidateType.None;

			if (Equals(EInputValidateType.None, typeStr))
			{
                retval = EInputValidateType.None;
            }
            else if (Equals(EInputValidateType.Chinese, typeStr))
            {
                retval = EInputValidateType.Chinese;
            }
            else if (Equals(EInputValidateType.Currency, typeStr))
            {
                retval = EInputValidateType.Currency;
            }
			else if (Equals(EInputValidateType.Custom, typeStr))
			{
                retval = EInputValidateType.Custom;
            }
            else if (Equals(EInputValidateType.Email, typeStr))
            {
                retval = EInputValidateType.Email;
            }
            else if (Equals(EInputValidateType.English, typeStr))
            {
                retval = EInputValidateType.English;
            }
            else if (Equals(EInputValidateType.IdCard, typeStr))
            {
                retval = EInputValidateType.IdCard;
            }
            else if (Equals(EInputValidateType.Integer, typeStr))
            {
                retval = EInputValidateType.Integer;
            }
            else if (Equals(EInputValidateType.Mobile, typeStr))
            {
                retval = EInputValidateType.Mobile;
            }
            else if (Equals(EInputValidateType.Phone, typeStr))
            {
                retval = EInputValidateType.Phone;
            }
            else if (Equals(EInputValidateType.QQ, typeStr))
            {
                retval = EInputValidateType.QQ;
            }
            else if (Equals(EInputValidateType.Url, typeStr))
            {
                retval = EInputValidateType.Url;
            }
            else if (Equals(EInputValidateType.Zip, typeStr))
            {
                retval = EInputValidateType.Zip;
            }

			return retval;
		}

		public static bool Equals(EInputValidateType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EInputValidateType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EInputValidateType type, bool selected)
		{
			var item = new ListItem(GetText(type), GetValue(type));
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EInputValidateType.None, false));
                listControl.Items.Add(GetListItem(EInputValidateType.Chinese, false));
                listControl.Items.Add(GetListItem(EInputValidateType.English, false));
                listControl.Items.Add(GetListItem(EInputValidateType.Email, false));
                listControl.Items.Add(GetListItem(EInputValidateType.Url, false));
                listControl.Items.Add(GetListItem(EInputValidateType.Phone, false));
                listControl.Items.Add(GetListItem(EInputValidateType.Mobile, false));
                listControl.Items.Add(GetListItem(EInputValidateType.Integer, false));
                listControl.Items.Add(GetListItem(EInputValidateType.Currency, false));
                listControl.Items.Add(GetListItem(EInputValidateType.Zip, false));
                listControl.Items.Add(GetListItem(EInputValidateType.IdCard, false));
                listControl.Items.Add(GetListItem(EInputValidateType.QQ, false));
                listControl.Items.Add(GetListItem(EInputValidateType.Custom, false));
            }
        }
	}
}
