using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.IO;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
    public class ServiceManager
    {
        private const string CacheKeyStatus = "SiteServer.CMS.Core.ServiceManager.Status";
        private const string CacheKeyAllTaskInfoList = "SiteServer.CMS.Core.ServiceManager.AllTaskInfoList";
        private const string CacheKeyIsPendingCreate = "SiteServer.CMS.Core.ServiceManager.CacheKeyIsPendingCreateTask";

        private static readonly FileWatcherClass StatusCacheFileWatcher;
        private static readonly FileWatcherClass TaskCacheFileWatcher;
        private static readonly FileWatcherClass IsPendingCreateCacheFileWatcher;

        static ServiceManager()
        {
            StatusCacheFileWatcher = new FileWatcherClass(FileWatcherClass.ServiceStatus);
            StatusCacheFileWatcher.OnFileChange += StatusCache_OnFileChange;
            StatusCacheFileWatcher.OnFileDeleted += StatusCache_OnFileDeleted;

            TaskCacheFileWatcher = new FileWatcherClass(FileWatcherClass.ServiceTask);
            TaskCacheFileWatcher.OnFileChange += TaskCache_OnFileChange;

            IsPendingCreateCacheFileWatcher = new FileWatcherClass(FileWatcherClass.ServiceIsPendingCreate);
            IsPendingCreateCacheFileWatcher.OnFileChange += IsPendingCreateCache_OnFileChange;
        }

        private static void StatusCache_OnFileChange(object sender, EventArgs e)
        {
            CacheUtils.InsertMinutes(CacheKeyStatus, true.ToString(), 10);
        }

        private static void StatusCache_OnFileDeleted(object sender, EventArgs e)
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
            CacheUtils.InsertMinutes(CacheKeyIsPendingCreate, isPendingTask, 5);
            return isPendingTask;
        }

        public static void ClearStatusCache()
        {
            StatusCacheFileWatcher.UpdateCacheFile();
        }

        public static void ClearTaskCache()
        {
            TaskCacheFileWatcher.UpdateCacheFile();
        }

        public static void ClearIsPendingCreateCache()
        {
            IsPendingCreateCacheFileWatcher.UpdateCacheFile();
        }

        /// <summary>
        /// 服务组件是否启用
        /// </summary>
        /// <returns></returns>
        public static bool IsServiceOnline
        {
            get
            {
                var cacheValue = CacheUtils.Get<string>(CacheKeyStatus);
                return TranslateUtils.ToBool(cacheValue);
            }
        }

        private static DateTime _lastOnlineDateTime = DateTime.MinValue;

        public static void SetServiceOnline(DateTime now)
        {
            var sp = now - _lastOnlineDateTime;
            if (sp.Minutes <= 5) return;

            _lastOnlineDateTime = now;
            StatusCacheFileWatcher.UpdateCacheFile();
        }

        public static void SetServiceOffline()
        {
            _lastOnlineDateTime = DateTime.MinValue;
            StatusCacheFileWatcher.DeleteCacheFile();
        }
    }
}
