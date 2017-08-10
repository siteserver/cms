using System;
using System.Collections.Generic;
using System.Globalization;
using BaiRong.Core;
using BaiRong.Core.IO;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
    public class ServiceManager
    {
        private const string CacheKeyStatus = "SiteServer.CMS.Core.ServiceCacheManager.Status";
        private const string CacheFileNameStatus = "ServiceStatusCache.txt";
        private const string CacheKeyAllTaskInfoList = "SiteServer.CMS.Core.ServiceCacheManager.AllTaskInfoList";
        private const string CacheFileNameTaskCache = "ServiceTaskCache.txt";
        private const string CacheKeyIsPendingCreate = "SiteServer.CMS.Core.ServiceCacheManager.CacheKeyIsPendingCreateTask";
        private const string CacheFileNameIsPendingCreate = "ServiceIsPendingCreateCache.txt";

        private static readonly FileWatcherClass statusCacheFileWatcher;
        private static readonly FileWatcherClass taskCacheFileWatcher;
        private static readonly FileWatcherClass isPendingCreateCacheFileWatcher;

        static ServiceManager()
        {
            statusCacheFileWatcher = new FileWatcherClass(CacheUtils.GetCacheFilePath(CacheFileNameStatus));
            statusCacheFileWatcher.OnFileChange += StatusCache_OnFileChange;

            taskCacheFileWatcher = new FileWatcherClass(CacheUtils.GetCacheFilePath(CacheFileNameTaskCache));
            taskCacheFileWatcher.OnFileChange += TaskCache_OnFileChange;

            isPendingCreateCacheFileWatcher = new FileWatcherClass(CacheUtils.GetCacheFilePath(CacheFileNameIsPendingCreate));
            isPendingCreateCacheFileWatcher.OnFileChange += IsPendingCreateCache_OnFileChange;
        }

        private static void StatusCache_OnFileChange(object sender, EventArgs e)
        {
            CacheUtils.Remove(CacheKeyStatus);
        }

        private static void TaskCache_OnFileChange(object sender, EventArgs e)
        {
            CacheUtils.Remove(CacheKeyAllTaskInfoList);
        }

        private static void IsPendingCreateCache_OnFileChange(object sender, EventArgs e)
        {
            CacheUtils.Remove(CacheKeyIsPendingCreate);
        }

        public static List<TaskInfo> GetAllTaskInfoList()
        {
            if (CacheUtils.IsCache(CacheKeyAllTaskInfoList))
            {
                return CacheUtils.Get<List<TaskInfo>>(CacheKeyAllTaskInfoList);
            }
            var list = DataProvider.TaskDao.GetAllTaskInfoList();
            CacheUtils.Insert(CacheKeyAllTaskInfoList, list);
            return list;
        }

        public static bool IsPendingCreateTask()
        {
            if (CacheUtils.IsCache(CacheKeyIsPendingCreate))
            {
                return (bool)CacheUtils.Get(CacheKeyIsPendingCreate);
            }
            var isPendingTask = DataProvider.CreateTaskDao.IsPendingTask();
            CacheUtils.Insert(CacheKeyIsPendingCreate, isPendingTask);
            return isPendingTask;
        }

        public static void ClearStatusCache()
        {
            if (!CacheUtils.IsCache(CacheKeyStatus)) return;

            CacheUtils.Remove(CacheKeyStatus);
            CacheUtils.UpdateTemporaryCacheFile(CacheFileNameStatus);
        }

        public static void ClearTaskCache()
        {
            if (!CacheUtils.IsCache(CacheKeyAllTaskInfoList)) return;

            CacheUtils.Remove(CacheKeyAllTaskInfoList);
            CacheUtils.UpdateTemporaryCacheFile(CacheFileNameTaskCache);
        }

        public static void ClearIsPendingCreateCache()
        {
            if (!CacheUtils.IsCache(CacheKeyIsPendingCreate)) return;

            CacheUtils.Remove(CacheKeyIsPendingCreate);
            CacheUtils.UpdateTemporaryCacheFile(CacheFileNameIsPendingCreate);
        }

        /// <summary>
        /// 服务组件是否启用
        /// </summary>
        /// <returns></returns>
        public static bool IsServiceOnline()
        {
            var cacheValue = CacheUtils.Get<string>(CacheKeyStatus);
            if (TranslateUtils.ToBool(cacheValue))
            {
                return true;
            }

            var retval = true;
            
            var value = CacheDbUtils.GetValue(CacheKeyStatus);
            if (string.IsNullOrEmpty(value))
            {
                retval = false;
            }
            else
            {
                var ts = DateTime.Now - TranslateUtils.ToDateTime(value);
                if (ts.TotalMinutes > 30)
                {
                    retval = false;
                }
                else
                {
                    CacheUtils.InsertMinutes(CacheKeyStatus, true.ToString(), 10);
                }
            }

            if (!retval)
            {
                CacheUtils.InsertMinutes(CacheKeyStatus, false.ToString(), 10);
            }
            
            return retval;
        }

        public static void SetServiceOnline(bool isOnline)
        {
            if (isOnline)
            {
                var cacheValue = CacheUtils.Get<string>(CacheKeyStatus);
                if (TranslateUtils.ToBool(cacheValue)) return;

                CacheDbUtils.RemoveAndInsert(CacheKeyStatus, DateTime.Now.ToString(CultureInfo.InvariantCulture));
                CacheUtils.InsertMinutes(CacheKeyStatus, true.ToString(), 10);
            }
            else
            {
                CacheDbUtils.GetValueAndRemove(CacheKeyStatus);
                ClearStatusCache();
                ClearIsPendingCreateCache();
                ClearTaskCache();
            }
        }
    }
}
