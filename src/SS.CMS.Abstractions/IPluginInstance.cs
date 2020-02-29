namespace SS.CMS.Abstractions
{
    public interface IPluginInstance
    {
        string Id { get;  }

        IPackageMetadata Metadata { get;  }

        PluginBase Plugin { get;  }

        IPluginService PluginService { get;  }

        long InitTime { get;  }

        string ErrorMessage { get;  }

        bool IsRunnable { get;  }

        bool IsDisabled { get; set; }

        int Taxis { get; set; }
    }
}
