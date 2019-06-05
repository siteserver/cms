using System.Collections.Generic;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class PluginDao : IDatabaseDao
    {
        private readonly Repository<PluginInfo> _repository;
        public PluginDao()
        {
            _repository = new Repository<PluginInfo>(AppSettings.DatabaseType, AppSettings.ConnectionString);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string PluginId = nameof(PluginInfo.PluginId);
            public const string IsDisabled = "IsDisabled";
            public const string Taxis = nameof(PluginInfo.Taxis);
        }

        public void DeleteById(string pluginId)
        {
            _repository.Delete(Q.Where(Attr.PluginId, pluginId));
        }

        public void UpdateIsDisabled(string pluginId, bool isDisabled)
        {
            _repository.Update(Q
                .Set(Attr.IsDisabled, isDisabled.ToString())
                .Where(Attr.PluginId, pluginId)
            );
        }

        public void UpdateTaxis(string pluginId, int taxis)
        {
            _repository.Update(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.PluginId, pluginId)
            );
        }

        public void SetIsDisabledAndTaxis(string pluginId, out bool isDisabled, out int taxis)
        {
            isDisabled = false;
            taxis = 0;

            var exists = _repository.Exists(Q
                .Where(Attr.PluginId, pluginId));

            if (!exists)
            {
                _repository.Insert(new PluginInfo
                {
                    PluginId = pluginId,
                    Disabled = false,
                    Taxis = 0
                });
            }

            var result = _repository.Get<(string IsDisabled, int Taxis)?>(Q
                .Select(Attr.IsDisabled, Attr.Taxis)
                .Where(Attr.PluginId, pluginId));

            if (result == null) return;

            isDisabled = TranslateUtils.ToBool(result.Value.IsDisabled);
            taxis = result.Value.Taxis;
        }
    }
}

// using System.Collections.Generic;
// using System.Data;
// using Datory;
// using SiteServer.Utils;
// using SiteServer.CMS.Model;
// using SiteServer.CMS.Plugin;

// namespace SiteServer.CMS.Provider
// {
//     public class PluginDao
//     {
//         public override string TableName => "siteserver_Plugin";

//         public override List<TableColumn> TableColumns => new List<TableColumn>
//         {
//             new TableColumn
//             {
//                 AttributeName = nameof(PluginInfo.Id),
//                 DataType = DataType.Integer,
//                 IsIdentity = true,
//                 IsPrimaryKey = true
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(PluginInfo.PluginId),
//                 DataType = DataType.VarChar,
//                 DataLength = 50
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(PluginInfo.IsDisabled),
//                 DataType = DataType.VarChar,
//                 DataLength = 18
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(PluginInfo.Taxis),
//                 DataType = DataType.Integer
//             }
//         };

//         public void Delete(string pluginId)
//         {
//             const string sqlString = "DELETE FROM siteserver_Plugin WHERE PluginId = @PluginId";

//             var parms = new IDataParameter[]
//             {
//                 GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId)
//             };

//             ExecuteNonQuery(sqlString, parms);
//         }

//         public void UpdateIsDisabled(string pluginId, bool isDisabled)
//         {
//             const string sqlString = "UPDATE siteserver_Plugin SET IsDisabled = @IsDisabled WHERE PluginId = @PluginId";

//             var parms = new IDataParameter[]
//             {
//                 GetParameter(nameof(PluginInstance.IsDisabled), DataType.VarChar, 18, isDisabled.ToString()),
//                 GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId)
//             };

//             ExecuteNonQuery(sqlString, parms);
//         }

//         public void UpdateTaxis(string pluginId, int taxis)
//         {
//             const string sqlString = "UPDATE siteserver_Plugin SET Taxis = @Taxis WHERE PluginId = @PluginId";

//             var parms = new IDataParameter[]
//             {
//                 GetParameter(nameof(PluginInstance.Taxis), DataType.Integer, taxis),
//                 GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId)
//             };

//             ExecuteNonQuery(sqlString, parms);
//         }

//         public void SetIsDisabledAndTaxis(string pluginId, out bool isDisabled, out int taxis)
//         {
//             isDisabled = false;
//             taxis = 0;

//             var exists = false;

//             var sqlString = "SELECT Id FROM siteserver_Plugin WHERE PluginId = @PluginId";

//             var parameters = new IDataParameter[]
//             {
//                 GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId)
//             };

//             using (var rdr = ExecuteReader(sqlString, parameters))
//             {
//                 if (rdr.Read() && !rdr.IsDBNull(0))
//                 {
//                     exists = true;
//                 }
//                 rdr.Close();
//             }

//             if (!exists)
//             {
//                 sqlString = "INSERT INTO siteserver_Plugin(PluginId, IsDisabled, Taxis) VALUES (@PluginId, @IsDisabled, @Taxis)";

//                 parameters = new IDataParameter[]
//                 {
//                     GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId),
//                     GetParameter(nameof(PluginInstance.IsDisabled), DataType.VarChar, 18, false.ToString()),
//                     GetParameter(nameof(PluginInstance.Taxis), DataType.Integer, 0)
//                 };

//                 ExecuteNonQuery(sqlString, parameters);
//             }

//             sqlString = "SELECT IsDisabled, Taxis FROM siteserver_Plugin WHERE PluginId = @PluginId";

//             parameters = new IDataParameter[]
//             {
//                 GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId)
//             };

//             using (var rdr = ExecuteReader(sqlString, parameters))
//             {
//                 if (rdr.Read() && !rdr.IsDBNull(0))
//                 {
//                     isDisabled = TranslateUtils.ToBool(rdr.GetString(0));
//                     taxis = rdr.GetInt32(1);
//                 }
//                 rdr.Close();
//             }
//         }
//     }
// }
