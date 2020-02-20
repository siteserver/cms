using System.Web;

namespace SiteServer.API.Context.UEditor
{
    /// <summary>
    /// Config 的摘要说明
    /// </summary>
    public class ConfigHandler : Handler
    {
        public ConfigHandler(HttpContext context) : base(context) { }

        public override void Process()
        {
            WriteJson(Config.Items);
        }
    }
}