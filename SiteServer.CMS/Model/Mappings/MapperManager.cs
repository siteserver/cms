using AutoMapper;

namespace SiteServer.CMS.Model.Mappings
{
    public static class MapperManager
    {
        static MapperManager()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<AdministratorProfile>();
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<SiteProfile>();
            });

            Mapper = config.CreateMapper();
        }

        private static IMapper Mapper { get;  }

        /// <summary>
        ///  类型映射
        /// </summary>
        public static T MapTo<T>(object obj)
        {
            return obj == null ? default : Mapper.Map<T>(obj);
        }
    }
}
