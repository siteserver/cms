using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class WebMenuDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_WebMenu";

        public int Insert(WebMenuInfo menuInfo)
        {
            var menuID = 0;

            menuInfo.Taxis = GetMaxTaxis(menuInfo.ParentID) + 1;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(menuInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        menuID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

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

        public void Update(WebMenuInfo menuInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(menuInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void Delete(int menuID)
        {
            if (menuID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {menuID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int publishmentSystemID)
        {
            string sqlString =
                $"DELETE FROM {TABLE_NAME} WHERE {WebMenuAttribute.PublishmentSystemID} = {publishmentSystemID}";
            ExecuteNonQuery(sqlString);
        }

        public WebMenuInfo GetMenuInfo(int menuID)
        {
            WebMenuInfo menuInfo = null;

            string SQL_WHERE = $"WHERE ID = {menuID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    menuInfo = new WebMenuInfo(rdr);
                }
                rdr.Close();
            }

            return menuInfo;
        }

        public List<WebMenuInfo> GetMenuInfoList(int publishmentSystemID, int parentID)
        {
            var menuInfoList = new List<WebMenuInfo>();

            string SQL_WHERE =
                $"WHERE {WebMenuAttribute.PublishmentSystemID} = {publishmentSystemID} AND {WebMenuAttribute.ParentID} = {parentID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var menuInfo = new WebMenuInfo(rdr);
                    menuInfoList.Add(menuInfo);
                }
                rdr.Close();
            }

            return menuInfoList;
        }

        public IEnumerable GetDataSource(int publishmentSystemID, int parentID)
        {
            string whereString =
                $"WHERE {WebMenuAttribute.PublishmentSystemID} = {publishmentSystemID} AND {WebMenuAttribute.ParentID} = {parentID}";
            var sqlString = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);

            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }

        public int GetCount(int publishmentSystemID, int parentID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {WebMenuAttribute.PublishmentSystemID} = {publishmentSystemID} AND {WebMenuAttribute.ParentID} = {parentID}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public bool UpdateTaxisToUp(int publishmentSystemID, int parentID, int menuID)
        {
            string sqlString =
                $"SELECT TOP 1 MenuID, Taxis FROM wx_WebMenu WHERE (Taxis > (SELECT Taxis FROM wx_WebMenu WHERE MenuID = {menuID} AND ParentID = {parentID} AND PublishmentSystemID = {publishmentSystemID})) AND ParentID = {parentID} AND PublishmentSystemID = {publishmentSystemID} ORDER BY Taxis";
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

        public bool UpdateTaxisToDown(int publishmentSystemID, int parentID, int menuID)
        {
            string sqlString =
                $"SELECT TOP 1 MenuID, Taxis FROM wx_WebMenu WHERE (Taxis < (SELECT Taxis FROM wx_WebMenu WHERE MenuID = {menuID} AND ParentID = {parentID} AND PublishmentSystemID = {publishmentSystemID})) AND ParentID = {parentID} AND PublishmentSystemID = {publishmentSystemID} ORDER BY Taxis DESC";
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
            string sqlString = $"SELECT MAX(Taxis) FROM wx_WebMenu WHERE ParentID = {parentID}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int menuID)
        {
            string sqlString = $"SELECT Taxis FROM wx_WebMenu WHERE MenuID = {menuID}";
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
            string sqlString = $"UPDATE wx_WebMenu SET Taxis = {taxis} WHERE MenuID = {menuID}";
            ExecuteNonQuery(sqlString);
        }

        public void Sync(int publishmentSystemID)
        {
            DeleteAll(publishmentSystemID);

            var menuInfoList = DataProviderWX.MenuDAO.GetMenuInfoList(publishmentSystemID, 0);
            foreach (var menuInfo in menuInfoList)
            {
                var navigationType = ENavigationType.Url;
                if (menuInfo.MenuType == EMenuType.Site)
                {
                    navigationType = ENavigationType.Site;
                }
                else if (menuInfo.MenuType == EMenuType.Keyword)
                {
                    navigationType = ENavigationType.Function;
                }
                var keywordType = EKeywordType.Text;
                var functionID = 0;
                if (menuInfo.MenuType == EMenuType.Keyword && !string.IsNullOrEmpty(menuInfo.Keyword))
                {
                    var keywordID = DataProviderWX.KeywordMatchDAO.GetKeywordIDByMPController(publishmentSystemID, menuInfo.Keyword);
                    if (keywordID > 0)
                    {
                        var keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(keywordID);
                        functionID = KeywordManager.GetFunctionID(keywordInfo);
                    }
                }

                var webMenuInfo = new WebMenuInfo { PublishmentSystemID = publishmentSystemID, MenuName = menuInfo.MenuName, NavigationType = ENavigationTypeUtils.GetValue(navigationType), Url = menuInfo.Url, ChannelID = menuInfo.ChannelID, ContentID = menuInfo.ContentID, KeywordType = EKeywordTypeUtils.GetValue(keywordType), FunctionID = functionID, ParentID = 0 };

                var menuID = Insert(webMenuInfo);

                var subMenuInfoList = DataProviderWX.MenuDAO.GetMenuInfoList(publishmentSystemID, menuInfo.MenuID);
                if (subMenuInfoList != null && subMenuInfoList.Count > 0)
                {
                    foreach (var subMenuInfo in subMenuInfoList)
                    {
                        navigationType = ENavigationType.Url;
                        if (subMenuInfo.MenuType == EMenuType.Site)
                        {
                            navigationType = ENavigationType.Site;
                        }
                        else if (subMenuInfo.MenuType == EMenuType.Keyword)
                        {
                            navigationType = ENavigationType.Function;
                        }
                        keywordType = EKeywordType.Text;
                        functionID = 0;
                        if (subMenuInfo.MenuType == EMenuType.Keyword && !string.IsNullOrEmpty(subMenuInfo.Keyword))
                        {
                            var keywordID = DataProviderWX.KeywordMatchDAO.GetKeywordIDByMPController(publishmentSystemID, subMenuInfo.Keyword);
                            if (keywordID > 0)
                            {
                                var keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(keywordID);
                                functionID = KeywordManager.GetFunctionID(keywordInfo);
                            }
                        }

                        var subWebMenuInfo = new WebMenuInfo { PublishmentSystemID = publishmentSystemID, MenuName = subMenuInfo.MenuName, NavigationType = ENavigationTypeUtils.GetValue(navigationType), Url = subMenuInfo.Url, ChannelID = subMenuInfo.ChannelID, ContentID = subMenuInfo.ContentID, KeywordType = EKeywordTypeUtils.GetValue(keywordType), FunctionID = functionID, ParentID = menuID };

                        Insert(subWebMenuInfo);
                    }
                }
            }
        }

        public List<WebMenuInfo> GetWebMenuInfoList(int publishmentSystemID)
        {
            var menuInfoList = new List<WebMenuInfo>();

            string SQL_WHERE = $"WHERE {WebMenuAttribute.PublishmentSystemID} = {publishmentSystemID} AND ParentID = 0";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var menuInfo = new WebMenuInfo(rdr);
                    menuInfoList.Add(menuInfo);
                }
                rdr.Close();
            }

            return menuInfoList;
        }

    }
}
