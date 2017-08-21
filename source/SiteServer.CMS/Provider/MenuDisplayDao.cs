using System;
using System.Collections;
using System.Data;
using BaiRong.Core.Data;
using SiteServer.CMS.Core.SystemData;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
	public class MenuDisplayDao : DataProviderBase
	{
		private const string SqlSelectMenuDisplay = "SELECT MenuDisplayID, PublishmentSystemID, MenuDisplayName, Vertical, FontFamily, FontSize, FontWeight, FontStyle, MenuItemHAlign, MenuItemVAlign, FontColor, MenuItemBgColor, FontColorHilite, MenuHiliteBgColor, XPosition, YPosition, HideOnMouseOut, MenuWidth, MenuItemHeight, MenuItemPadding, MenuItemSpacing, MenuItemIndent, HideTimeout, MenuBgOpaque, MenuBorder, BGColor, MenuBorderBgColor, MenuLiteBgColor, ChildMenuIcon, AddDate, IsDefault, Description FROM siteserver_MenuDisplay WHERE MenuDisplayID = @MenuDisplayID";

		private const string SqlSelectMenuDisplayByMenuDisplayName = "SELECT MenuDisplayID, PublishmentSystemID, MenuDisplayName, Vertical, FontFamily, FontSize, FontWeight, FontStyle, MenuItemHAlign, MenuItemVAlign, FontColor, MenuItemBgColor, FontColorHilite, MenuHiliteBgColor, XPosition, YPosition, HideOnMouseOut, MenuWidth, MenuItemHeight, MenuItemPadding, MenuItemSpacing, MenuItemIndent, HideTimeout, MenuBgOpaque, MenuBorder, BGColor, MenuBorderBgColor, MenuLiteBgColor, ChildMenuIcon, AddDate, IsDefault, Description FROM siteserver_MenuDisplay WHERE PublishmentSystemID = @PublishmentSystemID AND MenuDisplayName = @MenuDisplayName";

		private const string SqlSelectAllMenuDisplay = "SELECT MenuDisplayID, PublishmentSystemID, MenuDisplayName, Vertical, FontFamily, FontSize, FontWeight, FontStyle, MenuItemHAlign, MenuItemVAlign, FontColor, MenuItemBgColor, FontColorHilite, MenuHiliteBgColor, XPosition, YPosition, HideOnMouseOut, MenuWidth, MenuItemHeight, MenuItemPadding, MenuItemSpacing, MenuItemIndent, HideTimeout, MenuBgOpaque, MenuBorder, BGColor, MenuBorderBgColor, MenuLiteBgColor, ChildMenuIcon, AddDate, IsDefault, Description FROM siteserver_MenuDisplay WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY MenuDisplayID";

		private const string SqlSelectAllMenuDisplayName = "SELECT MenuDisplayName FROM siteserver_MenuDisplay WHERE PublishmentSystemID = @PublishmentSystemID";

		private const string SqlSelectMenuDisplayName = "SELECT MenuDisplayName FROM siteserver_MenuDisplay WHERE MenuDisplayID = @MenuDisplayID";

		private const string SqlSelectMenuDisplayIdByName = "SELECT MenuDisplayID FROM siteserver_MenuDisplay WHERE PublishmentSystemID = @PublishmentSystemID AND MenuDisplayName = @MenuDisplayName";

		private const string SqlUpdateMenuDisplay = "UPDATE siteserver_MenuDisplay SET MenuDisplayName = @MenuDisplayName, Vertical = @Vertical, FontFamily = @FontFamily, FontSize = @FontSize, FontWeight = @FontWeight, FontStyle = @FontStyle, MenuItemHAlign = @MenuItemHAlign, MenuItemVAlign = @MenuItemVAlign, FontColor = @FontColor, MenuItemBgColor = @MenuItemBgColor, FontColorHilite = @FontColorHilite, MenuHiliteBgColor = @MenuHiliteBgColor, XPosition = @XPosition, YPosition = @YPosition, HideOnMouseOut = @HideOnMouseOut, MenuWidth = @MenuWidth, MenuItemHeight = @MenuItemHeight, MenuItemPadding = @MenuItemPadding, MenuItemSpacing = @MenuItemSpacing, MenuItemIndent = @MenuItemIndent, HideTimeout = @HideTimeout, MenuBgOpaque = @MenuBgOpaque, MenuBorder = @MenuBorder, BGColor = @BGColor, MenuBorderBgColor = @MenuBorderBgColor, MenuLiteBgColor = @MenuLiteBgColor, ChildMenuIcon = @ChildMenuIcon, AddDate = @AddDate, IsDefault = @IsDefault, Description = @Description WHERE  MenuDisplayID = @MenuDisplayID";

		private const string SqlDeleteMenuDisplay = "DELETE FROM siteserver_MenuDisplay WHERE  MenuDisplayID = @MenuDisplayID";

		private const string ParmMenuDisplayId = "@MenuDisplayID";
		private const string ParmPublishmentSystemId = "@PublishmentSystemID";
		private const string ParmMenuDisplayName = "@MenuDisplayName";
		private const string ParmVertical = "@Vertical";
		private const string ParmFontFamily = "@FontFamily";
		private const string ParmFontSize = "@FontSize";

		private const string ParmFontWeight = "@FontWeight";
		private const string ParmFontStyle = "@FontStyle";
		private const string ParmMenuItemHAlign = "@MenuItemHAlign";
		private const string ParmMenuItemVAlign = "@MenuItemVAlign";
		private const string ParmFontColor = "@FontColor";
		private const string ParmMenuItemBgColor = "@MenuItemBgColor";
		private const string ParmFontColorHilite = "@FontColorHilite";
		private const string ParmMenuHiliteBgColor = "@MenuHiliteBgColor";
		private const string ParmXPosition = "@XPosition";
		private const string ParmYPosition = "@YPosition";
		private const string ParmHideOnMouseOut = "@HideOnMouseOut";
		private const string ParmMenuWidth = "@MenuWidth";
		private const string ParmMenuItemHeight = "@MenuItemHeight";
		private const string ParmMenuItemPadding = "@MenuItemPadding";
		private const string ParmMenuItemSpacing = "@MenuItemSpacing";
		private const string ParmMenuItemIndent = "@MenuItemIndent";
		private const string ParmHideTimeout = "@HideTimeout";
		private const string ParmMenuBgOpaque = "@MenuBgOpaque";
		private const string ParmMenuBorder = "@MenuBorder";
		private const string ParmBgColor = "@BGColor";
		private const string ParmMenuBorderBgColor = "@MenuBorderBgColor";
		private const string ParmMenuLiteBgColor = "@MenuLiteBgColor";
		private const string ParmChildMenuIcon = "@ChildMenuIcon";
		private const string ParmAddDate = "@AddDate";
		private const string ParmIsDefault = "@IsDefault";
		private const string ParmDescription = "@Description";

		public void Insert(MenuDisplayInfo info) 
		{
			if (info.IsDefault)
			{
				SetAllMenuDisplayDefaultToFalse(info.PublishmentSystemId);
			}

            var sqlString = "INSERT INTO siteserver_MenuDisplay (PublishmentSystemID, MenuDisplayName, Vertical, FontFamily, FontSize, FontWeight, FontStyle, MenuItemHAlign, MenuItemVAlign, FontColor, MenuItemBgColor, FontColorHilite, MenuHiliteBgColor, XPosition, YPosition, HideOnMouseOut, MenuWidth, MenuItemHeight, MenuItemPadding, MenuItemSpacing, MenuItemIndent, HideTimeout, MenuBgOpaque, MenuBorder, BGColor, MenuBorderBgColor, MenuLiteBgColor, ChildMenuIcon, AddDate, IsDefault, Description) VALUES (@PublishmentSystemID, @MenuDisplayName, @Vertical, @FontFamily, @FontSize, @FontWeight, @FontStyle, @MenuItemHAlign, @MenuItemVAlign, @FontColor, @MenuItemBgColor, @FontColorHilite, @MenuHiliteBgColor, @XPosition, @YPosition, @HideOnMouseOut, @MenuWidth, @MenuItemHeight, @MenuItemPadding, @MenuItemSpacing, @MenuItemIndent, @HideTimeout, @MenuBgOpaque, @MenuBorder, @BGColor, @MenuBorderBgColor, @MenuLiteBgColor, @ChildMenuIcon, @AddDate, @IsDefault, @Description)";

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, info.PublishmentSystemId),
				GetParameter(ParmMenuDisplayName, DataType.VarChar, 50, info.MenuDisplayName),
				GetParameter(ParmVertical, DataType.VarChar, 50, info.Vertical),
				GetParameter(ParmFontFamily, DataType.VarChar, 200, info.FontFamily),
				GetParameter(ParmFontSize, DataType.Integer, info.FontSize),
				GetParameter(ParmFontWeight, DataType.VarChar, 50, info.FontWeight),
				GetParameter(ParmFontStyle, DataType.VarChar, 50, info.FontStyle),
				GetParameter(ParmMenuItemHAlign, DataType.VarChar, 50, info.MenuItemHAlign),
				GetParameter(ParmMenuItemVAlign, DataType.VarChar, 50, info.MenuItemVAlign),
				GetParameter(ParmFontColor, DataType.VarChar, 50, info.FontColor),
				GetParameter(ParmMenuItemBgColor, DataType.VarChar, 50, info.MenuItemBgColor),
				GetParameter(ParmFontColorHilite, DataType.VarChar, 50, info.FontColorHilite),
				GetParameter(ParmMenuHiliteBgColor, DataType.VarChar, 200, info.MenuHiliteBgColor),
				GetParameter(ParmXPosition, DataType.VarChar, 50, info.XPosition),
				GetParameter(ParmYPosition, DataType.VarChar, 50, info.YPosition),
				GetParameter(ParmHideOnMouseOut, DataType.VarChar, 50, info.HideOnMouseOut),
				GetParameter(ParmMenuWidth, DataType.Integer, info.MenuWidth),
				GetParameter(ParmMenuItemHeight, DataType.Integer, info.MenuItemHeight),
				GetParameter(ParmMenuItemPadding, DataType.Integer, info.MenuItemPadding),
				GetParameter(ParmMenuItemSpacing, DataType.Integer, info.MenuItemSpacing),
				GetParameter(ParmMenuItemIndent, DataType.Integer, info.MenuItemIndent),
				GetParameter(ParmHideTimeout, DataType.Integer, info.HideTimeout),
				GetParameter(ParmMenuBgOpaque, DataType.VarChar, 50, info.MenuBgOpaque),
				GetParameter(ParmMenuBorder, DataType.Integer, info.MenuBorder),
				GetParameter(ParmBgColor, DataType.VarChar, 50, info.BgColor),
				GetParameter(ParmMenuBorderBgColor, DataType.VarChar, 50, info.MenuBorderBgColor),
				GetParameter(ParmMenuLiteBgColor, DataType.VarChar, 50, info.MenuLiteBgColor),
				GetParameter(ParmChildMenuIcon, DataType.VarChar, 200, info.ChildMenuIcon),
				GetParameter(ParmAddDate, DataType.DateTime, info.AddDate),
				GetParameter(ParmIsDefault, DataType.VarChar, 18, info.IsDefault.ToString()),
				GetParameter(ParmDescription, DataType.NText, info.Description)
			};

            ExecuteNonQuery(sqlString, insertParms);
		}

		public void Update(MenuDisplayInfo info) 
		{
			if (info.IsDefault)
			{
				SetAllMenuDisplayDefaultToFalse(info.PublishmentSystemId);
			}

			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmMenuDisplayName, DataType.VarChar, 50, info.MenuDisplayName),
				GetParameter(ParmVertical, DataType.VarChar, 50, info.Vertical),
				GetParameter(ParmFontFamily, DataType.VarChar, 200, info.FontFamily),
				GetParameter(ParmFontSize, DataType.Integer, info.FontSize),
				GetParameter(ParmFontWeight, DataType.VarChar, 50, info.FontWeight),
				GetParameter(ParmFontStyle, DataType.VarChar, 50, info.FontStyle),
				GetParameter(ParmMenuItemHAlign, DataType.VarChar, 50, info.MenuItemHAlign),
				GetParameter(ParmMenuItemVAlign, DataType.VarChar, 50, info.MenuItemVAlign),
				GetParameter(ParmFontColor, DataType.VarChar, 50, info.FontColor),
				GetParameter(ParmMenuItemBgColor, DataType.VarChar, 50, info.MenuItemBgColor),
				GetParameter(ParmFontColorHilite, DataType.VarChar, 50, info.FontColorHilite),
				GetParameter(ParmMenuHiliteBgColor, DataType.VarChar, 200, info.MenuHiliteBgColor),
				GetParameter(ParmXPosition, DataType.VarChar, 50, info.XPosition),
				GetParameter(ParmYPosition, DataType.VarChar, 50, info.YPosition),
				GetParameter(ParmHideOnMouseOut, DataType.VarChar, 50, info.HideOnMouseOut),
				GetParameter(ParmMenuWidth, DataType.Integer, info.MenuWidth),
				GetParameter(ParmMenuItemHeight, DataType.Integer, info.MenuItemHeight),
				GetParameter(ParmMenuItemPadding, DataType.Integer, info.MenuItemPadding),
				GetParameter(ParmMenuItemSpacing, DataType.Integer, info.MenuItemSpacing),
				GetParameter(ParmMenuItemIndent, DataType.Integer, info.MenuItemIndent),
				GetParameter(ParmHideTimeout, DataType.Integer, info.HideTimeout),
				GetParameter(ParmMenuBgOpaque, DataType.VarChar, 50, info.MenuBgOpaque),
				GetParameter(ParmMenuBorder, DataType.Integer, info.MenuBorder),
				GetParameter(ParmBgColor, DataType.VarChar, 50, info.BgColor),
				GetParameter(ParmMenuBorderBgColor, DataType.VarChar, 50, info.MenuBorderBgColor),
				GetParameter(ParmMenuLiteBgColor, DataType.VarChar, 50, info.MenuLiteBgColor),
				GetParameter(ParmChildMenuIcon, DataType.VarChar, 200, info.ChildMenuIcon),
				GetParameter(ParmAddDate, DataType.DateTime, info.AddDate),
				GetParameter(ParmIsDefault, DataType.VarChar, 18, info.IsDefault.ToString()),
				GetParameter(ParmDescription, DataType.NText, info.Description),
				GetParameter(ParmMenuDisplayId, DataType.Integer, info.MenuDisplayId)
			};

            ExecuteNonQuery(SqlUpdateMenuDisplay, updateParms);
		}

		private void SetAllMenuDisplayDefaultToFalse(int publishmentSystemId)
		{
            var sqlString = "UPDATE siteserver_MenuDisplay SET IsDefault = @IsDefault WHERE PublishmentSystemID = @PublishmentSystemID";

			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsDefault, DataType.VarChar, 18, false.ToString()),
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

            ExecuteNonQuery(sqlString, updateParms);
		}

		public void SetDefault(int menuDisplayId)
		{
			var info = GetMenuDisplayInfo(menuDisplayId);
			SetAllMenuDisplayDefaultToFalse(info.PublishmentSystemId);

            var sqlString = "UPDATE siteserver_MenuDisplay SET IsDefault = @IsDefault WHERE MenuDisplayID = @MenuDisplayID";

			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsDefault, DataType.VarChar, 18, true.ToString()),
				GetParameter(ParmMenuDisplayId, DataType.Integer, menuDisplayId)
			};

            ExecuteNonQuery(sqlString, updateParms);
		}

		public void Delete(int menuDisplayId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmMenuDisplayId, DataType.Integer, menuDisplayId)
			};

            ExecuteNonQuery(SqlDeleteMenuDisplay, parms);
		}

		public MenuDisplayInfo GetMenuDisplayInfo(int menuDisplayId)
		{
			MenuDisplayInfo info = null;
			
			var parms = new IDataParameter[]
			{
				GetParameter(ParmMenuDisplayId, DataType.Integer, menuDisplayId)
			};
			
			using (var rdr = ExecuteReader(SqlSelectMenuDisplay, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new MenuDisplayInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
                }
				rdr.Close();
			}

			return info;
		}

		public string GetMenuDisplayName(int menuDisplayId)
		{
			string menuDisplayName = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmMenuDisplayId, DataType.Integer, menuDisplayId)
			};

			using (var rdr = ExecuteReader(SqlSelectMenuDisplayName, parms)) 
			{
				if (rdr.Read()) 
				{
                    menuDisplayName = GetString(rdr, 0);
				}
				rdr.Close();
			}

			return menuDisplayName;
		}

		public int GetMenuDisplayIdByName(int publishmentSystemId, string menuDisplayName)
		{
			var menuDisplayId = 0;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmMenuDisplayName, DataType.VarChar, 50, menuDisplayName)
			};

			using (var rdr = ExecuteReader(SqlSelectMenuDisplayIdByName, parms)) 
			{
				if (rdr.Read()) 
				{
                    menuDisplayId = GetInt(rdr, 0);
                }
				rdr.Close();
			}

			return menuDisplayId;
		}

		public MenuDisplayInfo GetMenuDisplayInfoByMenuDisplayName(int publishmentSystemId, string menuDisplayName)
		{
			MenuDisplayInfo info = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmMenuDisplayName, DataType.VarChar, 50, menuDisplayName)
			};

			using (var rdr = ExecuteReader(SqlSelectMenuDisplayByMenuDisplayName, parms))
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new MenuDisplayInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
				}
				rdr.Close();
			}

			return info;
		}

		public MenuDisplayInfo GetDefaultMenuDisplayInfo(int publishmentSystemId)
		{
			MenuDisplayInfo info = null;
			
			var sqlString = "SELECT MenuDisplayID, PublishmentSystemID, MenuDisplayName, Vertical, FontFamily, FontSize, FontWeight, FontStyle, MenuItemHAlign, MenuItemVAlign, FontColor, MenuItemBgColor, FontColorHilite, MenuHiliteBgColor, XPosition, YPosition, HideOnMouseOut, MenuWidth, MenuItemHeight, MenuItemPadding, MenuItemSpacing, MenuItemIndent, HideTimeout, MenuBgOpaque, MenuBorder, BGColor, MenuBorderBgColor, MenuLiteBgColor, ChildMenuIcon, AddDate, IsDefault, Description FROM siteserver_MenuDisplay WHERE PublishmentSystemID = @PublishmentSystemID AND IsDefault = @IsDefault";

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmIsDefault, DataType.VarChar, 18, true.ToString())
			};

            using (var rdr = ExecuteReader(sqlString, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new MenuDisplayInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
                }
				rdr.Close();
			}

			return info;
		}

		public string GetImportMenuDisplayName(int publishmentSystemId, string menuDisplayName)
		{
			string importMenuDisplayName;
			if (menuDisplayName.IndexOf("_", StringComparison.Ordinal) != -1)
			{
				var menuDisplayNameCount = 0;
				var lastMenuDisplayName = menuDisplayName.Substring(menuDisplayName.LastIndexOf("_", StringComparison.Ordinal) + 1);
				var firstMenuDisplayName = menuDisplayName.Substring(0, menuDisplayName.Length - lastMenuDisplayName.Length);
				try
				{
					menuDisplayNameCount = int.Parse(lastMenuDisplayName);
				}
			    catch
			    {
			        // ignored
			    }
			    menuDisplayNameCount++;
				importMenuDisplayName = firstMenuDisplayName + menuDisplayNameCount;
			}
			else
			{
				importMenuDisplayName = menuDisplayName + "_1";
			}

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmMenuDisplayName, DataType.VarChar, 50, importMenuDisplayName)
			};

			using (var rdr = ExecuteReader(SqlSelectMenuDisplayByMenuDisplayName, parms))
			{
				if (rdr.Read())
				{
					importMenuDisplayName = GetImportMenuDisplayName(publishmentSystemId, importMenuDisplayName);
				}
				rdr.Close();
			}

			return importMenuDisplayName;
		}

		public IEnumerable GetDataSource(int publishmentSystemId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllMenuDisplay, parms);
			return enumerable;
		}

		public ArrayList GetMenuDisplayInfoArrayList(int publishmentSystemId)
		{
			var arraylist = new ArrayList();

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			using (var rdr = ExecuteReader(SqlSelectAllMenuDisplay, parms))
			{
				while (rdr.Read())
				{
				    var i = 0;
                    var info = new MenuDisplayInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
                    arraylist.Add(info);
				}
				rdr.Close();
			}

			return arraylist;
		}

		public ArrayList GetMenuDisplayNameCollection(int publishmentSystemId)
		{
			var list = new ArrayList();

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			using (var rdr = ExecuteReader(SqlSelectAllMenuDisplayName, parms)) 
			{
				while (rdr.Read()) 
				{					
					list.Add(GetString(rdr, 0));
				}
				rdr.Close();
			}

			return list;
		}

		public void CreateDefaultMenuDisplayInfo(int publishmentSystemId)
		{
			var arraylist = BaseTable.GetDefaultMenuDisplayArrayList(publishmentSystemId);
			foreach (MenuDisplayInfo menuDisplayInfo in arraylist)
			{
				Insert(menuDisplayInfo);
			}
		}
	}
}
