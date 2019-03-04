using System.Collections.Generic;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class PluginRepository : GenericRepository<PluginInfo>
    {
        private static class Attr
        {
            public const string PluginId = nameof(PluginInfo.PluginId);
            public const string IsDisabled = "IsDisabled";
            public const string Taxis = nameof(PluginInfo.Taxis);
        }

        //public void DeleteById(string pluginId)
        //{
        //    //const string sqlString = "DELETE FROM siteserver_Plugin WHERE PluginId = @PluginId";

        //    //IDataParameter[] parameters =
        //    //{
        //    //    GetParameter(nameof(PluginConfigInfo.PluginId), pluginId)
        //    //};

        //    //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

        //    DeleteAll(new Query().Equal(Attr.PluginId, pluginId));
        //}

        public void UpdateIsDisabled(string pluginId, bool isDisabled)
        {
            //const string sqlString = "UPDATE siteserver_Plugin SET IsDisabled = @IsDisabled WHERE PluginId = @PluginId";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(nameof(PluginInstance.IsDisabled), isDisabled.ToString()),
            //    GetParameter(nameof(PluginConfigInfo.PluginId), pluginId)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            
            UpdateAll(Q
                .Set(Attr.IsDisabled, isDisabled.ToString())
                .Where(Attr.PluginId, pluginId)
            );
        }

        public void UpdateTaxis(string pluginId, int taxis)
        {
            //const string sqlString = "UPDATE siteserver_Plugin SET Taxis = @Taxis WHERE PluginId = @PluginId";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(nameof(PluginInstance.Taxis), taxis),
            //    GetParameter(nameof(PluginConfigInfo.PluginId), pluginId)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            
            UpdateAll(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.PluginId, pluginId)
            );
        }

        public void SetIsDisabledAndTaxis(string pluginId, out bool isDisabled, out int taxis)
        {
            isDisabled = false;
            taxis = 0;

            //var exists = false;

            //var sqlString = "SELECT Id FROM siteserver_Plugin WHERE PluginId = @PluginId";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(nameof(PluginConfigInfo.PluginId), pluginId)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read() && !rdr.IsDBNull(0))
            //    {
            //        exists = true;
            //    }
            //    rdr.Close();
            //}

            var exists = Exists(Q
                .Where(Attr.PluginId, pluginId));

            if (!exists)
            {
                //sqlString = "INSERT INTO siteserver_Plugin(PluginId, IsDisabled, Taxis) VALUES (@PluginId, @IsDisabled, @Taxis)";

                //parameters = new[]
                //{
                //    GetParameter(nameof(PluginConfigInfo.PluginId), pluginId),
                //    GetParameter(nameof(PluginInstance.IsDisabled), false.ToString()),
                //    GetParameter(nameof(PluginInstance.Taxis), 0)
                //};

                //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

                InsertObject(new PluginInfo
                {
                    PluginId = pluginId,
                    Disabled = false,
                    Taxis = 0
                });
            }

            //sqlString = "SELECT IsDisabled, Taxis FROM siteserver_Plugin WHERE PluginId = @PluginId";

            //parameters = new[]
            //{
            //    GetParameter(nameof(PluginConfigInfo.PluginId), pluginId)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read() && !rdr.IsDBNull(0))
            //    {
            //        isDisabled = TranslateUtils.ToBool(rdr.GetString(0));
            //        taxis = rdr.GetInt32(1);
            //    }
            //    rdr.Close();
            //}

            var result = GetValue<(string IsDisabled, int Taxis)?> (Q
                .Select(Attr.IsDisabled, Attr.Taxis)
                .Where(Attr.PluginId, pluginId));

            if (result == null) return;

            isDisabled = TranslateUtils.ToBool(result.Value.IsDisabled);
            taxis = result.Value.Taxis;
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.CMS.Plugin;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class PluginDao : DataProviderBase
//    {
//        public override string TableName => "siteserver_Plugin";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(PluginInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(PluginInfo.PluginId),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(PluginInfo.IsDisabled),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(PluginInfo.Taxis),
//                DataType = DataType.Integer
//            }
//        };

//        public void DeleteById(string pluginId)
//        {
//            const string sqlString = "DELETE FROM siteserver_Plugin WHERE PluginId = @PluginId";

//            IDataParameter[] parameters =
//            {
//                GetParameter(nameof(PluginConfigInfo.PluginId), pluginId)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public void UpdateIsDisabled(string pluginId, bool isDisabled)
//        {
//            const string sqlString = "UPDATE siteserver_Plugin SET IsDisabled = @IsDisabled WHERE PluginId = @PluginId";

//            IDataParameter[] parameters =
//            {
//                GetParameter(nameof(PluginInstance.IsDisabled), isDisabled.ToString()),
//                GetParameter(nameof(PluginConfigInfo.PluginId), pluginId)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public void UpdateTaxis(string pluginId, int taxis)
//        {
//            const string sqlString = "UPDATE siteserver_Plugin SET Taxis = @Taxis WHERE PluginId = @PluginId";

//            IDataParameter[] parameters =
//            {
//                GetParameter(nameof(PluginInstance.Taxis), taxis),
//                GetParameter(nameof(PluginConfigInfo.PluginId), pluginId)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public void SetIsDisabledAndTaxis(string pluginId, out bool isDisabled, out int taxis)
//        {
//            isDisabled = false;
//            taxis = 0;

//            var exists = false;

//            var sqlString = "SELECT Id FROM siteserver_Plugin WHERE PluginId = @PluginId";

//            IDataParameter[] parameters =
//            {
//                GetParameter(nameof(PluginConfigInfo.PluginId), pluginId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read() && !rdr.IsDBNull(0))
//                {
//                    exists = true;
//                }
//                rdr.Close();
//            }

//            if (!exists)
//            {
//                sqlString = "INSERT INTO siteserver_Plugin(PluginId, IsDisabled, Taxis) VALUES (@PluginId, @IsDisabled, @Taxis)";

//                parameters = new[]
//                {
//                    GetParameter(nameof(PluginConfigInfo.PluginId), pluginId),
//                    GetParameter(nameof(PluginInstance.IsDisabled), false.ToString()),
//                    GetParameter(nameof(PluginInstance.Taxis), 0)
//                };

//                DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//            }

//            sqlString = "SELECT IsDisabled, Taxis FROM siteserver_Plugin WHERE PluginId = @PluginId";

//            parameters = new []
//            {
//                GetParameter(nameof(PluginConfigInfo.PluginId), pluginId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read() && !rdr.IsDBNull(0))
//                {
//                    isDisabled = TranslateUtils.ToBool(rdr.GetString(0));
//                    taxis = rdr.GetInt32(1);
//                }
//                rdr.Close();
//            }
//        }
//    }
//}
