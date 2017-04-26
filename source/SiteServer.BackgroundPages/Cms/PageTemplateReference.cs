using System;
using System.Collections;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateReference : BasePageCms
    {
		public DataGrid DataGrid;

		public Literal ltlTemplateElements;
		public Literal ltlTemplateEntities;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, "模板语言参考", AppManager.Cms.Permission.WebSite.Template);

                var elementsDictionary = StlAll.StlElements.GetElementsNameDictionary();
                var attributesDictionary = StlAll.StlElements.GetElementsAttributesDictionary();
                ltlTemplateElements.Text = GetElementsString(elementsDictionary, attributesDictionary, "StlElements", false);

                elementsDictionary = StlAll.StlEntities.GetEntitiesNameDictionary();
                attributesDictionary = StlAll.StlEntities.GetEntitiesAttributesDictionary();
                ltlTemplateEntities.Text = GetElementsString(elementsDictionary, attributesDictionary, "StlEntities", true);

                //this.TemplateEntities.Text = this.GetEntitiesString(StlEntities.GetStlEntitiesDictionary(), "通用实体");
                //this.TemplateEntities.Text += this.GetEntitiesString(StlContentEntities.GetContentEntitiesDirectory(base.PublishmentSystemInfo), "内容实体");
                //this.TemplateEntities.Text += this.GetEntitiesString(StlChannelEntities.GetChannelEntitiesDirectory(base.PublishmentSystemInfo), "栏目实体");
                //this.TemplateEntities.Text += this.GetEntitiesString(StlCommentEntities.GetCommentEntitiesDirectory(base.PublishmentSystemInfo), "评论实体");
                //this.TemplateEntities.Text += this.GetEntitiesString(StlSqlEntities.GetSqlEntitiesDirectory(), "Sql实体");
			}
		}


        private string GetElementsString(IDictionary elementsDictionary, IDictionary attributesDictionary, string elementsName, bool isEntities)
		{
			var retval = string.Empty;

			if (elementsDictionary != null)
			{
				var builder = new StringBuilder();
				var tr = "<tr>";
				foreach (string label in elementsDictionary.Keys)
				{
					var labelName = (string)elementsDictionary[label];
                    var helpUrl = string.Empty;
                    var target = string.Empty;
                    helpUrl = StringUtils.Constants.GetStlUrl(isEntities, label);
                    target = " target=\"_blank\"";
                    string tdName =
                        $"<td width=\"0\" align=\"Left\" ><a href=\"{helpUrl}\"{target}>&lt;{label}&gt;</a></td><td width=\"0\" align=\"Left\" >{labelName}&nbsp;</td>";
                    if (isEntities)
                    {
                        tdName =
                            $"<td width=\"0\" align=\"Left\" ><a href=\"{helpUrl}\"{target}>{{{label}}}</a></td><td width=\"0\" align=\"Left\" >{labelName}&nbsp;</td>";
                    }

					var attributesList = (IDictionary)attributesDictionary[label];
					var attributesBuilder = new StringBuilder();
                    if (attributesList != null)
                    {
                        foreach (string attributeName in attributesList.Keys)
                        {
                            attributesBuilder.Append($"{attributeName}({attributesList[attributeName]})&nbsp;&nbsp;");
                        }
                    }

					string tdAttributes = $"<td width=\"0\" align=\"Left\" >{attributesBuilder}<br /></td>";

					builder.Append(tr + tdName + tdAttributes);
				}
				retval = builder.ToString();
			}

			return retval;
		}
	}
}
