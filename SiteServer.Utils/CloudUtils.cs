namespace SiteServer.Utils
{
    public static class CloudUtils
    {
        public static class Root
        {
            public const string Host = "https://sscms.com";

            public static string IconUrl => $"{Host}/assets/images/favicon.png";

            public static string DocsCliUrl => $"{Host}/docs/cli/";

            public static string GetDocsStlUrl(string tagName)
            {
                return $"{Host}/docs/stl/{tagName}/";
            }

            public static string GetDocsStlUrl(string tagName, string fieldName, string attrTitle)
            {
                return $"{Host}/docs/stl/{tagName}/#{fieldName.ToLower()}-{attrTitle.ToLower()}";
            }
        }

        public static class Dl
        {
            private const string Host = "https://dl.sscms.com";

            public static string GetTemplatesUrl(string fileName)
            {
                return $"{Host}/templates/{fileName}";
            }

            public static string GetPackagesUrl(string packageId, string version)
            {
                return $"{Host}/packages/{packageId}.{version}.nupkg";
            }
        }
    }
}
