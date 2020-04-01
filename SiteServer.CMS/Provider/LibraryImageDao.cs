using System.Collections.Generic;
using System.Data;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class LibraryImageDao : DataProviderBase
    {
        public override string TableName => "siteserver_LibraryImage";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(LibraryImageInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(LibraryImageInfo.Title),
                DataType = DataType.VarChar,
                DataLength = 500
            },
            new TableColumn
            {
                AttributeName = nameof(LibraryImageInfo.GroupId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(LibraryImageInfo.Url),
                DataType = DataType.VarChar,
                DataLength = 500
            }
        };

        public int Insert(LibraryImageInfo group)
        {
            var sqlString = $@"INSERT INTO {TableName}
              ({nameof(LibraryImageInfo.Title)}, 
                {nameof(LibraryImageInfo.GroupId)},
               {nameof(LibraryImageInfo.Url)})
        VALUES
              (@{nameof(LibraryImageInfo.Title)}, 
            @{nameof(LibraryImageInfo.GroupId)},
           @{nameof(LibraryImageInfo.Url)})";

            IDataParameter[] parameters =
            {
               GetParameter(nameof(LibraryImageInfo.Title), DataType.VarChar, 500, group.Title),
               GetParameter(nameof(LibraryImageInfo.GroupId), DataType.Integer, group.GroupId),
               GetParameter(nameof(LibraryImageInfo.Url), DataType.VarChar, 500, group.Url),
           };

            return ExecuteNonQueryAndReturnId(TableName, nameof(LibraryImageInfo.Id), sqlString, parameters);
        }

        public void Update(LibraryImageInfo group)
        {
            var sqlString = $@"UPDATE {TableName} SET
                   {nameof(LibraryImageInfo.Title)} = @{nameof(LibraryImageInfo.Title)},
                    {nameof(LibraryImageInfo.GroupId)} = @{nameof(LibraryImageInfo.GroupId)},
                    {nameof(LibraryImageInfo.Url)} = @{nameof(LibraryImageInfo.Url)}
               WHERE {nameof(LibraryImageInfo.Id)} = @{nameof(LibraryImageInfo.Id)}";

            IDataParameter[] parameters =
            {
                GetParameter(nameof(LibraryImageInfo.Title), DataType.VarChar, 500, group.Title),
                GetParameter(nameof(LibraryImageInfo.GroupId), DataType.Integer, group.GroupId),
                GetParameter(nameof(LibraryImageInfo.Url), DataType.VarChar, 500, group.Url),
                   GetParameter(nameof(LibraryImageInfo.Id), DataType.Integer, group.Id)
               };

            ExecuteNonQuery(sqlString, parameters);
        }

        public void Delete(int libraryId)
        {
            if (libraryId <= 0) return;

            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(LibraryImageInfo.Id)} = {libraryId}";
            ExecuteNonQuery(sqlString);
        }

        public int GetCount(int groupId, string keyword)
        {
            var whereString = string.Empty;

            if (groupId > 0)
            {
                whereString = $@"{nameof(LibraryImageInfo.GroupId)} = {groupId}";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!string.IsNullOrEmpty(whereString))
                {
                    whereString += " AND ";
                }
                whereString += $@"{nameof(LibraryImageInfo.Title)} LIKE '%{keyword}%'";
            }

            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = "WHERE " + whereString;
            }

            return DataProvider.DatabaseDao.GetPageTotalCount(TableName, whereString);
        }

        public List<LibraryImageInfo> GetAll(int groupId, string keyword, int page, int perPage)
        {
            var list = new List<LibraryImageInfo>();

            var orderString = "ORDER BY Id Desc";
            var whereString = string.Empty;

            if (groupId > 0)
            {
                whereString = $@"{nameof(LibraryImageInfo.GroupId)} = {groupId}";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!string.IsNullOrEmpty(whereString))
                {
                    whereString += " AND ";
                }
                whereString += $@"{nameof(LibraryImageInfo.Title)} LIKE '%{keyword}%'";
            }

            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = "WHERE " + whereString;
            }

            var sqlString = DataProvider.DatabaseDao.GetPageSqlString(TableName, $"{nameof(LibraryImageInfo.Id)}, {nameof(LibraryImageInfo.Title)}, { nameof(LibraryImageInfo.GroupId)}, { nameof(LibraryImageInfo.Url)}", whereString, orderString, perPage * (page - 1),
                perPage);

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetLibraryImageInfo(rdr));
                }
                rdr.Close();
            }

            return list;
        }

        public LibraryImageInfo Get(int libraryId)
        {
            LibraryImageInfo accessTokenInfo = null;

            var sqlString = $@"SELECT {nameof(LibraryImageInfo.Id)}, 
                       {nameof(LibraryImageInfo.Title)}, 
                        {nameof(LibraryImageInfo.GroupId)},
                       {nameof(LibraryImageInfo.Url)}
                   FROM {TableName} WHERE {nameof(LibraryImageInfo.Id)} = {libraryId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    accessTokenInfo = GetLibraryImageInfo(rdr);
                }
                rdr.Close();
            }

            return accessTokenInfo;
        }

        public string GetUrlByTitle(string name)
        {
            var content = string.Empty;

            var sqlString = $@"SELECT 
                        {nameof(LibraryImageInfo.Url)}
                   FROM {TableName} WHERE {nameof(LibraryImageInfo.Title)} = '{name}'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    content = rdr.GetString(0);
                }
                rdr.Close();
            }

            return content;
        }

        public string GetUrlById(int id)
        {
            var content = string.Empty;

            var sqlString = $@"SELECT 
                        {nameof(LibraryImageInfo.Url)}
                   FROM {TableName} WHERE {nameof(LibraryImageInfo.Id)} = {id}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    content = rdr.GetString(0);
                }
                rdr.Close();
            }

            return content;
        }

        private static LibraryImageInfo GetLibraryImageInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var libraryGroupInfo = new LibraryImageInfo();

            var i = 0;
            libraryGroupInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            libraryGroupInfo.Title = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            libraryGroupInfo.GroupId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            libraryGroupInfo.Url = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);

            return libraryGroupInfo;
        }
    }
}
