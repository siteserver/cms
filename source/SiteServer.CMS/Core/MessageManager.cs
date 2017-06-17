using BaiRong.Core;
using System.Collections.Specialized;
using System.Text;
using System.Collections;
using SiteServer.CMS.Model;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;
using System.Collections.Generic;

namespace SiteServer.CMS.Core
{
    public class MessageManager
    {
        private MessageManager()
        {
        }

        public static string GetSmsContent(List<TableStyleInfo> styleInfoList)
        {
            var builder = new StringBuilder();

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.IsVisible == false) continue;

                builder.Append($@"{styleInfo.DisplayName}:[{styleInfo.AttributeName}],");
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
            }

            return builder.ToString();
        }

        public static void SendSmsByGovPublicApply(PublishmentSystemInfo publishmentSystemInfo, TagStyleGovPublicApplyInfo tagStyleInfo, GovPublicApplyInfo applyInfo)
        {
            try
            {
                if (tagStyleInfo.IsSMS && !string.IsNullOrEmpty(tagStyleInfo.SMSTo))
                {
                    var mobiles = tagStyleInfo.SMSTo.Split(';', ',');
                    var mobileArrayList = new ArrayList();

                    foreach (var mobile in mobiles)
                    {
                        if (!string.IsNullOrEmpty(mobile) && StringUtils.IsMobile(mobile) && !mobileArrayList.Contains(mobile))
                        {
                            mobileArrayList.Add(mobile);
                        }
                    }

                    var builder = new StringBuilder(tagStyleInfo.SMSTitle);
                    var attributes = new NameValueCollection {["申请人类型"] = "公民"};
                    if (TranslateUtils.ToBool(applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.IsOrganization)))
                    {
                        attributes["申请人类型"] = "法人/其他组织";
                    }
                    attributes["申请时间"] = DateUtils.GetDateAndTimeString(applyInfo.AddDate);
                    foreach (string key in attributes.Keys)
                    {
                        var theValue = attributes[key];

                        builder.Append($@"{key}：{theValue},");
                    }

                    if (builder.Length > 0)
                    {
                        builder.Length = builder.Length - 1;
                    }

                    //var errorMessage = string.Empty;
                    //var providerInfo = BaiRongDataProvider.SmsProviderDAO.GetFirstSmsProviderInfo();
                    //if (providerInfo != null)
                    //{
                    //    SmsProviderManager.Send(providerInfo, mobileArrayList, builder.ToString(), out errorMessage);
                    //}
                }
            }
            catch
            {
                // ignored
            }
        }

        public static void SendSms(PublishmentSystemInfo publishmentSystemInfo, ITagStyleMailSMSBaseInfo mailSmsInfo, ETableStyle tableStyle, string tableName, int relatedIdentity, ExtendedAttributes contentInfo)
        {
            try
            {
                if (mailSmsInfo.IsSMS)
                {
                    var styleInfoList = RelatedIdentities.GetTableStyleInfoList(publishmentSystemInfo, tableStyle, relatedIdentity);

                    var smsToArrayList = new ArrayList();
                    if (mailSmsInfo.SMSReceiver == ETriState.All || mailSmsInfo.SMSReceiver == ETriState.True)
                    {
                        if (!string.IsNullOrEmpty(mailSmsInfo.SMSTo))
                        {
                            var mobiles = mailSmsInfo.SMSTo.Split(';', ',');
                            foreach (var mobile in mobiles)
                            {
                                if (!string.IsNullOrEmpty(mobile) && StringUtils.IsMobile(mobile) && !smsToArrayList.Contains(mobile))
                                {
                                    smsToArrayList.Add(mobile);
                                }
                            }
                        }
                    }
                    if (mailSmsInfo.SMSReceiver == ETriState.All || mailSmsInfo.SMSReceiver == ETriState.False)
                    {
                        var smsTo = contentInfo.GetExtendedAttribute(mailSmsInfo.SMSFiledName);
                        if (!string.IsNullOrEmpty(smsTo))
                        {
                            var mobiles = smsTo.Split(';', ',');
                            foreach (var mobile in mobiles)
                            {
                                if (!string.IsNullOrEmpty(mobile) && StringUtils.IsMobile(mobile) && !smsToArrayList.Contains(mobile))
                                {
                                    smsToArrayList.Add(mobile);
                                }
                            }
                        }
                    }

                    var builder = new StringBuilder();

                    if (mailSmsInfo.IsSMSTemplate && !string.IsNullOrEmpty(mailSmsInfo.SMSContent))
                    {
                        builder.Append(mailSmsInfo.SMSContent);
                    }
                    else
                    {
                        builder.Append(GetSmsContent(styleInfoList));
                    }

                    var content = builder.ToString();
                    content = StringUtils.ReplaceIgnoreCase(content, "[AddDate]", DateUtils.GetDateAndTimeString(TranslateUtils.ToDateTime(contentInfo.GetExtendedAttribute(ContentAttribute.AddDate))));

                    foreach (var styleInfo in styleInfoList)
                    {
                        var theValue = InputParserUtility.GetContentByTableStyle(contentInfo.GetExtendedAttribute(styleInfo.AttributeName), publishmentSystemInfo, tableStyle, styleInfo);
                        content = StringUtils.ReplaceIgnoreCase(content, $"[{styleInfo.AttributeName}]", theValue);
                    }

                    var attributeNameList = TableManager.GetAttributeNameList(tableStyle, tableName);
                    foreach (var attributeName in attributeNameList)
                    {
                        var theValue = contentInfo.GetExtendedAttribute(attributeName);
                        content = StringUtils.ReplaceIgnoreCase(content, $"[{attributeName}]", theValue);
                    }

                    //var errorMessage = string.Empty;
                    //var providerInfo = BaiRongDataProvider.SmsProviderDAO.GetFirstSmsProviderInfo();
                    //if (providerInfo != null)
                    //{
                    //    SmsProviderManager.Send(providerInfo, smsToArrayList, content, out errorMessage);
                    //}
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
