using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum EWxAccountType
	{
        Subscribe,
        AuthenticatedSubscribe,
        Service,
        AuthenticatedService
	}

    public class EWxAccountTypeUtils
	{
        public static string GetValue(EWxAccountType type)
		{
            if (type == EWxAccountType.Subscribe)
            {
                return "Subscribe";
            }
            else if (type == EWxAccountType.AuthenticatedSubscribe)
            {
                return "AuthenticatedSubscribe";
            }
            else if (type == EWxAccountType.Service)
            {
                return "Service";
            }
            else if (type == EWxAccountType.AuthenticatedService)
            {
                return "AuthenticatedService";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EWxAccountType type)
		{
            if (type == EWxAccountType.Subscribe)
            {
                return "订阅号";
            }
            else if (type == EWxAccountType.AuthenticatedSubscribe)
            {
                return "认证订阅号";
            }
            else if (type == EWxAccountType.Service)
            {
                return "服务号";
            }
            else if (type == EWxAccountType.AuthenticatedService)
            {
                return "认证服务号";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EWxAccountType GetEnumType(string typeStr)
		{
            var retval = EWxAccountType.Subscribe;

            if (Equals(EWxAccountType.Subscribe, typeStr))
            {
                retval = EWxAccountType.Subscribe;
            }
            else if (Equals(EWxAccountType.AuthenticatedSubscribe, typeStr))
            {
                retval = EWxAccountType.AuthenticatedSubscribe;
            }
            else if (Equals(EWxAccountType.Service, typeStr))
            {
                retval = EWxAccountType.Service;
            }
            else if (Equals(EWxAccountType.AuthenticatedService, typeStr))
            {
                retval = EWxAccountType.AuthenticatedService;
            }

			return retval;
		}

		public static bool Equals(EWxAccountType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EWxAccountType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EWxAccountType type, bool selected)
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
                listControl.Items.Add(GetListItem(EWxAccountType.Subscribe, false));
                listControl.Items.Add(GetListItem(EWxAccountType.AuthenticatedSubscribe, false));
                listControl.Items.Add(GetListItem(EWxAccountType.Service, false));
                listControl.Items.Add(GetListItem(EWxAccountType.AuthenticatedService, false));
            }
        }
	}
}
