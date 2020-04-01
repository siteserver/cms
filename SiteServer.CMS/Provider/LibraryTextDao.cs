using System.Collections.Generic;
using System.Data;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class LibraryTextDao : DataProviderBase
    {
        public override string TableName => "siteserver_LibraryText";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(LibraryTextInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(LibraryTextInfo.Title),
                DataType = DataType.VarChar,
                DataLength = 500
            },
            new TableColumn
            {
                AttributeName = nameof(LibraryTextInfo.GroupId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(LibraryTextInfo.ImageUrl),
                DataType = DataType.VarChar,
                DataLength = 500
            },
            new TableColumn
            {
                AttributeName = nameof(LibraryTextInfo.Summary),
                DataType = DataType.VarChar,
                DataLength = 500
            },
            new TableColumn
            {
                AttributeName = nameof(LibraryTextInfo.Content),
                DataType = DataType.Text
            }
        };

        public int Insert(LibraryTextInfo group)
        {
            var sqlString = $@"INSERT INTO {TableName}
              ({nameof(LibraryTextInfo.Title)}, 
                {nameof(LibraryTextInfo.GroupId)},
               {nameof(LibraryTextInfo.ImageUrl)}, {nameof(LibraryTextInfo.Summary)}, {nameof(LibraryTextInfo.Content)})
        VALUES
              (@{nameof(LibraryTextInfo.Title)}, 
            @{nameof(LibraryTextInfo.GroupId)},
           @{nameof(LibraryTextInfo.ImageUrl)}, @{nameof(LibraryTextInfo.Summary)}, @{nameof(LibraryTextInfo.Content)})";

            IDataParameter[] parameters =
            {
               GetParameter(nameof(LibraryTextInfo.Title), DataType.VarChar, 500, group.Title),
               GetParameter(nameof(LibraryTextInfo.GroupId), DataType.Integer, group.GroupId),
               GetParameter(nameof(LibraryTextInfo.ImageUrl), DataType.VarChar, 500, group.ImageUrl),
               GetParameter(nameof(LibraryTextInfo.Summary), DataType.VarChar, 500, group.Summary),
               GetParameter(nameof(LibraryTextInfo.Content), DataType.Text, group.Content),
           };

            return ExecuteNonQueryAndReturnId(TableName, nameof(LibraryTextInfo.Id), sqlString, parameters);
        }

        public void Update(LibraryTextInfo group)
        {
            var sqlString = $@"UPDATE {TableName} SET
                   {nameof(LibraryTextInfo.Title)} = @{nameof(LibraryTextInfo.Title)},
                    {nameof(LibraryTextInfo.GroupId)} = @{nameof(LibraryTextInfo.GroupId)},
                    {nameof(LibraryTextInfo.ImageUrl)} = @{nameof(LibraryTextInfo.ImageUrl)},
                    {nameof(LibraryTextInfo.Summary)} = @{nameof(LibraryTextInfo.Summary)},
                    {nameof(LibraryTextInfo.Content)} = @{nameof(LibraryTextInfo.Content)}
               WHERE {nameof(LibraryTextInfo.Id)} = @{nameof(LibraryTextInfo.Id)}";

            IDataParameter[] parameters =
            {
                GetParameter(nameof(LibraryTextInfo.Title), DataType.VarChar, 500, group.Title),
                GetParameter(nameof(LibraryTextInfo.GroupId), DataType.Integer, group.GroupId),
                GetParameter(nameof(LibraryTextInfo.ImageUrl), DataType.VarChar, 500, group.ImageUrl),
                GetParameter(nameof(LibraryTextInfo.Summary), DataType.VarChar, 500, group.Summary),
                GetParameter(nameof(LibraryTextInfo.Content), DataType.Text, group.Content),
                   GetParameter(nameof(LibraryTextInfo.Id), DataType.Integer, group.Id)
               };

            ExecuteNonQuery(sqlString, parameters);
        }

        public void Delete(int libraryId)
        {
            if (libraryId <= 0) return;

            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(LibraryTextInfo.Id)} = {libraryId}";
            ExecuteNonQuery(sqlString);
        }

        public int GetCount(int groupId, string keyword)
        {
            var whereString = string.Empty;

            if (groupId > 0)
            {
                whereString = $@"{nameof(LibraryTextInfo.GroupId)} = {groupId}";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!string.IsNullOrEmpty(whereString))
                {
                    whereString += " AND ";
                }
                whereString += $@"{nameof(LibraryTextInfo.Title)} LIKE '%{keyword}%'";
            }

            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = "WHERE " + whereString;
            }

            return DataProvider.DatabaseDao.GetPageTotalCount(TableName, whereString);
        }

        public List<LibraryTextInfo> GetAll(int groupId, string keyword, int page, int perPage)
        {
            var list = new List<LibraryTextInfo>();

            var orderString = "ORDER BY Id Desc";
            var whereString = string.Empty;

            if (groupId > 0)
            {
                whereString = $@"{nameof(LibraryTextInfo.GroupId)} = {groupId}";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!string.IsNullOrEmpty(whereString))
                {
                    whereString += " AND ";
                }
                whereString += $@"{nameof(LibraryTextInfo.Title)} LIKE '%{keyword}%'";
            }

            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = "WHERE " + whereString;
            }

            var sqlString = DataProvider.DatabaseDao.GetPageSqlString(TableName, $"{nameof(LibraryTextInfo.Id)}, {nameof(LibraryTextInfo.Title)}, { nameof(LibraryTextInfo.GroupId)}, { nameof(LibraryTextInfo.ImageUrl)}, {nameof(LibraryTextInfo.Summary)}, {nameof(LibraryTextInfo.Content)}", whereString, orderString, perPage * (page - 1),
                perPage);

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetLibraryTextInfo(rdr));
                }
                rdr.Close();
            }

            return list;
        }

        public LibraryTextInfo Get(int libraryId)
        {
            LibraryTextInfo accessTokenInfo = null;

            var sqlString = $@"SELECT {nameof(LibraryTextInfo.Id)}, 
                       {nameof(LibraryTextInfo.Title)}, 
                        {nameof(LibraryTextInfo.GroupId)},
                       {nameof(LibraryTextInfo.ImageUrl)},
                        {nameof(LibraryTextInfo.Summary)},
                        {nameof(LibraryTextInfo.Content)}
                   FROM {TableName} WHERE {nameof(LibraryTextInfo.Id)} = {libraryId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    accessTokenInfo = GetLibraryTextInfo(rdr);
                }
                rdr.Close();
            }

            return accessTokenInfo;
        }

        public string GetContentByTitle(string name)
        {
            var content = string.Empty;

            var sqlString = $@"SELECT 
                        {nameof(LibraryTextInfo.Content)}
                   FROM {TableName} WHERE {nameof(LibraryTextInfo.Title)} = '{name}'";

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

        public string GetContentById(int id)
        {
            var content = string.Empty;

            var sqlString = $@"SELECT 
                        {nameof(LibraryTextInfo.Content)}
                   FROM {TableName} WHERE {nameof(LibraryTextInfo.Id)} = {id}";

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

        private static LibraryTextInfo GetLibraryTextInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var libraryGroupInfo = new LibraryTextInfo();

            var i = 0;
            libraryGroupInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            libraryGroupInfo.Title = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            libraryGroupInfo.GroupId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            libraryGroupInfo.ImageUrl = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            libraryGroupInfo.Summary = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            libraryGroupInfo.Content = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);

            return libraryGroupInfo;
        }
    }
}
