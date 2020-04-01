using System.Collections.Generic;
using System.Data;
using Datory;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class LibraryGroupDao : DataProviderBase
    {
        public override string TableName => "siteserver_LibraryGroup";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(LibraryGroupInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = Attr.Type,
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(LibraryGroupInfo.GroupName),
                DataType = DataType.VarChar,
                DataLength = 200
            }
        };

        private static class Attr
        {
            public const string Type = nameof(Type);
        }

        public int Insert(LibraryGroupInfo group)
        {
            var sqlString = $@"INSERT INTO {TableName}
           ({Attr.Type}, 
            {nameof(LibraryGroupInfo.GroupName)})
     VALUES
           (@{Attr.Type}, 
            @{nameof(LibraryGroupInfo.GroupName)})";

            IDataParameter[] parameters =
            {
                GetParameter(Attr.Type, DataType.VarChar, 200, LibraryGroupInfo.GetValue(group.LibraryType)),
                GetParameter(nameof(LibraryGroupInfo.GroupName), DataType.VarChar, 200, group.GroupName)
            };

            return ExecuteNonQueryAndReturnId(TableName, nameof(LibraryGroupInfo.Id), sqlString, parameters);
        }

        public void Update(LibraryGroupInfo group)
        {
            var sqlString = $@"UPDATE {TableName} SET
                {nameof(LibraryGroupInfo.GroupName)} = @{nameof(LibraryGroupInfo.GroupName)}
            WHERE {nameof(LibraryGroupInfo.Id)} = @{nameof(LibraryGroupInfo.Id)}";

            IDataParameter[] parameters =
            {
                GetParameter(nameof(LibraryGroupInfo.GroupName), DataType.VarChar, 200, group.GroupName),
                GetParameter(nameof(LibraryGroupInfo.Id), DataType.Integer, group.Id)
            };

            ExecuteNonQuery(sqlString, parameters);
        }

        public void Delete(LibraryType type, int groupId)
        {
            if (groupId <= 0) return;

            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(LibraryGroupInfo.Id)} = {groupId}";
            ExecuteNonQuery(sqlString);
        }

        public List<LibraryGroupInfo> GetAll(LibraryType type)
        {
            //var list = await _repository.GetAllAsync(Q
            //    .Where(Attr.Type, type.GetValue())
            //    .OrderByDesc(nameof(LibraryGroup.Id))
            //    .CachingGet(CacheKey(type))
            //);
            //return list.ToList();

            var list = new List<LibraryGroupInfo>();

            var sqlString = $@"SELECT {nameof(LibraryGroupInfo.Id)}, 
                {Attr.Type}, 
                {nameof(LibraryGroupInfo.GroupName)}
            FROM {TableName} ORDER BY {nameof(LibraryGroupInfo.Id)}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetLibraryGroupInfo(rdr));
                }
                rdr.Close();
            }

            return list;
        }

        public LibraryGroupInfo Get(int id)
        {
            LibraryGroupInfo accessTokenInfo = null;

            var sqlString = $@"SELECT {nameof(LibraryGroupInfo.Id)}, 
                {Attr.Type},
                {nameof(LibraryGroupInfo.GroupName)}
            FROM {TableName} WHERE {nameof(LibraryGroupInfo.Id)} = {id}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    accessTokenInfo = GetLibraryGroupInfo(rdr);
                }
                rdr.Close();
            }

            return accessTokenInfo;
        }

        public bool IsExists(LibraryType type, string groupName)
        {
            var exists = false;

            var sqlString = $@"SELECT {nameof(LibraryGroupInfo.Id)} FROM {TableName} WHERE {Attr.Type} = @{Attr.Type} AND {nameof(LibraryGroupInfo.GroupName)} = @{nameof(LibraryGroupInfo.GroupName)}";

            IDataParameter[] parameters =
            {
                GetParameter(Attr.Type, DataType.VarChar, 200, LibraryGroupInfo.GetValue(type)),
                GetParameter(nameof(LibraryGroupInfo.GroupName), DataType.VarChar, 200, groupName)
            };

            using (var rdr = ExecuteReader(sqlString, parameters))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        private static LibraryGroupInfo GetLibraryGroupInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var libraryGroupInfo = new LibraryGroupInfo();

            var i = 0;
            libraryGroupInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            libraryGroupInfo.LibraryType = LibraryGroupInfo.GetEnumType(rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i));
            i++;
            libraryGroupInfo.GroupName = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);

            return libraryGroupInfo;
        }
    }
}
