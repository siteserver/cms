using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Settings;
using Xunit;

namespace SS.CMS.Utils.Tests
{
    public class TestYamlUtils
    {
        [Fact]
        public void TestWriteYaml()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var testsDirectoryPath = DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(Path.GetDirectoryName(codeBasePath))));

            var assetsDirectory = PathUtils.Combine(testsDirectoryPath, "assets/menus");
            var outputDirectoryPath = PathUtils.Combine(testsDirectoryPath, "output", "menus");
            DirectoryUtils.DeleteDirectoryIfExists(outputDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(outputDirectoryPath);

            var siteContentMenu = new Menu
            {
                Id = "Content",
                Text = "信息管理",
                IconClass = "ion-compose",
                Selected = true,
                Permissions = new List<string>
                {
                    "cms_content",
                    "cms_contentView",
                    "cms_channelAdd",
                    "cms_channelEdit",
                    "cms_channelDelete",
                    "cms_channelTranslate",
                    "cms_createPage",
                    "cms_contentCheck",
                    "cms_contentModel",
                    "cms_category",
                    "cms_siteAnalysis",
                    "cms_contentTrash",
                },
                Menus = new List<Menu>
                {
                    new Menu
                    {
                        Text = "内容管理",
                        Link = "cms/pageContentMain.aspx",
                        Permissions = new List<string>{
                            "cms_contentView",
                            "cms_contentAdd"
                        }
                    },
                    new Menu
                    {
                        Text = "栏目管理",
                        Link = "cms/pageChannel.aspx",
                        Permissions = new List<string>{
                            "cms_channelAdd",
                            "cms_channelEdit",
                            "cms_channelDelete",
                            "cms_channelTranslate",
                            "cms_createPage",
                        }
                    },
                    new Menu
                    {
                        Text = "内容搜索",
                        Link = "cms/pageContentSearch.aspx",
                        Permissions = new List<string>{
                            "cms_contentView",
                        }
                    },
                    new Menu
                    {
                        Text = "投稿内容",
                        Link = "cms/pageContentSearch.aspx?isWritingOnly=true",
                        Permissions = new List<string>{
                            "cms_contentView",
                        }
                    },
                    new Menu
                    {
                        Text = "我的内容",
                        Link = "cms/pageContentSearch.aspx?isWritingOnly=true",
                        Permissions = new List<string>{
                            "cms_contentView",
                        }
                    },
                }
            };

            var menus = new List<Menu>
            {
                new Menu
                {
                    Id = "site",
                    Text = "站点管理",
                    Menus = new List<Menu>
                    {
                        siteContentMenu,
                        new Menu
                        {
                            Id = "preview",
                            Text = "预览站点",
                        }
                    }
                },
                new Menu
                {
                    Id = "link",
                    Text = "站点链接",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Id = "static",
                            Text = "访问站点",
                        },
                        new Menu
                        {
                            Id = "preview",
                            Text = "预览站点",
                        }
                    }
                },
                new Menu
                {
                    Id = "plugins",
                    Text = "插件管理",
                    Link = "plugins/manage.cshtml",
                    Permissions = new List<string>{
                        "plugins_add",
                        "plugins_management"
                    },
                    Menus = new List<Menu>{
                        new Menu{
                            Text = "添加插件",
                            Link = "plugins/add.cshtml",
                            Permissions = new List<string>{
                                "plugins_add"
                            }
                        },
                        new Menu{
                            Text = "管理插件",
                            Link = "plugins/manage.cshtml",
                            Permissions = new List<string>{
                                "plugins_management"
                            }
                        }
                    }
                },
                new Menu
                {
                    Id = "Settings",
                    Text = "系统管理",
                    Permissions = new List<string>{
                        "permission1",
                        "permission2",
                        "permission3"
                    }
                },
                new Menu
                {
                    Id = "MenuId2",
                    Text = "Text2",
                    Permissions = new List<string>{
                        "permission1",
                        "permission2",
                        "permission3"
                    },
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Id = "ChildId1",
                            Text = "ChildText1",
                        },
                        new Menu
                        {
                            Id = "ChildId2",
                            Text = "ChildText2",
                        }
                    }
                }
            };

            var permissions = new PermissionsSettings
            {
                App = new List<Permission>{
                    new Permission{
                        Id = "plugins_add",
                        Text = "插件管理 -> 添加插件"
                    },
                    new Permission{
                        Id = "plugins_management",
                        Text = "插件管理 -> 管理插件"
                    },
                    new Permission{
                        Id = "settings_siteAdd",
                        Text = "系统管理 -> 创建新站点"
                    },
                    new Permission{
                        Id = "settings_site",
                        Text = "系统管理 -> 站点管理"
                    },
                    new Permission{
                        Id = "settings_admin",
                        Text = "系统管理 -> 管理员管理"
                    },
                    new Permission{
                        Id = "settings_user",
                        Text = "系统管理 -> 用户管理"
                    },
                    new Permission{
                        Id = "settings_chart",
                        Text = "系统管理 -> 统计图表"
                    },
                    new Permission{
                        Id = "settings_log",
                        Text = "系统管理 -> 运行日志"
                    },
                    new Permission{
                        Id = "settings_utility",
                        Text = "系统管理 -> 实用工具"
                    },
                },
                Site = new List<Permission>{
                    new Permission{
                        Id = "cms_content",
                        Text = "信息管理"
                    },
                    new Permission{
                        Id = "cms_template",
                        Text = "显示管理"
                    },
                    new Permission{
                        Id = "cms_configuration",
                        Text = "设置管理"
                    },
                    new Permission{
                        Id = "cms_create",
                        Text = "生成管理"
                    },
                },
                Channel = new List<Permission>{
                    new Permission{
                        Id = "cms_contentView",
                        Text = "浏览内容"
                    },
                    new Permission{
                        Id = "cms_contentAdd",
                        Text = "添加内容"
                    },
                    new Permission{
                        Id = "cms_contentEdit",
                        Text = "修改内容"
                    },
                    new Permission{
                        Id = "cms_contentDelete",
                        Text = "删除内容"
                    },
                    new Permission{
                        Id = "cms_contentTranslate",
                        Text = "转移内容"
                    },
                    new Permission{
                        Id = "cms_contentOrder",
                        Text = "整理内容"
                    },
                    new Permission{
                        Id = "cms_channelAdd",
                        Text = "添加栏目"
                    },
                    new Permission{
                        Id = "cms_channelEdit",
                        Text = "修改栏目"
                    },
                    new Permission{
                        Id = "cms_channelDelete",
                        Text = "删除栏目"
                    },
                    new Permission{
                        Id = "cms_channelTranslate",
                        Text = "转移栏目"
                    },
                    new Permission{
                        Id = "cms_createPage",
                        Text = "生成页面"
                    },
                    new Permission{
                        Id = "cms_contentCheck",
                        Text = "审核内容"
                    },
                    new Permission{
                        Id = "cms_contentCheckLevel1",
                        Text = "一级内容审核权"
                    },
                    new Permission{
                        Id = "cms_contentCheckLevel2",
                        Text = "二级内容审核权"
                    },
                    new Permission{
                        Id = "cms_contentCheckLevel3",
                        Text = "三级内容审核权"
                    },
                    new Permission{
                        Id = "cms_contentCheckLevel4",
                        Text = "四级内容审核权"
                    },
                    new Permission{
                        Id = "cms_contentCheckLevel5",
                        Text = "五级内容审核权"
                    },
                },
            };

            YamlUtils.ObjectToFile(permissions, PathUtils.Combine(outputDirectoryPath, "permissions.yml"));

            YamlUtils.ObjectToFile<List<Menu>>(menus, PathUtils.Combine(outputDirectoryPath, "menus.yml"));

            var nav = new NavSettings
            {
                Permissions = permissions,
                Menus = menus
            };

            YamlUtils.ObjectToFile<NavSettings>(nav, PathUtils.Combine(outputDirectoryPath, "nav.yml"));

            var navRead = YamlUtils.FileToObject<NavSettings>(PathUtils.Combine(outputDirectoryPath, "nav.yml"));

            Assert.True(true);
            Assert.True(nav.Permissions.App.Count == navRead.Permissions.App.Count);
        }

        [Fact]
        public void TestReadYaml()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var testsDirectoryPath = DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(Path.GetDirectoryName(codeBasePath))));

            var yamlFilePath = PathUtils.Combine(testsDirectoryPath, "assets/nav.yml");

            var nav = YamlUtils.FileToObject<NavSettings>(yamlFilePath);

            Assert.NotNull(nav);
            Assert.NotNull(nav.Permissions);
            Assert.NotNull(nav.Permissions.App);
            Assert.NotNull(nav.Menus);
        }
    }
}
