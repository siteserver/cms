using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Utility
{
	public class DataListTemplate : ITemplate
	{
	    private readonly string _templateString;
        private readonly NameValueCollection _selectedItems;
        private readonly NameValueCollection _selectedValues;
        private readonly string _separatorRepeatTemplate;
        private readonly int _separatorRepeat;
        private readonly EContextType _contextType;
        private readonly ContextInfo _contextInfo;
        private readonly PageInfo _pageInfo;
        private int _i;

        public DataListTemplate(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string separatorRepeatTemplate, int separatorRepeat, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfo)
		{
			_templateString = templateString;
            _selectedItems = selectedItems;
            _selectedValues = selectedValues;
            _separatorRepeatTemplate = separatorRepeatTemplate;
            _separatorRepeat = separatorRepeat;
            _contextType = contextType;
            _contextInfo = contextInfo;
            _pageInfo = pageInfo;
		}

		public void InstantiateIn(Control container)
		{
			var noTagText = new Literal();
			noTagText.DataBinding += TemplateControl_DataBinding;
			container.Controls.Add(noTagText);
		}

		private void TemplateControl_DataBinding(object sender, EventArgs e)
		{
			var literal = (Literal) sender;
			var container = (DataListItem)literal.NamingContainer;

            var itemInfo = new DbItemInfo(container.DataItem, container.ItemIndex);

            if (_contextType == EContextType.Channel)
            {
                var channelItem = new ChannelItemInfo(SqlUtils.EvalInt(container.DataItem, ChannelAttribute.Id), container.ItemIndex);
                _pageInfo.ChannelItems.Push(channelItem);
                literal.Text = TemplateUtility.GetChannelsItemTemplateString(_templateString, _selectedItems, _selectedValues, container.ClientID, _pageInfo, _contextType, _contextInfo);
            }
            else if (_contextType == EContextType.Content)
            {
                var contentItem = new ContentItemInfo(SqlUtils.EvalInt(container.DataItem, ContentAttribute.ChannelId), SqlUtils.EvalInt(container.DataItem, ContentAttribute.Id), container.ItemIndex);
                _pageInfo.ContentItems.Push(contentItem);
                literal.Text = TemplateUtility.GetContentsItemTemplateString(_templateString, _selectedItems, _selectedValues, container.ClientID, _pageInfo, _contextType, _contextInfo);
            }
            else if (_contextType == EContextType.SqlContent)
            {
                _pageInfo.SqlItems.Push(itemInfo);
                literal.Text = TemplateUtility.GetSqlContentsTemplateString(_templateString, _selectedItems, _selectedValues, container.ClientID, _pageInfo, _contextType, _contextInfo);
            }
            else if (_contextType == EContextType.Site)
            {
                _pageInfo.SiteItems.Push(itemInfo);
                literal.Text = TemplateUtility.GetSitesTemplateString(_templateString, container.ClientID, _pageInfo, _contextType, _contextInfo);
            }
            else if (_contextType == EContextType.Each)
            {
                _pageInfo.EachItems.Push(itemInfo);
                literal.Text = TemplateUtility.GetEachsTemplateString(_templateString, _selectedItems, _selectedValues, container.ClientID, _pageInfo, _contextType, _contextInfo);
            }

            if (_separatorRepeat > 1)
            {
                _i++;
                if (_i % _separatorRepeat == 0)
                {
                    literal.Text += _separatorRepeatTemplate;
                }
            }
		}
	}
}
