using System.Collections.Generic;
using System.Data;
using Datory;
using SiteServer.CMS.Data;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Provider
{
    public class SitePermissionsDao : DataProviderBase
    {
        public override string TableName => "siteserver_SitePermissions";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.RoleName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.ChannelIdCollection),
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.ChannelPermissions),
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.WebsitePermissions),
                DataType = DataType.Text
            }
        };

        private const string SqlSelectAllByRoleName = "SELECT RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions FROM siteserver_SitePermissions WHERE RoleName = @RoleName ORDER BY SiteId DESC";

        private const string SqlSelectAllByRoleNameAndSiteId = "SELECT RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions FROM siteserver_SitePermissions WHERE RoleName = @RoleName AND SiteId = @SiteId";

        private const string SqlInsert = "INSERT INTO siteserver_SitePermissions (RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions) VALUES (@RoleName, @SiteId, @ChannelIdCollection, @ChannelPermissions, @WebsitePermissions)";

        private const string SqlDelete = "DELETE FROM siteserver_SitePermissions WHERE RoleName = @RoleName";

        private const string ParamRoleName = "@RoleName";
        private const string ParamSiteId = "@SiteId";
        private const string ParamChannelIdCollection = "@ChannelIdCollection";
        private const string ParamChannelPermissions = "@ChannelPermissions";
        private const string ParamWebsitePermissions = "@WebsitePermissions";

        public void Insert(SitePermissionsInfo info)
        {
            var insertParams = new IDataParameter[]
			{
				GetParameter(ParamRoleName, DataType.VarChar, 255, info.RoleName),
				GetParameter(ParamSiteId, DataType.Integer, info.SiteId),
				GetParameter(ParamChannelIdCollection, DataType.Text, info.ChannelIdCollection),
				GetParameter(ParamChannelPermissions, DataType.Text, info.ChannelPermissions),
				GetParameter(ParamWebsitePermissions, DataType.Text, info.WebsitePermissions)
			};

            ExecuteNonQuery(SqlInsert, insertParams);
        }


        public void Delete(string roleName)
        {
            var parameters = new IDataParameter[]
			{
				GetParameter(ParamRoleName, DataType.VarChar, 255, roleName)
			};

            ExecuteNonQuery(SqlDelete, parameters);
        }

        public List<SitePermissionsInfo> GetSystemPermissionsInfoList(string roleName)
        {
            var list = new List<SitePermissionsInfo>();

            var parameters = new IDataParameter[]
			{
				GetParameter(ParamRoleName, DataType.VarChar, 255, roleName)
			};

            using (var rdr = ExecuteReader(SqlSelectAllByRoleName, parameters))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new SitePermissionsInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }

            return list;
        }

        public SitePermissionsInfo GetSystemPermissionsInfo(string roleName, int siteId)
        {
            SitePermissionsInfo permissionsInfo = null; 

            var parameters = new IDataParameter[]
            {
                GetParameter(ParamRoleName, DataType.VarChar, 255, roleName),
                GetParameter(ParamSiteId, DataType.Integer, siteId)
            };

            using (var rdr = ExecuteReader(SqlSelectAllByRoleNameAndSiteId, parameters))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    permissionsInfo = new SitePermissionsInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return permissionsInfo;
        }

        public Dictionary<int, List<string>> GetWebsitePermissionSortedList(IEnumerable<string> roles)
        {
            var sortedList = new Dictionary<int, List<string>>();
            if (roles == null) return sortedList;

            foreach (var roleName in roles)
            {
                var systemPermissionsList = GetSystemPermissionsInfoList(roleName);
                foreach (var systemPermissionsInfo in systemPermissionsList)
                {
                    var list = new List<string>();
                    var websitePermissionList = TranslateUtils.StringCollectionToStringList(systemPermissionsInfo.WebsitePermissions);
                    foreach (var websitePermission in websitePermissionList)
                    {
                        if (!list.Contains(websitePermission)) list.Add(websitePermission);
                    }
                    sortedList[systemPermissionsInfo.SiteId] = list;
                }
            }

            return sortedList;
        }

        public Dictionary<string, List<string>> GetChannelPermissionSortedList(IList<string> roles)
        {
            var dict = new Dictionary<string, List<string>>();
            if (roles == null) return dict;

            foreach (var roleName in roles)
            {
                var systemPermissionsInfoList = GetSystemPermissionsInfoList(roleName);
                foreach (var systemPermissionsInfo in systemPermissionsInfoList)
                {
                    var channelIdList = TranslateUtils.StringCollectionToIntList(systemPermissionsInfo.ChannelIdCollection);
                    foreach (var channelId in channelIdList)
                    {
                        var key = PermissionsImpl.GetChannelPermissionDictKey(systemPermissionsInfo.SiteId, channelId);

                        if (!dict.TryGetValue(key, out var list))
                        {
                            list = new List<string>();
                            dict[key] = list;
                        }

                        var channelPermissionList = TranslateUtils.StringCollectionToStringList(systemPermissionsInfo.ChannelPermissions);
                        foreach (var channelPermission in channelPermissionList)
                        {
                            if (!list.Contains(channelPermission)) list.Add(channelPermission);
                        }
                    }
                }
            }

            return dict;
        }

        public List<string> GetChannelPermissionListIgnoreChannelId(IList<string> roles)
        {
            var list = new List<string>();
            if (roles == null) return list;

            foreach (var roleName in roles)
            {
                var systemPermissionsInfoList = GetSystemPermissionsInfoList(roleName);
                foreach (var systemPermissionsInfo in systemPermissionsInfoList)
                {
                    var channelPermissionList = TranslateUtils.StringCollectionToStringList(systemPermissionsInfo.ChannelPermissions);
                    foreach (var channelPermission in channelPermissionList)
                    {
                        if (!list.Contains(channelPermission))
                        {
                            list.Add(channelPermission);
                        }
                    }
                }
            }

            return list;
        }
    }
}
