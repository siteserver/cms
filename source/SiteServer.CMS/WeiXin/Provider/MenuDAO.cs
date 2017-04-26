using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class MenuDAO : DataProviderBase
    {
        private const string SQL_UPDATE = "UPDATE wx_Menu SET PublishmentSystemID = @PublishmentSystemID, MenuName = @MenuName, MenuType = @MenuType, Keyword = @Keyword, Url = @Url, ChannelID = @ChannelID, ContentID = @ContentID, ParentID = @ParentID, Taxis = @Taxis WHERE MenuID = @MenuID";

        private const string SQL_DELETE = "DELETE FROM wx_Menu WHERE MenuID = @MenuID OR ParentID = @MenuID";

        private const string SQL_SELECT = "SELECT MenuID, PublishmentSystemID, MenuName, MenuType, Keyword, Url, ChannelID, ContentID, ParentID, Taxis FROM wx_Menu WHERE MenuID = @MenuID";

        private const string SQL_SELECT_ALL = "SELECT MenuID, PublishmentSystemID, MenuName, MenuType, Keyword, Url, ChannelID, ContentID, ParentID, Taxis FROM wx_Menu WHERE PublishmentSystemID = @PublishmentSystemID AND ParentID = @ParentID ORDER BY Taxis";

        private const string SQL_SELECT_ALLBY = "SELECT MenuID, PublishmentSystemID, MenuName, MenuType, Keyword, Url, ChannelID, ContentID, ParentID, Taxis FROM wx_Menu WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis";


        private const string PARM_MENU_ID = "@MenuID";
        private const string PARM_PUBLISHMENT_SYSTEM_ID = "@PublishmentSystemID";
        private const string PARM_MENU_NAME = "@MenuName";
        private const string PARM_MENU_TYPE = "@MenuType";
        private const string PARM_KEYWORD = "@Keyword";
        private const string PARM_URL = "@Url";
        private const string PARM_CHANNEL_ID = "@ChannelID";
        private const string PARM_CONTENT_ID = "@ContentID";
        private const string PARM_PARENT_ID = "@ParentID";
        private const string PARM_TAXIS = "@Taxis";

        public int Insert(MenuInfo menuInfo)
        {
            var menuID = 0;

            var sqlString = "INSERT INTO wx_Menu (PublishmentSystemID, MenuName, MenuType, Keyword, Url, ChannelID, ContentID, ParentID, Taxis) VALUES (@PublishmentSystemID, @MenuName, @MenuType, @Keyword, @Url, @ChannelID, @ContentID, @ParentID, @Taxis)";

            var taxis = GetMaxTaxis(menuInfo.ParentID) + 1;
            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, menuInfo.PublishmentSystemID),
                GetParameter(PARM_MENU_NAME, EDataType.NVarChar, 50, menuInfo.MenuName),
                GetParameter(PARM_MENU_TYPE, EDataType.VarChar, 50, EMenuTypeUtils.GetValue(menuInfo.MenuType)),
                GetParameter(PARM_KEYWORD, EDataType.NVarChar, 50, menuInfo.Keyword),
                GetParameter(PARM_URL, EDataType.VarChar, 200, menuInfo.Url),
                GetParameter(PARM_CHANNEL_ID, EDataType.Integer, menuInfo.ChannelID),
                GetParameter(PARM_CONTENT_ID, EDataType.Integer, menuInfo.ContentID),
                GetParameter(PARM_PARENT_ID, EDataType.Integer, menuInfo.ParentID),
                GetParameter(PARM_TAXIS, EDataType.Integer, taxis)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, sqlString, parms);
                        menuID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, "wx_Menu");
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return menuID;
        }

        public void Update(MenuInfo menuInfo)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, menuInfo.PublishmentSystemID),
                GetParameter(PARM_MENU_NAME, EDataType.NVarChar, 50, menuInfo.MenuName),
                GetParameter(PARM_MENU_TYPE, EDataType.VarChar, 50, EMenuTypeUtils.GetValue(menuInfo.MenuType)),
                GetParameter(PARM_KEYWORD, EDataType.NVarChar, 50, menuInfo.Keyword),
                GetParameter(PARM_URL, EDataType.VarChar, 200, menuInfo.Url),
                GetParameter(PARM_CHANNEL_ID, EDataType.Integer, menuInfo.ChannelID),
                GetParameter(PARM_CONTENT_ID, EDataType.Integer, menuInfo.ContentID),
                GetParameter(PARM_PARENT_ID, EDataType.Integer, menuInfo.ParentID),
                GetParameter(PARM_TAXIS, EDataType.Integer, menuInfo.Taxis),
                GetParameter(PARM_MENU_ID, EDataType.Integer, menuInfo.MenuID)
			};

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void Delete(int menuID)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(PARM_MENU_ID, EDataType.Integer, menuID)
			};

            ExecuteNonQuery(SQL_DELETE, parms);
        }

        public MenuInfo GetMenuInfo(int menuID)
        {
            MenuInfo menuInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_MENU_ID, EDataType.Integer, menuID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT, parms))
            {
                if (rdr.Read())
                {
                    menuInfo = new MenuInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), EMenuTypeUtils.GetEnumType(rdr.GetValue(3).ToString()), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), rdr.GetInt32(6), rdr.GetInt32(7), rdr.GetInt32(8), rdr.GetInt32(9));
                }
                rdr.Close();
            }

            return menuInfo;
        }

        public IEnumerable GetDataSource(int publishmentSystemID, int parentID)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
                GetParameter(PARM_PARENT_ID, EDataType.Integer, parentID)
			};

            var enumerable = (IEnumerable)ExecuteReader(SQL_SELECT_ALL, parms);
            return enumerable;
        }

        public int GetCount(int parentID)
        {
            var sqlString = "SELECT COUNT(*) FROM wx_Menu WHERE ParentID = " + parentID;
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<MenuInfo> GetMenuInfoList(int publishmentSystemID, int parentID)
        {
            var list = new List<MenuInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
                GetParameter(PARM_PARENT_ID, EDataType.Integer, parentID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT_ALL, parms))
            {
                while (rdr.Read())
                {
                    var menuInfo = new MenuInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), EMenuTypeUtils.GetEnumType(rdr.GetValue(3).ToString()), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), rdr.GetInt32(6), rdr.GetInt32(7), rdr.GetInt32(8), rdr.GetInt32(9));
                    list.Add(menuInfo);
                }
                rdr.Close();
            }

            if (parentID > 0)
            {
                list.Reverse();
            }

            return list;
        }

        public bool UpdateTaxisToUp(int parentID, int menuID)
        {
            string sqlString =
                $"SELECT TOP 1 MenuID, Taxis FROM wx_Menu WHERE (Taxis > (SELECT Taxis FROM wx_Menu WHERE MenuID = {menuID} AND ParentID = {parentID})) AND ParentID = {parentID} ORDER BY Taxis";
            var higherID = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherID = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(menuID);

            if (higherID > 0)
            {
                SetTaxis(menuID, higherTaxis);
                SetTaxis(higherID, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int parentID, int menuID)
        {
            string sqlString =
                $"SELECT TOP 1 MenuID, Taxis FROM wx_Menu WHERE (Taxis < (SELECT Taxis FROM wx_Menu WHERE MenuID = {menuID} AND ParentID = {parentID})) AND ParentID = {parentID} ORDER BY Taxis DESC";
            var lowerID = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerID = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(menuID);

            if (lowerID > 0)
            {
                SetTaxis(menuID, lowerTaxis);
                SetTaxis(lowerID, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis(int parentID)
        {
            string sqlString = $"SELECT MAX(Taxis) FROM wx_Menu WHERE ParentID = {parentID}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int menuID)
        {
            string sqlString = $"SELECT Taxis FROM wx_Menu WHERE MenuID = {menuID}";
            var taxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    taxis = Convert.ToInt32(rdr[0]);
                }
                rdr.Close();
            }

            return taxis;
        }

        private void SetTaxis(int menuID, int taxis)
        {
            string sqlString = $"UPDATE wx_Menu SET Taxis = {taxis} WHERE MenuID = {menuID}";
            ExecuteNonQuery(sqlString);
        }

        public List<MenuInfo> GetMenuInfoList(int publishmentSystemID)
        {
            var list = new List<MenuInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
                GetParameter(PARM_PARENT_ID, EDataType.Integer, 0)
			};

            using (var rdr = ExecuteReader(SQL_SELECT_ALLBY, parms))
            {
                while (rdr.Read())
                {
                    var menuInfo = new MenuInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), EMenuTypeUtils.GetEnumType(rdr.GetValue(3).ToString()), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), rdr.GetInt32(6), rdr.GetInt32(7), rdr.GetInt32(8), rdr.GetInt32(9));
                    list.Add(menuInfo);
                }
                rdr.Close();
            }

            return list;
        }
    }
}