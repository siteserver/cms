namespace SiteServer.Plugin.Hooks
{
    public interface IRestful : IHooks
    {
        object Get(IRequestContext context, string name, int id);
        object Post(IRequestContext context, string name, int id);
        object Put(IRequestContext context, string name, int id);
        object Delete(IRequestContext context, string name, int id);
        object Patch(IRequestContext context, string name, int id);
    }
}
