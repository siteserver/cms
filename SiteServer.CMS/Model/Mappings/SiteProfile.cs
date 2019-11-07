using AutoMapper;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Db;
using SiteServer.Utils;

namespace SiteServer.CMS.Model.Mappings
{
    public class SiteProfile : Profile
    {
        public SiteProfile()
        {
            CreateMap<SiteInfo, Site>().AfterMap<ToSiteAction>();
            CreateMap<Site, SiteInfo>().AfterMap<ToSiteInfoAction>();
        }

        private class ToSiteAction : IMappingAction<SiteInfo, Site>
        {
            public void Process(SiteInfo source, Site destination, ResolutionContext context)
            {
                destination.Root = TranslateUtils.ToBool(source.IsRoot);
                destination.Additional = new SiteInfoExtend(source.SiteDir, source.SettingsXml);
            }
        }

        private class ToSiteInfoAction : IMappingAction<Site, SiteInfo>
        {
            public void Process(Site source, SiteInfo destination, ResolutionContext context)
            {
                destination.IsRoot = source.Root.ToString();
                destination.SettingsXml = source.Additional.ToString();
            }
        }
    }
}
