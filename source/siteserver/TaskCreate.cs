using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser;

namespace siteserver
{
    public class TaskCreate
    {
        public static bool Execute(TaskInfo taskInfo)
        {
            var taskCreateInfo = new TaskCreateInfo(taskInfo.ServiceParameters);
            if (!string.IsNullOrEmpty(taskCreateInfo.CreateTypes))
            {
                var createTypeArrayList = TranslateUtils.StringCollectionToStringList(taskCreateInfo.CreateTypes);
                var createChannel = createTypeArrayList.Contains(ECreateTypeUtils.GetValue(ECreateType.Channel));
                var createContent = createTypeArrayList.Contains(ECreateTypeUtils.GetValue(ECreateType.Content));
                var createFile = createTypeArrayList.Contains(ECreateTypeUtils.GetValue(ECreateType.File));
                if (taskInfo.PublishmentSystemID != 0)
                {
                    List<int> nodeIdList;
                    if (taskCreateInfo.IsCreateAll)
                    {
                        nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(taskInfo.PublishmentSystemID);
                    }
                    else
                    {
                        nodeIdList = TranslateUtils.StringCollectionToIntList(taskCreateInfo.ChannelIDCollection);
                    }

                    Create(createChannel, createContent, createFile, taskInfo, taskInfo.PublishmentSystemID, nodeIdList);
                }
                else
                {
                    List<int> publishmentSystemIdArrayList;
                    if (taskCreateInfo.IsCreateAll)
                    {
                        publishmentSystemIdArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();
                    }
                    else
                    {
                        publishmentSystemIdArrayList = TranslateUtils.StringCollectionToIntList(taskCreateInfo.ChannelIDCollection);
                    }
                    foreach (var publishmentSystemId in publishmentSystemIdArrayList)
                    {
                        var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemId);
                        Create(createChannel, createContent, createFile, taskInfo, publishmentSystemId, nodeIdList);
                    }
                }
            }

            return true;
        }

        private static void Create(bool createChannel, bool createContent, bool createFile, TaskInfo taskInfo, int publishmentSystemId, List<int> nodeIdList)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo != null)
            {
                var fso = new FileSystemObject(publishmentSystemId);
                var errorChannelNodeIdSortedList = new SortedList();
                var errorContentNodeIdSortedList = new SortedList();
                var errorFileTemplateIdSortedList = new SortedList();

                if (nodeIdList != null && nodeIdList.Count > 0)
                {
                    if (createChannel)
                    {
                        foreach (var nodeId in nodeIdList)
                        {
                            try
                            {
                                fso.CreateChannel(nodeId);
                            }
                            catch (Exception ex)
                            {
                                errorChannelNodeIdSortedList.Add(nodeId, ex.Message);
                            }
                        }
                        if (errorChannelNodeIdSortedList.Count > 0)
                        {
                            foreach (int nodeId in errorChannelNodeIdSortedList.Keys)
                            {
                                try
                                {
                                    fso.CreateChannel(nodeId);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                        }
                    }
                    if (createContent)
                    {
                        foreach (var nodeId in nodeIdList)
                        {
                            try
                            {
                                fso.CreateContents(nodeId);
                            }
                            catch (Exception ex)
                            {
                                errorContentNodeIdSortedList.Add(nodeId, ex.Message);
                            }
                        }
                        if (errorContentNodeIdSortedList.Count > 0)
                        {
                            foreach (int nodeId in errorContentNodeIdSortedList.Keys)
                            {
                                try
                                {
                                    fso.CreateContents(nodeId);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                        }
                    }
                }
                if (createFile)
                {
                    var templateIdList = DataProvider.TemplateDao.GetTemplateIdListByType(publishmentSystemId, ETemplateType.FileTemplate);
                    foreach (var templateId in templateIdList)
                    {
                        try
                        {
                            fso.CreateFile(templateId);
                        }
                        catch (Exception ex)
                        {
                            errorFileTemplateIdSortedList.Add(templateId, ex.Message);
                        }
                    }
                    if (errorFileTemplateIdSortedList.Count > 0)
                    {
                        foreach (int templateId in errorFileTemplateIdSortedList.Keys)
                        {
                            try
                            {
                                fso.CreateFile(templateId);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                }
                if (errorChannelNodeIdSortedList.Count > 0 || errorContentNodeIdSortedList.Count > 0 || errorFileTemplateIdSortedList.Count > 0)
                {
                    var errorMessage = new StringBuilder();
                    foreach (int nodeId in errorChannelNodeIdSortedList.Keys)
                    {
                        errorMessage.AppendFormat("Create channel {0} error:{1}", nodeId, errorChannelNodeIdSortedList[nodeId]).Append(StringUtils.Constants.ReturnAndNewline);
                    }
                    foreach (int nodeId in errorContentNodeIdSortedList.Keys)
                    {
                        errorMessage.AppendFormat("Create content {0} error:{1}", nodeId, errorContentNodeIdSortedList[nodeId]).Append(StringUtils.Constants.ReturnAndNewline);
                    }
                    foreach (int templateId in errorFileTemplateIdSortedList.Keys)
                    {
                        errorMessage.AppendFormat("Create file {0} error:{1}", templateId, errorFileTemplateIdSortedList[templateId]).Append(StringUtils.Constants.ReturnAndNewline);
                    }
                }

                if (taskInfo.ServiceType == EServiceType.Create && taskInfo.FrequencyType == EFrequencyType.OnlyOnce)
                {
                    DataProvider.TaskDao.Delete(taskInfo.TaskID);
                }
            }
        }
    }
}
