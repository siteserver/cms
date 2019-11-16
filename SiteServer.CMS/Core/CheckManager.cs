using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using Content = SiteServer.CMS.Model.Content;

namespace SiteServer.CMS.Core
{
    public static class CheckManager
	{
	    public static class LevelInt
	    {
	        public const int CaoGao = -99;//草稿
	        public const int DaiShen = 0;//待审核

            public const int Pass1 = 1;//初审通过
	        public const int Pass2 = 2;//二审通过
	        public const int Pass3 = 3;//三审通过
	        public const int Pass4 = 4;//四审通过
	        public const int Pass5 = 5;//已审核

	        public const int Fail1 = -1;//初审退稿
	        public const int Fail2 = -2;//二审退稿
	        public const int Fail3 = -3;//三审退稿
	        public const int Fail4 = -4;//四审退稿
	        public const int Fail5 = -5;//终审退稿

	        //public const int NotChange = -100;//保持不变
	        public const int All = -200;//全部
	    }

	    public static class Level
	    {
	        public const string All = "全部";//全部
            public const string CaoGao = "草稿";//草稿
	        public const string DaiShen = "待审核";//待审核
            public const string YiShenHe = "已审核";//已审核

	        public const string NotChange = "保持不变";//保持不变
	    }

	    private static class Level5
	    {
	        public const string Pass1 = "初审通过，等待二审";
	        public const string Pass2 = "二审通过，等待三审";
	        public const string Pass3 = "三审通过，等待四审";
	        public const string Pass4 = "四审通过，等待终审";
	        public const string Pass5 = "已审核";

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
	        public const string Pass4 = "已审核";

	        public const string Fail1 = "初审退稿";
	        public const string Fail2 = "二审退稿";
	        public const string Fail3 = "三审退稿";
	        public const string Fail4 = "终审退稿";
	    }

	    private static class Level3
	    {
	        public const string Pass1 = "初审通过，等待二审";
	        public const string Pass2 = "二审通过，等待终审";
	        public const string Pass3 = "已审核";

	        public const string Fail1 = "初审退稿";
	        public const string Fail2 = "二审退稿";
	        public const string Fail3 = "终审退稿";
	    }

	    private static class Level2
	    {
	        public const string Pass1 = "初审通过，等待终审";
	        public const string Pass2 = "已审核";

	        public const string Fail1 = "初审退稿";
	        public const string Fail2 = "终审退稿";
	    }

	    private static class Level1
	    {
	        public const string Pass1 = "已审核";

	        public const string Fail1 = "终审退稿";
	    }

