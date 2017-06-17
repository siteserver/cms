using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core.Model;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.Utility
{
	public class RepeaterTemplate : ITemplate
	{
        private readonly string _templateString;
        private readonly LowerNameValueCollection _selectedItems;
        private readonly LowerNameValueCollection _selectedValues;
        private readonly string _separatorRepeatTemplate;
        private readonly int _separatorRepeat;
        private readonly PageInfo _pageInfo;
        private readonly EContextType _contextType;
        private readonly ContextInfo _contextInfo;
        private int _i;

        public RepeaterTemplate(string templateString, LowerNameValueCollection selectedItems, LowerNameValueCollection selectedValues, string separatorRepeatTemplate, int separatorRepeat, PageInfo pageInfo, EContextType contextType, ContextInfo contextInfo)
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
                _pageInfo.ChannelItems.Push(itemInfo);
                literal.Text = TemplateUtility.GetChannelsItemTemplateString(_templateString, _selectedItems, _selectedValues, container.ClientID, _pageInfo, _contextType, _contextInfo);
            }
            else if (_contextType == EContextType.Content)
            {
                _pageInfo.ContentItems.Push(itemInfo);
                literal.Text = TemplateUtility.GetContentsItemTemplateString(_templateString, _selectedItems, _selectedValues, container.ClientID, _pageInfo, _contextType, _contextInfo);
            }
            else if (_contextType == EContextType.Comment)
            {
                _pageInfo.CommentItems.Push(itemInfo);
                literal.Text = TemplateUtility.GetCommentsTemplateString(_templateString, container.ClientID, _pageInfo, _contextType, _contextInfo);
            }
            else if (_contextType == EContextType.InputContent)
            {
                _pageInfo.InputItems.Push(itemInfo);
                literal.Text = TemplateUtility.GetInputContentsTemplateString(_templateString, container.ClientID, _pageInfo, _contextType, _contextInfo);
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
            else if (_contextType == EContextType.Photo)
            {
                _pageInfo.PhotoItems.Push(itemInfo);
                literal.Text = TemplateUtility.GetPhotosTemplateString(_templateString, _selectedItems, _selectedValues, container.ClientID, _pageInfo, _contextType, _contextInfo);
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
