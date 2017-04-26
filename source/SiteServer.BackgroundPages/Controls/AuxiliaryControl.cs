using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Controls
{
    public class AuxiliaryControl : Control
    {
        private NameValueCollection _formCollection;
        private PublishmentSystemInfo _publishmentSystemInfo;
        private int _nodeId;
        private List<int> _relatedIdentities;
        private ETableStyle _tableStyle;
        private string _tableName;
        private bool _isEdit;
        private bool _isPostBack;
        private readonly List<string> _excludeAttributeNames = new List<string>();

        public void SetParameters(NameValueCollection formCollection, PublishmentSystemInfo publishmentSystemInfo, int nodeId, List<int> relatedIdentities, ETableStyle tableStyle, string tableName, bool isEdit, bool isPostBack)
        {
            _formCollection = formCollection;
            _publishmentSystemInfo = publishmentSystemInfo;
            _nodeId = nodeId;
            _relatedIdentities = relatedIdentities;
            _tableStyle = tableStyle;
            _tableName = tableName;
            _isEdit = isEdit;
            _isPostBack = isPostBack;
        }

        public void AddExcludeAttributeNames(List<string> arraylist)
        {
            _excludeAttributeNames.AddRange(arraylist);
        }

        protected override void Render(HtmlTextWriter output)
        {
            if (string.IsNullOrEmpty(_tableName)) return;

            if (_formCollection == null)
            {
                _formCollection = HttpContext.Current.Request.Form.Count > 0 ? HttpContext.Current.Request.Form : new NameValueCollection();
            }

            var builder = new StringBuilder();
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, _tableName, _relatedIdentities);
            var pageScripts = new NameValueCollection();

            if (styleInfoList != null)
            {
                var isPreviousSingleLine = true;
                var isPreviousLeftColumn = false;
                foreach (var styleInfo in styleInfoList)
                {
                    if (styleInfo.IsVisible && !_excludeAttributeNames.Contains(styleInfo.AttributeName.ToLower()))
                    {
                        var text = $"{styleInfo.DisplayName}：";
                        var value = BackgroundInputTypeParser.Parse(_publishmentSystemInfo, _nodeId, styleInfo, _tableStyle, styleInfo.AttributeName, _formCollection, _isEdit, _isPostBack, null, pageScripts, true);

                        if (builder.Length > 0)
                        {
                            if (isPreviousSingleLine)
                            {
                                builder.Append("</tr>");
                            }
                            else
                            {
                                if (!isPreviousLeftColumn)
                                {
                                    builder.Append("</tr>");
                                }
                                else if (styleInfo.IsSingleLine)
                                {
                                    builder.Append(@"<td></td><td></td></tr>");
                                }
                            }
                        }

                        //this line

                        if (styleInfo.IsSingleLine || isPreviousSingleLine || !isPreviousLeftColumn)
                        {
                            builder.Append("<tr>");
                        }

                        if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.TextEditor))
                        {
                            var commands = WebUtils.GetTextEditorCommands(_publishmentSystemInfo, styleInfo.AttributeName);
                            builder.Append(
                                $@"<td>{text}</td><td colspan=""3"">{commands}</td></tr><tr><td colspan=""4"">{value}</td>");
                        }
                        else
                        {
                            if (styleInfo.AttributeName == "Title" || styleInfo.AttributeName == "SubTitle")
                            {
                                builder.Append(
                                    $@"<td>{text}</td><td {(styleInfo.IsSingleLine ? @"colspan=""3""" : string.Empty)}>{value}</td>");
                            }
                            else
                            {
                                builder.Append(
                                    $@"<td>{text}</td><td {(styleInfo.IsSingleLine ? @"colspan=""3""" : string.Empty)}>{value}</td>");
                            }
                        }

                        if (styleInfo.IsSingleLine)
                        {
                            isPreviousSingleLine = true;
                            isPreviousLeftColumn = false;
                        }
                        else
                        {
                            isPreviousSingleLine = false;
                            isPreviousLeftColumn = !isPreviousLeftColumn;
                        }
                    }
                }

                if (builder.Length > 0)
                {
                    if (isPreviousSingleLine || !isPreviousLeftColumn)
                    {
                        builder.Append("</tr>");
                    }
                    else
                    {
                        builder.Append(@"<td></td><td></td></tr>");
                    }
                }

                output.Write(builder.ToString());

                foreach (string key in pageScripts.Keys)
                {
                    output.Write(pageScripts[key]);
                }
            }
        }
    }
}
