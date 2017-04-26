using System;
using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Manager
{
    public class WeiXinManager
    {
        private const string COOKIE_SN_NAME = "CookieSN";

        private const string COOKIE_WXOPENID_NAME = "CookieWXOpenID";

        public static string GetCookieSN()
        {
            if (CookieUtils.IsExists(COOKIE_SN_NAME))
            {
                return CookieUtils.GetCookie(COOKIE_SN_NAME);
            }
            else
            {
                var value = StringUtils.GetShortGuid();
                CookieUtils.SetCookie(COOKIE_SN_NAME, value, DateTime.MaxValue);
                return value;
            }
        }

        public static string GetCookieWXOpenID(string wxOpenID)
        {
            if (CookieUtils.IsExists(COOKIE_WXOPENID_NAME))
            {
                return CookieUtils.GetCookie(COOKIE_WXOPENID_NAME);
            }
            else
            {
                CookieUtils.SetCookie(COOKIE_WXOPENID_NAME, wxOpenID, DateTime.MaxValue);
                return wxOpenID;
            }
        }

        public static AccountInfo GetAccountInfo(int publishmentSystemID)
        {
            return DataProviderWX.AccountDAO.GetAccountInfo(publishmentSystemID);
        }
        public static bool IsBinding(AccountInfo accountInfo)
        {
            var isBinding = false;

            var accountType = EWXAccountTypeUtils.GetEnumType(accountInfo.AccountType);
            if (accountType == EWXAccountType.Subscribe)
            {
                isBinding = accountInfo.IsBinding;
            }
            else
            {
                isBinding = accountInfo.IsBinding && !string.IsNullOrEmpty(accountInfo.AppID) && !string.IsNullOrEmpty(accountInfo.AppSecret);
            }

            return isBinding;
        }

        public static int Lottery(Dictionary<int, decimal> idWithProbabilityDictionary)
        {
            var id = 0;

            decimal totalProbability = 0;
            foreach (var item in idWithProbabilityDictionary)
            {
                totalProbability += item.Value;
            }
            if (totalProbability < 100)
            {
                idWithProbabilityDictionary.Add(0, 100 - totalProbability);
            }

            decimal startProbability = 0;
            var idWithAreaDictionary = new Dictionary<int, KeyValuePair<decimal, decimal>>();
            foreach (var item in idWithProbabilityDictionary)
            {
                var start = startProbability;
                var end = start + item.Value;

                idWithAreaDictionary.Add(item.Key, new KeyValuePair<decimal, decimal>(start, end));

                startProbability = end + 1;
            }

            var random = new Random();
            var r = random.Next(1, 10000);

            foreach (var item in idWithAreaDictionary)
            {
                var area = item.Value;
                if (r >= area.Key * 100 && r <= area.Value * 100)
                {
                    id = item.Key;
                    break;
                }
            }

            return id;
        }
	}
}