        public static List<KeyValuePair<int, string>> GetCheckedLevels(Site site, bool isChecked, int checkedLevel, bool includeFail)
        {
            var checkedLevels = new List<KeyValuePair<int, string>>();

            var checkContentLevel = site.CheckContentLevel;
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

        public static void LoadContentLevelToEdit(ListControl listControl, Site site, Content content, bool isChecked, int checkedLevel)
	    {
	        var checkContentLevel = site.CheckContentLevel;
	        if (isChecked)
	        {
	            checkedLevel = checkContentLevel;
	        }

	        ListItem listItem;

	        //var isCheckable = false;
	        //if (content != null)
	        //{
	        //    isCheckable = IsCheckable(content.IsChecked, content.CheckedLevel, isChecked, checkedLevel);
	        //    if (isCheckable)
	        //    {
	        //        listItem = new ListItem(Level.NotChange, LevelInt.NotChange.ToString());
	        //        listControl.Items.Add(listItem);
	        //    }
	        //}

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

	        //if (content == null)
	        //{
	            ControlUtils.SelectSingleItem(listControl, checkedLevel.ToString());
	        //}
	        //else
	        //{
	        //    ControlUtils.SelectSingleItem(listControl,
	        //        isCheckable ? LevelInt.NotChange.ToString() : checkedLevel.ToString());
	        //}
	    }

	    public static void LoadContentLevelToList(ListControl listControl, Site site, bool isCheckOnly, bool isChecked, int checkedLevel)
	    {
	        var checkContentLevel = site.CheckContentLevel;

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

	    public static void LoadContentLevelToCheck(ListControl listControl, Site site, bool isChecked, int checkedLevel)
	    {
	        var checkContentLevel = site.CheckContentLevel;
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

	    public static string GetCheckState(Site site, Content content)
	    {
	        if (content.Checked)
	        {
	            return Level.YiShenHe;
	        }

	        var retVal = string.Empty;

	        if (content.CheckedLevel == LevelInt.CaoGao)
	        {
	            retVal = Level.CaoGao;
	        }
	        else if (content.CheckedLevel == LevelInt.DaiShen)
	        {
	            retVal = Level.DaiShen;
	        }
	        else
	        {
	            var checkContentLevel = site.CheckContentLevel;

	            if (checkContentLevel == 1)
	            {
	                if (content.CheckedLevel == LevelInt.Fail1)
	                {
	                    retVal = Level1.Fail1;
	                }
	            }
	            else if (checkContentLevel == 2)
	            {
	                if (content.CheckedLevel == LevelInt.Pass1)
	                {
	                    retVal = Level2.Pass1;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail1)
	                {
	                    retVal = Level2.Fail1;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail2)
	                {
	                    retVal = Level2.Fail2;
	                }
	            }
	            else if (checkContentLevel == 3)
	            {
	                if (content.CheckedLevel == LevelInt.Pass1)
	                {
	                    retVal = Level3.Pass1;
	                }
	                else if (content.CheckedLevel == LevelInt.Pass2)
	                {
	                    retVal = Level3.Pass2;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail1)
	                {
	                    retVal = Level3.Fail1;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail2)
	                {
	                    retVal = Level3.Fail2;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail3)
	                {
	                    retVal = Level3.Fail3;
	                }
	            }
	            else if (checkContentLevel == 4)
	            {
	                if (content.CheckedLevel == LevelInt.Pass1)
	                {
	                    retVal = Level4.Pass1;
	                }
	                else if (content.CheckedLevel == LevelInt.Pass2)
	                {
	                    retVal = Level4.Pass2;
	                }
	                else if (content.CheckedLevel == LevelInt.Pass3)
	                {
	                    retVal = Level4.Pass3;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail1)
	                {
	                    retVal = Level4.Fail1;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail2)
	                {
	                    retVal = Level4.Fail2;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail3)
	                {
	                    retVal = Level4.Fail3;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail4)
	                {
	                    retVal = Level4.Fail4;
	                }
	            }
	            else if (checkContentLevel == 5)
	            {
	                if (content.CheckedLevel == LevelInt.Pass1)
	                {
	                    retVal = Level5.Pass1;
	                }
	                else if (content.CheckedLevel == LevelInt.Pass2)
	                {
	                    retVal = Level5.Pass2;
	                }
	                else if (content.CheckedLevel == LevelInt.Pass3)
	                {
	                    retVal = Level5.Pass3;
	                }
	                else if (content.CheckedLevel == LevelInt.Pass4)
	                {
	                    retVal = Level5.Pass4;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail1)
	                {
	                    retVal = Level5.Fail1;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail2)
	                {
	                    retVal = Level5.Fail2;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail3)
	                {
	                    retVal = Level5.Fail3;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail4)
	                {
	                    retVal = Level5.Fail4;
	                }
	                else if (content.CheckedLevel == LevelInt.Fail5)
	                {
	                    retVal = Level5.Fail5;
	                }
	            }

	            if (string.IsNullOrEmpty(retVal))
	            {
	                if (checkContentLevel == 1)
	                {
	                    retVal = Level.DaiShen;
	                }
	                else if (checkContentLevel == 2)
	                {
	                    retVal = Level2.Pass1;
	                }
	                else if (checkContentLevel == 3)
	                {
	                    retVal = Level3.Pass2;
	                }
	                else if (checkContentLevel == 4)
	                {
	                    retVal = Level4.Pass3;
	                }
	            }
	        }

	        return retVal;
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

	    public static async Task<KeyValuePair<bool, int>> GetUserCheckLevelPairAsync(PermissionsImpl permissionsImpl, Site site, int channelId)
        {
            if (await permissionsImpl.IsSiteAdminAsync())
            {
                return new KeyValuePair<bool, int>(true, site.CheckContentLevel);
            }

            var isChecked = false;
            var checkedLevel = 0;
            if (site.IsCheckContentLevel == false)
            {
                if (await permissionsImpl.HasChannelPermissionsAsync(site.Id, channelId, ConfigManager.ChannelPermissions.ContentCheck))
                {
                    isChecked = true;
                }
            }
            else
            {
                if (await permissionsImpl.HasChannelPermissionsAsync(site.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel5))
                {
                    isChecked = true;
                }
                else if (await permissionsImpl.HasChannelPermissionsAsync(site.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel4))
                {
                    if (site.CheckContentLevel <= 4)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 4;
                    }
                }
                else if (await permissionsImpl.HasChannelPermissionsAsync(site.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel3))
                {
                    if (site.CheckContentLevel <= 3)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 3;
                    }
                }
                else if (await permissionsImpl.HasChannelPermissionsAsync(site.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel2))
                {
                    if (site.CheckContentLevel <= 2)
                    {
                        isChecked = true;
                    }
                    else
                    {
                        checkedLevel = 2;
                    }
                }
                else if (await permissionsImpl.HasChannelPermissionsAsync(site.Id, channelId, ConfigManager.ChannelPermissions.ContentCheckLevel1))
                {
                    if (site.CheckContentLevel <= 1)
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

        public static async Task<(bool IsChecked, int UserCheckedLevel)> GetUserCheckLevelAsync(PermissionsImpl permissionsImpl, Site site, int channelId)
        {
            var checkContentLevel = site.CheckContentLevel;

            var pair = await GetUserCheckLevelPairAsync(permissionsImpl, site, channelId);
            var isChecked = pair.Key;
            var checkedLevel = pair.Value;
            if (isChecked)
            {
                checkedLevel = checkContentLevel;
            }
            return (isChecked, checkedLevel);
        }
    }
}
