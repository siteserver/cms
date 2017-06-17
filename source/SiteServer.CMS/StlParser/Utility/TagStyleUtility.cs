using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.Utility
{
    public class TagStyleUtility
    {
        public static string GetTagStyleTitle(string elementName)
        {
            if (StringUtils.EqualsIgnoreCase(elementName, StlGovPublicApply.ElementName))
            {
                return "依申请公开提交";
            }
            else if (StringUtils.EqualsIgnoreCase(elementName, StlGovPublicQuery.ElementName))
            {
                return "依申请公开查询";
            }
            else if (StringUtils.EqualsIgnoreCase(elementName, StlGovInteractApply.ElementName))
            {
                return "互动交流提交";
            }
            else if (StringUtils.EqualsIgnoreCase(elementName, StlGovInteractQuery.ElementName))
            {
                return "互动交流查询";
            }
            return string.Empty;
        }

        public static bool IsStyleVisible(string elementName)
        {
            if (StringUtils.EqualsIgnoreCase(StlComments.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlResume.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlGovPublicApply.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlGovPublicQuery.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlGovInteractApply.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlGovInteractQuery.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlCommentInput.ElementName, elementName))
            {
                return false;
            }
            return true;
        }

        public static bool IsScriptVisible(string elementName)
        {
            if (StringUtils.EqualsIgnoreCase(StlComments.ElementName, elementName))
            {
                return false;
            }
            return true;
        }

        public static bool IsSuccessVisible(string elementName)
        {
            if (StringUtils.EqualsIgnoreCase(StlGovInteractApply.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlGovInteractQuery.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlGovPublicApply.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlGovPublicQuery.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlCommentInput.ElementName, elementName))
            {
                return true;
            }
            return false;
        }

        public static bool IsFailureVisible(string elementName)
        {
            if (StringUtils.EqualsIgnoreCase(StlGovInteractApply.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlGovInteractQuery.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlGovPublicApply.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlGovPublicQuery.ElementName, elementName) || StringUtils.EqualsIgnoreCase(StlCommentInput.ElementName, elementName))
            {
                return true;
            }
            return false;
        }

        public static void IsCreateTemplate_CheckedChanged(TagStyleInfo styleInfo, PublishmentSystemInfo publishmentSystemInfo, TextBox tbContent, TextBox tbSuccess, TextBox tbFailure, TextBox tbStyle, TextBox tbScript)
        {
            if (StringUtils.EqualsIgnoreCase(StlGovPublicApply.ElementName, styleInfo.ElementName))
            {
                var applyInfo = new TagStyleGovPublicApplyInfo(styleInfo.SettingsXML);
                var applyTemplate = new GovPublicApplyTemplate(publishmentSystemInfo, styleInfo, applyInfo);
                tbContent.Text = applyTemplate.GetFileInputTemplate();
                tbSuccess.Text = applyTemplate.GetFileSuccessTemplate();
                tbFailure.Text = applyTemplate.GetFileFailureTemplate();
                tbScript.Text = applyTemplate.GetScript();
            }
            else if (StringUtils.EqualsIgnoreCase(StlGovPublicQuery.ElementName, styleInfo.ElementName))
            {
                var queryTemplate = new GovPublicQueryTemplate(publishmentSystemInfo, styleInfo);
                tbContent.Text = queryTemplate.GetFileInputTemplate();
                tbSuccess.Text = queryTemplate.GetFileSuccessTemplate();
                tbFailure.Text = queryTemplate.GetFileFailureTemplate();
                tbScript.Text = queryTemplate.GetScript();
            }
            else if (StringUtils.EqualsIgnoreCase(StlGovInteractApply.ElementName, styleInfo.ElementName))
            {
                var applyInfo = new TagStyleGovInteractApplyInfo(styleInfo.SettingsXML);
                var nodeID = DataProvider.GovInteractChannelDao.GetNodeIdByApplyStyleId(styleInfo.StyleID);
                var applyTemplate = new GovInteractApplyTemplate(publishmentSystemInfo, nodeID, styleInfo, applyInfo);
                tbContent.Text = applyTemplate.GetFileInputTemplate();
                tbSuccess.Text = applyTemplate.GetFileSuccessTemplate();
                tbFailure.Text = applyTemplate.GetFileFailureTemplate();
                tbScript.Text = applyTemplate.GetScript();
            }
            else if (StringUtils.EqualsIgnoreCase(StlGovInteractQuery.ElementName, styleInfo.ElementName))
            {
                var nodeID = DataProvider.GovInteractChannelDao.GetNodeIdByQueryStyleId(styleInfo.StyleID);
                var queryTemplate = new GovInteractQueryTemplate(publishmentSystemInfo, nodeID, styleInfo);
                tbContent.Text = queryTemplate.GetFileInputTemplate();
                tbSuccess.Text = queryTemplate.GetFileSuccessTemplate();
                tbFailure.Text = queryTemplate.GetFileFailureTemplate();
                tbScript.Text = queryTemplate.GetScript();
            }
        }
    }
}
