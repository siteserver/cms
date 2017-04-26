using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalFilePathRule : BasePageCms
    {
        protected Literal ltlRules;
        protected TextBox tbRule;
        protected Literal ltlTips;

        private int _nodeId;
        private bool _isChannel;
        private string _textBoxClientId = string.Empty;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, bool isChannel, string textBoxclientId)
        {
            return PageUtils.GetOpenWindowStringWithTextBoxValue(isChannel ? "栏目页文件名规则" : "内容页文件名规则", PageUtils.GetCmsUrl(nameof(ModalFilePathRule), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"IsChannel", isChannel.ToString()},
                {"TextBoxClientID", textBoxclientId}
            }), textBoxclientId);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _nodeId = Body.GetQueryInt("NodeID");
            _isChannel = Body.GetQueryBool("IsChannel");
            _textBoxClientId = Body.GetQueryString("TextBoxClientID");
			if (!IsPostBack)
			{
                ltlRules.Text = GetRulesString();
                if (!string.IsNullOrEmpty(_textBoxClientId))
                {
                    tbRule.Text = Body.GetQueryString(_textBoxClientId);
                }
                if (_isChannel)
                {
                    ltlTips.Text = "系统生成栏目页时采取的文件名规则，建议保留{@ChannelID}栏目ID项，否则可能出现重复的文件名称";
                }
                else
                {
                    ltlTips.Text = "系统生成内容页时采取的文件名规则，建议保留{@ContentID}内容ID项，否则可能出现重复的文件名称";
                }
			}
		}

        private string GetRulesString()
        {
            var retval = string.Empty;

            var builder = new StringBuilder();
            var mod = 0;
            var count = 0;
            IDictionary entitiesDictionary = null;
            if (_isChannel)
            {
                entitiesDictionary = PathUtility.ChannelFilePathRules.GetDictionary(PublishmentSystemInfo, _nodeId);
            }
            else
            {
                entitiesDictionary = PathUtility.ContentFilePathRules.GetDictionary(PublishmentSystemInfo, _nodeId);
            }
            
            foreach (string label in entitiesDictionary.Keys)
            {
                count++;
                var labelName = (string)entitiesDictionary[label];
                string td =
                    $@"<td><a href=""javascript:;"" onclick=""AddOnPos('{label}');return false;"">{label}</a></td><td>{labelName}</td>";
                if (count == entitiesDictionary.Count)
                {
                    td =
                        $@"<td><a href=""javascript:;"" onclick=""AddOnPos('{label}');return false;"">{label}</a></td><td colspan=""5"">{labelName}</td></tr>";
                }
                if (mod++ % 3 == 0)
                {
                    builder.Append("<tr>" + td);
                }
                else
                {
                    builder.Append(td);
                }
            }
            retval = builder.ToString();

            return retval;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            string scripts = $"window.parent.document.all.{_textBoxClientId}.value = '{tbRule.Text}';";
            PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
		}
	}
}
