using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTaskAdd : BasePageCms
    {
        public TextBox TaskName;
        public DropDownList FrequencyType;

        public PlaceHolder PlaceHolder_PeriodIntervalMinute;
        public TextBox PeriodInterval;
        public DropDownList PeriodIntervalType;

        public PlaceHolder PlaceHolder_NotPeriod;
        public DropDownList StartDay;
        public DropDownList StartWeekday;
        public DropDownList StartHour;

        public TextBox Description;

        public PlaceHolder PlaceHolder_Create;
        public ListBox CreateChannelIDCollection;
        public CheckBox CreateIsCreateAll;
        public CheckBoxList CreateCreateTypes;

        public PlaceHolder PlaceHolder_Gather;
        public Help GatherHelp;
        public ListBox GatherListBox;

        public PlaceHolder PlaceHolder_Backup;
        public DropDownList BackupType;
        public PlaceHolder PlaceHolder_Backup_PublishmentSystem;
        public ListBox BackupPublishmentSystemIDCollection;
        public CheckBox BackupIsBackupAll;

        private EServiceType _serviceType;

        public static string GetOpenWindowStringToAdd(EServiceType serviceType, int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加任务", PageUtils.GetCmsUrl(nameof(ModalTaskAdd), new NameValueCollection
            {
                {"ServiceType", EServiceTypeUtils.GetValue(serviceType)},
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }));
        }

        public static string GetOpenWindowStringToEdit(int taskId, EServiceType serviceType, int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("修改任务", PageUtils.GetCmsUrl(nameof(ModalTaskAdd), new NameValueCollection
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
            if (!IsPostBack)
            {
                EFrequencyTypeUtils.AddListItems(FrequencyType, false);
                for (var i = 1; i < 32; i++)
                {
                    StartDay.Items.Add(new ListItem(i + "日", i.ToString()));
                }
                for (var i = 1; i < 8; i++)
                {
                    var weekName = string.Empty;
                    if (i == 1)
                    {
                        weekName = "星期一";
                    }
                    else if (i == 2)
                    {
                        weekName = "星期二";
                    }
                    else if (i == 3)
                    {
                        weekName = "星期三";
                    }
                    else if (i == 4)
                    {
                        weekName = "星期四";
                    }
                    else if (i == 5)
                    {
                        weekName = "星期五";
                    }
                    else if (i == 6)
                    {
                        weekName = "星期六";
                    }
                    else if (i == 7)
                    {
                        weekName = "星期日";
                    }
                    StartWeekday.Items.Add(new ListItem(weekName, i.ToString()));
                }
                for (var i = 0; i < 24; i++)
                {
                    StartHour.Items.Add(new ListItem(i + "点", i.ToString()));
                }

                var listItem = new ListItem("周", "5040");
                PeriodIntervalType.Items.Add(listItem);
                listItem = new ListItem("天", "720");
                PeriodIntervalType.Items.Add(listItem);
                listItem = new ListItem("小时", "12");
                PeriodIntervalType.Items.Add(listItem);
                listItem = new ListItem("分钟", "1");
                listItem.Selected = true;
                PeriodIntervalType.Items.Add(listItem);

                if (_serviceType == EServiceType.Create)
                {
                    PlaceHolder_Create.Visible = true;

                    if (PublishmentSystemId != 0)
                    {
                        NodeManager.AddListItems(CreateChannelIDCollection.Items, PublishmentSystemInfo, false, true, Body.AdministratorName);
                    }
                    else
                    {
                        var arraylist = PublishmentSystemManager.GetPublishmentSystemIdList();
                        foreach (int publishmentSystemID in arraylist)
                        {
                            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
                            var item = new ListItem(publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemId.ToString());
                            CreateChannelIDCollection.Items.Add(item);
                        }
                    }

                    ECreateTypeUtils.AddListItems(CreateCreateTypes);
                }
                else if (_serviceType == EServiceType.Gather)
                {
                    PlaceHolder_Gather.Visible = true;

                    if (PublishmentSystemId != 0)
                    {
                        var gatherRuleNameArrayList = DataProvider.GatherRuleDao.GetGatherRuleNameArrayList(PublishmentSystemId);
                        var gatherTypeValue = EGatherTypeUtils.GetValue(EGatherType.Web);
                        var gatherTypeText = EGatherTypeUtils.GetText(EGatherType.Web);
                        foreach (string gatherName in gatherRuleNameArrayList)
                        {
                            GatherListBox.Items.Add(new ListItem(gatherName + "(" + gatherTypeText + ")", gatherTypeValue + "_" + gatherName));
                        }
                        gatherRuleNameArrayList = DataProvider.GatherDatabaseRuleDao.GetGatherRuleNameArrayList(PublishmentSystemId);
                        gatherTypeValue = EGatherTypeUtils.GetValue(EGatherType.Database);
                        gatherTypeText = EGatherTypeUtils.GetText(EGatherType.Database);
                        foreach (string gatherName in gatherRuleNameArrayList)
                        {
                            GatherListBox.Items.Add(new ListItem(gatherName + "(" + gatherTypeText + ")", gatherTypeValue + "_" + gatherName));
                        }
                        gatherRuleNameArrayList = DataProvider.GatherFileRuleDao.GetGatherRuleNameArrayList(PublishmentSystemId);
                        gatherTypeValue = EGatherTypeUtils.GetValue(EGatherType.File);
                        gatherTypeText = EGatherTypeUtils.GetText(EGatherType.File);
                        foreach (string gatherName in gatherRuleNameArrayList)
                        {
                            GatherListBox.Items.Add(new ListItem(gatherName + "(" + gatherTypeText + ")", gatherTypeValue + "_" + gatherName));
                        }

                        GatherHelp.Text = "选择需要定时执行的采集名称";
                    }
                    else
                    {
                        var arraylist = PublishmentSystemManager.GetPublishmentSystemIdList();
                        foreach (int publishmentSystemID in arraylist)
                        {
                            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
                            var item = new ListItem(publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemId.ToString());
                            GatherListBox.Items.Add(item);
                        }

                        GatherHelp.Text = "选择需要定时采集的站点";
                    }
                }
                else if (_serviceType == EServiceType.Backup)
                {
                    PlaceHolder_Backup.Visible = true;

                    if (PublishmentSystemId != 0)
                    {
                        PlaceHolder_Backup_PublishmentSystem.Visible = false;
                    }
                    else
                    {
                        PlaceHolder_Backup_PublishmentSystem.Visible = true;
                        var arraylist = PublishmentSystemManager.GetPublishmentSystemIdList();
                        foreach (int publishmentSystemID in arraylist)
                        {
                            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
                            var item = new ListItem(publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemId.ToString());
                            BackupPublishmentSystemIDCollection.Items.Add(item);
                        }
                    }

                    EBackupTypeUtils.AddListItems(BackupType);
                }

                if (Body.IsQueryExists("TaskID"))
                {
                    var taskId = Body.GetQueryInt("TaskID");
                    var taskInfo = DataProvider.TaskDao.GetTaskInfo(taskId);
                    if (taskInfo != null)
                    {
                        TaskName.Text = taskInfo.TaskName;
                        TaskName.Enabled = false;

                        ControlUtils.SelectListItems(FrequencyType, EFrequencyTypeUtils.GetValue(taskInfo.FrequencyType));
                        ControlUtils.SelectListItems(StartDay, taskInfo.StartDay.ToString());
                        ControlUtils.SelectListItems(StartWeekday, taskInfo.StartWeekday.ToString());
                        ControlUtils.SelectListItems(StartHour, taskInfo.StartHour.ToString());

                        if (taskInfo.PeriodIntervalMinute % 5040 == 0)
                        {
                            PeriodInterval.Text = Convert.ToInt32(taskInfo.PeriodIntervalMinute / 5040).ToString();
                            ControlUtils.SelectListItems(PeriodIntervalType, "5040");
                        }
                        else if (taskInfo.PeriodIntervalMinute % 720 == 0)
                        {
                            PeriodInterval.Text = Convert.ToInt32(taskInfo.PeriodIntervalMinute / 720).ToString();
                            ControlUtils.SelectListItems(PeriodIntervalType, "720");
                        }
                        else if (taskInfo.PeriodIntervalMinute % 12 == 0)
                        {
                            PeriodInterval.Text = Convert.ToInt32(taskInfo.PeriodIntervalMinute / 12).ToString();
                            ControlUtils.SelectListItems(PeriodIntervalType, "12");
                        }
                        else
                        {
                            PeriodInterval.Text = taskInfo.PeriodIntervalMinute.ToString();
                            ControlUtils.SelectListItems(PeriodIntervalType, "1");
                        }

                        Description.Text = taskInfo.Description;

                        if (_serviceType == EServiceType.Create)
                        {
                            var taskCreateInfo = new TaskCreateInfo(taskInfo.ServiceParameters);
                            if (taskCreateInfo.IsCreateAll)
                            {
                                foreach (ListItem item in CreateChannelIDCollection.Items)
                                {
                                    item.Selected = true;
                                }
                                CreateIsCreateAll.Checked = true;
                            }
                            else
                            {
                                var channelIdList = TranslateUtils.StringCollectionToStringList(taskCreateInfo.ChannelIDCollection);
                                ControlUtils.SelectListItems(CreateChannelIDCollection, channelIdList);
                                CreateIsCreateAll.Checked = false;
                            }
                            var createTypeArrayList = TranslateUtils.StringCollectionToStringList(taskCreateInfo.CreateTypes);
                            foreach (ListItem item in CreateCreateTypes.Items)
                            {
                                if (createTypeArrayList.Contains(item.Value))
                                {
                                    item.Selected = true;
                                }
                                else
                                {
                                    item.Selected = false;
                                }
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
                                foreach (ListItem item in GatherListBox.Items)
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
                                var publishmentSystemIdList = TranslateUtils.StringCollectionToStringList(taskGatherInfo.PublishmentSystemIDCollection);
                                ControlUtils.SelectListItems(GatherListBox, publishmentSystemIdList);
                            }
                        }
                        else if (_serviceType == EServiceType.Backup)
                        {
                            var taskBackupInfo = new TaskBackupInfo(taskInfo.ServiceParameters);

                            if (taskInfo.PublishmentSystemID == 0)
                            {
                                if (taskBackupInfo.IsBackupAll)
                                {
                                    foreach (ListItem item in BackupPublishmentSystemIDCollection.Items)
                                    {
                                        item.Selected = true;
                                    }
                                    BackupIsBackupAll.Checked = true;
                                }
                                else
                                {
                                    var publishmentSystemIdList = TranslateUtils.StringCollectionToStringList(taskBackupInfo.PublishmentSystemIDCollection);
                                    ControlUtils.SelectListItems(BackupPublishmentSystemIDCollection, publishmentSystemIdList);
                                    BackupIsBackupAll.Checked = false;
                                }
                            }
                            else
                            {
                                ControlUtils.SelectListItems(BackupPublishmentSystemIDCollection, taskInfo.PublishmentSystemID.ToString());
                            }

                            ControlUtils.SelectListItems(BackupType, EBackupTypeUtils.GetValue(taskBackupInfo.BackupType));
                        }
                    }
                }

                FrequencyType_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        public void FrequencyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var frequencyType = EFrequencyTypeUtils.GetEnumType(FrequencyType.SelectedValue);
            PlaceHolder_NotPeriod.Visible = true;
            PlaceHolder_PeriodIntervalMinute.Visible = false;
            if (frequencyType == EFrequencyType.Month)
            {
                StartWeekday.Enabled = false;
                StartDay.Enabled = StartHour.Enabled = true;
            }
            else if (frequencyType == EFrequencyType.Week)
            {
                StartDay.Enabled = false;
                StartWeekday.Enabled = StartHour.Enabled = true;
            }
            else if (frequencyType == EFrequencyType.Day)
            {
                StartDay.Enabled = StartWeekday.Enabled = false;
                StartHour.Enabled = true;
            }
            else if (frequencyType == EFrequencyType.Hour)
            {
                StartDay.Enabled = StartWeekday.Enabled = StartHour.Enabled = false;
            }
            else
            {
                PlaceHolder_NotPeriod.Visible = false;
                PlaceHolder_PeriodIntervalMinute.Visible = true;

                if (frequencyType == EFrequencyType.JustInTime)
                {
                    PlaceHolder_PeriodIntervalMinute.Visible = false;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            var serviceParamters = new ExtendedAttributes();
            if (_serviceType == EServiceType.Create)
            {
                var taskCreateInfo = new TaskCreateInfo(string.Empty);
                taskCreateInfo.IsCreateAll = CreateIsCreateAll.Checked;
                taskCreateInfo.ChannelIDCollection = TranslateUtils.ObjectCollectionToString(ControlUtils.GetSelectedListControlValueArrayList(CreateChannelIDCollection));
                taskCreateInfo.CreateTypes = TranslateUtils.ObjectCollectionToString(ControlUtils.GetSelectedListControlValueArrayList(CreateCreateTypes));
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
                    foreach (ListItem item in GatherListBox.Items)
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
                    taskGatherInfo.PublishmentSystemIDCollection = TranslateUtils.ObjectCollectionToString(ControlUtils.GetSelectedListControlValueArrayList(GatherListBox));
                }
                serviceParamters = taskGatherInfo;
            }
            else if (_serviceType == EServiceType.Backup)
            {
                var taskBackupInfo = new TaskBackupInfo(string.Empty);
                taskBackupInfo.BackupType = EBackupTypeUtils.GetEnumType(BackupType.SelectedValue);
                taskBackupInfo.IsBackupAll = BackupIsBackupAll.Checked;
                taskBackupInfo.PublishmentSystemIDCollection = TranslateUtils.ObjectCollectionToString(ControlUtils.GetSelectedListControlValueArrayList(BackupPublishmentSystemIDCollection));
                serviceParamters = taskBackupInfo;
            }

            if (Body.IsQueryExists("TaskID"))
            {
                try
                {
                    var taskId = Body.GetQueryInt("TaskID");
                    var taskInfo = DataProvider.TaskDao.GetTaskInfo(taskId);
                    taskInfo.FrequencyType = EFrequencyTypeUtils.GetEnumType(FrequencyType.SelectedValue);
                    if (taskInfo.FrequencyType == EFrequencyType.Period)
                    {
                        taskInfo.PeriodIntervalMinute = TranslateUtils.ToInt(PeriodInterval.Text) * TranslateUtils.ToInt(PeriodIntervalType.SelectedValue);
                    }
                    else if (taskInfo.FrequencyType != EFrequencyType.JustInTime)
                    {
                        taskInfo.StartDay = TranslateUtils.ToInt(StartDay.SelectedValue);
                        taskInfo.StartWeekday = TranslateUtils.ToInt(StartWeekday.SelectedValue);
                        taskInfo.StartHour = TranslateUtils.ToInt(StartHour.SelectedValue);
                    }
                    taskInfo.Description = Description.Text;
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
                if (DataProvider.TaskDao.IsExists(TaskName.Text))
                {
                    FailMessage("任务添加失败，任务名称已存在！");
                }
                else
                {
                    try
                    {
                        var taskInfo = new TaskInfo();
                        taskInfo.TaskName = TaskName.Text;
                        taskInfo.PublishmentSystemID = PublishmentSystemId;
                        taskInfo.ServiceType = _serviceType;
                        taskInfo.FrequencyType = EFrequencyTypeUtils.GetEnumType(FrequencyType.SelectedValue);
                        if (taskInfo.FrequencyType == EFrequencyType.Period)
                        {
                            taskInfo.PeriodIntervalMinute = TranslateUtils.ToInt(PeriodInterval.Text) * TranslateUtils.ToInt(PeriodIntervalType.SelectedValue);
                        }
                        else if (taskInfo.FrequencyType != EFrequencyType.JustInTime)
                        {
                            taskInfo.StartDay = TranslateUtils.ToInt(StartDay.SelectedValue);
                            taskInfo.StartWeekday = TranslateUtils.ToInt(StartWeekday.SelectedValue);
                            taskInfo.StartHour = TranslateUtils.ToInt(StartHour.SelectedValue);
                        }
                        taskInfo.Description = Description.Text;

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
