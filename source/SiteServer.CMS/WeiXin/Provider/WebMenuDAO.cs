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
    public class WebMenuDao : DataProviderBase
    {
        private const string TableName = "wx_WebMenu";

        public int Insert(WebMenuInfo menuInfo)
        {
            var menuId = 0;

            menuInfo.Taxis = GetMaxTaxis(menuInfo.ParentId) + 1;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(menuInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        menuId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

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

        public void Update(WebMenuInfo menuInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(menuInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void Delete(int menuId)
        {
            if (menuId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {menuId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int publishmentSystemId)
        {
            string sqlString =
                $"DELETE FROM {TableName} WHERE {WebMenuAttribute.PublishmentSystemId} = {publishmentSystemId}";
            ExecuteNonQuery(sqlString);
        }

        public WebMenuInfo GetMenuInfo(int menuId)
        {
            WebMenuInfo menuInfo = null;

            string sqlWhere = $"WHERE ID = {menuId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    menuInfo = new WebMenuInfo(rdr);
                }
                rdr.Close();
            }

            return menuInfo;
        }

        public List<WebMenuInfo> GetMenuInfoList(int publishmentSystemId, int parentId)
        {
            var menuInfoList = new List<WebMenuInfo>();

            string sqlWhere =
                $"WHERE {WebMenuAttribute.PublishmentSystemId} = {publishmentSystemId} AND {WebMenuAttribute.ParentId} = {parentId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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

        public IEnumerable GetDataSource(int publishmentSystemId, int parentId)
        {
            string whereString =
                $"WHERE {WebMenuAttribute.PublishmentSystemId} = {publishmentSystemId} AND {WebMenuAttribute.ParentId} = {parentId}";
            var sqlString = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);

            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }

        public int GetCount(int publishmentSystemId, int parentId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {WebMenuAttribute.PublishmentSystemId} = {publishmentSystemId} AND {WebMenuAttribute.ParentId} = {parentId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public bool UpdateTaxisToUp(int publishmentSystemId, int parentId, int menuId)
        {
            string sqlString =
                $"SELECT TOP 1 MenuID, Taxis FROM wx_WebMenu WHERE (Taxis > (SELECT Taxis FROM wx_WebMenu WHERE MenuID = {menuId} AND ParentID = {parentId} AND PublishmentSystemID = {publishmentSystemId})) AND ParentID = {parentId} AND PublishmentSystemID = {publishmentSystemId} ORDER BY Taxis";
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

        public bool UpdateTaxisToDown(int publishmentSystemId, int parentId, int menuId)
        {
            string sqlString =
                $"SELECT TOP 1 MenuID, Taxis FROM wx_WebMenu WHERE (Taxis < (SELECT Taxis FROM wx_WebMenu WHERE MenuID = {menuId} AND ParentID = {parentId} AND PublishmentSystemID = {publishmentSystemId})) AND ParentID = {parentId} AND PublishmentSystemID = {publishmentSystemId} ORDER BY Taxis DESC";
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
            string sqlString = $"SELECT MAX(Taxis) FROM wx_WebMenu WHERE ParentID = {parentId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int menuId)
        {
            string sqlString = $"SELECT Taxis FROM wx_WebMenu WHERE MenuID = {menuId}";
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
            string sqlString = $"UPDATE wx_WebMenu SET Taxis = {taxis} WHERE MenuID = {menuId}";
            ExecuteNonQuery(sqlString);
        }

        public void Sync(int publishmentSystemId)
        {
            DeleteAll(publishmentSystemId);

            var menuInfoList = DataProviderWx.MenuDao.GetMenuInfoList(publishmentSystemId, 0);
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
                var functionId = 0;
                if (menuInfo.MenuType == EMenuType.Keyword && !string.IsNullOrEmpty(menuInfo.Keyword))
                {
                    var keywordId = DataProviderWx.KeywordMatchDao.GetKeywordIdbyMpController(publishmentSystemId, menuInfo.Keyword);
                    if (keywordId > 0)
                    {
                        var keywordInfo = DataProviderWx.KeywordDao.GetKeywordInfo(keywordId);
                        functionId = KeywordManager.GetFunctionID(keywordInfo);
                    }
                }

                var webMenuInfo = new WebMenuInfo { PublishmentSystemId = publishmentSystemId, MenuName = menuInfo.MenuName, NavigationType = ENavigationTypeUtils.GetValue(navigationType), Url = menuInfo.Url, ChannelId = menuInfo.ChannelId, ContentId = menuInfo.ContentId, KeywordType = EKeywordTypeUtils.GetValue(keywordType), FunctionId = functionId, ParentId = 0 };

                var menuId = Insert(webMenuInfo);

                var subMenuInfoList = DataProviderWx.MenuDao.GetMenuInfoList(publishmentSystemId, menuInfo.MenuId);
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
                        functionId = 0;
                        if (subMenuInfo.MenuType == EMenuType.Keyword && !string.IsNullOrEmpty(subMenuInfo.Keyword))
                        {
                            var keywordId = DataProviderWx.KeywordMatchDao.GetKeywordIdbyMpController(publishmentSystemId, subMenuInfo.Keyword);
                            if (keywordId > 0)
                            {
                                var keywordInfo = DataProviderWx.KeywordDao.GetKeywordInfo(keywordId);
                                functionId = KeywordManager.GetFunctionID(keywordInfo);
                            }
                        }

                        var subWebMenuInfo = new WebMenuInfo { PublishmentSystemId = publishmentSystemId, MenuName = subMenuInfo.MenuName, NavigationType = ENavigationTypeUtils.GetValue(navigationType), Url = subMenuInfo.Url, ChannelId = subMenuInfo.ChannelId, ContentId = subMenuInfo.ContentId, KeywordType = EKeywordTypeUtils.GetValue(keywordType), FunctionId = functionId, ParentId = menuId };

                        Insert(subWebMenuInfo);
                    }
                }
            }
        }

        public List<WebMenuInfo> GetWebMenuInfoList(int publishmentSystemId)
        {
            var menuInfoList = new List<WebMenuInfo>();

            string sqlWhere = $"WHERE {WebMenuAttribute.PublishmentSystemId} = {publishmentSystemId} AND ParentID = 0";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
