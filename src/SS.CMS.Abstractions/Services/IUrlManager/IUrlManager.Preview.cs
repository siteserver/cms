namespace SS.CMS.Services.IUrlManager
{
    public partial interface IUrlManager
    {
        string PreviewRoute { get; }
        string PreviewRouteChannel { get; }
        string PreviewRouteContent { get; }
        string PreviewRouteFile { get; }
        string PreviewRouteSpecial { get; }

        string GetPreviewSiteUrl(int siteId);

        string GetPreviewChannelUrl(int siteId, int channelId);

        string GetPreviewContentUrl(int siteId, int channelId, int contentId);

        string GetPreviewContentPreviewUrl(int siteId, int channelId, int contentId, int previewId);

        string GetPreviewFileUrl(int siteId, int fileTemplateId);

        string GetPreviewSpecialUrl(int siteId, int specialId);

        string GetPreviewUrl(int siteId, int channelId, int contentId, int fileTemplateId, int specialId);
    }
}