using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Utils
{
    public static class SystemWebUtils
    {
        private static string GetSelectText(SiteInfo siteInfo, ChannelInfo channelInfo, PermissionsImpl adminPermissions, bool[] isLastNodeArray, bool isShowContentNum)
        {
            var retVal = string.Empty;
            if (channelInfo.Id == channelInfo.SiteId)
            {
                channelInfo.LastNode = true;
            }
            if (channelInfo.LastNode == false)
            {
                isLastNodeArray[channelInfo.ParentsCount] = false;
            }
            else
            {
                isLastNodeArray[channelInfo.ParentsCount] = true;
            }
            for (var i = 0; i < channelInfo.ParentsCount; i++)
            {
                retVal = string.Concat(retVal, isLastNodeArray[i] ? "　" : "│");
            }
            retVal = string.Concat(retVal, channelInfo.LastNode ? "└" : "├");
            retVal = string.Concat(retVal, channelInfo.ChannelName);

            if (isShowContentNum)
            {
                var onlyAdminId = adminPermissions.GetOnlyAdminId(siteInfo.Id, channelInfo.Id);
                var count = ContentManager.GetCount(siteInfo, channelInfo, onlyAdminId);
                retVal = string.Concat(retVal, " (", count, ")");
            }

            return retVal;
        }

        public static void AddListItemsForChannel(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, bool isShowContentNum, PermissionsImpl permissionsImpl)
        {
            var list = ChannelManager.GetChannelIdList(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = permissionsImpl.IsOwningChannelId(channelId);
                    if (!enabled)
                    {
                        if (!permissionsImpl.IsDescendantOwningChannelId(siteInfo.Id, channelId)) continue;
                    }
                }
                var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

                var listItem = new ListItem(GetSelectText(siteInfo, channelInfo, permissionsImpl, isLastNodeArray, isShowContentNum), channelInfo.Id.ToString());
                if (!enabled)
                {
                    listItem.Attributes.Add("style", "color:gray;");
                }
                listItemCollection.Add(listItem);
            }
        }

        public static void AddListItemsForAddContent(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, PermissionsImpl permissionsImpl)
        {
            var list = ChannelManager.GetChannelIdList(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = permissionsImpl.IsOwningChannelId(channelId);
                }

                var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                if (enabled)
                {
                    if (channelInfo.IsContentAddable == false) enabled = false;
                }

                if (!enabled)
                {
                    continue;
                }

                var listItem = new ListItem(GetSelectText(siteInfo, channelInfo, permissionsImpl, isLastNodeArray, true), channelInfo.Id.ToString());
                listItemCollection.Add(listItem);
            }
        }

        /// <summary>
        /// 得到栏目，并且不对（栏目是否可添加内容）进行判断
        /// 提供给触发器页面使用
        /// 使用场景：其他栏目的内容变动之后，设置某个栏目（此栏目不能添加内容）触发生成
        /// </summary>
        public static void AddListItemsForCreateChannel(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, PermissionsImpl permissionsImpl)
        {
            var list = ChannelManager.GetChannelIdList(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = permissionsImpl.IsOwningChannelId(channelId);
                }

                var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

                if (!enabled)
                {
                    continue;
                }

                var listItem = new ListItem(GetSelectText(siteInfo, nodeInfo, permissionsImpl, isLastNodeArray, true), nodeInfo.Id.ToString());
                listItemCollection.Add(listItem);
            }
        }

        public static void LoadChannelIdListBox(ListBox channelIdListBox, SiteInfo siteInfo, int psId, ChannelInfo channelInfo, PermissionsImpl permissionsImpl)
        {
            channelIdListBox.Items.Clear();

            var transType = ECrossSiteTransTypeUtils.GetEnumType(channelInfo.TransType);

            var isUseNodeNames = transType == ECrossSiteTransType.AllParentSite || transType == ECrossSiteTransType.AllSite;

            if (!isUseNodeNames)
            {
                var channelIdList = TranslateUtils.StringCollectionToIntList(channelInfo.TransChannelIds);
                foreach (var theChannelId in channelIdList)
                {
                    var theNodeInfo = ChannelManager.GetChannelInfo(psId, theChannelId);
                    if (theNodeInfo != null)
                    {
                        var listitem = new ListItem(theNodeInfo.ChannelName, theNodeInfo.Id.ToString());
                        channelIdListBox.Items.Add(listitem);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(channelInfo.TransChannelNames))
                {
                    var nodeNameArrayList = TranslateUtils.StringCollectionToStringList(channelInfo.TransChannelNames);
                    var channelIdList = ChannelManager.GetChannelIdList(psId);
                    foreach (var nodeName in nodeNameArrayList)
                    {
                        foreach (var theChannelId in channelIdList)
                        {
                            var theNodeInfo = ChannelManager.GetChannelInfo(psId, theChannelId);
                            if (theNodeInfo.ChannelName == nodeName)
                            {
                                var listitem = new ListItem(theNodeInfo.ChannelName, theNodeInfo.Id.ToString());
                                channelIdListBox.Items.Add(listitem);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    AddListItemsForAddContent(channelIdListBox.Items, SiteManager.GetSiteInfo(psId), false, permissionsImpl);
                }
            }
        }

        public static void AddListItemsForSite(ListControl listControl)
        {
            var siteIdList = SiteManager.GetSiteIdList();
            var mySystemInfoList = new List<SiteInfo>();
            var parentWithChildren = new Hashtable();
            SiteInfo hqSiteInfo = null;
            foreach (var siteId in siteIdList)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo.Root)
                {
                    hqSiteInfo = siteInfo;
                }
                else
                {
                    if (siteInfo.ParentId == 0)
                    {
                        mySystemInfoList.Add(siteInfo);
                    }
                    else
                    {
                        var children = new List<SiteInfo>();
                        if (parentWithChildren.Contains(siteInfo.ParentId))
                        {
                            children = (List<SiteInfo>)parentWithChildren[siteInfo.ParentId];
                        }
                        children.Add(siteInfo);
                        parentWithChildren[siteInfo.ParentId] = children;
                    }
                }
            }
            if (hqSiteInfo != null)
            {
                AddListItemForSite(listControl, hqSiteInfo, parentWithChildren, 0);
            }
            foreach (var siteInfo in mySystemInfoList)
            {
                AddListItemForSite(listControl, siteInfo, parentWithChildren, 0);
            }
        }

        private static void AddListItemForSite(ListControl listControl, SiteInfo siteInfo, Hashtable parentWithChildren, int level)
        {
            var padding = string.Empty;
            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            if (parentWithChildren[siteInfo.Id] != null)
            {
                var children = (List<SiteInfo>)parentWithChildren[siteInfo.Id];
                listControl.Items.Add(new ListItem(padding + siteInfo.SiteName + $"({children.Count})", siteInfo.Id.ToString()));
                level++;
                foreach (SiteInfo subSiteInfo in children)
                {
                    AddListItemForSite(listControl, subSiteInfo, parentWithChildren, level);
                }
            }
            else
            {
                listControl.Items.Add(new ListItem(padding + siteInfo.SiteName, siteInfo.Id.ToString()));
            }
        }

        public static string SelectedItemsValueToStringCollection(ListItemCollection items)
        {
            var builder = new StringBuilder();
            if (items != null)
            {
                foreach (ListItem item in items)
                {
                    if (item.Selected)
                    {
                        builder.Append(item.Value).Append(",");
                    }
                }
                if (builder.Length != 0)
                    builder.Remove(builder.Length - 1, 1);
            }
            return builder.ToString();
        }

        private static ListItem GetListItem(ECrossSiteTransType type, bool selected)
        {
            var item = new ListItem(ECrossSiteTransTypeUtils.GetText(type), ECrossSiteTransTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddAllListItemsToECrossSiteTransType(ListControl listControl, bool isParentSite)
        {
            if (listControl == null) return;

            listControl.Items.Add(GetListItem(ECrossSiteTransType.None, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.SelfSite, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.SpecifiedSite, false));
            if (isParentSite)
            {
                listControl.Items.Add(GetListItem(ECrossSiteTransType.ParentSite, false));
                listControl.Items.Add(GetListItem(ECrossSiteTransType.AllParentSite, false));
            }
            listControl.Items.Add(GetListItem(ECrossSiteTransType.AllSite, false));
        }

        public static void AddListItemsToEKeywordGrade(ListControl listControl)
        {
            if (listControl != null)
            {
                var item = new ListItem(EKeywordGradeUtils.GetText(EKeywordGrade.Normal), EKeywordGradeUtils.GetValue(EKeywordGrade.Normal));
                listControl.Items.Add(item);
                item = new ListItem(EKeywordGradeUtils.GetText(EKeywordGrade.Sensitive), EKeywordGradeUtils.GetValue(EKeywordGrade.Sensitive));
                listControl.Items.Add(item);
                item = new ListItem(EKeywordGradeUtils.GetText(EKeywordGrade.Dangerous), EKeywordGradeUtils.GetValue(EKeywordGrade.Dangerous));
                listControl.Items.Add(item);
            }
        }

        private static ListItem GetListItem(ELinkType type, bool selected)
        {
            var item = new ListItem(ELinkTypeUtils.GetText(type), ELinkTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToELinkType(ListControl listControl)
        {
            if (listControl == null) return;

            listControl.Items.Add(GetListItem(ELinkType.None, false));
            listControl.Items.Add(GetListItem(ELinkType.NoLinkIfContentNotExists, false));
            listControl.Items.Add(GetListItem(ELinkType.LinkToOnlyOneContent, false));
            listControl.Items.Add(GetListItem(ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent, false));
            listControl.Items.Add(GetListItem(ELinkType.LinkToFirstContent, false));
            listControl.Items.Add(GetListItem(ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent, false));
            listControl.Items.Add(GetListItem(ELinkType.NoLinkIfChannelNotExists, false));
            listControl.Items.Add(GetListItem(ELinkType.LinkToLastAddChannel, false));
            listControl.Items.Add(GetListItem(ELinkType.LinkToFirstChannel, false));
            listControl.Items.Add(GetListItem(ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel, false));
            listControl.Items.Add(GetListItem(ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel, false));
            listControl.Items.Add(GetListItem(ELinkType.NoLink, false));
        }

        private static ListItem GetListItem(TemplateType type, bool selected)
        {
            var item = new ListItem(TemplateTypeUtils.GetText(type), type.Value);
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToTemplateType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(TemplateType.IndexPageTemplate, false));
                listControl.Items.Add(GetListItem(TemplateType.ChannelTemplate, false));
                listControl.Items.Add(GetListItem(TemplateType.ContentTemplate, false));
                listControl.Items.Add(GetListItem(TemplateType.FileTemplate, false));
            }
        }

        private static ListItem GetListItem(ERelatedFieldStyle type, bool selected)
        {
            var item = new ListItem(ERelatedFieldStyleUtils.GetText(type), ERelatedFieldStyleUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToERelatedFieldStyle(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(ERelatedFieldStyle.Horizontal, false));
                listControl.Items.Add(GetListItem(ERelatedFieldStyle.Virtical, false));
            }
        }

        public static ListItem GetListItem(ETableRule type, bool selected)
        {
            var item = new ListItem(ETableRuleUtils.GetText(type), ETableRuleUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        private static ListItem GetListItem(ETaxisType type, bool selected)
        {
            var item = new ListItem(ETaxisTypeUtils.GetText(type), ETaxisTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToETaxisTypeForChannelEdit(ListControl listControl)
        {
            if (listControl == null) return;

            listControl.Items.Add(GetListItem(ETaxisType.OrderById, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByIdDesc, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDate, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDateDesc, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByLastEditDate, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByLastEditDateDesc, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxis, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxisDesc, false));
        }

        private static ListItem GetListItem(ETranslateContentType type, bool selected)
        {
            var item = new ListItem(ETranslateContentTypeUtils.GetText(type), ETranslateContentTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToETranslateContentType(ListControl listControl, bool isCut)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(ETranslateContentType.Copy, false));
                if (isCut)
                {
                    listControl.Items.Add(GetListItem(ETranslateContentType.Cut, false));
                }
                listControl.Items.Add(GetListItem(ETranslateContentType.Reference, false));
                listControl.Items.Add(GetListItem(ETranslateContentType.ReferenceContent, false));
            }
        }

        private static ListItem GetListItem(ETranslateType type, bool selected)
        {
            var item = new ListItem(ETranslateTypeUtils.GetText(type), ETranslateTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToETranslateType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(ETranslateType.Content, false));
                listControl.Items.Add(GetListItem(ETranslateType.Channel, false));
                listControl.Items.Add(GetListItem(ETranslateType.All, false));
            }
        }

        public static Control FindControlBySelfAndChildren(string id, Control baseControl)
        {
            Control ctrl = null;
            if (baseControl != null)
            {
                ctrl = baseControl.FindControl(id);
                if (ctrl == baseControl) ctrl = null;//DropDownList中FindControl将返回自身
                if (ctrl == null && baseControl.HasControls())
                {
                    ctrl = FindControlByChildren(id, baseControl.Controls);
                }
            }
            return ctrl;
        }

        public static Control FindControlByChildren(string id, ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                var ctrl = FindControlBySelfAndChildren(id, control);
                if (ctrl != null)
                {
                    return ctrl;
                }
            }
            return null;
        }

        public static void LoadContentLevelToCheckEdit(ListControl listControl, SiteInfo siteInfo, ContentInfo contentInfo, bool isChecked, int checkedLevel)
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
                SelectSingleItem(listControl, checkedLevel.ToString());
            }
            else
            {
                SelectSingleItem(listControl,
                    isCheckable ? CheckManager.LevelInt.NotChange.ToString() : checkedLevel.ToString());
            }
        }

        public static void LoadContentLevelToCheckList(ListControl listControl, SiteInfo siteInfo, bool isCheckOnly, bool isChecked, int checkedLevel)
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

            SelectSingleItem(listControl, checkedLevel.ToString());
        }

        public static void AddListControlItems(ListControl listControl, List<string> list)
        {
            if (listControl == null) return;

            foreach (var value in list)
            {
                var item = new ListItem(value, value);
                listControl.Items.Add(item);
            }
        }

        public static string[] GetSelectedListControlValueArray(ListControl listControl)
        {
            var arraylist = new ArrayList();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        arraylist.Add(item.Value);
                    }
                }
            }
            var retval = new string[arraylist.Count];
            arraylist.CopyTo(retval);
            return retval;
        }

        public static string GetSelectedListControlValueCollection(ListControl listControl)
        {
            var list = new List<string>();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        list.Add(item.Value);
                    }
                }
            }
            return TranslateUtils.ObjectCollectionToString(list);
        }

        public static ArrayList GetSelectedListControlValueArrayList(ListControl listControl)
        {
            var arraylist = new ArrayList();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        arraylist.Add(item.Value);
                    }
                }
            }
            return arraylist;
        }

        public static List<string> GetSelectedListControlValueStringList(ListControl listControl)
        {
            var list = new List<string>();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        list.Add(item.Value);
                    }
                }
            }
            return list;
        }

        public static string[] GetListControlValues(ListControl listControl)
        {
            var arraylist = new ArrayList();
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    arraylist.Add(item.Value);
                }
            }
            var retval = new string[arraylist.Count];
            arraylist.CopyTo(retval);
            return retval;
        }

        public static void SelectSingleItem(ListControl listControl, string value)
        {
            if (listControl == null) return;

            listControl.ClearSelection();

            foreach (ListItem item in listControl.Items)
            {
                if (string.Equals(item.Value, value))
                {
                    item.Selected = true;
                    break;
                }
            }
        }

        public static void SelectSingleItemIgnoreCase(ListControl listControl, string value)
        {
            if (listControl == null) return;

            listControl.ClearSelection();
            foreach (ListItem item in listControl.Items)
            {
                if (StringUtils.EqualsIgnoreCase(item.Value, value))
                {
                    item.Selected = true;
                    break;
                }
            }
        }

        public static void SelectMultiItems(ListControl listControl, List<string> values)
        {
            if (listControl == null) return;

            listControl.ClearSelection();
            foreach (ListItem item in listControl.Items)
            {
                foreach (var value in values)
                {
                    if (string.Equals(item.Value, value))
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }

        public static void SelectMultiItems(ListControl listControl, List<int> values)
        {
            if (listControl == null) return;

            listControl.ClearSelection();
            foreach (ListItem item in listControl.Items)
            {
                foreach (var intVal in values)
                {
                    if (string.Equals(item.Value, intVal.ToString()))
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }

        public static void LoadSiteIdDropDownList(DropDownList siteIdDropDownList, SiteInfo siteInfo, int channelId)
        {
            siteIdDropDownList.Items.Clear();

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

            var transType = ECrossSiteTransTypeUtils.GetEnumType(channelInfo.TransType);

            if (transType == ECrossSiteTransType.SelfSite || transType == ECrossSiteTransType.SpecifiedSite || transType == ECrossSiteTransType.ParentSite)
            {
                int theSiteId;
                if (transType == ECrossSiteTransType.SelfSite)
                {
                    theSiteId = siteInfo.Id;
                }
                else if (transType == ECrossSiteTransType.SpecifiedSite)
                {
                    theSiteId = channelInfo.TransSiteId;
                }
                else
                {
                    theSiteId = SiteManager.GetParentSiteId(siteInfo.Id);
                }
                if (theSiteId > 0)
                {
                    var theSiteInfo = SiteManager.GetSiteInfo(theSiteId);
                    if (theSiteInfo != null)
                    {
                        var listitem = new ListItem(theSiteInfo.SiteName, theSiteInfo.Id.ToString());
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (transType == ECrossSiteTransType.AllParentSite)
            {
                var siteIdList = SiteManager.GetSiteIdList();

                var allParentSiteIdList = new List<int>();
                SiteManager.GetAllParentSiteIdList(allParentSiteIdList, siteIdList, siteInfo.Id);

                foreach (var psId in siteIdList)
                {
                    if (psId == siteInfo.Id) continue;
                    var psInfo = SiteManager.GetSiteInfo(psId);
                    var show = psInfo.Root || allParentSiteIdList.Contains(psInfo.Id);
                    if (show)
                    {
                        var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                        if (psInfo.Root) listitem.Selected = true;
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (transType == ECrossSiteTransType.AllSite)
            {
                var siteIdList = SiteManager.GetSiteIdList();

                foreach (var psId in siteIdList)
                {
                    var psInfo = SiteManager.GetSiteInfo(psId);
                    var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                    if (psInfo.Root) listitem.Selected = true;
                    siteIdDropDownList.Items.Add(listitem);
                }
            }
        }

        public static ListItem GetListItem(InputType type, bool selected)
        {
            var item = new ListItem(InputTypeUtils.GetText(type), type.Value);
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToInputType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(InputType.Text, false));
                listControl.Items.Add(GetListItem(InputType.TextArea, false));
                listControl.Items.Add(GetListItem(InputType.TextEditor, false));
                listControl.Items.Add(GetListItem(InputType.CheckBox, false));
                listControl.Items.Add(GetListItem(InputType.Radio, false));
                listControl.Items.Add(GetListItem(InputType.SelectOne, false));
                listControl.Items.Add(GetListItem(InputType.SelectMultiple, false));
                listControl.Items.Add(GetListItem(InputType.SelectCascading, false));
                listControl.Items.Add(GetListItem(InputType.Date, false));
                listControl.Items.Add(GetListItem(InputType.DateTime, false));
                listControl.Items.Add(GetListItem(InputType.Image, false));
                listControl.Items.Add(GetListItem(InputType.Video, false));
                listControl.Items.Add(GetListItem(InputType.File, false));
                listControl.Items.Add(GetListItem(InputType.Customize, false));
                listControl.Items.Add(GetListItem(InputType.Hidden, false));
            }
        }
    }
}
