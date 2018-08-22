using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
    public static class CheckManager
	{
	    public static class LevelInt
	    {
	        public const int CaoGao = -99;//草稿
	        public const int DaiShen = 0;//待审

	        public const int Pass1 = 1;//初审通过
	        public const int Pass2 = 2;//二审通过
	        public const int Pass3 = 3;//三审通过
	        public const int Pass4 = 4;//四审通过
	        public const int Pass5 = 5;//终审通过

	        public const int Fail1 = -1;//初审退稿
	        public const int Fail2 = -2;//二审退稿
	        public const int Fail3 = -3;//三审退稿
	        public const int Fail4 = -4;//四审退稿
	        public const int Fail5 = -5;//终审退稿

	        public const int NotChange = -100;//保持不变
	        public const int All = -200;//全部

            public static List<int> GetFailLevelList()
	        {
	            return new List<int>
	            {
	                Fail1,
	                Fail2,
	                Fail3,
	                Fail4,
	                Fail5
	            };
	        }

	        public static List<int> GetUnCheckedLevelList()
	        {
	            return new List<int>
	            {
	                DaiShen,
	                Pass1,
	                Pass2,
	                Pass3,
	                Pass4
	            };
	        }

	        public static List<int> GetCheckLevelList(SiteInfo siteInfo, bool isChecked, int checkedLevel)
	        {
	            if (isChecked)
	            {
	                checkedLevel = 5;
	            }

	            var list = new List<int>
	            {
	                DaiShen
	            };

	            if (checkedLevel >= 1)
	            {
	                list.Add(Fail1);
	            }

	            if (checkedLevel >= 2)
	            {
	                list.Add(Pass1);
	                list.Add(Fail2);
	            }

	            if (checkedLevel >= 3)
	            {
	                list.Add(Pass2);
	                list.Add(Fail3);
	            }

	            if (checkedLevel >= 4)
	            {
	                list.Add(Pass3);
	                list.Add(Fail4);
	            }

	            if (checkedLevel >= 5)
	            {
	                list.Add(Pass4);
	                list.Add(Fail5);
	            }

	            return list;
	        }

	        public static List<int> GetCheckLevelListOfNeedCheck(SiteInfo siteInfo, bool isChecked, int checkedLevel)
	        {
	            if (isChecked)
	            {
	                checkedLevel = 5;
	            }

	            var list = new List<int>
	            {
	                DaiShen
	            };

	            if (checkedLevel >= 2)
	            {
	                list.Add(Pass1);
	            }

	            if (checkedLevel >= 3)
	            {
	                list.Add(Pass2);
	            }

	            if (checkedLevel >= 4)
	            {
	                list.Add(Pass3);
	            }

	            if (checkedLevel >= 5)
	            {
	                list.Add(Pass4);
	            }

	            return list;
	        }

	        public static string GetLevelName(int level, SiteInfo siteInfo)
	        {
	            var retval = string.Empty;
	            if (level == CaoGao)
	            {
	                retval = "草稿";
	            }
	            else if (level == DaiShen)
	            {
	                retval = "待审核";
	            }
	            else if (level == Pass1)
	            {
	                retval = "初审通过";
	            }
	            else if (level == Pass2)
	            {
	                retval = "二审通过";
	            }
	            else if (level == Pass3)
	            {
	                retval = "三审通过";
	            }
	            else if (level == Pass4)
	            {
	                retval = "四审通过";
	            }
	            else if (level == Pass5)
	            {
	                retval = "终审通过";
	            }
	            else if (level == Fail1)
	            {
	                retval = "初审退稿";
	            }
	            else if (level == Fail2)
	            {
	                retval = "二审退稿";
	            }
	            else if (level == Fail3)
	            {
	                retval = "三审退稿";
	            }
	            else if (level == Fail4)
	            {
	                retval = "四审退稿";
	            }
	            else if (level == Fail5)
	            {
	                retval = "终审退稿";
	            }
	            else if (level == NotChange)
	            {
	                retval = "保持不变";
	            }

	            if (siteInfo.Additional.IsCheckContentLevel)
	            {
	                if (siteInfo.Additional.CheckContentLevel <= level)
	                {
	                    retval = "终审通过";
	                }
	            }
	            else
	            {
	                if (level > 1)
	                {
	                    retval = "终审通过";
	                }
	            }

	            return retval;
	        }
	    }

	    public class Level
	    {
	        public const string All = "全部";//全部
            public const string CaoGao = "草稿";//草稿
	        public const string DaiShen = "待审核";//待审
	        public const string YiShenHe = "已审核";//已审核

	        public const string NotChange = "保持不变";//保持不变
	    }

	    public class Level5
	    {
	        public const string Pass1 = "初审通过，等待二审";
	        public const string Pass2 = "二审通过，等待三审";
	        public const string Pass3 = "三审通过，等待四审";
	        public const string Pass4 = "四审通过，等待终审";
	        public const string Pass5 = "终审通过";

	        public const string Fail1 = "初审退稿";
	        public const string Fail2 = "二审退稿";
	        public const string Fail3 = "三审退稿";
	        public const string Fail4 = "四审退稿";
	        public const string Fail5 = "终审退稿";
	    }

	    public class Level4
	    {
	        public const string Pass1 = "初审通过，等待二审";
	        public const string Pass2 = "二审通过，等待三审";
	        public const string Pass3 = "三审通过，等待终审";
	        public const string Pass4 = "终审通过";

	        public const string Fail1 = "初审退稿";
	        public const string Fail2 = "二审退稿";
	        public const string Fail3 = "三审退稿";
	        public const string Fail4 = "终审退稿";
	    }

	    public class Level3
	    {
	        public const string Pass1 = "初审通过，等待二审";
	        public const string Pass2 = "二审通过，等待终审";
	        public const string Pass3 = "终审通过";

	        public const string Fail1 = "初审退稿";
	        public const string Fail2 = "二审退稿";
	        public const string Fail3 = "终审退稿";
	    }

	    public class Level2
	    {
	        public const string Pass1 = "初审通过，等待终审";
	        public const string Pass2 = "终审通过";

	        public const string Fail1 = "初审退稿";
	        public const string Fail2 = "终审退稿";
	    }

	    public class Level1
	    {
	        public const string Pass1 = "终审通过";

	        public const string Fail1 = "终审退稿";
	    }

	    public static void LoadContentLevelToEdit(ListControl listControl, SiteInfo siteInfo, int channelId, ContentInfo contentInfo, bool isChecked, int checkedLevel)
	    {
	        var checkContentLevel = siteInfo.Additional.CheckContentLevel;
	        if (isChecked)
	        {
	            checkedLevel = checkContentLevel;
	        }

	        ListItem listItem;

	        var isCheckable = false;
	        if (contentInfo != null)
	        {
	            isCheckable = IsCheckable(siteInfo, channelId, contentInfo.IsChecked, contentInfo.CheckedLevel, isChecked, checkedLevel);
	            if (isCheckable)
	            {
	                listItem = new ListItem(Level.NotChange, LevelInt.NotChange.ToString());
	                listControl.Items.Add(listItem);
	            }
	        }

	        listItem = new ListItem(Level.CaoGao, LevelInt.CaoGao.ToString());
	        listControl.Items.Add(listItem);
	        listItem = new ListItem(Level.DaiShen, LevelInt.DaiShen.ToString());
	        listControl.Items.Add(listItem);

	        if (checkContentLevel == 0 || checkContentLevel == 1)
	        {
	            listItem = new ListItem(Level1.Pass1, LevelInt.Pass1.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 2)
	        {
	            listItem = new ListItem(Level2.Pass1, LevelInt.Pass1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level2.Pass2, LevelInt.Pass2.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 3)
	        {
	            listItem = new ListItem(Level3.Pass1, LevelInt.Pass1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level3.Pass2, LevelInt.Pass2.ToString())
	            {
	                Enabled = checkedLevel >= 2
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level3.Pass3, LevelInt.Pass3.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 4)
	        {
	            listItem = new ListItem(Level4.Pass1, LevelInt.Pass1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level4.Pass2, LevelInt.Pass2.ToString())
	            {
	                Enabled = checkedLevel >= 2
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level4.Pass3, LevelInt.Pass3.ToString())
	            {
	                Enabled = checkedLevel >= 3
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level4.Pass4, LevelInt.Pass4.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 5)
	        {
	            listItem = new ListItem(Level5.Pass1, LevelInt.Pass1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level5.Pass2, LevelInt.Pass2.ToString())
	            {
	                Enabled = checkedLevel >= 2
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level5.Pass3, LevelInt.Pass3.ToString())
	            {
	                Enabled = checkedLevel >= 3
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level5.Pass4, LevelInt.Pass4.ToString())
	            {
	                Enabled = checkedLevel >= 4
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level5.Pass5, LevelInt.Pass5.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }

	        if (contentInfo == null)
	        {
	            ControlUtils.SelectSingleItem(listControl, checkedLevel.ToString());
	        }
	        else
	        {
	            ControlUtils.SelectSingleItem(listControl,
	                isCheckable ? LevelInt.NotChange.ToString() : checkedLevel.ToString());
	        }
	    }

	    public static void LoadContentLevelToList(ListControl listControl, SiteInfo siteInfo, bool isCheckOnly, bool isChecked, int checkedLevel)
	    {
	        var checkContentLevel = siteInfo.Additional.CheckContentLevel;

	        if (isChecked)
	        {
	            checkedLevel = checkContentLevel;
	        }

	        listControl.Items.Add(new ListItem(Level.All, LevelInt.All.ToString()));
	        listControl.Items.Add(new ListItem(Level.CaoGao, LevelInt.CaoGao.ToString()));
            listControl.Items.Add(new ListItem(Level.DaiShen, LevelInt.DaiShen.ToString()));

	        if (checkContentLevel == 1)
	        {
	            if (isChecked)
	            {
	                listControl.Items.Add(new ListItem(Level1.Fail1, LevelInt.Fail1.ToString()));
	            }
	        }
	        else if (checkContentLevel == 2)
	        {
	            if (checkedLevel >= 1)
	            {
	                listControl.Items.Add(new ListItem(Level2.Fail1, LevelInt.Fail1.ToString()));
	            }

	            if (isChecked)
	            {
	                listControl.Items.Add(new ListItem(Level2.Fail2, LevelInt.Fail2.ToString()));
	            }
	        }
	        else if (checkContentLevel == 3)
	        {
	            if (checkedLevel >= 1)
	            {
	                listControl.Items.Add(new ListItem(Level3.Fail1, LevelInt.Fail1.ToString()));
	            }

	            if (checkedLevel >= 2)
	            {
	                listControl.Items.Add(new ListItem(Level3.Fail2, LevelInt.Fail2.ToString()));
	            }

	            if (isChecked)
	            {
	                listControl.Items.Add(new ListItem(Level3.Fail3, LevelInt.Fail3.ToString()));
	            }
	        }
	        else if (checkContentLevel == 4)
	        {
	            if (checkedLevel >= 1)
	            {
	                listControl.Items.Add(new ListItem(Level4.Fail1, LevelInt.Fail1.ToString()));
	            }

	            if (checkedLevel >= 2)
	            {
	                listControl.Items.Add(new ListItem(Level4.Fail2, LevelInt.Fail2.ToString()));
	            }

	            if (checkedLevel >= 3)
	            {
	                listControl.Items.Add(new ListItem(Level4.Fail3, LevelInt.Fail3.ToString()));
	            }

	            if (isChecked)
	            {
	                listControl.Items.Add(new ListItem(Level4.Fail4, LevelInt.Fail4.ToString()));
	            }
	        }
	        else if (checkContentLevel == 5)
	        {
	            if (checkedLevel >= 1)
	            {
	                listControl.Items.Add(new ListItem(Level5.Fail1, LevelInt.Fail1.ToString()));
	            }

	            if (checkedLevel >= 2)
	            {
	                listControl.Items.Add(new ListItem(Level5.Fail2, LevelInt.Fail2.ToString()));
	            }

	            if (checkedLevel >= 3)
	            {
	                listControl.Items.Add(new ListItem(Level5.Fail3, LevelInt.Fail3.ToString()));
	            }

	            if (checkedLevel >= 4)
	            {
	                listControl.Items.Add(new ListItem(Level5.Fail4, LevelInt.Fail4.ToString()));
	            }

	            if (isChecked)
	            {
	                listControl.Items.Add(new ListItem(Level5.Fail5, LevelInt.Fail5.ToString()));
	            }
	        }

	        if (isCheckOnly) return;

            if (checkContentLevel == 1)
	        {
	            if (isChecked)
	            {
	                listControl.Items.Add(new ListItem(Level1.Pass1, LevelInt.Pass1.ToString()));
                }
            }
            if (checkContentLevel == 2)
	        {
	            if (checkedLevel >= 1)
	            {
	                listControl.Items.Add(new ListItem(Level2.Pass1, LevelInt.Pass1.ToString()));
	            }

	            if (isChecked)
	            {
	                listControl.Items.Add(new ListItem(Level2.Pass2, LevelInt.Pass2.ToString()));
	            }
	        }
	        else if (checkContentLevel == 3)
	        {
	            if (checkedLevel >= 1)
	            {
	                listControl.Items.Add(new ListItem(Level3.Pass1, LevelInt.Pass1.ToString()));
	            }

	            if (checkedLevel >= 2)
	            {
	                listControl.Items.Add(new ListItem(Level3.Pass2, LevelInt.Pass2.ToString()));
	            }

	            if (isChecked)
	            {
	                listControl.Items.Add(new ListItem(Level3.Pass3, LevelInt.Pass3.ToString()));
                }
            }
	        else if (checkContentLevel == 4)
	        {
	            if (checkedLevel >= 1)
	            {
	                listControl.Items.Add(new ListItem(Level4.Pass1, LevelInt.Pass1.ToString()));
	            }

	            if (checkedLevel >= 2)
	            {
	                listControl.Items.Add(new ListItem(Level4.Pass2, LevelInt.Pass2.ToString()));
	            }

	            if (checkedLevel >= 3)
	            {
	                listControl.Items.Add(new ListItem(Level4.Pass3, LevelInt.Pass3.ToString()));
	            }

	            if (isChecked)
	            {
	                listControl.Items.Add(new ListItem(Level4.Pass4, LevelInt.Pass4.ToString()));
	            }
            }
	        else if (checkContentLevel == 5)
	        {
	            if (checkedLevel >= 2)
	            {
	                listControl.Items.Add(new ListItem(Level5.Pass1, LevelInt.Pass1.ToString()));
	            }

	            if (checkedLevel >= 3)
	            {
	                listControl.Items.Add(new ListItem(Level5.Pass2, LevelInt.Pass2.ToString()));
	            }

	            if (checkedLevel >= 4)
	            {
	                listControl.Items.Add(new ListItem(Level5.Pass3, LevelInt.Pass3.ToString()));
	            }

	            if (checkedLevel >= 5)
	            {
	                listControl.Items.Add(new ListItem(Level5.Pass4, LevelInt.Pass4.ToString()));
	            }

	            if (isChecked)
	            {
	                listControl.Items.Add(new ListItem(Level5.Pass5, LevelInt.Pass5.ToString()));
	            }
            }
	    }

	    public static void LoadContentLevelToCheck(ListControl listControl, SiteInfo siteInfo, bool isChecked, int checkedLevel)
	    {
	        var checkContentLevel = siteInfo.Additional.CheckContentLevel;
	        if (isChecked)
	        {
	            checkedLevel = checkContentLevel;
	        }

	        var listItem = new ListItem(Level.CaoGao, LevelInt.CaoGao.ToString());
	        listControl.Items.Add(listItem);

	        listItem = new ListItem(Level.DaiShen, LevelInt.DaiShen.ToString());
	        listControl.Items.Add(listItem);

	        if (checkContentLevel == 1)
	        {
	            listItem = new ListItem(Level1.Fail1, LevelInt.Fail1.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 2)
	        {
	            listItem = new ListItem(Level2.Fail1, LevelInt.Fail1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level2.Fail2, LevelInt.Fail2.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 3)
	        {
	            listItem = new ListItem(Level3.Fail1, LevelInt.Fail1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level3.Fail2, LevelInt.Fail2.ToString())
	            {
	                Enabled = checkedLevel >= 2
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level3.Fail3, LevelInt.Fail3.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 4)
	        {
	            listItem = new ListItem(Level4.Fail1, LevelInt.Fail1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level4.Fail2, LevelInt.Fail2.ToString())
	            {
	                Enabled = checkedLevel >= 2
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level4.Fail3, LevelInt.Fail3.ToString())
	            {
	                Enabled = checkedLevel >= 3
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level4.Fail4, LevelInt.Fail4.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 5)
	        {
	            listItem = new ListItem(Level5.Fail1, LevelInt.Fail1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level5.Fail2, LevelInt.Fail2.ToString())
	            {
	                Enabled = checkedLevel >= 2
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level5.Fail3, LevelInt.Fail3.ToString())
	            {
	                Enabled = checkedLevel >= 3
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level5.Fail4, LevelInt.Fail4.ToString())
	            {
	                Enabled = checkedLevel >= 4
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level5.Fail5, LevelInt.Fail5.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }

	        if (checkContentLevel == 0 || checkContentLevel == 1)
	        {
	            listItem = new ListItem(Level1.Pass1, LevelInt.Pass1.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 2)
	        {
	            listItem = new ListItem(Level2.Pass1, LevelInt.Pass1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);

	            listItem = new ListItem(Level2.Pass2, LevelInt.Pass2.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 3)
	        {
	            listItem = new ListItem(Level3.Pass1, LevelInt.Pass1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level3.Pass2, LevelInt.Pass2.ToString())
	            {
	                Enabled = checkedLevel >= 2
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level3.Pass3, LevelInt.Pass3.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 4)
	        {
	            listItem = new ListItem(Level4.Pass1, LevelInt.Pass1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level4.Pass2, LevelInt.Pass2.ToString())
	            {
	                Enabled = checkedLevel >= 2
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level4.Pass3, LevelInt.Pass3.ToString())
	            {
	                Enabled = checkedLevel >= 3
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level4.Pass4, LevelInt.Pass4.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }
	        else if (checkContentLevel == 5)
	        {
	            listItem = new ListItem(Level5.Pass1, LevelInt.Pass1.ToString())
	            {
	                Enabled = checkedLevel >= 1
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level5.Pass2, LevelInt.Pass2.ToString())
	            {
	                Enabled = checkedLevel >= 2
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level5.Pass3, LevelInt.Pass3.ToString())
	            {
	                Enabled = checkedLevel >= 3
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level5.Pass4, LevelInt.Pass4.ToString())
	            {
	                Enabled = checkedLevel >= 4
	            };
	            listControl.Items.Add(listItem);
	            listItem = new ListItem(Level5.Pass5, LevelInt.Pass5.ToString())
	            {
	                Enabled = isChecked
	            };
	            listControl.Items.Add(listItem);
	        }

	        ControlUtils.SelectSingleItem(listControl, checkedLevel.ToString());
	    }

	    public static List<int> GetCheckLevelToPassList(SiteInfo siteInfo)
	    {
	        var list = new List<int>();
	        var checkContentLevel = siteInfo.Additional.CheckContentLevel;

	        list.Add(LevelInt.DaiShen);

	        if (checkContentLevel == 0 || checkContentLevel == 1)
	        {
	            list.Add(LevelInt.Pass1);
	        }
	        else if (checkContentLevel == 2)
	        {
	            list.Add(LevelInt.Pass1);
	            list.Add(LevelInt.Pass2);
	        }
	        else if (checkContentLevel == 3)
	        {
	            list.Add(LevelInt.Pass1);
	            list.Add(LevelInt.Pass2);
	            list.Add(LevelInt.Pass3);
	        }
	        else if (checkContentLevel == 4)
	        {
	            list.Add(LevelInt.Pass1);
	            list.Add(LevelInt.Pass2);
	            list.Add(LevelInt.Pass3);
	            list.Add(LevelInt.Pass4);
	        }
	        else if (checkContentLevel == 5)
	        {
	            list.Add(LevelInt.Pass1);
	            list.Add(LevelInt.Pass2);
	            list.Add(LevelInt.Pass3);
	            list.Add(LevelInt.Pass4);
	            list.Add(LevelInt.Pass5);
	        }

	        return list;
	    }

	    public static string GetCheckState(SiteInfo siteInfo, bool isChecked, int level)
	    {
	        if (isChecked)
	        {
	            return Level.YiShenHe;
	        }

	        var retval = String.Empty;

	        if (level == LevelInt.CaoGao)
	        {
	            retval = Level.CaoGao;
	        }
	        else if (level == LevelInt.DaiShen)
	        {
	            retval = Level.DaiShen;
	        }
	        else
	        {
	            var checkContentLevel = siteInfo.Additional.CheckContentLevel;

	            if (checkContentLevel == 1)
	            {
	                if (level == LevelInt.Fail1)
	                {
	                    retval = Level1.Fail1;
	                }
	            }
	            else if (checkContentLevel == 2)
	            {
	                if (level == LevelInt.Pass1)
	                {
	                    retval = Level2.Pass1;
	                }
	                else if (level == LevelInt.Fail1)
	                {
	                    retval = Level2.Fail1;
	                }
	                else if (level == LevelInt.Fail2)
	                {
	                    retval = Level2.Fail2;
	                }
	            }
	            else if (checkContentLevel == 3)
	            {
	                if (level == LevelInt.Pass1)
	                {
	                    retval = Level3.Pass1;
	                }
	                else if (level == LevelInt.Pass2)
	                {
	                    retval = Level3.Pass2;
	                }
	                else if (level == LevelInt.Fail1)
	                {
	                    retval = Level3.Fail1;
	                }
	                else if (level == LevelInt.Fail2)
	                {
	                    retval = Level3.Fail2;
	                }
	                else if (level == LevelInt.Fail3)
	                {
	                    retval = Level3.Fail3;
	                }
	            }
	            else if (checkContentLevel == 4)
	            {
	                if (level == LevelInt.Pass1)
	                {
	                    retval = Level4.Pass1;
	                }
	                else if (level == LevelInt.Pass2)
	                {
	                    retval = Level4.Pass2;
	                }
	                else if (level == LevelInt.Pass3)
	                {
	                    retval = Level4.Pass3;
	                }
	                else if (level == LevelInt.Fail1)
	                {
	                    retval = Level4.Fail1;
	                }
	                else if (level == LevelInt.Fail2)
	                {
	                    retval = Level4.Fail2;
	                }
	                else if (level == LevelInt.Fail3)
	                {
	                    retval = Level4.Fail3;
	                }
	                else if (level == LevelInt.Fail4)
	                {
	                    retval = Level4.Fail4;
	                }
	            }
	            else if (checkContentLevel == 5)
	            {
	                if (level == LevelInt.Pass1)
	                {
	                    retval = Level5.Pass1;
	                }
	                else if (level == LevelInt.Pass2)
	                {
	                    retval = Level5.Pass2;
	                }
	                else if (level == LevelInt.Pass3)
	                {
	                    retval = Level5.Pass3;
	                }
	                else if (level == LevelInt.Pass4)
	                {
	                    retval = Level5.Pass4;
	                }
	                else if (level == LevelInt.Fail1)
	                {
	                    retval = Level5.Fail1;
	                }
	                else if (level == LevelInt.Fail2)
	                {
	                    retval = Level5.Fail2;
	                }
	                else if (level == LevelInt.Fail3)
	                {
	                    retval = Level5.Fail3;
	                }
	                else if (level == LevelInt.Fail4)
	                {
	                    retval = Level5.Fail4;
	                }
	                else if (level == LevelInt.Fail5)
	                {
	                    retval = Level5.Fail5;
	                }
	            }

	            if (String.IsNullOrEmpty(retval))
	            {
	                if (checkContentLevel == 1)
	                {
	                    retval = Level.DaiShen;
	                }
	                else if (checkContentLevel == 2)
	                {
	                    retval = Level2.Pass1;
	                }
	                else if (checkContentLevel == 3)
	                {
	                    retval = Level3.Pass2;
	                }
	                else if (checkContentLevel == 4)
	                {
	                    retval = Level4.Pass3;
	                }
	            }
	        }

	        return $"<span style='color:red'>{retval}</span>";
	    }

	    public static bool IsCheckable(SiteInfo siteInfo, int channelId, bool contentIsChecked, int contentCheckLevel, bool isChecked, int checkedLevel)
	    {
	        if (isChecked || checkedLevel >= 5)
	        {
	            return true;
	        }
	        if (contentIsChecked)
	        {
	            return false;
	        }
	        if (checkedLevel == 0)
	        {
	            return false;
	        }
	        if (checkedLevel == 1)
	        {
	            if (contentCheckLevel == LevelInt.CaoGao || contentCheckLevel == LevelInt.DaiShen || contentCheckLevel == LevelInt.Pass1 || contentCheckLevel == LevelInt.Fail1)
	            {
	                return true;
	            }
	            return false;
	        }
	        if (checkedLevel == 2)
	        {
	            if (contentCheckLevel == LevelInt.CaoGao || contentCheckLevel == LevelInt.DaiShen || contentCheckLevel == LevelInt.Pass1 || contentCheckLevel == LevelInt.Pass2 || contentCheckLevel == LevelInt.Fail1 || contentCheckLevel == LevelInt.Fail2)
	            {
	                return true;
	            }
	            return false;
	        }
	        if (checkedLevel == 3)
	        {
	            if (contentCheckLevel == LevelInt.CaoGao || contentCheckLevel == LevelInt.DaiShen || contentCheckLevel == LevelInt.Pass1 || contentCheckLevel == LevelInt.Pass2 || contentCheckLevel == LevelInt.Pass3 || contentCheckLevel == LevelInt.Fail1 || contentCheckLevel == LevelInt.Fail2 || contentCheckLevel == LevelInt.Fail3)
	            {
	                return true;
	            }
	            return false;
	        }
	        if (checkedLevel == 4)
	        {
	            if (contentCheckLevel == LevelInt.CaoGao || contentCheckLevel == LevelInt.DaiShen || contentCheckLevel == LevelInt.Pass1 || contentCheckLevel == LevelInt.Pass2 || contentCheckLevel == LevelInt.Pass3 || contentCheckLevel == LevelInt.Pass4 || contentCheckLevel == LevelInt.Fail1 || contentCheckLevel == LevelInt.Fail2 || contentCheckLevel == LevelInt.Fail3 || contentCheckLevel == LevelInt.Fail4)
	            {
	                return true;
	            }
	            return false;
	        }

	        return false;
	    }

	    public static KeyValuePair<bool, int> GetUserCheckLevel(PermissionManager permissionManager, SiteInfo siteInfo, int channelId)
        {
            if (permissionManager.IsSystemAdministrator)
            {
                return new KeyValuePair<bool, int>(true, siteInfo.Additional.CheckContentLevel);
            }

            var isChecked = false;
            var checkedLevel = 0;
            if (siteInfo.Additional.IsCheckContentLevel == false)
            {
                if (permissionManager.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheck))
                {
                    isChecked = true;
                }
            }
            else
            {
                if (permissionManager.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel5))
                {
                    isChecked = true;
                }
                else if (permissionManager.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel4))
                {
                    if (siteInfo.Additional.CheckContentLevel <= 4)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 4;
                    }
                }
                else if (permissionManager.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel3))
                {
                    if (siteInfo.Additional.CheckContentLevel <= 3)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 3;
                    }
                }
                else if (permissionManager.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel2))
                {
                    if (siteInfo.Additional.CheckContentLevel <= 2)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 2;
                    }
                }
                else if (permissionManager.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel1))
                {
                    if (siteInfo.Additional.CheckContentLevel <= 1)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 1;
                    }
                }
                else
                {
                    checkedLevel = 0;
                }
            }
            return new KeyValuePair<bool, int>(isChecked, checkedLevel);
        }

        public static bool GetUserCheckLevel(PermissionManager permissionManager, SiteInfo siteInfo, int channelId, out int userCheckedLevel)
        {
            var checkContentLevel = siteInfo.Additional.CheckContentLevel;

            var pair = GetUserCheckLevel(permissionManager, siteInfo, channelId);
            var isChecked = pair.Key;
            var checkedLevel = pair.Value;
            if (isChecked)
            {
                checkedLevel = checkContentLevel;
            }
            userCheckedLevel = checkedLevel;
            return isChecked;
        }

        public static List<KeyValuePair<int, int>> GetUserCountListUnChecked(PermissionManager permissionManager)
        {
            var list = new List<KeyValuePair<int, int>>();

            var tableNameList = DataProvider.TableDao.GetTableNameListCreatedInDb();

            foreach (var tableName in tableNameList)
            {
                list.AddRange(GetUserCountListUnChecked(permissionManager, tableName));
            }

            return list;
        }

        private static List<KeyValuePair<int, int>> GetUserCountListUnChecked(PermissionManager permissionManager, string tableName)
        {
            return DataProvider.ContentDao.GetCountListUnChecked(permissionManager, tableName);
        }
	}
}
