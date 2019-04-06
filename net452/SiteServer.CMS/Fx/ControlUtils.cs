using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.Utils;

namespace SiteServer.CMS.Fx
{
	/// <summary>
	/// 对控件的帮助类
	/// </summary>
	public class ControlUtils
	{
		private ControlUtils()
		{

		}

	    /// <summary>
	    /// 得到代表控件的HTML代码
	    /// </summary>
	    /// <param name="control">控件</param>
	    /// <returns></returns>
	    public static string GetControlRenderHtml(Control control)
	    {
	        if (control == null) return string.Empty;

	        var builder = new StringBuilder();
	        var sw = new System.IO.StringWriter(builder);
	        var htw = new HtmlTextWriter(sw);
	        control.RenderControl(htw);
	        return builder.ToString();
	    }

		/// <summary>
		/// 如果此控件不存在此属性，将属性添加到控件中
		/// </summary>
		/// <param name="accessor">控件</param>
		/// <param name="attributes">属性集合</param>
		public static void AddAttributesIfNotExists(IAttributeAccessor accessor, NameValueCollection attributes)
		{
		    if (accessor == null || attributes == null) return;

		    foreach (var key in attributes.AllKeys)
		    {
		        if (accessor.GetAttribute(key) == null)
		        {
		            accessor.SetAttribute(key, attributes[key]);
		        }
		    }
		}

		/// <summary>
		/// 如果此控件不存在此属性，将属性添加到控件中
		/// </summary>
		/// <param name="accessor"></param>
		/// <param name="attributeName"></param>
		/// <param name="attributeValue"></param>
		public static void AddAttributeIfNotExists(IAttributeAccessor accessor, string attributeName, string attributeValue)
		{
		    if (accessor == null || attributeName == null) return;

		    if (accessor.GetAttribute(attributeName) == null)
		    {
		        accessor.SetAttribute(attributeName, attributeValue);
		    }
		}

		/// <summary>
		/// 将属性添加到控件中
		/// </summary>
		/// <param name="accessor"></param>
		/// <param name="attributeName"></param>
		/// <param name="attributeValue"></param>
		public static void AddAttribute(IAttributeAccessor accessor, string attributeName, string attributeValue)
		{
			if (accessor != null && attributeName != null)
			{
				accessor.SetAttribute(attributeName, attributeValue);
			}
		}

        

        
	}
}
