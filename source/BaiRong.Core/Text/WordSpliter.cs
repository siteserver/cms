using System.Collections;
using System.Text.RegularExpressions;

namespace BaiRong.Core.Text
{
    /// <summary>
    /// 分词类
    /// </summary>
    public static class WordSpliter
    {
        private const string SplitChar = " "; //分隔符

        /// <summary>
        /// 数据缓存函数
        /// </summary>
        /// <param name="key">索引键</param>
        /// <param name="val">缓存的数据</param>
        private static void SetCache(string key, object val)
        {
            if (val == null)
                val = " ";
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application.Set(key, val);
            System.Web.HttpContext.Current.Application.UnLock();
        }
        /// <summary>
        /// 读取缓存
        /// </summary>
        private static object GetCache(string key)
        {
            return System.Web.HttpContext.Current.Application.Get(key);
        }

        private static SortedList ReadContent(int publishmentSystemId)
        {
            var cacheKey = "BaiRong.Core.WordSpliter." + publishmentSystemId;
            if (GetCache(cacheKey) == null)
            {
                var arrText = new SortedList();

                var tagList = BaiRongDataProvider.TagDao.GetTagList(publishmentSystemId);
                if (tagList.Count > 0)
                {
                    foreach (var line in tagList)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            arrText.Add(line.Trim(), line.Trim());
                        }
                    }
                }
                SetCache(cacheKey, arrText);
            }
            return (SortedList)GetCache(cacheKey);
        }

        private static bool IsMatch(string str, string reg)
        {
            return new Regex(reg).IsMatch(str);
        }
        // 首先格式化字符串(粗分)

        private static string FormatStr(string val)
        {
            var result = "";
            if (string.IsNullOrEmpty(val))
                return "";
            //
            var charList = val.ToCharArray();
            //
            const string spc = SplitChar; //分隔符
            var strLen = charList.Length;
            var charType = 0; //0-空白 1-英文 2-中文 3-符号
            //
            for (var i = 0; i < strLen; i++)
            {
                var strList = charList[i].ToString();
                if (string.IsNullOrEmpty(strList))
                    continue;
                //
                if (charList[i] < 0x81)
                {
                    if (charList[i] < 33)
                    {
                        if (charType != 0 && strList != "\n" && strList != "\r")
                        {
                            result += " ";
                            charType = 0;
                        }
                    }
                    else if (IsMatch(strList, "[^0-9a-zA-Z@\\.%#:\\/&_-]"))//排除这些字符
                    {
                        if (charType == 0)
                            result += strList;
                        else
                            result += spc + strList;
                        charType = 3;
                    }
                    else
                    {
                        if (charType == 2 || charType == 3)
                        {
                            result += spc + strList;
                            charType = 1;
                        }
                        else
                        {
                            if (IsMatch(strList, "[@%#:]"))
                            {
                                result += strList;
                                charType = 3;
                            }
                            else
                            {
                                result += strList;
                                charType = 1;
                            }//end if No.4
                        }//end if No.3
                    }//end if No.2
                }//if No.1
                else
                {
                    //如果上一个字符为非中文和非空格，则加一个空格
                    if (charType != 0 && charType != 2)
                        result += spc;
                    //如果是中文标点符号
                    if (!IsMatch(strList, "^[\u4e00-\u9fa5]+$"))
                    {
                        if (charType != 0)
                            result += spc + strList;
                        else
                            result += strList;
                        charType = 3;
                    }
                    else //中文
                    {
                        result += strList;
                        charType = 2;
                    }
                }
                //end if No.1

            }//exit for
            //
            return result;
        }

        /// <summary>
        /// 分词
        /// </summary>
        /// <returns></returns>
        private static ArrayList StringSpliter(string[] key, int publishmentSystemId)
        {
            var list = new ArrayList();
            var dict = ReadContent(publishmentSystemId);//载入词典
            //
            for (var i = 0; i < key.Length; i++)
            {
                if (IsMatch(key[i], @"^(?!^\.$)([a-zA-Z0-9\.\u4e00-\u9fa5]+)$")) //中文、英文、数字
                {
                    if (IsMatch(key[i], "^[\u4e00-\u9fa5]+$"))//如果是纯中文
                    {
                        var keyLen = key[i].Length;
                        if (keyLen < 2)
                            continue;
                        else if (keyLen <= 7)
                            list.Add(key[i]);
                        //开始分词
                        for (var x = 0; x < keyLen; x++)
                        {
                            //x：起始位置//y：结束位置
                            for (var y = x; y < keyLen; y++)
                            {
                                var val = key[i].Substring(x, keyLen - y);
                                if (val.Length < 2)
                                    break;
                                else if (val.Length > 10)
                                    continue;
                                if (dict.Contains(val))
                                    list.Add(val);
                            }
                        }
                    }
                    else if (!IsMatch(key[i], @"^(\.*)$"))//不全是小数点
                    {
                        list.Add(key[i]);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 得到分词结果
        /// </summary>
        public static string[] DoSplit(string content, int publishmentSystemId)
        {
            var keyList = StringSpliter(FormatStr(content).Split(SplitChar.ToCharArray()), publishmentSystemId);
            keyList.Insert(0, content);
            //去掉重复的关键词
            for (var i = 0; i < keyList.Count; i++)
            {
                for (var j = 0; j < keyList.Count; j++)
                {
                    if (keyList[i].ToString() == keyList[j].ToString())
                    {
                        if (i != j)
                        {
                            keyList.RemoveAt(j); j--;
                        }
                    }
                }
            }
            return (string[])keyList.ToArray(typeof(string));
        }
        /// <summary>
        /// 得到分词关键字，以逗号隔开
        /// </summary>
        public static string GetKeywords(string content, int publishmentSystemId, int totalNum)
        {
            var value = "";
            var _key = DoSplit(content, publishmentSystemId);
            var currentNum = 1;
            for (var i = 1; i < _key.Length; i++)
            {
                if (totalNum > 0 && currentNum > totalNum) break;
                var key = _key[i].Trim();
                if (key.Length == 1) continue;
                if (i == 1)
                    value = key;
                else
                    value += " " + key;
                currentNum++;
            }
            return value;
        }
    }
}
