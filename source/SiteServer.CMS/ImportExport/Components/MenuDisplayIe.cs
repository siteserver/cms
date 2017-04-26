using System;
using Atom.Core;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class MenuDisplayIe
	{
		private readonly int _publishmentSystemId;
		private readonly string _filePath;

		public MenuDisplayIe(int publishmentSystemId, string filePath)
		{
			_publishmentSystemId = publishmentSystemId;
			_filePath = filePath;
		}

		public void ExportMenuDisplay()
		{
			var feed = AtomUtility.GetEmptyFeed();

			var menuDisplayArrayList = DataProvider.MenuDisplayDao.GetMenuDisplayInfoArrayList(_publishmentSystemId);

			foreach (MenuDisplayInfo menuDisplayInfo in menuDisplayArrayList)
			{
				var entry = ExportMenuDisplayInfo(menuDisplayInfo);
				feed.Entries.Add(entry);
			}

			feed.Save(_filePath);
		}

		private static AtomEntry ExportMenuDisplayInfo(MenuDisplayInfo menuDisplayInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuDisplayID", menuDisplayInfo.MenuDisplayId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "PublishmentSystemID", menuDisplayInfo.PublishmentSystemId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuDisplayName", menuDisplayInfo.MenuDisplayName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "Vertical", menuDisplayInfo.Vertical);
			AtomUtility.AddDcElement(entry.AdditionalElements, "FontFamily", menuDisplayInfo.FontFamily);
			AtomUtility.AddDcElement(entry.AdditionalElements, "FontSize", menuDisplayInfo.FontSize.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "FontWeight", menuDisplayInfo.FontWeight);
			AtomUtility.AddDcElement(entry.AdditionalElements, "FontStyle", menuDisplayInfo.FontStyle);
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuItemHAlign", menuDisplayInfo.MenuItemHAlign);
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuItemVAlign", menuDisplayInfo.MenuItemVAlign);
			AtomUtility.AddDcElement(entry.AdditionalElements, "FontColor", menuDisplayInfo.FontColor);
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuItemBgColor", menuDisplayInfo.MenuItemBgColor);
			AtomUtility.AddDcElement(entry.AdditionalElements, "FontColorHilite", menuDisplayInfo.FontColorHilite);
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuHiliteBgColor", menuDisplayInfo.MenuHiliteBgColor);
			AtomUtility.AddDcElement(entry.AdditionalElements, "XPosition", menuDisplayInfo.XPosition);
			AtomUtility.AddDcElement(entry.AdditionalElements, "YPosition", menuDisplayInfo.YPosition);
			AtomUtility.AddDcElement(entry.AdditionalElements, "HideOnMouseOut", menuDisplayInfo.HideOnMouseOut);
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuWidth", menuDisplayInfo.MenuWidth.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuItemHeight", menuDisplayInfo.MenuItemHeight.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuItemPadding", menuDisplayInfo.MenuItemPadding.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuItemSpacing", menuDisplayInfo.MenuItemSpacing.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuItemIndent", menuDisplayInfo.MenuItemIndent.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "HideTimeout", menuDisplayInfo.HideTimeout.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuBgOpaque", menuDisplayInfo.MenuBgOpaque);
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuBorder", menuDisplayInfo.MenuBorder.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "BGColor", menuDisplayInfo.BgColor);
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuBorderBgColor", menuDisplayInfo.MenuBorderBgColor);
			AtomUtility.AddDcElement(entry.AdditionalElements, "MenuLiteBgColor", menuDisplayInfo.MenuLiteBgColor);
			AtomUtility.AddDcElement(entry.AdditionalElements, "ChildMenuIcon", menuDisplayInfo.ChildMenuIcon);
			AtomUtility.AddDcElement(entry.AdditionalElements, "AddDate", menuDisplayInfo.AddDate.ToLongDateString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "IsDefault", menuDisplayInfo.IsDefault.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "Description", menuDisplayInfo.Description);

			return entry;
		}


		public void ImportMenuDisplay(bool overwrite)
		{
			if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

			foreach (AtomEntry entry in feed.Entries)
			{
				var menuDisplayName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuDisplayName");

				if (!string.IsNullOrEmpty(menuDisplayName))
				{
				    var menuDisplayInfo = new MenuDisplayInfo
				    {
				        PublishmentSystemId = _publishmentSystemId,
				        MenuDisplayName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuDisplayName"),
				        Vertical = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Vertical"),
				        FontFamily = AtomUtility.GetDcElementContent(entry.AdditionalElements, "FontFamily"),
				        FontSize =
				            (AtomUtility.GetDcElementContent(entry.AdditionalElements, "FontSize") != string.Empty)
				                ? int.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, "FontSize"))
				                : 0,
				        FontWeight = AtomUtility.GetDcElementContent(entry.AdditionalElements, "FontWeight"),
				        FontStyle = AtomUtility.GetDcElementContent(entry.AdditionalElements, "FontStyle"),
				        MenuItemHAlign = AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemHAlign"),
				        MenuItemVAlign = AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemVAlign"),
				        FontColor = AtomUtility.GetDcElementContent(entry.AdditionalElements, "FontColor"),
				        MenuItemBgColor = AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemBgColor"),
				        FontColorHilite = AtomUtility.GetDcElementContent(entry.AdditionalElements, "FontColorHilite"),
				        MenuHiliteBgColor = AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuHiliteBgColor"),
				        XPosition = AtomUtility.GetDcElementContent(entry.AdditionalElements, "XPosition"),
				        YPosition = AtomUtility.GetDcElementContent(entry.AdditionalElements, "YPosition"),
				        HideOnMouseOut = AtomUtility.GetDcElementContent(entry.AdditionalElements, "HideOnMouseOut"),
				        MenuWidth =
				            (AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuWidth") != string.Empty)
				                ? int.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuWidth"))
				                : 0,
				        MenuItemHeight =
				            (AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemHeight") != string.Empty)
				                ? int.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemHeight"))
				                : 0,
				        MenuItemPadding =
				            (AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemPadding") != string.Empty)
				                ? int.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemPadding"))
				                : 0,
				        MenuItemSpacing =
				            (AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemSpacing") != string.Empty)
				                ? int.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemSpacing"))
				                : 0,
				        MenuItemIndent =
				            (AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemIndent") != string.Empty)
				                ? int.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuItemIndent"))
				                : 0,
				        HideTimeout =
				            (AtomUtility.GetDcElementContent(entry.AdditionalElements, "HideTimeout") != string.Empty)
				                ? int.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, "HideTimeout"))
				                : 0,
				        MenuBgOpaque = AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuBgOpaque"),
				        MenuBorder =
				            (AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuBorder") != string.Empty)
				                ? int.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuBorder"))
				                : 0,
				        BgColor = AtomUtility.GetDcElementContent(entry.AdditionalElements, "BGColor"),
				        MenuBorderBgColor = AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuBorderBgColor"),
				        MenuLiteBgColor = AtomUtility.GetDcElementContent(entry.AdditionalElements, "MenuLiteBgColor"),
				        ChildMenuIcon = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ChildMenuIcon"),
				        Description = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Description"),
				        AddDate = DateTime.Now
				    };
				    var srcMenuDisplayInfo = DataProvider.MenuDisplayDao.GetMenuDisplayInfoByMenuDisplayName(_publishmentSystemId, menuDisplayInfo.MenuDisplayName);
					if (srcMenuDisplayInfo != null)
					{
						if (overwrite)
						{
							menuDisplayInfo.IsDefault = srcMenuDisplayInfo.IsDefault;
							menuDisplayInfo.MenuDisplayId = srcMenuDisplayInfo.MenuDisplayId;
                            DataProvider.MenuDisplayDao.Update(menuDisplayInfo);
						}
						else
						{
                            menuDisplayInfo.MenuDisplayName = DataProvider.MenuDisplayDao.GetImportMenuDisplayName(_publishmentSystemId, menuDisplayInfo.MenuDisplayName);
							menuDisplayInfo.IsDefault = false;
                            DataProvider.MenuDisplayDao.Insert(menuDisplayInfo);
						}
					}
					else
					{
						menuDisplayInfo.IsDefault = false;
                        DataProvider.MenuDisplayDao.Insert(menuDisplayInfo);
					}
				}
			}
		}

	}
}
