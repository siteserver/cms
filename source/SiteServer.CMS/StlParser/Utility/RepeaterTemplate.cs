using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core.Model;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.Utility
{
	public class RepeaterTemplate : ITemplate
	{
	    readonly string templateString;
	    readonly LowerNameValueCollection selectedItems;
	    readonly LowerNameValueCollection selectedValues;
        readonly string separatorRepeatTemplate;
        readonly int separatorRepeat;
	    readonly PageInfo pageInfo;
        readonly EContextType contextType;
        readonly ContextInfo contextInfo;
        private int i = 0;

        public RepeaterTemplate(string templateString, LowerNameValueCollection selectedItems, LowerNameValueCollection selectedValues, string separatorRepeatTemplate, int separatorRepeat, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfo)
		{
			this.templateString = templateString;
            this.selectedItems = selectedItems;
            this.selectedValues = selectedValues;
            this.separatorRepeatTemplate = separatorRepeatTemplate;
            this.separatorRepeat = separatorRepeat;
            this.pageInfo = pageInfo;
            this.contextType = contextType;
            this.contextInfo = contextInfo;
		}

		public void InstantiateIn(Control container)
		{
			var noTagText = new Literal();
			noTagText.DataBinding += TemplateControl_DataBinding;
			container.Controls.Add(noTagText);
		}

		private void TemplateControl_DataBinding(object sender, EventArgs e)
		{
			var noTagText = (Literal) sender;
			var container = (RepeaterItem) noTagText.NamingContainer;

            var itemInfo = new DbItemInfo(container.DataItem, container.ItemIndex);

            if (contextType == EContextType.Channel)
            {
                pageInfo.ChannelItems.Push(itemInfo);
                noTagText.Text = TemplateUtility.GetChannelsItemTemplateString(templateString, selectedItems, selectedValues, container.ClientID, pageInfo, contextType, contextInfo);
            }
            else if (contextType == EContextType.Content)
            {
                pageInfo.ContentItems.Push(itemInfo);
                noTagText.Text = TemplateUtility.GetContentsItemTemplateString(templateString, selectedItems, selectedValues, container.ClientID, pageInfo, contextType, contextInfo);
            }
            else if (contextType == EContextType.Comment)
            {
                pageInfo.CommentItems.Push(itemInfo);
                noTagText.Text = TemplateUtility.GetCommentsTemplateString(templateString, container.ClientID, pageInfo, contextType, contextInfo);
            }
            else if (contextType == EContextType.InputContent)
            {
                pageInfo.InputItems.Push(itemInfo);
                noTagText.Text = TemplateUtility.GetInputContentsTemplateString(templateString, container.ClientID, pageInfo, contextType, contextInfo);
            }
            else if (contextType == EContextType.SqlContent)
            {
                pageInfo.SqlItems.Push(itemInfo);
                noTagText.Text = TemplateUtility.GetSqlContentsTemplateString(templateString, selectedItems, selectedValues, container.ClientID, pageInfo, contextType, contextInfo);
            }
            else if (contextType == EContextType.Site)
            {
                pageInfo.SiteItems.Push(itemInfo);
                noTagText.Text = TemplateUtility.GetSitesTemplateString(templateString, container.ClientID, pageInfo, contextType, contextInfo);
            }
            else if (contextType == EContextType.Photo)
            {
                pageInfo.PhotoItems.Push(itemInfo);
                noTagText.Text = TemplateUtility.GetPhotosTemplateString(templateString, selectedItems, selectedValues, container.ClientID, pageInfo, contextType, contextInfo);
            }
            else if (contextType == EContextType.Each)
            {
                pageInfo.EachItems.Push(itemInfo);
                noTagText.Text = TemplateUtility.GetEachsTemplateString(templateString, selectedItems, selectedValues, container.ClientID, pageInfo, contextType, contextInfo);
            }

            if (separatorRepeat > 1)
            {
                i++;
                if (i % separatorRepeat == 0)
                {
                    noTagText.Text += separatorRepeatTemplate;
                }
            }
		}
	}
}
