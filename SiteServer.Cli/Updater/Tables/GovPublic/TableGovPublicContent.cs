using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.Cli.Updater.Tables.GovPublic
{
    public partial class TableGovPublicContent
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("addUserName")]
        public string AddUserName { get; set; }

        [JsonProperty("lastEditUserName")]
        public string LastEditUserName { get; set; }

        [JsonProperty("lastEditDate")]
        public DateTime LastEditDate { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("contentGroupNameCollection")]
        public string ContentGroupNameCollection { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("sourceID")]
        public long SourceId { get; set; }

        [JsonProperty("referenceID")]
        public long ReferenceId { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("checkedLevel")]
        public long CheckedLevel { get; set; }

        [JsonProperty("comments")]
        public long Comments { get; set; }

        [JsonProperty("hits")]
        public long Hits { get; set; }

        [JsonProperty("hitsByDay")]
        public long HitsByDay { get; set; }

        [JsonProperty("hitsByWeek")]
        public long HitsByWeek { get; set; }

        [JsonProperty("hitsByMonth")]
        public long HitsByMonth { get; set; }

        [JsonProperty("lastHitsDate")]
        public DateTime LastHitsDate { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("category1ID")]
        public long Category1Id { get; set; }

        [JsonProperty("category2ID")]
        public long Category2Id { get; set; }

        [JsonProperty("category3ID")]
        public long Category3Id { get; set; }

        [JsonProperty("category4ID")]
        public long Category4Id { get; set; }

        [JsonProperty("category5ID")]
        public long Category5Id { get; set; }

        [JsonProperty("category6ID")]
        public long Category6Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("publishDate")]
        public DateTime PublishDate { get; set; }

        [JsonProperty("effectDate")]
        public DateTime EffectDate { get; set; }

        [JsonProperty("isAbolition")]
        public string IsAbolition { get; set; }

        [JsonProperty("abolitionDate")]
        public DateTime AbolitionDate { get; set; }

        [JsonProperty("documentNo")]
        public string DocumentNo { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("keywords")]
        public string Keywords { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("isRecommend")]
        public string IsRecommend { get; set; }

        [JsonProperty("isTop")]
        public string IsTop { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("addDate")]
        public DateTime AddDate { get; set; }
    }

    public partial class TableGovPublicContent
    {
        public static readonly string NewTableName = "ss_govpublic_content";

        private static List<TableColumn> NewColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = "Identifier",
                DataType = DataType.VarChar,
                DataLength = 200,
                InputStyle = new InputStyle
                {
                    InputType = InputType.Customize,
                    DisplayName = "索引号"
                }
            },
            new TableColumn
            {
                AttributeName = "DocumentNo",
                DataType = DataType.VarChar,
                DataLength = 200,
                InputStyle = new InputStyle
                {
                    InputType = InputType.Text,
                    DisplayName = "文号",
                    IsRequired = true
                }
            },
            new TableColumn
            {
                AttributeName = "DepartmentId",
                DataType = DataType.Integer,
                InputStyle = new InputStyle
                {
                    InputType = InputType.Hidden,
                    DisplayName = "部门",
                }
            },
            new TableColumn
            {
                AttributeName = "Publisher",
                DataType = DataType.VarChar,
                DataLength = 200,
                InputStyle = new InputStyle
                {
                    InputType = InputType.Text,
                    DisplayName = "发布机构"
                }
            },
            new TableColumn
            {
                AttributeName = "Keywords",
                DataType = DataType.VarChar,
                DataLength = 200,
                InputStyle = new InputStyle
                {
                    InputType = InputType.Text,
                    DisplayName = "关键词"
                }
            },
            new TableColumn
            {
                AttributeName = "PublishDate",
                DataType = DataType.DateTime,
                InputStyle = new InputStyle
                {
                    InputType = InputType.DateTime,
                    DisplayName = "发文日期",
                    IsRequired = true,
                    DefaultValue = "{Current}"
                }
            },
            new TableColumn
            {
                AttributeName = "EffectDate",
                DataType = DataType.DateTime,
                InputStyle = new InputStyle
                {
                    InputType = InputType.DateTime,
                    DisplayName = "生效日期",
                    IsRequired = true,
                    DefaultValue = "{Current}"
                }
            },
            new TableColumn
            {
                AttributeName = "IsAbolition",
                DataType = DataType.VarChar,
                DataLength = 10,
                InputStyle = new InputStyle
                {
                    InputType = InputType.Radio,
                    DisplayName = "是否废止",
                    IsRequired = true,
                    ListItems = new List<InputListItem>
                    {
                        new InputListItem
                        {
                            Text = "是",
                            Value = true.ToString(),
                            Selected = false
                        },
                        new InputListItem
                        {
                            Text = "否",
                            Value = false.ToString(),
                            Selected = true
                        },
                    }
                }
            },
            new TableColumn
            {
                AttributeName = "AbolitionDate",
                DataType = DataType.DateTime,
                InputStyle = new InputStyle
                {
                    InputType = InputType.DateTime,
                    DisplayName = "废止日期",
                    IsRequired = true,
                    DefaultValue = "{Current}"
                }
            },
            new TableColumn
            {
                AttributeName = "Description",
                DataType = DataType.VarChar,
                DataLength = 2000,
                InputStyle = new InputStyle
                {
                    InputType = InputType.TextArea,
                    DisplayName = "内容概述"
                }
            },
            new TableColumn
            {
                AttributeName = "ImageUrl",
                DataType = DataType.VarChar,
                DataLength = 200,
                InputStyle = new InputStyle
                {
                    InputType = InputType.Image,
                    DisplayName = "图片"
                }
            },
            new TableColumn
            {
                AttributeName = "FileUrl",
                DataType = DataType.VarChar,
                DataLength = 200,
                InputStyle = new InputStyle
                {
                    InputType = InputType.File,
                    DisplayName = "附件",
                }
            },
            new TableColumn
            {
                AttributeName = "Content",
                DataType = DataType.Text,
                InputStyle = new InputStyle
                {
                    InputType = InputType.TextEditor,
                    DisplayName = "内容"
                }
            }
        };

        private static List<TableColumn> GetNewColumns(List<TableColumn> oldColumns)
        {
            var columns = new List<TableColumn>();
            columns.AddRange(DataProvider.ContentDao.TableColumns);
            columns.AddRange(NewColumns);

            foreach (var tableColumnInfo in oldColumns)
            {
                if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(NodeId)))
                {
                    tableColumnInfo.AttributeName = nameof(ContentInfo.ChannelId);
                }
                else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(PublishmentSystemId)))
                {
                    tableColumnInfo.AttributeName = nameof(ContentInfo.SiteId);
                }
                else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(ContentGroupNameCollection)))
                {
                    tableColumnInfo.AttributeName = nameof(ContentInfo.GroupNameCollection);
                }

                if (!columns.Exists(c => StringUtils.EqualsIgnoreCase(c.AttributeName, tableColumnInfo.AttributeName)))
                {
                    columns.Add(tableColumnInfo);
                }
            }

            return columns;
        }

        public static ConvertInfo GetConverter(List<TableColumn> oldColumns)
        {
            return new ConvertInfo
            {
                NewTableName = NewTableName,
                NewColumns = GetNewColumns(oldColumns),
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict
            };
        }

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(ContentInfo.ChannelId), nameof(NodeId)},
                {nameof(ContentInfo.SiteId), nameof(PublishmentSystemId)},
                {nameof(ContentInfo.GroupNameCollection), nameof(ContentGroupNameCollection)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
