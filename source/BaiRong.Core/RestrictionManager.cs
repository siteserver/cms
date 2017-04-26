using System.Collections.Specialized;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core
{
	public sealed class RestrictionManager
	{
        private RestrictionManager()
		{
		}

        public static bool IsVisitAllowed(ERestrictionType restrictionType, StringCollection restrictionBlackList, StringCollection restrictionWhiteList)
        {
            if (restrictionType == ERestrictionType.NoRestriction) return true;
            var restrictionList = new StringCollection();
            if (restrictionType == ERestrictionType.BlackList)
            {
                restrictionList = restrictionBlackList;
            }
            else if (restrictionType == ERestrictionType.WhiteList)
            {
                restrictionList = restrictionWhiteList;
            }
            return IsVisitAllowed(restrictionType, restrictionList);
        }

        private static bool IsVisitAllowed(ERestrictionType restrictionType, StringCollection restrictionList)
        {
            var isAllowed = true;
            if (restrictionType != ERestrictionType.NoRestriction)
            {
                var userIP = PageUtils.GetIpAddress();
                if (restrictionType == ERestrictionType.BlackList)
                {
                    var list = new IPList();
                    foreach (var restriction in restrictionList)
                    {
                        AddRestrictionToIPList(list, restriction);
                    }
                    if (list.CheckNumber(userIP))
                    {
                        isAllowed = false;
                    }
                }
                else if (restrictionType == ERestrictionType.WhiteList)
                {
                    if (restrictionList.Count > 0)
                    {
                        isAllowed = false;
                        var list = new IPList();
                        foreach (var restriction in restrictionList)
                        {
                            AddRestrictionToIPList(list, restriction);
                        }
                        if (list.CheckNumber(userIP))
                        {
                            isAllowed = true;
                        }
                    }
                }
            }
            return isAllowed;
        }

        private static void AddRestrictionToIPList(IPList list, string restriction)
        {
            if (!string.IsNullOrEmpty(restriction))
            {
                if (StringUtils.Contains(restriction, "-"))
                {
                    restriction = restriction.Trim(' ', '-');
                    var arr = restriction.Split('-');
                    list.AddRange(arr[0].Trim(), arr[1].Trim());
                }
                else if (StringUtils.Contains(restriction, "*"))
                {
                    var ipPrefix = restriction.Substring(0, restriction.IndexOf('*'));
                    ipPrefix = ipPrefix.Trim(' ', '.');
                    var dotNum = StringUtils.GetCount(".", ipPrefix);

                    var ipNumber = ipPrefix;
                    var mask = "255.255.255.255";
                    if (dotNum == 0)
                    {
                        ipNumber = ipPrefix + ".0.0.0";
                        mask = "255.0.0.0";
                    }
                    else if (dotNum == 1)
                    {
                        ipNumber = ipPrefix + ".0.0";
                        mask = "255.255.0.0";
                    }
                    else
                    {
                        ipNumber = ipPrefix + ".0";
                        mask = "255.255.255.0";
                    }
                    list.Add(ipNumber, mask);
                }
                else
                {
                    list.Add(restriction);
                }
            }
        }
	}
}
