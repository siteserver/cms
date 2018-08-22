using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentAttributes : BasePageCms
    {
        protected CheckBox CbIsRecommend;
        protected CheckBox CbIsHot;
        protected CheckBox CbIsColor;
        protected CheckBox CbIsTop;
        protected HtmlInputHidden HihType;
        protected TextBox TbHits;

        private int _channelId;
        private string _tableName;
        private List<int> _idList;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("设置内容属性", PageUtils.GetCmsUrl(siteId, nameof(ModalContentAttributes), new NameValueCollection
            {
                {"channelId", channelId.ToString()}
            }), "contentIdCollection", "请选择需要设置属性的内容！", 450, 350);
        }

        public static string GetOpenWindowStringWithCheckBoxValue(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("设置内容属性", PageUtils.GetCmsUrl(siteId, nameof(ModalContentAttributes), new NameValueCollection
            {
                {"channelId", channelId.ToString()}
            }), "contentIdCollection", "请选择需要设置属性的内容！", 450, 350);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId");

            _channelId = AuthRequest.GetQueryInt("channelId");
            _tableName = ChannelManager.GetTableName(SiteInfo, _channelId);
            _idList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("contentIdCollection"));
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            switch (HihType.Value)
            {
                case "1":
                    if (CbIsRecommend.Checked || CbIsHot.Checked || CbIsColor.Checked || CbIsTop.Checked)
                    {
                        foreach (var contentId in _idList)
                        {
                            var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableName, contentId);
                            if (contentInfo != null)
                            {
                                if (CbIsRecommend.Checked)
                                {
                                    contentInfo.IsRecommend = true;
                                }
                                if (CbIsHot.Checked)
                                {
                                    contentInfo.IsHot = true;
                                }
                                if (CbIsColor.Checked)
                                {
                                    contentInfo.IsColor = true;
                                }
                                if (CbIsTop.Checked)
                                {
                                    contentInfo.IsTop = true;
                                }
                                DataProvider.ContentDao.Update(_tableName, SiteInfo, contentInfo);
                            }
                        }

                        AuthRequest.AddSiteLog(SiteId, "设置内容属性");

                        isChanged = true;
                    }

                    break;
                case "2":
                    if (CbIsRecommend.Checked || CbIsHot.Checked || CbIsColor.Checked || CbIsTop.Checked)
                    {
                        foreach (var contentId in _idList)
                        {
                            var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableName, contentId);
                            if (contentInfo != null)
                            {
                                if (CbIsRecommend.Checked)
                                {
                                    contentInfo.IsRecommend = false;
                                }
                                if (CbIsHot.Checked)
                                {
                                    contentInfo.IsHot = false;
                                }
                                if (CbIsColor.Checked)
                                {
                                    contentInfo.IsColor = false;
                                }
                                if (CbIsTop.Checked)
                                {
                                    contentInfo.IsTop = false;
                                }
                                DataProvider.ContentDao.Update(_tableName, SiteInfo, contentInfo);
                            }
                        }

                        AuthRequest.AddSiteLog(SiteId, "取消内容属性");

                        isChanged = true;
                    }

                    break;
                case "3":
                    var hits = TranslateUtils.ToInt(TbHits.Text);

                    foreach (var contentId in _idList)
                    {
                        DataProvider.ContentDao.SetValue(_tableName, contentId, ContentAttribute.Hits, hits.ToString());
                    }

                    AuthRequest.AddSiteLog(SiteId, "设置内容点击量");

                    isChanged = true;
                    break;
            }

            if (isChanged)
			{
                LayerUtils.Close(Page);
			}
		}

	}
}
