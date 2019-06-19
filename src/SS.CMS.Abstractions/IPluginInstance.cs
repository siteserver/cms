namespace SS.CMS
{
    public interface IPluginInstance
    {
        string Id { get; }

        IPackageMetadata Metadata { get; }

        PluginBase Plugin { get; }

        IService Service { get; }

        long InitTime { get; }

        string ErrorMessage { get; }

        bool IsRunnable { get; set; }

        bool IsDisabled { get; set; }

        int Taxis { get; set; }
    }
}
