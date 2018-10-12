using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;

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
	    }

	    public static class Level
	    {
	        public const string All = "全部";//全部
            public const string CaoGao = "草稿";//草稿
	        public const string DaiShen = "待审核";//待审
	        public const string YiShenHe = "已审核";//已审核

	        public const string NotChange = "保持不变";//保持不变
	    }

	    private static class Level5
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

	    private static class Level4
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

	    private static class Level3
	    {
	        public const string Pass1 = "初审通过，等待二审";
	        public const string Pass2 = "二审通过，等待终审";
	        public const string Pass3 = "终审通过";

	        public const string Fail1 = "初审退稿";
	        public const string Fail2 = "二审退稿";
	        public const string Fail3 = "终审退稿";
	    }

	    private static class Level2
	    {
	        public const string Pass1 = "初审通过，等待终审";
	        public const string Pass2 = "终审通过";

	        public const string Fail1 = "初审退稿";
	        public const string Fail2 = "终审退稿";
	    }

	    private static class Level1
	    {
	        public const string Pass1 = "终审通过";

	        public const string Fail1 = "终审退稿";
	    }

        public static List<KeyValuePair<int, string>> GetCheckedLevels(SiteInfo siteInfo, bool isChecked, int checkedLevel, bool includeFail)
        {
            var checkedLevels = new List<KeyValuePair<int, string>>();

            var checkContentLevel = siteInfo.Additional.CheckContentLevel;
            if (isChecked)
            {
                checkedLevel = checkContentLevel;
            }

            checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.CaoGao, Level.CaoGao));
            checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.DaiShen, Level.DaiShen));

            if (checkContentLevel == 0 || checkContentLevel == 1)
            {
                if (isChecked)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass1, Level1.Pass1));
                }
            }
            else if (checkContentLevel == 2)
            {
                if (checkedLevel >= 1)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass1, Level2.Pass1));
                }

                if (isChecked)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass2, Level2.Pass2));
                }
            }
            else if (checkContentLevel == 3)
            {
                if (checkedLevel >= 1)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass1, Level3.Pass1));
                }

                if (checkedLevel >= 2)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass2, Level3.Pass2));
                }

                if (isChecked)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass3, Level3.Pass3));
                }
            }
            else if (checkContentLevel == 4)
            {
                if (checkedLevel >= 1)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass1, Level4.Pass1));
                }

                if (checkedLevel >= 2)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass2, Level4.Pass2));
                }

                if (checkedLevel >= 3)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass3, Level4.Pass3));
                }

                if (isChecked)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass4, Level4.Pass4));
                }
            }
            else if (checkContentLevel == 5)
            {
                if (checkedLevel >= 1)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass1, Level5.Pass1));
                }

                if (checkedLevel >= 2)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass2, Level5.Pass2));
                }

                if (checkedLevel >= 3)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass3, Level5.Pass3));
                }

                if (checkedLevel >= 4)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass4, Level5.Pass4));
                }

                if (isChecked)
                {
                    checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Pass5, Level5.Pass5));
                }
            }

            if (includeFail)
            {
                if (checkContentLevel == 1)
                {
                    if (isChecked)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail1, Level1.Fail1));
                    }
                }
                else if (checkContentLevel == 2)
                {
                    if (checkedLevel >= 1)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail1, Level2.Fail1));
                    }

                    if (isChecked)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail2, Level2.Fail2));
                    }
                }
                else if (checkContentLevel == 3)
                {
                    if (checkedLevel >= 1)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail1, Level3.Fail1));
                    }

                    if (checkedLevel >= 2)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail2, Level3.Fail2));
                    }

                    if (isChecked)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail3, Level3.Fail3));
                    }
                }
                else if (checkContentLevel == 4)
                {
                    if (checkedLevel >= 1)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail1, Level4.Fail1));
                    }

                    if (checkedLevel >= 2)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail2, Level4.Fail2));
                    }

                    if (checkedLevel >= 3)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail3, Level4.Fail3));
                    }

                    if (isChecked)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail4, Level4.Fail4));
                    }
                }
                else if (checkContentLevel == 5)
                {
                    if (checkedLevel >= 1)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail1, Level5.Fail1));
                    }

                    if (checkedLevel >= 2)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail2, Level5.Fail2));
                    }

                    if (checkedLevel >= 3)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail3, Level5.Fail3));
                    }

                    if (checkedLevel >= 4)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail4, Level5.Fail4));
                    }

                    if (isChecked)
                    {
                        checkedLevels.Add(new KeyValuePair<int, string>(LevelInt.Fail5, Level5.Fail5));
                    }
                }
            }

            return checkedLevels;
        }

        public static void LoadContentLevelToEdit(ListControl listControl, SiteInfo siteInfo, ContentInfo contentInfo, bool isChecked, int checkedLevel)
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
	            isCheckable = IsCheckable(contentInfo.IsChecked, contentInfo.CheckedLevel, isChecked, checkedLevel);
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

	    public static string GetCheckState(SiteInfo siteInfo, ContentInfo contentInfo)
	    {
	        if (contentInfo.IsChecked)
	        {
	            return Level.YiShenHe;
	        }

	        var retval = string.Empty;

	        if (contentInfo.CheckedLevel == LevelInt.CaoGao)
	        {
	            retval = Level.CaoGao;
	        }
	        else if (contentInfo.CheckedLevel == LevelInt.DaiShen)
	        {
	            retval = Level.DaiShen;
	        }
	        else
	        {
	            var checkContentLevel = siteInfo.Additional.CheckContentLevel;

	            if (checkContentLevel == 1)
	            {
	                if (contentInfo.CheckedLevel == LevelInt.Fail1)
	                {
	                    retval = Level1.Fail1;
	                }
	            }
	            else if (checkContentLevel == 2)
	            {
	                if (contentInfo.CheckedLevel == LevelInt.Pass1)
	                {
	                    retval = Level2.Pass1;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail1)
	                {
	                    retval = Level2.Fail1;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail2)
	                {
	                    retval = Level2.Fail2;
	                }
	            }
	            else if (checkContentLevel == 3)
	            {
	                if (contentInfo.CheckedLevel == LevelInt.Pass1)
	                {
	                    retval = Level3.Pass1;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Pass2)
	                {
	                    retval = Level3.Pass2;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail1)
	                {
	                    retval = Level3.Fail1;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail2)
	                {
	                    retval = Level3.Fail2;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail3)
	                {
	                    retval = Level3.Fail3;
	                }
	            }
	            else if (checkContentLevel == 4)
	            {
	                if (contentInfo.CheckedLevel == LevelInt.Pass1)
	                {
	                    retval = Level4.Pass1;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Pass2)
	                {
	                    retval = Level4.Pass2;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Pass3)
	                {
	                    retval = Level4.Pass3;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail1)
	                {
	                    retval = Level4.Fail1;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail2)
	                {
	                    retval = Level4.Fail2;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail3)
	                {
	                    retval = Level4.Fail3;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail4)
	                {
	                    retval = Level4.Fail4;
	                }
	            }
	            else if (checkContentLevel == 5)
	            {
	                if (contentInfo.CheckedLevel == LevelInt.Pass1)
	                {
	                    retval = Level5.Pass1;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Pass2)
	                {
	                    retval = Level5.Pass2;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Pass3)
	                {
	                    retval = Level5.Pass3;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Pass4)
	                {
	                    retval = Level5.Pass4;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail1)
	                {
	                    retval = Level5.Fail1;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail2)
	                {
	                    retval = Level5.Fail2;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail3)
	                {
	                    retval = Level5.Fail3;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail4)
	                {
	                    retval = Level5.Fail4;
	                }
	                else if (contentInfo.CheckedLevel == LevelInt.Fail5)
	                {
	                    retval = Level5.Fail5;
	                }
	            }

	            if (string.IsNullOrEmpty(retval))
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

	        return retval;
	    }

	    public static bool IsCheckable(bool contentIsChecked, int contentCheckLevel, bool isChecked, int checkedLevel)
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

	    public static KeyValuePair<bool, int> GetUserCheckLevel(PermissionsImpl permissionsImpl, SiteInfo siteInfo, int channelId)
        {
            if (permissionsImpl.IsSystemAdministrator)
            {
                return new KeyValuePair<bool, int>(true, siteInfo.Additional.CheckContentLevel);
            }

            var isChecked = false;
            var checkedLevel = 0;
            if (siteInfo.Additional.IsCheckContentLevel == false)
            {
                if (permissionsImpl.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheck))
                {
                    isChecked = true;
                }
            }
            else
            {
                if (permissionsImpl.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel5))
                {
                    isChecked = true;
                }
                else if (permissionsImpl.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel4))
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
                else if (permissionsImpl.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel3))
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
                else if (permissionsImpl.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel2))
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
                else if (permissionsImpl.HasChannelPermissions(siteInfo.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel1))
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

        public static bool GetUserCheckLevel(PermissionsImpl permissionsImpl, SiteInfo siteInfo, int channelId, out int userCheckedLevel)
        {
            var checkContentLevel = siteInfo.Additional.CheckContentLevel;

            var pair = GetUserCheckLevel(permissionsImpl, siteInfo, channelId);
            var isChecked = pair.Key;
            var checkedLevel = pair.Value;
            if (isChecked)
            {
                checkedLevel = checkContentLevel;
            }
            userCheckedLevel = checkedLevel;
            return isChecked;
        }
    }
}
