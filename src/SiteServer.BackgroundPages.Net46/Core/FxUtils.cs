using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Core
{
    public static partial class FxUtils
    {
        public static void CheckRequestParameter(params string[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (!string.IsNullOrEmpty(parameter) && HttpContext.Current.Request.QueryString[parameter] == null)
                {
                    FxUtils.Page.Redirect(FxUtils.Page.GetErrorPageUrl(MessageUtils.PageErrorParameterIsNotCorrect));
                    return;
                }
            }
        }

        private static object Eval(object dataItem, string name)
        {
            object o = null;
            try
            {
                o = DataBinder.Eval(dataItem, name);
            }
            catch
            {
                // ignored
            }
            if (o == DBNull.Value)
            {
                o = null;
            }
            return o;
        }

        public static int EvalInt(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o == null ? 0 : Convert.ToInt32(o);
        }

        public static string EvalString(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            var value = o?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                value = AttackUtils.UnFilterSql(value);
            }
            if (AppSettings.DatabaseType == DatabaseType.Oracle && value == SqlUtils.OracleEmptyValue)
            {
                value = string.Empty;
            }
            return value;
        }

        public static DateTime EvalDateTime(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            if (o == null)
            {
                return DateUtils.SqlMinValue;
            }
            return (DateTime)o;
        }

        public static bool EvalBool(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o != null && TranslateUtils.ToBool(o.ToString());
        }

        public static HorizontalAlign ToHorizontalAlign(string typeStr)
        {
            return (HorizontalAlign)ToEnum(typeof(HorizontalAlign), typeStr, HorizontalAlign.Left);
        }

        public static VerticalAlign ToVerticalAlign(string typeStr)
        {
            return (VerticalAlign)ToEnum(typeof(VerticalAlign), typeStr, VerticalAlign.Middle);
        }

        public static GridLines ToGridLines(string typeStr)
        {
            return (GridLines)ToEnum(typeof(GridLines), typeStr, GridLines.None);
        }

        public static RepeatDirection ToRepeatDirection(string typeStr)
        {
            return (RepeatDirection)ToEnum(typeof(RepeatDirection), typeStr, RepeatDirection.Vertical);
        }

        public static RepeatLayout ToRepeatLayout(string typeStr)
        {
            return (RepeatLayout)ToEnum(typeof(RepeatLayout), typeStr, RepeatLayout.Table);
        }

        public static object ToEnum(Type enumType, string value, object defaultType)
        {
            object retVal;
            try
            {
                retVal = Enum.Parse(enumType, value, true);
            }
            catch
            {
                retVal = defaultType;
            }
            return retVal;
        }

        public static Unit ToUnit(string unitStr)
        {
            var type = Unit.Empty;
            try
            {
                type = Unit.Parse(unitStr.Trim());
            }
            catch
            {
                // ignored
            }
            return type;
        }

        public static ListItem GetListItem(DatabaseType type, bool selected)
        {
            var item = new ListItem(type.Value, type.Value);
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToDatabaseType(ListControl listControl)
        {
            if (listControl == null) return;
            listControl.Items.Add(GetListItem(DatabaseType.MySql, false));
            listControl.Items.Add(GetListItem(DatabaseType.SqlServer, false));
            listControl.Items.Add(GetListItem(DatabaseType.PostgreSql, false));
            listControl.Items.Add(GetListItem(DatabaseType.Oracle, false));
        }

        public static ListItem GetListItem(EBoolean type, bool selected)
        {
            var item = new ListItem(EBooleanUtils.GetText(type), EBooleanUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EBoolean.True, false));
                listControl.Items.Add(GetListItem(EBoolean.False, false));
            }
        }

        public static void AddListItems(ListControl listControl, string trueText, string falseText)
        {
            if (listControl != null)
            {
                var item = new ListItem(trueText, EBooleanUtils.GetValue(EBoolean.True));
                listControl.Items.Add(item);
                item = new ListItem(falseText, EBooleanUtils.GetValue(EBoolean.False));
                listControl.Items.Add(item);
            }
        }

        public static ListItem GetListItem(ECharset type, bool selected)
        {
            var item = new ListItem(ECharsetUtils.GetText(type), ECharsetUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }


        public static void AddListItemsToECharset(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(ECharset.utf_8, false));
                listControl.Items.Add(GetListItem(ECharset.gb2312, false));
                listControl.Items.Add(GetListItem(ECharset.big5, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_1, false));
                listControl.Items.Add(GetListItem(ECharset.euc_kr, false));
                listControl.Items.Add(GetListItem(ECharset.euc_jp, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_6, false));
                listControl.Items.Add(GetListItem(ECharset.windows_874, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_9, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_5, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_8, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_7, false));
                listControl.Items.Add(GetListItem(ECharset.windows_1258, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_2, false));
            }
        }

        public static ListItem GetListItem(EDataFormat type, bool selected)
        {
            var item = new ListItem(EDataFormatUtils.GetText(type), EDataFormatUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEDataFormat(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EDataFormat.String, false));
                listControl.Items.Add(GetListItem(EDataFormat.Json, false));
                listControl.Items.Add(GetListItem(EDataFormat.Xml, false));
            }
        }

        public static ListItem GetListItem(EDateFormatType type, bool selected)
        {
            var item = new ListItem(EDateFormatTypeUtils.GetText(type), EDateFormatTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEDateFormatType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EDateFormatType.Month, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Day, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Year, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Chinese, false));
            }
        }

        public static ListItem GetListItem(EFileSystemType type, bool selected)
        {
            var item = new ListItem(EFileSystemTypeUtils.GetValue(type), EFileSystemTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEFileSystemType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EFileSystemType.Html, false));
                listControl.Items.Add(GetListItem(EFileSystemType.Htm, false));
                listControl.Items.Add(GetListItem(EFileSystemType.SHtml, false));
                listControl.Items.Add(GetListItem(EFileSystemType.Xml, false));
                listControl.Items.Add(GetListItem(EFileSystemType.Json, false));
            }
        }

        public static ListItem GetListItem(EPredefinedRole type, bool selected)
        {
            var item = new ListItem(EPredefinedRoleUtils.GetText(type), EPredefinedRoleUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static ListItem GetListItem(EScopeType type, bool selected)
        {
            var item = new ListItem(EScopeTypeUtils.GetValue(type) + " (" + EScopeTypeUtils.GetText(type) + ")", EScopeTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEScopeType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EScopeType.Self, false));
                listControl.Items.Add(GetListItem(EScopeType.Children, false));
                listControl.Items.Add(GetListItem(EScopeType.SelfAndChildren, false));
                listControl.Items.Add(GetListItem(EScopeType.Descendant, false));
                listControl.Items.Add(GetListItem(EScopeType.All, false));
            }
        }

        public static ListItem GetListItem(EStatictisXType type, bool selected)
        {
            var item = new ListItem(EStatictisXTypeUtils.GetText(type), EStatictisXTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEStatictisXType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EStatictisXType.Day, false));
                listControl.Items.Add(GetListItem(EStatictisXType.Month, false));
                listControl.Items.Add(GetListItem(EStatictisXType.Year, false));
            }
        }

        public static void AddListItemsToETriState(ListControl listControl, string allText, string trueText, string falseText)
        {
            if (listControl != null)
            {
                var item = new ListItem(allText, ETriStateUtils.GetValue(ETriState.All));
                listControl.Items.Add(item);
                item = new ListItem(trueText, ETriStateUtils.GetValue(ETriState.True));
                listControl.Items.Add(item);
                item = new ListItem(falseText, ETriStateUtils.GetValue(ETriState.False));
                listControl.Items.Add(item);
            }
        }

        public static ListItem GetListItem(EUserPasswordRestriction type, bool selected)
        {
            var item = new ListItem(EUserPasswordRestrictionUtils.GetText(type), EUserPasswordRestrictionUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEUserPasswordRestriction(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EUserPasswordRestriction.None, false));
                listControl.Items.Add(GetListItem(EUserPasswordRestriction.LetterAndDigit, false));
                listControl.Items.Add(GetListItem(EUserPasswordRestriction.LetterAndDigitAndSymbol, false));
            }
        }

        public static string GetCurrentPagePath()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.PhysicalPath;
            }
            return string.Empty;
        }

        public static string MapPath(string virtualPath)
        {
            virtualPath = PathUtils.RemovePathInvalidChar(virtualPath);
            string retVal;
            if (!string.IsNullOrEmpty(virtualPath))
            {
                if (virtualPath.StartsWith("~"))
                {
                    virtualPath = virtualPath.Substring(1);
                }
                virtualPath = PageUtils.Combine("~", virtualPath);
            }
            else
            {
                virtualPath = "~/";
            }
            if (HttpContext.Current != null)
            {
                retVal = HttpContext.Current.Server.MapPath(virtualPath);
            }
            else
            {
                var rootPath = AppSettings.PhysicalApplicationPath;

                virtualPath = !string.IsNullOrEmpty(virtualPath) ? virtualPath.Substring(2) : string.Empty;
                retVal = PathUtils.Combine(rootPath, virtualPath);
            }

            if (retVal == null) retVal = string.Empty;
            return retVal.Replace("/", "\\");
        }

        public static string GetSiteFilesPath(params string[] paths)
        {
            return MapPath(PathUtils.Combine("~/" + DirectoryUtils.SiteFiles.DirectoryName, PathUtils.Combine(paths)));
        }

        public static string PluginsPath => GetSiteFilesPath(DirectoryUtils.SiteFiles.Plugins);

        public static string GetPluginPath(string pluginId, params string[] paths)
        {
            return GetSiteFilesPath(DirectoryUtils.SiteFiles.Plugins, pluginId, PathUtils.Combine(paths));
        }

        public static string GetPluginNuspecPath(string pluginId)
        {
            return GetPluginPath(pluginId, pluginId + ".nuspec");
        }

        public static string GetPluginDllDirectoryPath(string pluginId)
        {
            var fileName = pluginId + ".dll";

            var filePaths = Directory.GetFiles(GetPluginPath(pluginId, "Bin"), fileName, SearchOption.AllDirectories);

            var dict = new Dictionary<DateTime, string>();
            foreach (var filePath in filePaths)
            {
                var lastModifiedDate = File.GetLastWriteTime(filePath);
                dict[lastModifiedDate] = filePath;
            }

            if (dict.Count > 0)
            {
                var filePath = dict.OrderByDescending(x => x.Key).First().Value;
                return Path.GetDirectoryName(filePath);
            }

            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Debug", "net4.6.1", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Debug");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Debug", "net4.6.1", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Debug");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Debug", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Debug");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Release", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Release");
            //}

            return string.Empty;
        }

        public static string GetPackagesPath(params string[] paths)
        {
            return GetSiteFilesPath(DirectoryUtils.SiteFiles.Packages, PathUtils.Combine(paths));
        }

        public static void GetValidateAttributesForListItem(ListControl control, bool isValidate, string displayName, bool isRequire, int minNum, int maxNum, string validateType, string regExp, string errorMessage)
        {
            if (!isValidate) return;

            control.Attributes.Add("isValidate", true.ToString().ToLower());
            control.Attributes.Add("displayName", displayName);
            control.Attributes.Add("isRequire", isRequire.ToString().ToLower());
            control.Attributes.Add("minNum", minNum.ToString());
            control.Attributes.Add("maxNum", maxNum.ToString());
            control.Attributes.Add("validateType", validateType);
            control.Attributes.Add("regExp", regExp);
            control.Attributes.Add("errorMessage", errorMessage);
            control.Attributes.Add("isListItem", true.ToString().ToLower());
        }

        public static ListItem GetListItem(ValidateType type, bool selected)
        {
            var item = new ListItem(ValidateTypeUtils.GetText(type), type.Value);
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToValidateType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(ValidateType.None, false));
                listControl.Items.Add(GetListItem(ValidateType.Chinese, false));
                listControl.Items.Add(GetListItem(ValidateType.English, false));
                listControl.Items.Add(GetListItem(ValidateType.Email, false));
                listControl.Items.Add(GetListItem(ValidateType.Url, false));
                listControl.Items.Add(GetListItem(ValidateType.Phone, false));
                listControl.Items.Add(GetListItem(ValidateType.Mobile, false));
                listControl.Items.Add(GetListItem(ValidateType.Integer, false));
                listControl.Items.Add(GetListItem(ValidateType.Currency, false));
                listControl.Items.Add(GetListItem(ValidateType.Zip, false));
                listControl.Items.Add(GetListItem(ValidateType.IdCard, false));
                listControl.Items.Add(GetListItem(ValidateType.RegExp, false));
            }
        }

        public static void LoadContentLevelToEdit(ListControl listControl, SiteInfo siteInfo, ContentInfo contentInfo, bool isChecked, int checkedLevel)
        {
            var checkContentLevel = siteInfo.CheckContentLevel;
            if (isChecked)
            {
                checkedLevel = checkContentLevel;
            }

            ListItem listItem;

            var isCheckable = false;
            if (contentInfo != null)
            {
                isCheckable = CheckManager.IsCheckable(contentInfo.Checked, contentInfo.CheckedLevel, isChecked, checkedLevel);
                if (isCheckable)
                {
                    listItem = new ListItem(CheckManager.Level.NotChange, CheckManager.LevelInt.NotChange.ToString());
                    listControl.Items.Add(listItem);
                }
            }

            listItem = new ListItem(CheckManager.Level.CaoGao, CheckManager.LevelInt.CaoGao.ToString());
            listControl.Items.Add(listItem);
            listItem = new ListItem(CheckManager.Level.DaiShen, CheckManager.LevelInt.DaiShen.ToString());
            listControl.Items.Add(listItem);

            if (checkContentLevel == 0 || checkContentLevel == 1)
            {
                listItem = new ListItem(CheckManager.Level1.Pass1, CheckManager.LevelInt.Pass1.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 2)
            {
                listItem = new ListItem(CheckManager.Level2.Pass1, CheckManager.LevelInt.Pass1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level2.Pass2, CheckManager.LevelInt.Pass2.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 3)
            {
                listItem = new ListItem(CheckManager.Level3.Pass1, CheckManager.LevelInt.Pass1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level3.Pass2, CheckManager.LevelInt.Pass2.ToString())
                {
                    Enabled = checkedLevel >= 2
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level3.Pass3, CheckManager.LevelInt.Pass3.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 4)
            {
                listItem = new ListItem(CheckManager.Level4.Pass1, CheckManager.LevelInt.Pass1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level4.Pass2, CheckManager.LevelInt.Pass2.ToString())
                {
                    Enabled = checkedLevel >= 2
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level4.Pass3, CheckManager.LevelInt.Pass3.ToString())
                {
                    Enabled = checkedLevel >= 3
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level4.Pass4, CheckManager.LevelInt.Pass4.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 5)
            {
                listItem = new ListItem(CheckManager.Level5.Pass1, CheckManager.LevelInt.Pass1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level5.Pass2, CheckManager.LevelInt.Pass2.ToString())
                {
                    Enabled = checkedLevel >= 2
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level5.Pass3, CheckManager.LevelInt.Pass3.ToString())
                {
                    Enabled = checkedLevel >= 3
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level5.Pass4, CheckManager.LevelInt.Pass4.ToString())
                {
                    Enabled = checkedLevel >= 4
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level5.Pass5, CheckManager.LevelInt.Pass5.ToString())
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
                    isCheckable ? CheckManager.LevelInt.NotChange.ToString() : checkedLevel.ToString());
            }
        }

        public static void LoadContentLevelToList(ListControl listControl, SiteInfo siteInfo, bool isCheckOnly, bool isChecked, int checkedLevel)
        {
            var checkContentLevel = siteInfo.CheckContentLevel;

            if (isChecked)
            {
                checkedLevel = checkContentLevel;
            }

            listControl.Items.Add(new ListItem(CheckManager.Level.All, CheckManager.LevelInt.All.ToString()));
            listControl.Items.Add(new ListItem(CheckManager.Level.CaoGao, CheckManager.LevelInt.CaoGao.ToString()));
            listControl.Items.Add(new ListItem(CheckManager.Level.DaiShen, CheckManager.LevelInt.DaiShen.ToString()));

            if (checkContentLevel == 1)
            {
                if (isChecked)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level1.Fail1, CheckManager.LevelInt.Fail1.ToString()));
                }
            }
            else if (checkContentLevel == 2)
            {
                if (checkedLevel >= 1)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level2.Fail1, CheckManager.LevelInt.Fail1.ToString()));
                }

                if (isChecked)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level2.Fail2, CheckManager.LevelInt.Fail2.ToString()));
                }
            }
            else if (checkContentLevel == 3)
            {
                if (checkedLevel >= 1)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level3.Fail1, CheckManager.LevelInt.Fail1.ToString()));
                }

                if (checkedLevel >= 2)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level3.Fail2, CheckManager.LevelInt.Fail2.ToString()));
                }

                if (isChecked)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level3.Fail3, CheckManager.LevelInt.Fail3.ToString()));
                }
            }
            else if (checkContentLevel == 4)
            {
                if (checkedLevel >= 1)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level4.Fail1, CheckManager.LevelInt.Fail1.ToString()));
                }

                if (checkedLevel >= 2)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level4.Fail2, CheckManager.LevelInt.Fail2.ToString()));
                }

                if (checkedLevel >= 3)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level4.Fail3, CheckManager.LevelInt.Fail3.ToString()));
                }

                if (isChecked)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level4.Fail4, CheckManager.LevelInt.Fail4.ToString()));
                }
            }
            else if (checkContentLevel == 5)
            {
                if (checkedLevel >= 1)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level5.Fail1, CheckManager.LevelInt.Fail1.ToString()));
                }

                if (checkedLevel >= 2)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level5.Fail2, CheckManager.LevelInt.Fail2.ToString()));
                }

                if (checkedLevel >= 3)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level5.Fail3, CheckManager.LevelInt.Fail3.ToString()));
                }

                if (checkedLevel >= 4)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level5.Fail4, CheckManager.LevelInt.Fail4.ToString()));
                }

                if (isChecked)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level5.Fail5, CheckManager.LevelInt.Fail5.ToString()));
                }
            }

            if (isCheckOnly) return;

            if (checkContentLevel == 1)
            {
                if (isChecked)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level1.Pass1, CheckManager.LevelInt.Pass1.ToString()));
                }
            }
            if (checkContentLevel == 2)
            {
                if (checkedLevel >= 1)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level2.Pass1, CheckManager.LevelInt.Pass1.ToString()));
                }

                if (isChecked)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level2.Pass2, CheckManager.LevelInt.Pass2.ToString()));
                }
            }
            else if (checkContentLevel == 3)
            {
                if (checkedLevel >= 1)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level3.Pass1, CheckManager.LevelInt.Pass1.ToString()));
                }

                if (checkedLevel >= 2)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level3.Pass2, CheckManager.LevelInt.Pass2.ToString()));
                }

                if (isChecked)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level3.Pass3, CheckManager.LevelInt.Pass3.ToString()));
                }
            }
            else if (checkContentLevel == 4)
            {
                if (checkedLevel >= 1)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level4.Pass1, CheckManager.LevelInt.Pass1.ToString()));
                }

                if (checkedLevel >= 2)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level4.Pass2, CheckManager.LevelInt.Pass2.ToString()));
                }

                if (checkedLevel >= 3)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level4.Pass3, CheckManager.LevelInt.Pass3.ToString()));
                }

                if (isChecked)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level4.Pass4, CheckManager.LevelInt.Pass4.ToString()));
                }
            }
            else if (checkContentLevel == 5)
            {
                if (checkedLevel >= 2)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level5.Pass1, CheckManager.LevelInt.Pass1.ToString()));
                }

                if (checkedLevel >= 3)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level5.Pass2, CheckManager.LevelInt.Pass2.ToString()));
                }

                if (checkedLevel >= 4)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level5.Pass3, CheckManager.LevelInt.Pass3.ToString()));
                }

                if (checkedLevel >= 5)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level5.Pass4, CheckManager.LevelInt.Pass4.ToString()));
                }

                if (isChecked)
                {
                    listControl.Items.Add(new ListItem(CheckManager.Level5.Pass5, CheckManager.LevelInt.Pass5.ToString()));
                }
            }
        }

        public static void LoadContentLevelToCheck(ListControl listControl, SiteInfo siteInfo, bool isChecked, int checkedLevel)
        {
            var checkContentLevel = siteInfo.CheckContentLevel;
            if (isChecked)
            {
                checkedLevel = checkContentLevel;
            }

            var listItem = new ListItem(CheckManager.Level.CaoGao, CheckManager.LevelInt.CaoGao.ToString());
            listControl.Items.Add(listItem);

            listItem = new ListItem(CheckManager.Level.DaiShen, CheckManager.LevelInt.DaiShen.ToString());
            listControl.Items.Add(listItem);

            if (checkContentLevel == 1)
            {
                listItem = new ListItem(CheckManager.Level1.Fail1, CheckManager.LevelInt.Fail1.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 2)
            {
                listItem = new ListItem(CheckManager.Level2.Fail1, CheckManager.LevelInt.Fail1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level2.Fail2, CheckManager.LevelInt.Fail2.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 3)
            {
                listItem = new ListItem(CheckManager.Level3.Fail1, CheckManager.LevelInt.Fail1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level3.Fail2, CheckManager.LevelInt.Fail2.ToString())
                {
                    Enabled = checkedLevel >= 2
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level3.Fail3, CheckManager.LevelInt.Fail3.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 4)
            {
                listItem = new ListItem(CheckManager.Level4.Fail1, CheckManager.LevelInt.Fail1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level4.Fail2, CheckManager.LevelInt.Fail2.ToString())
                {
                    Enabled = checkedLevel >= 2
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level4.Fail3, CheckManager.LevelInt.Fail3.ToString())
                {
                    Enabled = checkedLevel >= 3
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level4.Fail4, CheckManager.LevelInt.Fail4.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 5)
            {
                listItem = new ListItem(CheckManager.Level5.Fail1, CheckManager.LevelInt.Fail1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level5.Fail2, CheckManager.LevelInt.Fail2.ToString())
                {
                    Enabled = checkedLevel >= 2
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level5.Fail3, CheckManager.LevelInt.Fail3.ToString())
                {
                    Enabled = checkedLevel >= 3
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level5.Fail4, CheckManager.LevelInt.Fail4.ToString())
                {
                    Enabled = checkedLevel >= 4
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level5.Fail5, CheckManager.LevelInt.Fail5.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }

            if (checkContentLevel == 0 || checkContentLevel == 1)
            {
                listItem = new ListItem(CheckManager.Level1.Pass1, CheckManager.LevelInt.Pass1.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 2)
            {
                listItem = new ListItem(CheckManager.Level2.Pass1, CheckManager.LevelInt.Pass1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);

                listItem = new ListItem(CheckManager.Level2.Pass2, CheckManager.LevelInt.Pass2.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 3)
            {
                listItem = new ListItem(CheckManager.Level3.Pass1, CheckManager.LevelInt.Pass1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level3.Pass2, CheckManager.LevelInt.Pass2.ToString())
                {
                    Enabled = checkedLevel >= 2
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level3.Pass3, CheckManager.LevelInt.Pass3.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 4)
            {
                listItem = new ListItem(CheckManager.Level4.Pass1, CheckManager.LevelInt.Pass1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level4.Pass2, CheckManager.LevelInt.Pass2.ToString())
                {
                    Enabled = checkedLevel >= 2
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level4.Pass3, CheckManager.LevelInt.Pass3.ToString())
                {
                    Enabled = checkedLevel >= 3
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level4.Pass4, CheckManager.LevelInt.Pass4.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }
            else if (checkContentLevel == 5)
            {
                listItem = new ListItem(CheckManager.Level5.Pass1, CheckManager.LevelInt.Pass1.ToString())
                {
                    Enabled = checkedLevel >= 1
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level5.Pass2, CheckManager.LevelInt.Pass2.ToString())
                {
                    Enabled = checkedLevel >= 2
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level5.Pass3, CheckManager.LevelInt.Pass3.ToString())
                {
                    Enabled = checkedLevel >= 3
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level5.Pass4, CheckManager.LevelInt.Pass4.ToString())
                {
                    Enabled = checkedLevel >= 4
                };
                listControl.Items.Add(listItem);
                listItem = new ListItem(CheckManager.Level5.Pass5, CheckManager.LevelInt.Pass5.ToString())
                {
                    Enabled = isChecked
                };
                listControl.Items.Add(listItem);
            }

            ControlUtils.SelectSingleItem(listControl, checkedLevel.ToString());
        }
    }
}
