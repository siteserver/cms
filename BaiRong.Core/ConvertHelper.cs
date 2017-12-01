using System;

namespace BaiRong.Core
{
    /// <summary>
    /// 用于把对象型数据转成特定数据类型的类
    /// </summary>
    public class ConvertHelper
    {
        #region 将对象变量转成字符串变量的方法
        /// <summary>
        /// 将对象变量转成字符串变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>字符串变量</returns>
        public static string GetString(object obj)
        {
            return (obj == DBNull.Value || obj == null) ? "" : obj.ToString();
        }
        #endregion

        #region 将对象变量转成32位整数型变量的方法
        /// <summary>
        /// 将对象变量转成32位整数型变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>32位整数型变量</returns>
        public static int GetInteger(object obj)
        {
            return ConvertStringToInteger(GetString(obj));
        }
        #endregion

        #region 将对象变量转成64位整数型变量的方法
        /// <summary>
        /// 将对象变量转成64位整数型变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>64位整数型变量</returns>
        public static long GetLong(object obj)
        {
            return ConvertStringToLong(GetString(obj));
        }
        #endregion

        #region 将对象变量转成双精度浮点型变量的方法
        /// <summary>
        /// 将对象变量转成双精度浮点型变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>双精度浮点型变量</returns>
        public static double GetDouble(object obj)
        {
            return ConvertStringToDouble(GetString(obj));
        }
        #endregion

        #region 将对象变量转成十进制数字变量的方法
        /// <summary>
        /// 将对象变量转成十进制数字变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>十进制数字变量</returns>
        public static decimal GetDecimal(object obj)
        {
            return ConvertStringToDecimal(GetString(obj));
        }
        #endregion

        #region 将对象变量转成布尔型变量的方法
        /// <summary>
        /// 将对象变量转成布尔型变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>布尔型变量</returns>
        public static bool GetBoolean(object obj)
        {
            return (obj == DBNull.Value || obj == null) ? false :
                GetString(obj).ToLower() == "true" ? true : false;
        }
        #endregion

        #region 将对象变量转成日期时间型字符串变量的方法
        /// <summary>
        /// 将对象变量转成日期时间型字符串变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <param name="sFormat">时间字符串格式，例：yyyy-MM-dd</param>
        /// <returns>时间型字符串变量</returns>
        public static string GetDateTimeString(object obj, string sFormat)
        {
            return GetDateTime(obj).ToString(sFormat);
        }
        #endregion

        #region 将对象变量转成日期字符串变量的方法
        /// <summary>
        /// 将对象变量转成日期字符串变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>日期字符串变量</returns>
        public static string GetShortDateString(object obj)
        {
            return GetDateTimeString(obj, "yyyy-MM-dd");
        }
        #endregion

        #region 将对象变量转成日期型变量的方法
        /// <summary>
        /// 将对象变量转成日期型变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>日期型变量</returns>
        public static DateTime GetDateTime(object obj)
        {
            DateTime result;
            DateTime.TryParse(GetString(obj), out result);
            return result;
        }
        #endregion

        #region 私有方法

        #region 将字符串转成32位整数型变量的方法
        /// <summary>
        /// 将字符串转成32位整数型变量的方法
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>32位整数型变量</returns>
        private static int ConvertStringToInteger(string s)
        {
            int result;
            int.TryParse(s, out result);
            return result;
        }
        #endregion

        #region 将字符串转成64位整数型变量的方法
        /// <summary>
        /// 将字符串转成64位整数型变量的方法
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>64位整数型变量</returns>
        private static long ConvertStringToLong(string s)
        {
            long result;
            long.TryParse(s, out result);
            return result;
        }
        #endregion

        #region 将字符串转成双精度浮点型变量的方法
        /// <summary>
        /// 将字符串转成双精度浮点型变量的方法
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>双精度浮点型变量</returns>
        private static double ConvertStringToDouble(string s)
        {
            double result;
            double.TryParse(s, out result);
            return result;
        }
        #endregion

        #region 将字符串转成十进制数字变量的方法
        /// <summary>
        /// 将字符串转成十进制数字变量的方法
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>十进制数字变量</returns>
        private static decimal ConvertStringToDecimal(string s)
        {
            decimal result;
            decimal.TryParse(s, out result);
            return result;
        }
        #endregion

        #endregion
    }
}
