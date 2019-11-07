using AutoMapper;
using SiteServer.CMS.Model.Db;
using SiteServer.Utils;

namespace SiteServer.CMS.Model.Mappings
{
    public class AdministratorProfile : Profile
    {
        public AdministratorProfile()
        {
            CreateMap<AdministratorInfo, Administrator>().AfterMap<ToAdministratorAction>();
            CreateMap<Administrator, AdministratorInfo>().AfterMap<ToAdministratorInfoAction>();
        }

        private class ToAdministratorAction : IMappingAction<AdministratorInfo, Administrator>
        {
            public void Process(AdministratorInfo source, Administrator destination, ResolutionContext context)
            {
                destination.Locked = TranslateUtils.ToBool(source.IsLockedOut);
                destination.DisplayName = string.IsNullOrEmpty(destination.DisplayName)
                    ? destination.UserName
                    : destination.DisplayName;
            }
        }

        private class ToAdministratorInfoAction : IMappingAction<Administrator, AdministratorInfo>
        {
            public void Process(Administrator source, AdministratorInfo destination, ResolutionContext context)
            {
                destination.IsLockedOut = source.Locked.ToString();
            }
        }
    }
}
