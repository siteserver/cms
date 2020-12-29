using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    /// <summary>
    /// 分词类
    /// </summary>
    public class WordSpliter
    {
        private const string SplitChar = " "; //分隔符

        private readonly IContentTagRepository _contentTagRepository;

        public WordSpliter(IContentTagRepository contentTagRepository)
        {
            _contentTagRepository = contentTagRepository;
        }

        private async Task<SortedList> ReadContentAsync(int siteId)
        {
            var arrText = new SortedList();

            var tagList = await _contentTagRepository.GetTagNamesAsync(siteId);
            if (tagList.Any())
            {
                foreach (var line in tagList)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        arrText.Add(line.Trim(), line.Trim());
                    }
                }
            }

            return arrText;
        }

        private bool IsMatch(string str, string reg)
        {
            return new Regex(reg).IsMatch(str);
        }
        // 首先格式化字符串(粗分)

        private string FormatStr(string val)
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
        private async Task<List<string>> StringSpliterAsync(string[] key, int siteId)
        {
            var list = new List<string>();
            var dict = await ReadContentAsync(siteId);//载入词典
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
        public async Task<List<string>> DoSplitAsync(string content, int siteId)
        {
            var keyList = await StringSpliterAsync(FormatStr(content).Split(SplitChar.ToCharArray()), siteId);
            keyList.Insert(0, content);
            //去掉重复的关键词
            for (var i = 0; i < keyList.Count; i++)
            {
                for (var j = 0; j < keyList.Count; j++)
                {
                    if (keyList[i] == keyList[j])
                    {
                        if (i != j)
                        {
                            keyList.RemoveAt(j); j--;
                        }
                    }
                }
            }
            return keyList;
        }

        /// <summary>
        /// 得到分词关键字，以逗号隔开
        /// </summary>
        public async Task<List<string>> GetKeywordsAsync(string content, int siteId, int totalNum)
        {
            content = StringUtils.StripTags(content);
            if (string.IsNullOrEmpty(content)) return new List<string>();

            var tags = new List<string>();
            var keys = await DoSplitAsync(content, siteId);
            var currentNum = 1;
            for (var i = 1; i < keys.Count; i++)
            {
                if (totalNum > 0 && currentNum > totalNum) break;
                var key = StringUtils.Trim(keys[i]);
                if (string.IsNullOrEmpty(key) || key.Length == 1) continue;

                tags.Add(key);
                currentNum++;
            }
            return tags;
        }
    }
}
