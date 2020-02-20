using System.Web;
using Newtonsoft.Json;

namespace SiteServer.API.Context.UEditor
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public abstract class Handler
    {
        public Handler(HttpContext context)
        {
            Request = context.Request;
            Response = context.Response;
            Context = context;
            Server = context.Server;
        }

        public abstract void Process();

        protected void WriteJson(object response)
        {
            string jsonpCallback = Request["callback"],
                json = JsonConvert.SerializeObject(response);
            if (string.IsNullOrWhiteSpace(jsonpCallback))
            {
                Response.AddHeader("Body-InputType", "text/plain");
                Response.Write(json);
            }
            else
            {
                Response.AddHeader("Body-InputType", "application/javascript");
                Response.Write($"{jsonpCallback}({json});");
            }
            Response.End();
        }

        public HttpRequest Request { get; private set; }
        public HttpResponse Response { get; private set; }
        public HttpContext Context { get; private set; }
        public HttpServerUtility Server { get; private set; }
    }
}