using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class MenuDao : DataProviderBase
    {
        private const string SqlUpdate = "UPDATE wx_Menu SET PublishmentSystemID = @PublishmentSystemID, MenuName = @MenuName, MenuType = @MenuType, Keyword = @Keyword, Url = @Url, ChannelID = @ChannelID, ContentID = @ContentID, ParentID = @ParentID, Taxis = @Taxis WHERE MenuID = @MenuID";

        private const string SqlDelete = "DELETE FROM wx_Menu WHERE MenuID = @MenuID OR ParentID = @MenuID";

        private const string SqlSelect = "SELECT MenuID, PublishmentSystemID, MenuName, MenuType, Keyword, Url, ChannelID, ContentID, ParentID, Taxis FROM wx_Menu WHERE MenuID = @MenuID";

        private const string SqlSelectAll = "SELECT MenuID, PublishmentSystemID, MenuName, MenuType, Keyword, Url, ChannelID, ContentID, ParentID, Taxis FROM wx_Menu WHERE PublishmentSystemID = @PublishmentSystemID AND ParentID = @ParentID ORDER BY Taxis";

        private const string SqlSelectAllby = "SELECT MenuID, PublishmentSystemID, MenuName, MenuType, Keyword, Url, ChannelID, ContentID, ParentID, Taxis FROM wx_Menu WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis";


        private const string ParmMenuId = "@MenuID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmMenuName = "@MenuName";
        private const string ParmMenuType = "@MenuType";
        private const string ParmKeyword = "@Keyword";
        private const string ParmUrl = "@Url";
        private const string ParmChannelId = "@ChannelID";
        private const string ParmContentId = "@ContentID";
        private const string ParmParentId = "@ParentID";
        private const string ParmTaxis = "@Taxis";

        public int Insert(MenuInfo menuInfo)
        {
            var menuId = 0;

            var sqlString = "INSERT INTO wx_Menu (PublishmentSystemID, MenuName, MenuType, Keyword, Url, ChannelID, ContentID, ParentID, Taxis) VALUES (@PublishmentSystemID, @MenuName, @MenuType, @Keyword, @Url, @ChannelID, @ContentID, @ParentID, @Taxis)";

            var taxis = GetMaxTaxis(menuInfo.ParentId) + 1;
            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, menuInfo.PublishmentSystemId),
                GetParameter(ParmMenuName, DataType.NVarChar, 50, menuInfo.MenuName),
                GetParameter(ParmMenuType, DataType.VarChar, 50, EMenuTypeUtils.GetValue(menuInfo.MenuType)),
                GetParameter(ParmKeyword, DataType.NVarChar, 50, menuInfo.Keyword),
                GetParameter(ParmUrl, DataType.VarChar, 200, menuInfo.Url),
                GetParameter(ParmChannelId, DataType.Integer, menuInfo.ChannelId),
                GetParameter(ParmContentId, DataType.Integer, menuInfo.ContentId),
                GetParameter(ParmParentId, DataType.Integer, menuInfo.ParentId),
                GetParameter(ParmTaxis, DataType.Integer, taxis)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        menuId = ExecuteNonQueryAndReturnId(trans, sqlString, parms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return menuId;
        }

        public void Update(MenuInfo menuInfo)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, menuInfo.PublishmentSystemId),
                GetParameter(ParmMenuName, DataType.NVarChar, 50, menuInfo.MenuName),
                GetParameter(ParmMenuType, DataType.VarChar, 50, EMenuTypeUtils.GetValue(menuInfo.MenuType)),
                GetParameter(ParmKeyword, DataType.NVarChar, 50, menuInfo.Keyword),
                GetParameter(ParmUrl, DataType.VarChar, 200, menuInfo.Url),
                GetParameter(ParmChannelId, DataType.Integer, menuInfo.ChannelId),
                GetParameter(ParmContentId, DataType.Integer, menuInfo.ContentId),
                GetParameter(ParmParentId, DataType.Integer, menuInfo.ParentId),
                GetParameter(ParmTaxis, DataType.Integer, menuInfo.Taxis),
                GetParameter(ParmMenuId, DataType.Integer, menuInfo.MenuId)
			};

            ExecuteNonQuery(SqlUpdate, parms);
        }

        public void Delete(int menuId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmMenuId, DataType.Integer, menuId)
			};

            ExecuteNonQuery(SqlDelete, parms);
        }

        public MenuInfo GetMenuInfo(int menuId)
        {
            MenuInfo menuInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmMenuId, DataType.Integer, menuId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    menuInfo = new MenuInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), EMenuTypeUtils.GetEnumType(rdr.GetValue(3).ToString()), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), rdr.GetInt32(6), rdr.GetInt32(7), rdr.GetInt32(8), rdr.GetInt32(9));
                }
                rdr.Close();
            }

            return menuInfo;
        }

        public IEnumerable GetDataSource(int publishmentSystemId, int parentId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
                GetParameter(ParmParentId, DataType.Integer, parentId)
			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAll, parms);
            return enumerable;
        }

        public int GetCount(int parentId)
        {
            var sqlString = "SELECT COUNT(*) FROM wx_Menu WHERE ParentID = " + parentId;
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<MenuInfo> GetMenuInfoList(int publishmentSystemId, int parentId)
        {
            var list = new List<MenuInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
                GetParameter(ParmParentId, DataType.Integer, parentId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, parms))
            {
                while (rdr.Read())
                {
                    var menuInfo = new MenuInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), EMenuTypeUtils.GetEnumType(rdr.GetValue(3).ToString()), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), rdr.GetInt32(6), rdr.GetInt32(7), rdr.GetInt32(8), rdr.GetInt32(9));
                    list.Add(menuInfo);
                }
                rdr.Close();
            }

            if (parentId > 0)
            {
                list.Reverse();
            }

            return list;
        }

        public bool UpdateTaxisToUp(int parentId, int menuId)
        {
            string sqlString =
                $"SELECT TOP 1 MenuID, Taxis FROM wx_Menu WHERE (Taxis > (SELECT Taxis FROM wx_Menu WHERE MenuID = {menuId} AND ParentID = {parentId})) AND ParentID = {parentId} ORDER BY Taxis";
            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherId = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(menuId);

            if (higherId > 0)
            {
                SetTaxis(menuId, higherTaxis);
                SetTaxis(higherId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int parentId, int menuId)
        {
            string sqlString =
                $"SELECT TOP 1 MenuID, Taxis FROM wx_Menu WHERE (Taxis < (SELECT Taxis FROM wx_Menu WHERE MenuID = {menuId} AND ParentID = {parentId})) AND ParentID = {parentId} ORDER BY Taxis DESC";
            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(menuId);

            if (lowerId > 0)
            {
                SetTaxis(menuId, lowerTaxis);
                SetTaxis(lowerId, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis(int parentId)
        {
            string sqlString = $"SELECT MAX(Taxis) FROM wx_Menu WHERE ParentID = {parentId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int menuId)
        {
            string sqlString = $"SELECT Taxis FROM wx_Menu WHERE MenuID = {menuId}";
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

        private void SetTaxis(int menuId, int taxis)
        {
            string sqlString = $"UPDATE wx_Menu SET Taxis = {taxis} WHERE MenuID = {menuId}";
            ExecuteNonQuery(sqlString);
        }

        public List<MenuInfo> GetMenuInfoList(int publishmentSystemId)
        {
            var list = new List<MenuInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
                GetParameter(ParmParentId, DataType.Integer, 0)
			};

            using (var rdr = ExecuteReader(SqlSelectAllby, parms))
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