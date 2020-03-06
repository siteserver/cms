using System.Threading.Tasks;
using Newtonsoft.Json;
using SS.CMS.Abstractions;

namespace SS.CMS.Core.Plugins
{
    public class PluginInstanceImpl : IPluginInstance
    {
        public PluginInstanceImpl()
        {

        }

        public PluginInstanceImpl(string directoryName, IPackageMetadata metadata, string errorMessage)
        {
            if (metadata != null)
            {
                Id = metadata.Id;
                Metadata = metadata;
            }
            else
            {
                Id = directoryName;
            }
            IsRunnable = false;
            ErrorMessage = errorMessage;
        }

        public string Id { get; set; }

        public IPackageMetadata Metadata { get; set; }

        [JsonIgnore]
        public PluginBase Plugin { get; set; }

        [JsonIgnore]
        public IPluginService PluginService { get; set; }

        public long InitTime { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsRunnable { get; set; }

        public bool IsDisabled { get; set; }

        public int Taxis { get; set; }
    }
}
