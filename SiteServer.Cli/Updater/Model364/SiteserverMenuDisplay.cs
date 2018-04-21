using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverMenuDisplay
    {
        [JsonProperty("menuDisplayID")]
        public long MenuDisplayId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("menuDisplayName")]
        public string MenuDisplayName { get; set; }

        [JsonProperty("vertical")]
        public string Vertical { get; set; }

        [JsonProperty("fontFamily")]
        public string FontFamily { get; set; }

        [JsonProperty("fontSize")]
        public long FontSize { get; set; }

        [JsonProperty("fontWeight")]
        public string FontWeight { get; set; }

        [JsonProperty("fontStyle")]
        public string FontStyle { get; set; }

        [JsonProperty("menuItemHAlign")]
        public string MenuItemHAlign { get; set; }

        [JsonProperty("menuItemVAlign")]
        public string MenuItemVAlign { get; set; }

        [JsonProperty("fontColor")]
        public string FontColor { get; set; }

        [JsonProperty("menuItemBgColor")]
        public string MenuItemBgColor { get; set; }

        [JsonProperty("fontColorHilite")]
        public string FontColorHilite { get; set; }

        [JsonProperty("menuHiliteBgColor")]
        public string MenuHiliteBgColor { get; set; }

        [JsonProperty("xPosition")]
        public string XPosition { get; set; }

        [JsonProperty("yPosition")]
        public string YPosition { get; set; }

        [JsonProperty("hideOnMouseOut")]
        public string HideOnMouseOut { get; set; }

        [JsonProperty("menuWidth")]
        public long MenuWidth { get; set; }

        [JsonProperty("menuItemHeight")]
        public long MenuItemHeight { get; set; }

        [JsonProperty("menuItemPadding")]
        public long MenuItemPadding { get; set; }

        [JsonProperty("menuItemSpacing")]
        public long MenuItemSpacing { get; set; }

        [JsonProperty("menuItemIndent")]
        public long MenuItemIndent { get; set; }

        [JsonProperty("hideTimeout")]
        public long HideTimeout { get; set; }

        [JsonProperty("menuBgOpaque")]
        public string MenuBgOpaque { get; set; }

        [JsonProperty("menuBorder")]
        public long MenuBorder { get; set; }

        [JsonProperty("bgColor")]
        public string BgColor { get; set; }

        [JsonProperty("menuBorderBgColor")]
        public string MenuBorderBgColor { get; set; }

        [JsonProperty("menuLiteBgColor")]
        public string MenuLiteBgColor { get; set; }

        [JsonProperty("childMenuIcon")]
        public string ChildMenuIcon { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("isDefault")]
        public string IsDefault { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class SiteserverMenuDisplay
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
