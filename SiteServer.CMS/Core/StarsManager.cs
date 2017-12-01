using System;

namespace SiteServer.CMS.Core
{
    public class StarsManager
    {
        private StarsManager()
        {
        }

        public static string GetStarString(int publishmentSystemID, int channelID, int contentID)
        {
            var counts = DataProvider.StarDao.GetCount(publishmentSystemID, channelID, contentID);
            var totalCount = counts[0];
            var totalPoint = counts[1];

            var totalCountAndPointAverage = DataProvider.StarSettingDao.GetTotalCountAndPointAverage(publishmentSystemID, contentID);
            var settingTotalCount = (int)totalCountAndPointAverage[0];
            var settingPointAverage = (decimal)totalCountAndPointAverage[1];
            if (settingTotalCount > 0 || settingPointAverage > 0)
            {
                totalCount += settingTotalCount;
                totalPoint += Convert.ToInt32(settingPointAverage * settingTotalCount);
            }

            decimal num = 0;
            var display = string.Empty;
            if (totalCount > 0)
            {
                num = Convert.ToDecimal(totalPoint) / Convert.ToDecimal(totalCount);
                var numString = num.ToString();
                if (numString.IndexOf('.') == -1)
                {
                    display = numString + ".0";
                }
                else
                {
                    var first = numString.Substring(0, numString.IndexOf('.'));
                    var second = numString.Substring(numString.IndexOf('.') + 1, 1);
                    display = first + "." + second;
                }

                display = $"评分：{display} 人数：{totalCount}";
            }
            else
            {
                display = "未设置";
            }

            return display;
        }

        public static void SetStarSetting(int publishmentSystemID, int channelID, int contentID, int totalCount, decimal pointAverage)
        {
            DataProvider.StarSettingDao.SetStarSetting(publishmentSystemID, channelID, contentID, totalCount, pointAverage);
        }

        public static string GetStarSettingToExport(int publishmentSystemID, int channelID, int contentID)
        {
            var counts = DataProvider.StarDao.GetCount(publishmentSystemID, channelID, contentID);
            var totalCount = counts[0];
            var totalPoint = counts[1];

            var totalCountAndPointAverage = DataProvider.StarSettingDao.GetTotalCountAndPointAverage(publishmentSystemID, contentID);
            var settingTotalCount = (int)totalCountAndPointAverage[0];
            var settingPointAverage = (decimal)totalCountAndPointAverage[1];
            if (settingTotalCount > 0 || settingPointAverage > 0)
            {
                totalCount += settingTotalCount;
                totalPoint += Convert.ToInt32(settingPointAverage * settingTotalCount);
            }

            if (totalCount > 0)
            {
                var display = string.Empty;
                var num = Convert.ToDecimal(totalPoint) / Convert.ToDecimal(totalCount);
                var numString = num.ToString();
                if (numString.IndexOf('.') == -1)
                {
                    display = numString + ".0";
                }
                else
                {
                    var first = numString.Substring(0, numString.IndexOf('.'));
                    var second = numString.Substring(numString.IndexOf('.') + 1, 1);
                    display = first + "." + second;
                }
                return $"{totalCount}_{display}";
            }

            return string.Empty;
        }
    }
}
