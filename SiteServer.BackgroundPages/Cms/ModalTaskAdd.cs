using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTaskAdd : BasePageCms
    {
        public TextBox TbTaskName;
        public DropDownList DdlFrequencyType;
        public PlaceHolder PhPeriodIntervalMinute;
        public TextBox TbPeriodInterval;
        public DropDownList DdlPeriodIntervalType;

        public PlaceHolder PhNotPeriod;
        public DropDownList DdlStartDay;
        public DropDownList DdlStartWeekday;
        public DropDownList DdlStartHour;

        public TextBox TbDescription;

        public PlaceHolder PhCreate;
        public ListBox LbCreateChannelId;
        public CheckBox CbCreateIsCreateAll;
        public CheckBoxList CblCreateCreateTypes;

        public PlaceHolder PhGather;
        public Literal LtlGather;
        public ListBox LbGather;

        public PlaceHolder PhBackup;
        public DropDownList DdlBackupType;
        public PlaceHolder PhBackupPublishmentSystem;
        public ListBox LbBackupPublishmentSystemId;
        public CheckBox CbBackupIsBackupAll;

        private EServiceType _serviceType;

        public static string GetOpenWindowStringToAdd(EServiceType serviceType, int publishmentSystemId)
        {
            return PageUtils.GetOpenLayerString("添加任务", PageUtils.GetCmsUrl(nameof(ModalTaskAdd), new NameValueCollection
            {
                {"ServiceType", EServiceTypeUtils.GetValue(serviceType)},
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }));
        }

        public static string GetOpenWindowStringToEdit(int taskId, EServiceType serviceType, int publishmentSystemId)
        {
            return PageUtils.GetOpenLayerString("修改任务", PageUtils.GetCmsUrl(nameof(ModalTaskAdd), new NameValueCollection
            {
                {"TaskID", taskId.ToString()},
                {"ServiceType", EServiceTypeUtils.GetValue(serviceType)},
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _serviceType = EServiceTypeUtils.GetEnumType(Body.GetQueryString("ServiceType"));
            if (IsPostBack) return;

            EFrequencyTypeUtils.AddListItems(DdlFrequencyType, false);
            for (var i = 1; i < 32; i++)
            {
                DdlStartDay.Items.Add(new ListItem(i + "日", i.ToString()));
            }
            for (var i = 1; i < 8; i++)
            {
                var weekName = string.Empty;
                switch (i)
                {
                    case 1:
                        weekName = "星期一";
                        break;
                    case 2:
                        weekName = "星期二";
                        break;
                    case 3:
                        weekName = "星期三";
                        break;
                    case 4:
                        weekName = "星期四";
                        break;
                    case 5:
                        weekName = "星期五";
                        break;
                    case 6:
                        weekName = "星期六";
                        break;
                    case 7:
                        weekName = "星期日";
                        break;
                }
                DdlStartWeekday.Items.Add(new ListItem(weekName, i.ToString()));
            }
            for (var i = 0; i < 24; i++)
            {
                DdlStartHour.Items.Add(new ListItem(i + "点", i.ToString()));
            }

            var listItem = new ListItem("周", "5040");
            DdlPeriodIntervalType.Items.Add(listItem);
            listItem = new ListItem("天", "720");
            DdlPeriodIntervalType.Items.Add(listItem);
            listItem = new ListItem("小时", "12");
            DdlPeriodIntervalType.Items.Add(listItem);
            listItem = new ListItem("分钟", "1")
            {
                Selected = true
            };
            DdlPeriodIntervalType.Items.Add(listItem);

            if (_serviceType == EServiceType.Create)
            {
                PhCreate.Visible = true;

                if (PublishmentSystemId != 0)
                {
                    NodeManager.AddListItems(LbCreateChannelId.Items, PublishmentSystemInfo, false, true, Body.AdminName);
                }
                else
                {
                    foreach (var publishmentSystemId in PublishmentSystemManager.GetPublishmentSystemIdList())
                    {
                        var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                        var item = new ListItem(publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemId.ToString());
                        LbCreateChannelId.Items.Add(item);
                    }
                }

                ECreateTypeUtils.AddListItems(CblCreateCreateTypes);
            }
            else if (_serviceType == EServiceType.Gather)
            {
                PhGather.Visible = true;

                if (PublishmentSystemId != 0)
                {
                    var gatherRuleNameList = DataProvider.GatherRuleDao.GetGatherRuleNameList(PublishmentSystemId);
                    var gatherTypeValue = EGatherTypeUtils.GetValue(EGatherType.Web);
                    var gatherTypeText = EGatherTypeUtils.GetText(EGatherType.Web);
                    foreach (var gatherName in gatherRuleNameList)
                    {
                        LbGather.Items.Add(new ListItem(gatherName + "(" + gatherTypeText + ")", gatherTypeValue + "_" + gatherName));
                    }
                    gatherRuleNameList = DataProvider.GatherDatabaseRuleDao.GetGatherRuleNameList(PublishmentSystemId);
                    gatherTypeValue = EGatherTypeUtils.GetValue(EGatherType.Database);
                    gatherTypeText = EGatherTypeUtils.GetText(EGatherType.Database);
                    foreach (var gatherName in gatherRuleNameList)
                    {
                        LbGather.Items.Add(new ListItem(gatherName + "(" + gatherTypeText + ")", gatherTypeValue + "_" + gatherName));
                    }
                    gatherRuleNameList = DataProvider.GatherFileRuleDao.GetGatherRuleNameList(PublishmentSystemId);
                    gatherTypeValue = EGatherTypeUtils.GetValue(EGatherType.File);
                    gatherTypeText = EGatherTypeUtils.GetText(EGatherType.File);
                    foreach (var gatherName in gatherRuleNameList)
                    {
                        LbGather.Items.Add(new ListItem(gatherName + "(" + gatherTypeText + ")", gatherTypeValue + "_" + gatherName));
                    }

                    LtlGather.Text = "选择需要定时执行的采集名称";
                }
                else
                {
                    foreach (var publishmentSystemId in PublishmentSystemManager.GetPublishmentSystemIdList())
                    {
                        var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                        var item = new ListItem(publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemId.ToString());
                        LbGather.Items.Add(item);
                    }

                    LtlGather.Text = "选择需要定时采集的站点";
                }
            }
            else if (_serviceType == EServiceType.Backup)
            {
                PhBackup.Visible = true;

                if (PublishmentSystemId != 0)
                {
                    PhBackupPublishmentSystem.Visible = false;
                }
                else
                {
                    PhBackupPublishmentSystem.Visible = true;
                    var arraylist = PublishmentSystemManager.GetPublishmentSystemIdList();
                    foreach (var publishmentSystemId in arraylist)
                    {
                        var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                        var item = new ListItem(publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemId.ToString());
                        LbBackupPublishmentSystemId.Items.Add(item);
                    }
                }

                EBackupTypeUtils.AddListItems(DdlBackupType);
            }

            if (Body.IsQueryExists("TaskID"))
            {
                var taskId = Body.GetQueryInt("TaskID");
                var taskInfo = DataProvider.TaskDao.GetTaskInfo(taskId);
                if (taskInfo != null)
                {
                    TbTaskName.Text = taskInfo.TaskName;
                    TbTaskName.Enabled = false;

                    ControlUtils.SelectListItems(DdlFrequencyType, EFrequencyTypeUtils.GetValue(taskInfo.FrequencyType));
                    ControlUtils.SelectListItems(DdlStartDay, taskInfo.StartDay.ToString());
                    ControlUtils.SelectListItems(DdlStartWeekday, taskInfo.StartWeekday.ToString());
                    ControlUtils.SelectListItems(DdlStartHour, taskInfo.StartHour.ToString());

                    if (taskInfo.PeriodIntervalMinute % 5040 == 0)
                    {
                        TbPeriodInterval.Text = Convert.ToInt32(taskInfo.PeriodIntervalMinute / 5040).ToString();
                        ControlUtils.SelectListItems(DdlPeriodIntervalType, "5040");
                    }
                    else if (taskInfo.PeriodIntervalMinute % 720 == 0)
                    {
                        TbPeriodInterval.Text = Convert.ToInt32(taskInfo.PeriodIntervalMinute / 720).ToString();
                        ControlUtils.SelectListItems(DdlPeriodIntervalType, "720");
                    }
                    else if (taskInfo.PeriodIntervalMinute % 12 == 0)
                    {
                        TbPeriodInterval.Text = Convert.ToInt32(taskInfo.PeriodIntervalMinute / 12).ToString();
                        ControlUtils.SelectListItems(DdlPeriodIntervalType, "12");
                    }
                    else
                    {
                        TbPeriodInterval.Text = taskInfo.PeriodIntervalMinute.ToString();
                        ControlUtils.SelectListItems(DdlPeriodIntervalType, "1");
                    }

                    TbDescription.Text = taskInfo.Description;

                    if (_serviceType == EServiceType.Create)
                    {
                        var taskCreateInfo = new TaskCreateInfo(taskInfo.ServiceParameters);
                        if (taskCreateInfo.IsCreateAll)
                        {
                            foreach (ListItem item in LbCreateChannelId.Items)
                            {
                                item.Selected = true;
                            }
                            CbCreateIsCreateAll.Checked = true;
                        }
                        else
                        {
                            var channelIdList = TranslateUtils.StringCollectionToStringList(taskCreateInfo.ChannelIdCollection);
                            ControlUtils.SelectListItems(LbCreateChannelId, channelIdList);
                            CbCreateIsCreateAll.Checked = false;
                        }
                        var createTypeList = TranslateUtils.StringCollectionToStringList(taskCreateInfo.CreateTypes);
                        foreach (ListItem item in CblCreateCreateTypes.Items)
                        {
                            item.Selected = createTypeList.Contains(item.Value);
                        }
                    }
                    else if (_serviceType == EServiceType.Gather)
                    {
                        var taskGatherInfo = new TaskGatherInfo(taskInfo.ServiceParameters);
                        if (PublishmentSystemId != 0)
                        {
                            var webGatherNames = TranslateUtils.StringCollectionToStringList(taskGatherInfo.WebGatherNames);
                            var databaseGatherNames = TranslateUtils.StringCollectionToStringList(taskGatherInfo.DatabaseGatherNames);
                            var fileGatherNames = TranslateUtils.StringCollectionToStringList(taskGatherInfo.FileGatherNames);
                            foreach (ListItem item in LbGather.Items)
                            {
                                var gatherType = EGatherTypeUtils.GetEnumType(item.Value.Split('_')[0]);
                                var gatherName = item.Value.Substring(item.Value.Split('_')[0].Length + 1);
                                if (gatherType == EGatherType.Web && webGatherNames.Contains(gatherName))
                                {
                                    item.Selected = true;
                                }
                                else if (gatherType == EGatherType.Database && databaseGatherNames.Contains(gatherName))
                                {
                                    item.Selected = true;
                                }
                                else if (gatherType == EGatherType.File && fileGatherNames.Contains(gatherName))
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                        else
                        {
                            var publishmentSystemIdList = TranslateUtils.StringCollectionToStringList(taskGatherInfo.PublishmentSystemIdCollection);
                            ControlUtils.SelectListItems(LbGather, publishmentSystemIdList);
                        }
                    }
                    else if (_serviceType == EServiceType.Backup)
                    {
                        var taskBackupInfo = new TaskBackupInfo(taskInfo.ServiceParameters);

                        if (taskInfo.PublishmentSystemId == 0)
                        {
                            if (taskBackupInfo.IsBackupAll)
                            {
                                foreach (ListItem item in LbBackupPublishmentSystemId.Items)
                                {
                                    item.Selected = true;
                                }
                                CbBackupIsBackupAll.Checked = true;
                            }
                            else
                            {
                                var publishmentSystemIdList = TranslateUtils.StringCollectionToStringList(taskBackupInfo.PublishmentSystemIdCollection);
                                ControlUtils.SelectListItems(LbBackupPublishmentSystemId, publishmentSystemIdList);
                                CbBackupIsBackupAll.Checked = false;
                            }
                        }
                        else
                        {
                            ControlUtils.SelectListItems(LbBackupPublishmentSystemId, taskInfo.PublishmentSystemId.ToString());
                        }

                        ControlUtils.SelectListItems(DdlBackupType, EBackupTypeUtils.GetValue(taskBackupInfo.BackupType));
                    }
                }
            }

            DdlFrequencyType_SelectedIndexChanged(null, EventArgs.Empty);
        }

        public void DdlFrequencyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var frequencyType = EFrequencyTypeUtils.GetEnumType(DdlFrequencyType.SelectedValue);
            PhNotPeriod.Visible = true;
            PhPeriodIntervalMinute.Visible = false;
            if (frequencyType == EFrequencyType.Month)
            {
                DdlStartWeekday.Enabled = false;
                DdlStartDay.Enabled = DdlStartHour.Enabled = true;
            }
            else if (frequencyType == EFrequencyType.Week)
            {
                DdlStartDay.Enabled = false;
                DdlStartWeekday.Enabled = DdlStartHour.Enabled = true;
            }
            else if (frequencyType == EFrequencyType.Day)
            {
                DdlStartDay.Enabled = DdlStartWeekday.Enabled = false;
                DdlStartHour.Enabled = true;
            }
            else if (frequencyType == EFrequencyType.Hour)
            {
                DdlStartDay.Enabled = DdlStartWeekday.Enabled = DdlStartHour.Enabled = false;
            }
            else
            {
                PhNotPeriod.Visible = false;
                PhPeriodIntervalMinute.Visible = true;

                if (frequencyType == EFrequencyType.JustInTime)
                {
                    PhPeriodIntervalMinute.Visible = false;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            var serviceParamters = new ExtendedAttributes();
            if (_serviceType == EServiceType.Create)
            {
                var taskCreateInfo = new TaskCreateInfo(string.Empty)
                {
                    IsCreateAll = CbCreateIsCreateAll.Checked,
                    ChannelIdCollection =
                        TranslateUtils.ObjectCollectionToString(
                            ControlUtils.GetSelectedListControlValueArrayList(LbCreateChannelId)),
                    CreateTypes =
                        TranslateUtils.ObjectCollectionToString(
                            ControlUtils.GetSelectedListControlValueArrayList(CblCreateCreateTypes))
                };
                serviceParamters = taskCreateInfo;
            }
            else if (_serviceType == EServiceType.Gather)
            {
                var taskGatherInfo = new TaskGatherInfo(string.Empty);
                if (PublishmentSystemId != 0)
                {
                    var webGatherNames = new ArrayList();
                    var databaseGatherNames = new ArrayList();
                    var fileGatherNames = new ArrayList();
                    foreach (ListItem item in LbGather.Items)
                    {
                        if (item.Selected)
                        {
                            var gatherType = EGatherTypeUtils.GetEnumType(item.Value.Split('_')[0]);
                            var gatherName = item.Value.Substring(item.Value.Split('_')[0].Length + 1);
                            if (gatherType == EGatherType.Web && !webGatherNames.Contains(gatherName))
                            {
                                webGatherNames.Add(gatherName);
                            }
                            else if (gatherType == EGatherType.Database && !databaseGatherNames.Contains(gatherName))
                            {
                                databaseGatherNames.Add(gatherName);
                            }
                            else if (gatherType == EGatherType.File && !fileGatherNames.Contains(gatherName))
                            {
                                fileGatherNames.Add(gatherName);
                            }
                        }
                    }
                    taskGatherInfo.WebGatherNames = TranslateUtils.ObjectCollectionToString(webGatherNames);
                    taskGatherInfo.DatabaseGatherNames = TranslateUtils.ObjectCollectionToString(databaseGatherNames);
                    taskGatherInfo.FileGatherNames = TranslateUtils.ObjectCollectionToString(fileGatherNames);
                }
                else
                {
                    taskGatherInfo.PublishmentSystemIdCollection = TranslateUtils.ObjectCollectionToString(ControlUtils.GetSelectedListControlValueArrayList(LbGather));
                }
                serviceParamters = taskGatherInfo;
            }
            else if (_serviceType == EServiceType.Backup)
            {
                var taskBackupInfo = new TaskBackupInfo(string.Empty)
                {
                    BackupType = EBackupTypeUtils.GetEnumType(DdlBackupType.SelectedValue),
                    IsBackupAll = CbBackupIsBackupAll.Checked,
                    PublishmentSystemIdCollection =
                        TranslateUtils.ObjectCollectionToString(
                            ControlUtils.GetSelectedListControlValueArrayList(LbBackupPublishmentSystemId))
                };
                serviceParamters = taskBackupInfo;
            }

            if (Body.IsQueryExists("TaskID"))
            {
                try
                {
                    var taskId = Body.GetQueryInt("TaskID");
                    var taskInfo = DataProvider.TaskDao.GetTaskInfo(taskId);
                    taskInfo.FrequencyType = EFrequencyTypeUtils.GetEnumType(DdlFrequencyType.SelectedValue);
                    if (taskInfo.FrequencyType == EFrequencyType.Period)
                    {
                        taskInfo.PeriodIntervalMinute = TranslateUtils.ToInt(TbPeriodInterval.Text) * TranslateUtils.ToInt(DdlPeriodIntervalType.SelectedValue);
                    }
                    else if (taskInfo.FrequencyType != EFrequencyType.JustInTime)
                    {
                        taskInfo.StartDay = TranslateUtils.ToInt(DdlStartDay.SelectedValue);
                        taskInfo.StartWeekday = TranslateUtils.ToInt(DdlStartWeekday.SelectedValue);
                        taskInfo.StartHour = TranslateUtils.ToInt(DdlStartHour.SelectedValue);
                    }
                    taskInfo.Description = TbDescription.Text;
                    taskInfo.ServiceParameters = serviceParamters.ToString();

                    DataProvider.TaskDao.Update(taskInfo);

                    Body.AddSiteLog(PublishmentSystemId, $"修改{EServiceTypeUtils.GetText(taskInfo.ServiceType)}任务", $"任务名称:{taskInfo.TaskName}");

                    SuccessMessage("任务修改成功！");
                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "任务修改失败！");
                }
            }
            else
            {
                if (DataProvider.TaskDao.IsExists(TbTaskName.Text))
                {
                    FailMessage("任务添加失败，任务名称已存在！");
                }
                else
                {
                    try
                    {
                        var taskInfo = new TaskInfo
                        {
                            TaskName = TbTaskName.Text,
                            PublishmentSystemId = PublishmentSystemId,
                            ServiceType = _serviceType,
                            FrequencyType = EFrequencyTypeUtils.GetEnumType(DdlFrequencyType.SelectedValue)
                        };
                        if (taskInfo.FrequencyType == EFrequencyType.Period)
                        {
                            taskInfo.PeriodIntervalMinute = TranslateUtils.ToInt(TbPeriodInterval.Text) * TranslateUtils.ToInt(DdlPeriodIntervalType.SelectedValue);
                        }
                        else if (taskInfo.FrequencyType != EFrequencyType.JustInTime)
                        {
                            taskInfo.StartDay = TranslateUtils.ToInt(DdlStartDay.SelectedValue);
                            taskInfo.StartWeekday = TranslateUtils.ToInt(DdlStartWeekday.SelectedValue);
                            taskInfo.StartHour = TranslateUtils.ToInt(DdlStartHour.SelectedValue);
                        }
                        taskInfo.Description = TbDescription.Text;

                        taskInfo.ServiceParameters = serviceParamters.ToString();

                        taskInfo.IsEnabled = true;
                        taskInfo.AddDate = DateTime.Now;
                        taskInfo.OnlyOnceDate = DateUtils.SqlMinValue;
                        taskInfo.LastExecuteDate = DateUtils.SqlMinValue;

                        DataProvider.TaskDao.Insert(taskInfo);

                        Body.AddSiteLog(PublishmentSystemId, $"添加{EServiceTypeUtils.GetText(taskInfo.ServiceType)}任务", $"任务名称:{taskInfo.TaskName}");

                        SuccessMessage("任务添加成功！");
                        isChanged = true;
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "任务添加失败！");
                    }
                }
            }

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, PageTask.GetRedirectUrl(PublishmentSystemId, _serviceType));
            }
        }
    }
}
