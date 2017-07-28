using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateReference : BasePageCms
    {
		public Literal LtlTemplateElements;
		public Literal LtlTemplateEntities;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, "模板语言参考", AppManager.Cms.Permission.WebSite.Template);

                var elementsDictionary = StlAll.StlElements.GetElementsNameDictionary();
                var attributesDictionary = StlAll.StlElements.ElementsAttributesDictionary;
                LtlTemplateElements.Text = GetElementsString(elementsDictionary, attributesDictionary, false);

                elementsDictionary = StlAll.StlEntities.GetEntitiesNameDictionary();
                attributesDictionary = StlAll.StlEntities.EntitiesAttributesDictionary;
                LtlTemplateEntities.Text = GetElementsString(elementsDictionary, attributesDictionary, true);
			}
		}


        private string GetElementsString(SortedList<string, StlAttribute> elementsDictionary, SortedList<string, SortedList<string, string>> attributesDictionary, bool isEntities)
		{
			var retval = string.Empty;

            if (elementsDictionary != null)
			{
				var builder = new StringBuilder();
				foreach (string name in elementsDictionary.Keys)
				{
					var stlAttribute = elementsDictionary[name];
				    var helpUrl = StringUtils.Constants.GetStlUrl(isEntities, name);
                    string tdName = $@"<a href=""{helpUrl}"" target=""_blank"">&lt;{name}&gt;</a>";
                    if (isEntities)
                    {
                        tdName = $@"<a href=""{helpUrl}"" target=""_blank"">{{{name}}}</a>";
                    }

					var attributesList = attributesDictionary[name];
					var attributesBuilder = new StringBuilder();
                    if (attributesList != null)
                    {
                        foreach (var attributeName in attributesList.Keys)
                        {
                            attributesBuilder.Append($@"<a href=""{helpUrl + "#" + attributeName}"" target=""_blank"">{attributeName}</a>=""{attributesList[attributeName]}""<br />");
                        }
                    }
				    if (attributesBuilder.Length > 0) attributesBuilder.Length = attributesBuilder.Length - 6;

                    string tdAttributes = $@"<td>{attributesBuilder}<br /></td>";

					builder.Append(@"<tr><td>" + tdName + $@"</td><td>{stlAttribute.Usage}</td><td>{stlAttribute.Description}</td>" + tdAttributes);
				}
				retval = builder.ToString();
			}

			return retval;
		}
	}
}
