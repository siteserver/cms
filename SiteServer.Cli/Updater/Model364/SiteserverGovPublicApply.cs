using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverGovPublicApply
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("styleID")]
        public long StyleId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("isOrganization")]
        public string IsOrganization { get; set; }

        [JsonProperty("civicName")]
        public string CivicName { get; set; }

        [JsonProperty("civicOrganization")]
        public string CivicOrganization { get; set; }

        [JsonProperty("civicCardType")]
        public string CivicCardType { get; set; }

        [JsonProperty("civicCardNo")]
        public string CivicCardNo { get; set; }

        [JsonProperty("civicPhone")]
        public string CivicPhone { get; set; }

        [JsonProperty("civicPostCode")]
        public string CivicPostCode { get; set; }

        [JsonProperty("civicAddress")]
        public string CivicAddress { get; set; }

        [JsonProperty("civicEmail")]
        public string CivicEmail { get; set; }

        [JsonProperty("civicFax")]
        public string CivicFax { get; set; }

        [JsonProperty("orgName")]
        public string OrgName { get; set; }

        [JsonProperty("orgUnitCode")]
        public string OrgUnitCode { get; set; }

        [JsonProperty("orgLegalPerson")]
        public string OrgLegalPerson { get; set; }

        [JsonProperty("orgLinkName")]
        public string OrgLinkName { get; set; }

        [JsonProperty("orgPhone")]
        public string OrgPhone { get; set; }

        [JsonProperty("orgPostCode")]
        public string OrgPostCode { get; set; }

        [JsonProperty("orgAddress")]
        public string OrgAddress { get; set; }

        [JsonProperty("orgEmail")]
        public string OrgEmail { get; set; }

        [JsonProperty("orgFax")]
        public string OrgFax { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("purpose")]
        public string Purpose { get; set; }

        [JsonProperty("isApplyFree")]
        public string IsApplyFree { get; set; }

        [JsonProperty("provideType")]
        public string ProvideType { get; set; }

        [JsonProperty("obtainType")]
        public string ObtainType { get; set; }

        [JsonProperty("departmentName")]
        public string DepartmentName { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("queryCode")]
        public string QueryCode { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public partial class SiteserverGovPublicApply
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
