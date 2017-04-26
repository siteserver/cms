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
            statusCacheFileWatcher = new FileWatcherClass(CacheManager.GetCacheFilePath(CacheFileNameStatus));
            statusCacheFileWatcher.OnFileChange += StatusCache_OnFileChange;

            taskCacheFileWatcher = new FileWatcherClass(CacheManager.GetCacheFilePath(CacheFileNameTaskCache));
            taskCacheFileWatcher.OnFileChange += TaskCache_OnFileChange;

            isPendingCreateCacheFileWatcher = new FileWatcherClass(CacheManager.GetCacheFilePath(CacheFileNameIsPendingCreate));
            isPendingCreateCacheFileWatcher.OnFileChange += IsPendingCreateCache_OnFileChange;
        }

        private static void StatusCache_OnFileChange(object sender, EventArgs e)
        {
            CacheManager.RemoveCache(CacheKeyStatus);
        }

        private static void TaskCache_OnFileChange(object sender, EventArgs e)
        {
            CacheManager.RemoveCache(CacheKeyAllTaskInfoList);
        }

        private static void IsPendingCreateCache_OnFileChange(object sender, EventArgs e)
        {
            CacheManager.RemoveCache(CacheKeyIsPendingCreate);
        }

        public static List<TaskInfo> GetAllTaskInfoList()
        {
            if (CacheManager.GetCache(CacheKeyAllTaskInfoList) != null)
            {
                return CacheManager.GetCache(CacheKeyAllTaskInfoList) as List<TaskInfo>;
            }
            var list = DataProvider.TaskDao.GetAllTaskInfoList();
            CacheManager.SetCache(CacheKeyAllTaskInfoList, list);
            return list;
        }

        public static bool IsPendingCreateTask()
        {
            if (CacheManager.GetCache(CacheKeyIsPendingCreate) != null)
            {
                return (bool)CacheManager.GetCache(CacheKeyIsPendingCreate);
            }
            var isPendingTask = DataProvider.CreateTaskDao.IsPendingTask();
            CacheManager.SetCache(CacheKeyIsPendingCreate, isPendingTask);
            return isPendingTask;
        }

        public static void ClearStatusCache()
        {
            CacheManager.RemoveCache(CacheKeyStatus);
            CacheManager.UpdateTemporaryCacheFile(CacheFileNameStatus);
        }

        public static void ClearTaskCache()
        {
            CacheManager.RemoveCache(CacheKeyAllTaskInfoList);
            CacheManager.UpdateTemporaryCacheFile(CacheFileNameTaskCache);
        }

        public static void ClearIsPendingCreateCache()
        {
            CacheManager.RemoveCache(CacheKeyIsPendingCreate);
            CacheManager.UpdateTemporaryCacheFile(CacheFileNameIsPendingCreate);
        }

        public static bool IsServiceOnline()
        {
            var cacheValue = CacheManager.GetCache(CacheKeyStatus) as string;
            if (TranslateUtils.ToBool(cacheValue))
            {
                return true;
            }

            var retval = true;
            
            var value = DbCacheManager.GetValue(CacheKeyStatus);
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
                    CacheManager.SetCache(CacheKeyStatus, true.ToString(), DateTime.Now.AddMinutes(10));
                }
            }

            if (!retval)
            {
                CacheManager.SetCache(CacheKeyStatus, false.ToString(), DateTime.Now.AddMinutes(10));
            }
            
            return retval;
        }

        public static void SetServiceOnline(bool isOnline)
        {
            if (isOnline)
            {
                var cacheValue = CacheManager.GetCache(CacheKeyStatus) as string;
                if (TranslateUtils.ToBool(cacheValue)) return;

                DbCacheManager.RemoveAndInsert(CacheKeyStatus, DateTime.Now.ToString(CultureInfo.InvariantCulture));
                CacheManager.SetCache(CacheKeyStatus, true.ToString(), DateTime.Now.AddMinutes(10));
            }
            else
            {
                DbCacheManager.GetValueAndRemove(CacheKeyStatus);
                ClearStatusCache();
                ClearIsPendingCreateCache();
                ClearTaskCache();
            }
        }
    }
}
