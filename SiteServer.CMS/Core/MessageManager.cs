using BaiRong.Core;
using System.Text;
using System.Collections;
using SiteServer.CMS.Model;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using System.Collections.Generic;
using BaiRong.Core.Table;

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
                builder.Append($@"{styleInfo.DisplayName}:[{styleInfo.AttributeName}],");
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
            }

            return builder.ToString();
        }

        public static void SendSms(PublishmentSystemInfo publishmentSystemInfo, ITagStyleMailSmsBaseInfo mailSmsInfo, string tableName, int relatedIdentity, ExtendedAttributes contentInfo)
        {
            try
            {
                if (mailSmsInfo.IsSms)
                {
                    var styleInfoList = RelatedIdentities.GetTableStyleInfoList(publishmentSystemInfo, relatedIdentity);

                    var smsToArrayList = new ArrayList();
                    if (mailSmsInfo.SmsReceiver == ETriState.All || mailSmsInfo.SmsReceiver == ETriState.True)
                    {
                        if (!string.IsNullOrEmpty(mailSmsInfo.SmsTo))
                        {
                            var mobiles = mailSmsInfo.SmsTo.Split(';', ',');
                            foreach (var mobile in mobiles)
                            {
                                if (!string.IsNullOrEmpty(mobile) && StringUtils.IsMobile(mobile) && !smsToArrayList.Contains(mobile))
                                {
                                    smsToArrayList.Add(mobile);
                                }
                            }
                        }
                    }
                    if (mailSmsInfo.SmsReceiver == ETriState.All || mailSmsInfo.SmsReceiver == ETriState.False)
                    {
                        var smsTo = contentInfo.GetString(mailSmsInfo.SmsFiledName);
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

                    if (mailSmsInfo.IsSmsTemplate && !string.IsNullOrEmpty(mailSmsInfo.SmsContent))
                    {
                        builder.Append(mailSmsInfo.SmsContent);
                    }
                    else
                    {
                        builder.Append(GetSmsContent(styleInfoList));
                    }

                    var content = builder.ToString();
                    content = StringUtils.ReplaceIgnoreCase(content, "[AddDate]", DateUtils.GetDateAndTimeString(TranslateUtils.ToDateTime(contentInfo.GetString(ContentAttribute.AddDate))));

                    foreach (var styleInfo in styleInfoList)
                    {
                        var theValue = InputParserUtility.GetContentByTableStyle(contentInfo.GetString(styleInfo.AttributeName), publishmentSystemInfo, styleInfo);
                        content = StringUtils.ReplaceIgnoreCase(content, $"[{styleInfo.AttributeName}]", theValue);
                    }

                    var attributeNameList = TableMetadataManager.GetAttributeNameList(tableName);
                    foreach (var attributeName in attributeNameList)
                    {
                        var theValue = contentInfo.GetString(attributeName);
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
