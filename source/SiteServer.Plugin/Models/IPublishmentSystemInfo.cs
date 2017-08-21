namespace SiteServer.Plugin.Models
{
    public interface IPublishmentSystemInfo
    {
        int PublishmentSystemId { get; set; }

        string PublishmentSystemName { get; set; }

        string AuxiliaryTableForContent { get; set; }

        bool IsCheckContentUseLevel { get; set; }

        int CheckContentLevel { get; set; }

        string PublishmentSystemDir { get; set; }

        string PublishmentSystemUrl { get; set; }

        bool IsHeadquarters { get; set; }

        int ParentPublishmentSystemId { get; set; }

        int Taxis { get; set; }

        ExtendedAttributes Attributes { get; }
    }
}
