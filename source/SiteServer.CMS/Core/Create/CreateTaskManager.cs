namespace SiteServer.CMS.Core.Create
{
    public class CreateTaskManager
    {
        private static readonly CreateTaskManagerForDb CreateTaskManagerForDb = new CreateTaskManagerForDb();
        private static readonly CreateTaskManagerForMomery CreateTaskManagerForMomery = new CreateTaskManagerForMomery();

        private CreateTaskManager() { }

        public static ICreateTaskManager Instance => ServiceManager.IsServiceOnline
            ? (ICreateTaskManager) CreateTaskManagerForDb
            : CreateTaskManagerForMomery;
    }
}
