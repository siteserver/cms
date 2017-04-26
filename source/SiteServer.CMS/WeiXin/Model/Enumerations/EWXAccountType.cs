using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum EWXAccountType
	{
        Subscribe,
        AuthenticatedSubscribe,
        Service,
        AuthenticatedService

	}

    public class EWXAccountTypeUtils
	{
        public static string GetValue(EWXAccountType type)
		{
            if (type == EWXAccountType.Subscribe)
            {
                return "Subscribe";
            }
            else if (type == EWXAccountType.AuthenticatedSubscribe)
            {
                return "AuthenticatedSubscribe";
            }
            else if (type == EWXAccountType.Service)
            {
                return "Service";
            }
            else if (type == EWXAccountType.AuthenticatedService)
            {
                return "AuthenticatedService";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EWXAccountType type)
		{
            if (type == EWXAccountType.Subscribe)
            {
                return "订阅号";
            }
            else if (type == EWXAccountType.AuthenticatedSubscribe)
            {
                return "认证订阅号";
            }
            else if (type == EWXAccountType.Service)
            {
                return "服务号";
            }
            else if (type == EWXAccountType.AuthenticatedService)
            {
                return "认证服务号";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EWXAccountType GetEnumType(string typeStr)
		{
            var retval = EWXAccountType.Subscribe;

            if (Equals(EWXAccountType.Subscribe, typeStr))
            {
                retval = EWXAccountType.Subscribe;
            }
            else if (Equals(EWXAccountType.AuthenticatedSubscribe, typeStr))
            {
                retval = EWXAccountType.AuthenticatedSubscribe;
            }
            else if (Equals(EWXAccountType.Service, typeStr))
            {
                retval = EWXAccountType.Service;
            }
            else if (Equals(EWXAccountType.AuthenticatedService, typeStr))
            {
                retval = EWXAccountType.AuthenticatedService;
            }

			return retval;
		}

		public static bool Equals(EWXAccountType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EWXAccountType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EWXAccountType type, bool selected)
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
                listControl.Items.Add(GetListItem(EWXAccountType.Subscribe, false));
                listControl.Items.Add(GetListItem(EWXAccountType.AuthenticatedSubscribe, false));
                listControl.Items.Add(GetListItem(EWXAccountType.Service, false));
                listControl.Items.Add(GetListItem(EWXAccountType.AuthenticatedService, false));
            }
        }
	}
}
