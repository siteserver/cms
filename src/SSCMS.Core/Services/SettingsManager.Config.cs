using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SSCMS.Core.Services
{
    public partial class SettingsManager
    {
        public SiteType GetSiteType(string key)
        {
            SiteType siteType = null;
            var section = _config.GetSection($"siteTypes:{key}");
            if (section.Exists())
            {
                siteType = section.Get<SiteType>();
                siteType.Id = key;
            }

            return siteType ?? new SiteType
            {
                Id = AuthTypes.Resources.Site,
                Text = "默认"
            };
        }

        public List<SiteType> GetSiteTypes()
        {
            var siteTypes = new List<SiteType>();
            var section = _config.GetSection("siteTypes");
            if (section.Exists())
            {
                var children = section.GetChildren();
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        var siteType = child.Get<SiteType>();
                        siteTypes.Add(new SiteType
                        {
                            Id = child.Key,
                            Text = siteType.Text,
                            Order = siteType.Order
                        });
                    }
                }

                siteTypes = siteTypes.OrderBy(x => x.Order).ToList();

                //var list = section.Get<Dictionary<string, SiteType>>();
                //foreach (var pair in list)
                //{
                //    siteTypes.Add(new SiteType
                //    {
                //        Id = pair.Key,
                //        Text = pair.Value.Text
                //    });
                //}
            }

            return siteTypes;
        }

        public List<Permission> GetPermissions()
        {
            var permissions = new List<Permission>();
            var section = _config.GetSection("permissions");
            if (section.Exists())
            {
                var children = section.GetChildren();
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        var permission = child.Get<Permission>();
                        permissions.Add(new Permission
                        {
                            Id = child.Key,
                            Text = permission.Text,
                            Type = permission.Type,
                            Order = permission.Order
                        });
                    }
                }

                permissions = permissions.OrderBy(x => x.Order).ToList();
            }

            return permissions;
        }

        public List<Menu> GetMenus()
        {
            var section = _config.GetSection("menus");
            return GetMenus(section);
        }

        private List<Menu> GetMenus(IConfigurationSection section)
        {
            var menus = new List<Menu>();
            if (section.Exists())
            {
                var children = section.GetChildren();
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        var childSection = child.GetSection("menus");
                        var menu = child.Get<Menu>();
                        menus.Add(new Menu
                        {
                            Id = child.Key,
                            Text = menu.Text,
                            Type = menu.Type,
                            IconClass = menu.IconClass,
                            Link = menu.Link,
                            Target = menu.Target,
                            Permissions = menu.Permissions,
                            Selected = menu.Selected,
                            Order = menu.Order,
                            Children = GetMenus(childSection)
                        });
                    }
                    menus = menus.OrderBy(x => x.Order).ToList();
                }
            }

            return menus;
        }
    }
}
