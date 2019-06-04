using SS.CMS.Core.Services.Admin;
using SS.CMS.Core.Services.Admin.Cms;

namespace SS.CMS.Core.Services
{
    public static class ServiceProvider
    {
        private static CreateStatusService _createStatusService;
        public static CreateStatusService CreateStatusService => _createStatusService ?? (_createStatusService = new CreateStatusService());

        private static RootService _rootService;
        public static RootService RootService => _rootService ?? (_rootService = new RootService());

        private static IndexService _indexService;
        public static IndexService IndexService => _indexService ?? (_indexService = new IndexService());

        private static LoginService _loginService;
        public static LoginService LoginService => _loginService ?? (_loginService = new LoginService());

        private static SyncService _syncService;
        public static SyncService SyncService => _syncService ?? (_syncService = new SyncService());
    }
}