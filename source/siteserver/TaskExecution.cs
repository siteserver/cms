using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace siteserver
{
    public class TaskExecution
    {
        public bool Execute(TaskInfo taskInfo)
        {
            if (taskInfo.ServiceType == EServiceType.Backup)
            {
                return TaskBackup.Execute(taskInfo);
            }
            if (taskInfo.ServiceType == EServiceType.Create)
            {
                return TaskCreate.Execute(taskInfo);
            }
            if (taskInfo.ServiceType == EServiceType.Gather)
            {
                return TaskGather.Execute(taskInfo);
            }

            return true;
        }
    }
}
