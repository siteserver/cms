using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Utility
{
	public class RepeaterTemplate : ITemplate
	{
        private readonly string _templateString;
        private readonly NameValueCollection _selectedItems;
        private readonly NameValueCollection _selectedValues;
        private readonly string _separatorRepeatTemplate;
        private readonly int _separatorRepeat;
        private readonly PageInfo _pageInfo;
        private readonly EContextType _contextType;
        private readonly ContextInfo _contextInfo;
        private int _i;

        public RepeaterTemplate(string templateString, NameValueCollection selectedItems, NameValueCollection selectedValues, string separatorRepeatTemplate, int separatorRepeat, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfo)
		{
			_templateString = templateString;
            _selectedItems = selectedItems;
            _selectedValues = selectedValues;
            _separatorRepeatTemplate = separatorRepeatTemplate;
            _separatorRepeat = separatorRepeat;
            _pageInfo = pageInfo;
            _contextType = contextType;
            _contextInfo = contextInfo;
		}

		public void InstantiateIn(Control container)
		{
			var literal = new Literal();
            literal.DataBinding += TemplateControl_DataBinding;
			container.Controls.Add(literal);
		}

		private void TemplateControl_DataBinding(object sender, EventArgs e)
		{
			var literal = (Literal) sender;
			var container = (RepeaterItem)literal.NamingContainer;

            var itemInfo = new DbItemInfo(container.DataItem, container.ItemIndex);

            if (_contextType == EContextType.Channel)
            {
                var channelItemInfo = new ChannelItemInfo(SqlUtils.EvalInt(container.DataItem, ChannelAttribute.Id), container.ItemIndex);
                _pageInfo.ChannelItems.Push(channelItemInfo);
                literal.Text = TemplateUtility.GetChannelsItemTemplateString(_templateString, _selectedItems, _selectedValues, container.ClientID, _pageInfo, _contextType, _contextInfo);
            }
            else if (_contextType == EContextType.Content)
            {
                var contentItemInfo = new ContentItemInfo(SqlUtils.EvalInt(container.DataItem, ContentAttribute.ChannelId), SqlUtils.EvalInt(container.DataItem, ContentAttribute.Id), container.ItemIndex);
                _pageInfo.ContentItems.Push(contentItemInfo);
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
