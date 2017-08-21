using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
    public class GatherRuleDao : DataProviderBase
    {
        private const string SqlSelectGatherRule = "SELECT GatherRuleName, PublishmentSystemID, CookieString, GatherUrlIsCollection, GatherUrlCollection, GatherUrlIsSerialize, GatherUrlSerialize, SerializeFrom, SerializeTo, SerializeInterval, SerializeIsOrderByDesc, SerializeIsAddZero, NodeID, Charset, UrlInclude, TitleInclude, ContentExclude, ContentHtmlClearCollection, ContentHtmlClearTagCollection, LastGatherDate, ListAreaStart, ListAreaEnd, ContentChannelStart, ContentChannelEnd, ContentTitleStart, ContentTitleEnd, ContentContentStart, ContentContentEnd, ContentNextPageStart, ContentNextPageEnd, ContentAttributes, ContentAttributesXML, ExtendValues FROM siteserver_GatherRule WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAllGatherRuleByPsId = "SELECT GatherRuleName, PublishmentSystemID, CookieString, GatherUrlIsCollection, GatherUrlCollection, GatherUrlIsSerialize, GatherUrlSerialize, SerializeFrom, SerializeTo, SerializeInterval, SerializeIsOrderByDesc, SerializeIsAddZero, NodeID, Charset, UrlInclude, TitleInclude, ContentExclude, ContentHtmlClearCollection, ContentHtmlClearTagCollection, LastGatherDate, ListAreaStart, ListAreaEnd, ContentChannelStart, ContentChannelEnd, ContentTitleStart, ContentTitleEnd, ContentContentStart, ContentContentEnd, ContentNextPageStart, ContentNextPageEnd, ContentAttributes, ContentAttributesXML, ExtendValues FROM siteserver_GatherRule WHERE PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectGatherRuleNameByPsId = "SELECT GatherRuleName FROM siteserver_GatherRule WHERE PublishmentSystemID = @PublishmentSystemID";

        private const string SqlInsertGatherRule = @"
INSERT INTO siteserver_GatherRule 
(GatherRuleName, PublishmentSystemID, CookieString, GatherUrlIsCollection, GatherUrlCollection, GatherUrlIsSerialize, GatherUrlSerialize, SerializeFrom, SerializeTo, SerializeInterval, SerializeIsOrderByDesc, SerializeIsAddZero, NodeID, Charset, UrlInclude, TitleInclude, ContentExclude, ContentHtmlClearCollection, ContentHtmlClearTagCollection, LastGatherDate, ListAreaStart, ListAreaEnd, ContentChannelStart, ContentChannelEnd, ContentTitleStart, ContentTitleEnd, ContentContentStart, ContentContentEnd, ContentNextPageStart, ContentNextPageEnd, ContentAttributes, ContentAttributesXML, ExtendValues) VALUES (@GatherRuleName, @PublishmentSystemID, @CookieString, @GatherUrlIsCollection, @GatherUrlCollection, @GatherUrlIsSerialize, @GatherUrlSerialize, @SerializeFrom, @SerializeTo, @SerializeInterval, @SerializeIsOrderByDesc, @SerializeIsAddZero, @NodeID, @Charset, @UrlInclude, @TitleInclude, @ContentExclude, @ContentHtmlClearCollection, @ContentHtmlClearTagCollection, @LastGatherDate, @ListAreaStart, @ListAreaEnd, @ContentChannelStart, @ContentChannelEnd, @ContentTitleStart, @ContentTitleEnd, @ContentContentStart, @ContentContentEnd, @ContentNextPageStart, @ContentNextPageEnd, @ContentAttributes, @ContentAttributesXML, @ExtendValues)";

        private const string SqlUpdateGatherRule = @"
UPDATE siteserver_GatherRule SET 
CookieString = @CookieString, GatherUrlIsCollection = @GatherUrlIsCollection, GatherUrlCollection = @GatherUrlCollection, GatherUrlIsSerialize = @GatherUrlIsSerialize, GatherUrlSerialize = @GatherUrlSerialize, SerializeFrom = @SerializeFrom, SerializeTo = @SerializeTo, SerializeInterval = @SerializeInterval, SerializeIsOrderByDesc = @SerializeIsOrderByDesc, SerializeIsAddZero = @SerializeIsAddZero, NodeID = @NodeID, Charset = @Charset, UrlInclude = @UrlInclude, TitleInclude = @TitleInclude, ContentExclude = @ContentExclude, ContentHtmlClearCollection = @ContentHtmlClearCollection, ContentHtmlClearTagCollection = @ContentHtmlClearTagCollection, LastGatherDate = @LastGatherDate, ListAreaStart = @ListAreaStart, ListAreaEnd = @ListAreaEnd, ContentChannelStart = @ContentChannelStart, ContentChannelEnd = @ContentChannelEnd, ContentTitleStart = @ContentTitleStart, ContentTitleEnd = @ContentTitleEnd, ContentContentStart = @ContentContentStart, ContentContentEnd = @ContentContentEnd, ContentNextPageStart = @ContentNextPageStart, ContentNextPageEnd = @ContentNextPageEnd, ContentAttributes = @ContentAttributes, ContentAttributesXML = @ContentAttributesXML, ExtendValues = @ExtendValues WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlUpdateLastGatherDate = "UPDATE siteserver_GatherRule SET LastGatherDate = @LastGatherDate WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlDeleteGatherRule = "DELETE FROM siteserver_GatherRule WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

        private const string ParmGatherRuleName = "@GatherRuleName";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";

        private const string ParmCookieString = "@CookieString";
        private const string ParmGatherUrlIsCollection = "@GatherUrlIsCollection";
        private const string ParmGatherUrlCollection = "@GatherUrlCollection";
        private const string ParmGatherUrlIsSerialize = "@GatherUrlIsSerialize";
        private const string ParmGatherUrlSerialize = "@GatherUrlSerialize";
        private const string ParmGatherSerializeFrom = "@SerializeFrom";
        private const string ParmGatherSerializeTo = "@SerializeTo";
        private const string ParmGatherSerializeInternal = "@SerializeInterval";
        private const string ParmGatherSerializeOrderByDesc = "@SerializeIsOrderByDesc";
        private const string ParmGatherSerializeIsAddZero = "@SerializeIsAddZero";

        private const string ParmNodeId = "@NodeID";
        private const string ParmCharset = "@Charset";
        private const string ParmUrlInclude = "@UrlInclude";
        private const string ParmTitleInclude = "@TitleInclude";
        private const string ParmContentExclude = "@ContentExclude";
        private const string ParmContentHtmlClearCollection = "@ContentHtmlClearCollection";
        private const string ParmContentHtmlClearTagCollection = "@ContentHtmlClearTagCollection";
        private const string ParmLastGatherDate = "@LastGatherDate";

        private const string ParmListAreaStart = "@ListAreaStart";
        private const string ParmListAreaEnd = "@ListAreaEnd";
        private const string ParmListContentChannelStart = "@ContentChannelStart";
        private const string ParmListContentChannelEnd = "@ContentChannelEnd";
        private const string ParmContentTitleStart = "@ContentTitleStart";
        private const string ParmContentTitleEnd = "@ContentTitleEnd";
        private const string ParmContentContentStart = "@ContentContentStart";
        private const string ParmContentContentEnd = "@ContentContentEnd";
        private const string ParmContentNextPageStart = "@ContentNextPageStart";
        private const string ParmContentNextPageEnd = "@ContentNextPageEnd";
        private const string ParmContentAttributes = "@ContentAttributes";
        private const string ParmContentAttributesXml = "@ContentAttributesXML";
        private const string ParmExtendValues = "@ExtendValues";

        public void Insert(GatherRuleInfo gatherRuleInfo)
        {
            var insertParms = new IDataParameter[]
            {
                GetParameter(ParmGatherRuleName, DataType.NVarChar, 50, gatherRuleInfo.GatherRuleName),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, gatherRuleInfo.PublishmentSystemId),
                GetParameter(ParmCookieString, DataType.Text, gatherRuleInfo.CookieString),
                GetParameter(ParmGatherUrlIsCollection, DataType.VarChar, 18, gatherRuleInfo.GatherUrlIsCollection.ToString()),
                GetParameter(ParmGatherUrlCollection, DataType.Text, gatherRuleInfo.GatherUrlCollection),
                GetParameter(ParmGatherUrlIsSerialize, DataType.VarChar, 18, gatherRuleInfo.GatherUrlIsSerialize.ToString()),
                GetParameter(ParmGatherUrlSerialize, DataType.VarChar, 200, gatherRuleInfo.GatherUrlSerialize),
                GetParameter(ParmGatherSerializeFrom, DataType.Integer, gatherRuleInfo.SerializeFrom),
                GetParameter(ParmGatherSerializeTo, DataType.Integer, gatherRuleInfo.SerializeTo),
                GetParameter(ParmGatherSerializeInternal, DataType.Integer, gatherRuleInfo.SerializeInterval),
                GetParameter(ParmGatherSerializeOrderByDesc, DataType.VarChar, 18, gatherRuleInfo.SerializeIsOrderByDesc.ToString()),
                GetParameter(ParmGatherSerializeIsAddZero, DataType.VarChar, 18, gatherRuleInfo.SerializeIsAddZero.ToString()),
                GetParameter(ParmNodeId, DataType.Integer, gatherRuleInfo.NodeId),
                GetParameter(ParmCharset, DataType.VarChar, 50, ECharsetUtils.GetValue(gatherRuleInfo.Charset)),
                GetParameter(ParmUrlInclude, DataType.VarChar, 200, gatherRuleInfo.UrlInclude),
                GetParameter(ParmTitleInclude, DataType.NVarChar, 255, gatherRuleInfo.TitleInclude),
                GetParameter(ParmContentExclude, DataType.NText, gatherRuleInfo.ContentExclude),
                GetParameter(ParmContentHtmlClearCollection, DataType.NVarChar, 255, gatherRuleInfo.ContentHtmlClearCollection),
                GetParameter(ParmContentHtmlClearTagCollection, DataType.NVarChar, 255, gatherRuleInfo.ContentHtmlClearTagCollection),
                GetParameter(ParmLastGatherDate, DataType.DateTime, gatherRuleInfo.LastGatherDate),
                GetParameter(ParmListAreaStart, DataType.NText, gatherRuleInfo.ListAreaStart),
                GetParameter(ParmListAreaEnd, DataType.NText, gatherRuleInfo.ListAreaEnd),
                GetParameter(ParmListContentChannelStart, DataType.NText, gatherRuleInfo.ContentChannelStart),
                GetParameter(ParmListContentChannelEnd, DataType.NText, gatherRuleInfo.ContentChannelEnd),
                GetParameter(ParmContentTitleStart, DataType.NText, gatherRuleInfo.ContentTitleStart),
                GetParameter(ParmContentTitleEnd, DataType.NText, gatherRuleInfo.ContentTitleEnd),
                GetParameter(ParmContentContentStart, DataType.NText, gatherRuleInfo.ContentContentStart),
                GetParameter(ParmContentContentEnd, DataType.NText, gatherRuleInfo.ContentContentEnd),
                GetParameter(ParmContentNextPageStart, DataType.NText, gatherRuleInfo.ContentNextPageStart),
                GetParameter(ParmContentNextPageEnd, DataType.NText, gatherRuleInfo.ContentNextPageEnd),
                GetParameter(ParmContentAttributes, DataType.NText, gatherRuleInfo.ContentAttributes),
                GetParameter(ParmContentAttributesXml, DataType.NText, gatherRuleInfo.ContentAttributesXml),
                GetParameter(ParmExtendValues, DataType.NText, gatherRuleInfo.Additional.ToString())
            };

            ExecuteNonQuery(SqlInsertGatherRule, insertParms);
        }

        public void UpdateLastGatherDate(string gatherRuleName, int publishmentSystemId)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmLastGatherDate, DataType.DateTime, DateTime.Now),
                GetParameter(ParmGatherRuleName, DataType.NVarChar, 50, gatherRuleName),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
            };

            ExecuteNonQuery(SqlUpdateLastGatherDate, parms);
        }

        public void Update(GatherRuleInfo gatherRuleInfo)
        {

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmCookieString, DataType.Text, gatherRuleInfo.CookieString),
                GetParameter(ParmGatherUrlIsCollection, DataType.VarChar, 18, gatherRuleInfo.GatherUrlIsCollection.ToString()),
                GetParameter(ParmGatherUrlCollection, DataType.Text, gatherRuleInfo.GatherUrlCollection),
                GetParameter(ParmGatherUrlIsSerialize, DataType.VarChar, 18, gatherRuleInfo.GatherUrlIsSerialize.ToString()),
                GetParameter(ParmGatherUrlSerialize, DataType.VarChar, 200, gatherRuleInfo.GatherUrlSerialize),
                GetParameter(ParmGatherSerializeFrom, DataType.Integer, gatherRuleInfo.SerializeFrom),
                GetParameter(ParmGatherSerializeTo, DataType.Integer, gatherRuleInfo.SerializeTo),
                GetParameter(ParmGatherSerializeInternal, DataType.Integer, gatherRuleInfo.SerializeInterval),
                GetParameter(ParmGatherSerializeOrderByDesc, DataType.VarChar, 18, gatherRuleInfo.SerializeIsOrderByDesc.ToString()),
                GetParameter(ParmGatherSerializeIsAddZero, DataType.VarChar, 18, gatherRuleInfo.SerializeIsAddZero.ToString()),
                GetParameter(ParmNodeId, DataType.Integer, gatherRuleInfo.NodeId),
                GetParameter(ParmCharset, DataType.VarChar, 50, ECharsetUtils.GetValue(gatherRuleInfo.Charset)),
                GetParameter(ParmUrlInclude, DataType.VarChar, 200, gatherRuleInfo.UrlInclude),
                GetParameter(ParmTitleInclude, DataType.NVarChar, 255, gatherRuleInfo.TitleInclude),
                GetParameter(ParmContentExclude, DataType.NText, gatherRuleInfo.ContentExclude),
                GetParameter(ParmContentHtmlClearCollection, DataType.NVarChar, 255, gatherRuleInfo.ContentHtmlClearCollection),
                GetParameter(ParmContentHtmlClearTagCollection, DataType.NVarChar, 255, gatherRuleInfo.ContentHtmlClearTagCollection),
                GetParameter(ParmLastGatherDate, DataType.DateTime, gatherRuleInfo.LastGatherDate),
                GetParameter(ParmListAreaStart, DataType.NText, gatherRuleInfo.ListAreaStart),
                GetParameter(ParmListAreaEnd, DataType.NText, gatherRuleInfo.ListAreaEnd),
                GetParameter(ParmListContentChannelStart, DataType.NText, gatherRuleInfo.ContentChannelStart),
                GetParameter(ParmListContentChannelEnd, DataType.NText, gatherRuleInfo.ContentChannelEnd),
                GetParameter(ParmContentTitleStart, DataType.NText, gatherRuleInfo.ContentTitleStart),
                GetParameter(ParmContentTitleEnd, DataType.NText, gatherRuleInfo.ContentTitleEnd),
                GetParameter(ParmContentContentStart, DataType.NText, gatherRuleInfo.ContentContentStart),
                GetParameter(ParmContentContentEnd, DataType.NText, gatherRuleInfo.ContentContentEnd),
                GetParameter(ParmContentNextPageStart, DataType.NText, gatherRuleInfo.ContentNextPageStart),
                GetParameter(ParmContentNextPageEnd, DataType.NText, gatherRuleInfo.ContentNextPageEnd),
                GetParameter(ParmContentAttributes, DataType.NText, gatherRuleInfo.ContentAttributes),
                GetParameter(ParmContentAttributesXml, DataType.NText, gatherRuleInfo.ContentAttributesXml),
                GetParameter(ParmExtendValues, DataType.NText, gatherRuleInfo.Additional.ToString()),
                GetParameter(ParmGatherRuleName, DataType.NVarChar, 50, gatherRuleInfo.GatherRuleName),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, gatherRuleInfo.PublishmentSystemId)
            };

            ExecuteNonQuery(SqlUpdateGatherRule, updateParms);
        }


        public void Delete(string gatherRuleName, int publishmentSystemId)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmGatherRuleName, DataType.NVarChar, 50, gatherRuleName),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
            };

            ExecuteNonQuery(SqlDeleteGatherRule, parms);
        }

        public GatherRuleInfo GetGatherRuleInfo(string gatherRuleName, int publishmentSystemId)
        {
            GatherRuleInfo gatherRuleInfo = null;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmGatherRuleName, DataType.NVarChar, 50, gatherRuleName),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectGatherRule, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    gatherRuleInfo = new GatherRuleInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return gatherRuleInfo;
        }

        public string GetImportGatherRuleName(int publishmentSystemId, string gatherRuleName)
        {
            string importGatherRuleName;
            if (gatherRuleName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var gatherRuleNameCount = 0;
                var lastGatherRuleName = gatherRuleName.Substring(gatherRuleName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstGatherRuleName = gatherRuleName.Substring(0, gatherRuleName.Length - lastGatherRuleName.Length);
                try
                {
                    gatherRuleNameCount = int.Parse(lastGatherRuleName);
                }
                catch
                {
                    // ignored
                }
                gatherRuleNameCount++;
                importGatherRuleName = firstGatherRuleName + gatherRuleNameCount;
            }
            else
            {
                importGatherRuleName = gatherRuleName + "_1";
            }

            var parms = new IDataParameter[]
            {
                GetParameter(ParmGatherRuleName, DataType.NVarChar, 50, importGatherRuleName),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectGatherRule, parms))
            {
                if (rdr.Read())
                {
                    importGatherRuleName = GetImportGatherRuleName(publishmentSystemId, importGatherRuleName);
                }
                rdr.Close();
            }

            return importGatherRuleName;
        }

        public IEnumerable GetDataSource(int publishmentSystemId)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
            };

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllGatherRuleByPsId, parms);
            return enumerable;
        }

        public ArrayList GetGatherRuleInfoArrayList(int publishmentSystemId)
        {
            var list = new ArrayList();

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectAllGatherRuleByPsId, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var gatherRuleInfo = new GatherRuleInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    list.Add(gatherRuleInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<string> GetGatherRuleNameArrayList(int publishmentSystemId)
        {
            var list = new List<string>();

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectGatherRuleNameByPsId, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public void OpenAuto(int publishmentSystemId, ArrayList gatherRuleNameCollection)
        {
            string sql =
                $"UPDATE siteserver_GatherRule SET IsAutoCreate = 'True' WHERE PublishmentSystemID = {publishmentSystemId} AND GatherRuleName in ({TranslateUtils.ToSqlInStringWithQuote(gatherRuleNameCollection)})";
            ExecuteNonQuery(sql);
        }

        public void CloseAuto(int publishmentSystemId, ArrayList gatherRuleNameCollection)
        {
            string sql =
                $"UPDATE siteserver_GatherRule SET IsAutoCreate = 'False' WHERE PublishmentSystemID = {publishmentSystemId} AND GatherRuleName in ({TranslateUtils.ToSqlInStringWithQuote(gatherRuleNameCollection)})";
            ExecuteNonQuery(sql);
        }
    }
}
