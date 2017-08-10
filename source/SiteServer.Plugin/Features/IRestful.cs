namespace SiteServer.Plugin.Features
{
    public interface IRestful : IPlugin
    {
        object RestfulGet(IRequestContext context);
        object RestfulGet(IRequestContext context, string name);
        object RestfulGet(IRequestContext context, string name, int id);

        object RestfulPost(IRequestContext context);
        object RestfulPost(IRequestContext context, string name);
        object RestfulPost(IRequestContext context, string name, int id);

        object RestfulPut(IRequestContext context);
        object RestfulPut(IRequestContext context, string name);
        object RestfulPut(IRequestContext context, string name, int id);

        object RestfulDelete(IRequestContext context);
        object RestfulDelete(IRequestContext context, string name);
        object RestfulDelete(IRequestContext context, string name, int id);

        object RestfulPatch(IRequestContext context);
        object RestfulPatch(IRequestContext context, string name);
        object RestfulPatch(IRequestContext context, string name, int id);
    }
}
