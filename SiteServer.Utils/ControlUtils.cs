using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace SiteServer.Utils
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

	    public static string GetControlRenderHtml(Control control, Page page)
        {
            if (control == null) return string.Empty;

            var builder = new StringBuilder();
            control.Page = page;
            control.DataBind();
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
		/// <param name="accessor">控件</param>
		/// <param name="attributes">属性集合</param>
		public static void AddAttributes(IAttributeAccessor accessor, StringDictionary attributes)
		{
		    if (accessor == null || attributes == null) return;

		    foreach (string key in attributes.Keys)
		    {
		        accessor.SetAttribute(key, attributes[key]);
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

        public static void AddListControlItems(ListControl listControl, List<string> list)
        {
            if (listControl == null) return;

            foreach (var value in list)
            {
                var item = new ListItem(value, value);
                listControl.Items.Add(item);
            }
        }


		public static string[] GetSelectedListControlValueArray(ListControl listControl)
		{
			var arraylist = new ArrayList();
			if (listControl != null)
			{
				foreach (ListItem item in listControl.Items)
				{
					if (item.Selected)
					{
						arraylist.Add(item.Value);
					}
				}
			}
			var retval = new string[arraylist.Count];
			arraylist.CopyTo(retval);
			return retval;
		}

        public static string GetSelectedListControlValueCollection(ListControl listControl)
        {
            var list = new List<string>();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        list.Add(item.Value);
                    }
                }
            }
            return TranslateUtils.ObjectCollectionToString(list);
        }

        public static ArrayList GetSelectedListControlValueArrayList(ListControl listControl)
        {
            var arraylist = new ArrayList();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        arraylist.Add(item.Value);
                    }
                }
            }
            return arraylist;
        }

        public static ArrayList GetSelectedListControlValueIntArrayList(ListControl listControl)
        {
            var arraylist = new ArrayList();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        arraylist.Add(TranslateUtils.ToInt(item.Value));
                    }
                }
            }
            return arraylist;
        }

        public static List<string> GetSelectedListControlValueStringList(ListControl listControl)
        {
            var list = new List<string>();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        list.Add(item.Value);
                    }
                }
            }
            return list;
        }

        public static List<int> GetSelectedListControlValueIntList(ListControl listControl)
        {
            var list = new List<int>();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        list.Add(TranslateUtils.ToInt(item.Value));
                    }
                }
            }
            return list;
        }

        public static string[] GetListControlValues(ListControl listControl)
		{
			var arraylist = new ArrayList();
			if (listControl != null)
			{
				foreach (ListItem item in listControl.Items)
				{
					arraylist.Add(item.Value);
				}
			}
			var retval = new string[arraylist.Count];
			arraylist.CopyTo(retval);
			return retval;
		}

        public static void SelectSingleItem(ListControl listControl, string value)
        {
            if (listControl == null) return;

            listControl.ClearSelection();

            foreach (ListItem item in listControl.Items)
            {
                if (string.Equals(item.Value, value))
                {
                    item.Selected = true;
                    break;
                }
            }
        }

        public static void SelectSingleItemIgnoreCase(ListControl listControl, string value)
        {
            if (listControl == null) return;

            listControl.ClearSelection();
            foreach (ListItem item in listControl.Items)
            {
                if (StringUtils.EqualsIgnoreCase(item.Value, value))
                {
                    item.Selected = true;
                    break;
                }
            }
        }

        public static void SelectMultiItems(ListControl listControl, params string[] values)
        {
            if (listControl == null) return;

            listControl.ClearSelection();
            foreach (ListItem item in listControl.Items)
            {
                foreach (var value in values)
                {
                    if (string.Equals(item.Value, value))
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }

        public static void SelectMultiItems(ListControl listControl, List<string> values)
        {
            if (listControl == null) return;

            listControl.ClearSelection();
            foreach (ListItem item in listControl.Items)
            {
                foreach (var value in values)
                {
                    if (string.Equals(item.Value, value))
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }

        public static void SelectMultiItems(ListControl listControl, List<int> values)
        {
            if (listControl == null) return;

            listControl.ClearSelection();
            foreach (ListItem item in listControl.Items)
            {
                foreach (var intVal in values)
                {
                    if (string.Equals(item.Value, intVal.ToString()))
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }

		public static void SelectMultiItemsIgnoreCase(ListControl listControl, params string[] values)
		{
		    if (listControl == null) return;

            listControl.ClearSelection();
            foreach (ListItem item in listControl.Items)
		    {
		        foreach (var value in values)
		        {
		            if (StringUtils.EqualsIgnoreCase(item.Value, value))
                    {
		                item.Selected = true;
		                break;
		            }
		        }
		    }
		}

	    public static void SelectMultiItemsIgnoreCase(ListControl listControl, List<string> values)
	    {
	        if (listControl == null) return;

	        listControl.ClearSelection();
	        foreach (ListItem item in listControl.Items)
	        {
	            foreach (var value in values)
	            {
	                if (StringUtils.EqualsIgnoreCase(item.Value, value))
	                {
	                    item.Selected = true;
	                    break;
	                }
	            }
	        }
	    }

        public static string SelectedItemsValueToStringCollection(ListItemCollection items)
		{
			var builder = new StringBuilder();
			if (items!= null)
			{
				foreach (ListItem item in items)
				{
					if (item.Selected)
					{
						builder.Append(item.Value).Append(",");
					}
				}
				if (builder.Length != 0)
					builder.Remove(builder.Length - 1, 1);
			}
			return builder.ToString();
		}

        public static Control FindControlBySelfAndChildren(string id, Control baseControl)
        {
            Control ctrl = null;
            if (baseControl != null)
            {
                ctrl = baseControl.FindControl(id);
                if (ctrl == baseControl) ctrl = null;//DropDownList中FindControl将返回自身
                if (ctrl == null && baseControl.HasControls())
                {
                    ctrl = FindControlByChildren(id, baseControl.Controls);
                }
            }            
            return ctrl;
        }

        public static Control FindControlByChildren(string id, ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                var ctrl = FindControlBySelfAndChildren(id, control);
                if (ctrl != null)
                {
                    return ctrl;
                }
            }
            return null;
        }

        //public static string GetInputValue(Control containerControl, string inputName)
        //{
        //    Control control;
        //    return GetInputValue(containerControl, inputName, out control);
        //}

        ///// <summary>
        ///// 返回null代表未找到控件。
        ///// </summary>
        //public static string GetInputValue(Control containerControl, string inputName, out Control control)
        //{
        //    string value = null;

        //    control = FindControlBySelfAndChildren(inputName, containerControl);
        //    if (control != null)
        //    {
        //        value = string.Empty;
        //        if (control is TextBox)
        //        {
        //            var input = (TextBox)control;
        //            value = input.Text;
        //        }
        //        else if (control is HtmlInputControl)
        //        {
        //            var input = (HtmlInputControl)control;
        //            value = input.Value;
        //        }
        //        else if (control is HtmlTextArea)
        //        {
        //            var input = (HtmlTextArea)control;
        //            value = input.Value;
        //        }
        //        else if (control is TextEditorBase)
        //        {
        //            var input = (TextEditorBase)control;
        //            value = input.Text;
        //        }
        //        else if (control is ListControl)
        //        {
        //            var select = (ListControl)control;
        //            var arraylist = GetSelectedListControlValueArrayList(select);
        //            value = TranslateUtils.ObjectCollectionToString(arraylist);
        //        }
        //        else if (control is HtmlSelect)
        //        {
        //            var select = (HtmlSelect)control;
        //            var arraylist = new ArrayList();
        //            foreach (ListItem item in select.Items)
        //            {
        //                if (item.Selected)
        //                {
        //                    arraylist.Add(item.Value);
        //                }
        //            }
        //            value = TranslateUtils.ObjectCollectionToString(arraylist);
        //        }
        //        else if (control is CheckBox)
        //        {
        //            var checkBox = (CheckBox)control;
        //            value = checkBox.Checked.ToString();
        //        }
        //    }
        //    return value;
        //}

        //public static void SetInputValue(Control containerControl, string inputName, TableStyleInfo styleInfo)
        //{
        //    var control = FindControlBySelfAndChildren(inputName, containerControl);

        //    if (control != null)
        //    {
        //        if (control is TextBox)
        //        {
        //            var input = (TextBox)control;
        //            if (string.IsNullOrEmpty(input.Text) && !string.IsNullOrEmpty(styleInfo.DefaultValue))
        //            {
        //                input.Text = styleInfo.DefaultValue;
        //            }
        //        }
        //        else if (control is HtmlInputControl)
        //        {
        //            var input = (HtmlInputControl)control;
        //            if (string.IsNullOrEmpty(input.Value) && !string.IsNullOrEmpty(styleInfo.DefaultValue))
        //            {
        //                input.Value = styleInfo.DefaultValue;
        //            }
        //        }
        //        else if (control is HtmlTextArea)
        //        {
        //            var input = (HtmlTextArea)control;
        //            if (string.IsNullOrEmpty(input.Value) && !string.IsNullOrEmpty(styleInfo.DefaultValue))
        //            {
        //                input.Value = styleInfo.DefaultValue;
        //            }
        //        }
        //        else if (control is TextEditorBase)
        //        {
        //            var input = (TextEditorBase)control;
        //            if (string.IsNullOrEmpty(input.Text) && !string.IsNullOrEmpty(styleInfo.DefaultValue))
        //            {
        //                input.Text = styleInfo.DefaultValue;
        //            }
        //        }
        //        else if (control is ListControl)
        //        {
        //            var select = (ListControl)control;
        //            if (select.Items.Count == 0)
        //            {
        //                var tableStyleItemInfoArrayList = BaiRongDataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.TableStyleId);
        //                if (tableStyleItemInfoArrayList != null && tableStyleItemInfoArrayList.Count > 0)
        //                {
        //                    foreach (var styleItemInfo in tableStyleItemInfoArrayList)
        //                    {
        //                        var listItem = new ListItem(styleItemInfo.ItemTitle, styleItemInfo.ItemValue);
        //                        listItem.Selected = styleItemInfo.IsSelected;
        //                        select.Items.Add(listItem);
        //                    }
        //                }
        //            }
        //        }
        //        else if (control is HtmlSelect)
        //        {
        //            var select = (HtmlSelect)control;
        //            if (select.Items.Count == 0)
        //            {
        //                var tableStyleItemInfoArrayList = BaiRongDataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.TableStyleId);
        //                if (tableStyleItemInfoArrayList != null && tableStyleItemInfoArrayList.Count > 0)
        //                {
        //                    foreach (var styleItemInfo in tableStyleItemInfoArrayList)
        //                    {
        //                        var listItem = new ListItem(styleItemInfo.ItemTitle, styleItemInfo.ItemValue);
        //                        listItem.Selected = styleItemInfo.IsSelected;
        //                        select.Items.Add(listItem);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //public static void SetInputValue(Control containerControl, string inputName, string value, TableStyleInfo styleInfo)
        //{
        //    var control = FindControlBySelfAndChildren(inputName, containerControl);

        //    if (control != null)
        //    {
        //        if (control is TextBox)
        //        {
        //            var input = (TextBox)control;
        //            input.Text = value;
        //        }
        //        else if (control is HtmlInputControl)
        //        {
        //            var input = (HtmlInputControl)control;
        //            input.Value = value;
        //        }
        //        else if (control is HtmlTextArea)
        //        {
        //            var input = (HtmlTextArea)control;
        //            input.Value = value;
        //        }
        //        else if (control is TextEditorBase)
        //        {
        //            var input = (TextEditorBase)control;
        //            input.Text = value;
        //        }
        //        else if (control is ListControl)
        //        {
        //            var select = (ListControl)control;
        //            if (select.Items.Count == 0)
        //            {
        //                var tableStyleItemInfoArrayList = BaiRongDataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.TableStyleId);
        //                if (tableStyleItemInfoArrayList != null && tableStyleItemInfoArrayList.Count > 0)
        //                {
        //                    foreach (var styleItemInfo in tableStyleItemInfoArrayList)
        //                    {
        //                        var listItem = new ListItem(styleItemInfo.ItemTitle, styleItemInfo.ItemValue);
        //                        listItem.Selected = styleItemInfo.IsSelected;
        //                        select.Items.Add(listItem);
        //                    }
        //                }
        //            }
        //        }
        //        else if (control is HtmlSelect)
        //        {
        //            var select = (HtmlSelect)control;
        //            if (select.Items.Count == 0)
        //            {
        //                var tableStyleItemInfoArrayList = BaiRongDataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.TableStyleId);
        //                if (tableStyleItemInfoArrayList != null && tableStyleItemInfoArrayList.Count > 0)
        //                {
        //                    foreach (var styleItemInfo in tableStyleItemInfoArrayList)
        //                    {
        //                        var listItem = new ListItem(styleItemInfo.ItemTitle, styleItemInfo.ItemValue);
        //                        listItem.Selected = styleItemInfo.IsSelected;
        //                        select.Items.Add(listItem);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
	}
}
