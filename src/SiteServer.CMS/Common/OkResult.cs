namespace SiteServer.CMS.Common
{
    public class OkResult : IControllerResult
    {
        public OkResult(object result)
        {
            StatusCode = 200;
            Result = result;
        }

        public int StatusCode { get; private set; }

        public object Result { get; private set; }
    }
}
