using System;
using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.Cli.Updater.Tables.GovInteract
{
    public partial class TableGovInteractContent
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
        public DateTimeOffset LastEditDate { get; set; }

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
        public DateTimeOffset LastHitsDate { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subTitle")]
        public string SubTitle { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("videoUrl")]
        public string VideoUrl { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("linkUrl")]
        public string LinkUrl { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("isRecommend")]
        public string IsRecommend { get; set; }

        [JsonProperty("isHot")]
        public string IsHot { get; set; }

        [JsonProperty("isColor")]
        public string IsColor { get; set; }

        [JsonProperty("isTop")]
        public string IsTop { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }
    }

    public partial class TableGovInteractContent
    {
        public const string NewTableName = "ss_govinteract_content";

        private static List<DatoryColumn> NewColumns => new List<DatoryColumn>
        {
            new DatoryColumn
            {
                AttributeName = "RealName",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Text,
                //    DisplayName = "姓名"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "Organization",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Text,
                //    DisplayName = "工作单位"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "CardType",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.SelectOne,
                //    DisplayName = "证件名称",
                //    IsRequired = true,
                //    ListItems = new List<InputListItem>
                //    {
                //        new InputListItem
                //        {
                //            Text = "身份证",
                //            Value = "身份证",
                //            Selected = true
                //        },
                //        new InputListItem
                //        {
                //            Text = "学生证",
                //            Value = "学生证",
                //            Selected = false
                //        },
                //        new InputListItem
                //        {
                //            Text = "军官证",
                //            Value = "军官证",
                //            Selected = false
                //        },
                //        new InputListItem
                //        {
                //            Text = "工作证",
                //            Value = "工作证",
                //            Selected = false
                //        }
                //    }
                //}
            },
            new DatoryColumn
            {
                AttributeName = "CardNo",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Text,
                //    DisplayName = "证件号码"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "Phone",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Text,
                //    DisplayName = "联系电话"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "PostCode",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Text,
                //    DisplayName = "邮政编码"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "Address",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Text,
                //    DisplayName = "联系地址"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "Email",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Text,
                //    DisplayName = "电子邮件"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "Fax",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Text,
                //    DisplayName = "传真"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "TypeId",
                DataType = DataType.Integer,

                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.SelectOne,
                //    DisplayName = "类型",
                //    IsRequired = true,
                //    ListItems = new List<InputListItem>
                //    {
                //        new InputListItem
                //        {
                //            Text = "求决",
                //            Value = "15",
                //            Selected = false
                //        },
                //        new InputListItem
                //        {
                //            Text = "举报",
                //            Value = "16",
                //            Selected = false
                //        },
                //        new InputListItem
                //        {
                //            Text = "投诉",
                //            Value = "17",
                //            Selected = false
                //        },
                //        new InputListItem
                //        {
                //            Text = "咨询",
                //            Value = "18",
                //            Selected = true
                //        },
                //        new InputListItem
                //        {
                //            Text = "建议",
                //            Value = "19",
                //            Selected = false
                //        },
                //        new InputListItem
                //        {
                //            Text = "感谢",
                //            Value = "20",
                //            Selected = false
                //        },
                //        new InputListItem
                //        {
                //            Text = "其他",
                //            Value = "21",
                //            Selected = false
                //        }
                //    }
                //}
            },
            new DatoryColumn
            {
                AttributeName = "IsPublic",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Radio,
                //    DisplayName = "是否公开",
                //    IsRequired = true,
                //    ListItems = new List<InputListItem>
                //    {
                //        new InputListItem
                //        {
                //            Text = "公开",
                //            Value = true.ToString(),
                //            Selected = true
                //        },
                //        new InputListItem
                //        {
                //            Text = "不公开",
                //            Value = false.ToString(),
                //            Selected = false
                //        }
                //    }
                //}
            },
            new DatoryColumn
            {
                AttributeName = "Content",
                DataType = DataType.Text,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.TextEditor,
                //    DisplayName = "内容"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "FileUrl",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.File,
                //    DisplayName = "附件"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "DepartmentId",
                DataType = DataType.Integer,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Customize,
                //    DisplayName = "提交部门"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "DepartmentName",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Hidden,
                //    DisplayName = "提交部门"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "QueryCode",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Hidden,
                //    DisplayName = "查询码"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "State",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Hidden,
                //    DisplayName = "状态"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "IpAddress",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Hidden,
                //    DisplayName = "IP地址"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "ReplyContent",
                DataType = DataType.Text,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Hidden,
                //    DisplayName = "回复内容"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "ReplyFileUrl",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Hidden,
                //    DisplayName = "回复附件"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "ReplyDepartmentName",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Hidden,
                //    DisplayName = "回复部门"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "ReplyUserName",
                DataType = DataType.VarChar,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Hidden,
                //    DisplayName = "回复人"
                //}
            },
            new DatoryColumn
            {
                AttributeName = "ReplyAddDate",
                DataType = DataType.DateTime,
                //InputStyle = new InputStyle
                //{
                //    InputType = InputType.Hidden,
                //    DisplayName = "回复时间"
                //}
            }
        };

        private static List<DatoryColumn> GetNewColumns(List<DatoryColumn> oldColumns)
        {
            var columns = new List<DatoryColumn>();
            columns.AddRange(DataProvider.ContentRepository.TableColumns);
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

        public static ConvertInfo GetConverter(List<DatoryColumn> oldColumns)
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