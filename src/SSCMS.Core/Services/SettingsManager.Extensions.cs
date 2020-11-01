using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SSCMS.Configuration;
using SSCMS.Utils;
using Menu = SSCMS.Configuration.Menu;

namespace SSCMS.Core.Services
{
    public partial class SettingsManager
    {
        private static readonly SiteType DefaultSiteType = new SiteType
        {
            Id = Types.SiteTypes.Web,
            IconClass = "ion-earth",
            Text = "网站"
        };

        public SiteType GetSiteType(string key)
        {
            SiteType siteType = null;
            var section = Configuration.GetSection($"extensions:siteTypes:{key}");
            if (section.Exists())
            {
                siteType = section.Get<SiteType>();
                siteType.Id = key;
            }

            return siteType ?? DefaultSiteType;
        }

        public List<SiteType> GetSiteTypes()
        {
            var siteTypes = new List<SiteType>();
            var section = Configuration.GetSection("extensions:siteTypes");
            if (!section.Exists()) return new List<SiteType>
            {
                DefaultSiteType
            };

            //var children = section.GetChildren();
            //if (children != null)
            //{
            //    foreach (var child in children)
            //    {
            //        var siteType = child.Get<SiteType>();
            //        siteTypes.Add(new SiteType
            //        {
            //            Id = child.Key,
            //            Text = siteType.Text,
            //            Order = siteType.Order
            //        });
            //    }
            //}

            //siteTypes = siteTypes.OrderBy(x => x.Order).ToList();

            var list = section.Get<Dictionary<string, SiteType>>();
            foreach (var (key, value) in list)
            {
                siteTypes.Add(new SiteType
                {
                    Id = key,
                    Text = value.Text,
                    Order = value.Order
                });
            }

            return siteTypes.OrderByDescending(x => x.Order.HasValue).ThenBy(x => x.Order).ToList();
        }

        public List<Permission> GetPermissions()
        {
            var permissions = new List<Permission>();
            var section = Configuration.GetSection("extensions:permissions");
            if (!section.Exists()) return permissions;

            //var children = section.GetChildren();
            //if (children != null)
            //{
            //    foreach (var child in children)
            //    {
            //        var permission = child.Get<Permission>();
            //        permissions.Add(new Permission
            //        {
            //            Id = child.Key,
            //            Text = permission.Text,
            //            Type = permission.Type,
            //            Order = permission.Order
            //        });
            //    }
            //}

            var list = section.Get<Dictionary<string, Permission>>();
            foreach (var (key, value) in list)
            {
                permissions.Add(new Permission
                {
                    Id = key,
                    Text = value.Text,
                    Type = value.Type,
                    Order = value.Order
                });
            }

            return permissions.OrderByDescending(x => x.Order.HasValue).ThenBy(x => x.Order).ToList();
        }

        public List<Menu> GetMenus()
        {
            var section = Configuration.GetSection("extensions:menus");
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
                        var menu = child.Get<Menu>();
                        var childSection = child.GetSection("menus");

                        menus.Add(new Menu
                        {
                            Id = child.Key,
                            Text = menu.Text,
                            Type = menu.Type,
                            IconClass = menu.IconClass,
                            Link = menu.Link,
                            Target = menu.Target,
                            Permissions = menu.Permissions,
                            Order = menu.Order,
                            Children = GetMenus(childSection)
                        });
                    }
                }
            }

            return menus.OrderByDescending(x => x.Order.HasValue).ThenBy(x => x.Order).ToList();
        }

        public List<Table> GetTables()
        {
            var tables = new List<Table>();
            var section = Configuration.GetSection("extensions:tables");
            if (!section.Exists()) return tables;

            var list = section.Get<Dictionary<string, Table>>();
            foreach (var (key, value) in list)
            {
                var table = value;
                table.Id = key;
                tables.Add(table);
            }

            return tables;
        }

        public List<string> GetContentTableNames()
        {
            var tables = GetTables();
            return tables
                .Where(x => StringUtils.EqualsIgnoreCase(x.Type, Types.TableTypes.Content)).Select(x => x.Id)
                .ToList();
        }
    }
}
