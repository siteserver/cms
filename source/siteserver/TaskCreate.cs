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
                    List<int> nodeIdList = null;
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
                    List<int> publishmentSystemIDArrayList = null;
                    if (taskCreateInfo.IsCreateAll)
                    {
                        publishmentSystemIDArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();
                    }
                    else
                    {
                        publishmentSystemIDArrayList = TranslateUtils.StringCollectionToIntList(taskCreateInfo.ChannelIDCollection);
                    }
                    foreach (int publishmentSystemID in publishmentSystemIDArrayList)
                    {
                        var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemID);
                        Create(createChannel, createContent, createFile, taskInfo, publishmentSystemID, nodeIdList);
                    }
                }
            }

            return true;
        }

        private static void Create(bool createChannel, bool createContent, bool createFile, TaskInfo taskInfo, int publishmentSystemID, List<int> nodeIdList)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
            if (publishmentSystemInfo != null)
            {
                var fso = new FileSystemObject(publishmentSystemID);
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
                            foreach (int nodeID in errorChannelNodeIdSortedList.Keys)
                            {
                                try
                                {
                                    fso.CreateChannel(nodeID);
                                }
                                catch { }
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
                            foreach (int nodeID in errorContentNodeIdSortedList.Keys)
                            {
                                try
                                {
                                    fso.CreateContents(nodeID);
                                }
                                catch { }
                            }
                        }
                    }
                }
                if (createFile)
                {
                    var templateIDArrayList = DataProvider.TemplateDao.GetTemplateIdArrayListByType(publishmentSystemID, ETemplateType.FileTemplate);
                    foreach (int templateID in templateIDArrayList)
                    {
                        try
                        {
                            fso.CreateFile(templateID);
                        }
                        catch (Exception ex)
                        {
                            errorFileTemplateIdSortedList.Add(templateID, ex.Message);
                        }
                    }
                    if (errorFileTemplateIdSortedList.Count > 0)
                    {
                        foreach (int templateID in errorFileTemplateIdSortedList.Keys)
                        {
                            try
                            {
                                fso.CreateFile(templateID);
                            }
                            catch { }
                        }
                    }
                }
                if (errorChannelNodeIdSortedList.Count > 0 || errorContentNodeIdSortedList.Count > 0 || errorFileTemplateIdSortedList.Count > 0)
                {
                    var errorMessage = new StringBuilder();
                    foreach (int nodeID in errorChannelNodeIdSortedList.Keys)
                    {
                        errorMessage.AppendFormat("Create channel {0} error:{1}", nodeID, errorChannelNodeIdSortedList[nodeID]).Append(StringUtils.Constants.ReturnAndNewline);
                    }
                    foreach (int nodeID in errorContentNodeIdSortedList.Keys)
                    {
                        errorMessage.AppendFormat("Create content {0} error:{1}", nodeID, errorContentNodeIdSortedList[nodeID]).Append(StringUtils.Constants.ReturnAndNewline);
                    }
                    foreach (int templateID in errorFileTemplateIdSortedList.Keys)
                    {
                        errorMessage.AppendFormat("Create file {0} error:{1}", templateID, errorFileTemplateIdSortedList[templateID]).Append(StringUtils.Constants.ReturnAndNewline);
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
