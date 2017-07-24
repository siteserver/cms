namespace SiteServer.Plugin.Hooks
{
    public interface IRestful : IPlugin
    {
        object Get(IRequestContext context);
        object Get(IRequestContext context, string name);
        object Get(IRequestContext context, string name, int id);

        object Post(IRequestContext context);
        object Post(IRequestContext context, string name);
        object Post(IRequestContext context, string name, int id);

        object Put(IRequestContext context);
        object Put(IRequestContext context, string name);
        object Put(IRequestContext context, string name, int id);

        object Delete(IRequestContext context);
        object Delete(IRequestContext context, string name);
        object Delete(IRequestContext context, string name, int id);

        object Patch(IRequestContext context);
        object Patch(IRequestContext context, string name);
        object Patch(IRequestContext context, string name, int id);
    }
}
