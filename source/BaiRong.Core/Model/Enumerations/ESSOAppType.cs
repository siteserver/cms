using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum ESSOAppType
    {
        SiteServer,     //SiteServer 产品
        Others,         //其他
    }
    public class ESSOAppTypeUtils
    {
       public static string GetValue(ESSOAppType type)
        {
            if (type == ESSOAppType.SiteServer)
            {
                return "SiteServer";
            }
            else if (type == ESSOAppType.Others)
            {
                return "Others";
            }
            else
            {
                throw new Exception();
            }
        }

       public static string GetText(ESSOAppType type)
       {
           if (type == ESSOAppType.SiteServer)
           {
               return "SiteServer 产品";
           }
           else if (type == ESSOAppType.Others)
           {
               return "其他";
           }
           else
           {
               throw new Exception();
           }
       }

       public static ESSOAppType GetEnumType(string typeStr)
       {
           var retval = ESSOAppType.SiteServer;

           if (Equals(ESSOAppType.Others, typeStr))
           {
               retval = ESSOAppType.Others;
           }

           return retval;
       }

       public static bool Equals(ESSOAppType type, string typeStr)
       {
           if (string.IsNullOrEmpty(typeStr)) return false;
           if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
           {
               return true;
           }
           return false;
       }

       public static bool Equals(string typeStr, ESSOAppType type)
       {
           return Equals(type, typeStr);
       }

       public static ListItem GetListItem(ESSOAppType type, bool selected)
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
               listControl.Items.Add(GetListItem(ESSOAppType.SiteServer, false));
               listControl.Items.Add(GetListItem(ESSOAppType.Others, false));
           }
       }
    }
}
