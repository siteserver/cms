using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalExportMessage : BasePageCms
    {
        private string _exportType;

        public const int Width = 380;
        public const int Height = 250;
        public const string ExportTypeContentZip = "ContentZip";
        public const string ExportTypeContentAccess = "ContentAccess";
        public const string ExportTypeContentExcel = "ContentExcel";
        public const string ExportTypeContentTxtZip = "ContentTxtZip";
        public const string ExportTypeInputContent = "InputContent";
        public const string ExportTypeComment = "Comment";
        public const string ExportTypeGatherRule = "GatherRule";
        public const string ExportTypeInput = "Input";
        public const string ExportTypeRelatedField = "RelatedField";
        public const string ExportTypeTagStyle = "TagStyle";
        public const string ExportTypeChannel = "Channel";
        public const string ExportTypeSingleTableStyle = "SingleTableStyle";
        public const string ExportTypeTrackerHour = "TrackerHour";
        public const string ExportTypeTrackerDay = "TrackerDay";
        public const string ExportTypeTrackerMonth = "TrackerMonth";
        public const string ExportTypeTrackerYear = "TrackerYear";
        public const string ExportTypeTrackerContent = "TrackerContent";

        public static string GetRedirectUrlStringToExportContent(int publishmentSystemId, int nodeId,
            string exportType, string contentIdCollection, string displayAttributes, bool isPeriods,
            string startDate, string endDate, ETriState checkedState)
        {
            return PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"NodeID", nodeId.ToString()},
                        {"ExportType", exportType},
                        {"ContentIDCollection", contentIdCollection},
                        {"DisplayAttributes", displayAttributes},
                        {"isPeriods", isPeriods.ToString()},
                        {"startDate", startDate},
                        {"endDate", endDate},
                        {"checkedState", ETriStateUtils.GetValue(checkedState)}
                    });
        }

        public static string GetOpenWindowStringToInputContent(int publishmentSystemId, int inputId)
        {
            return PageUtils.GetOpenWindowString("导出数据", PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"InputID", inputId.ToString()},
                        {"ExportType", ExportTypeInputContent}
                    }), Width, Height, true);
        }

        public static string GetOpenWindowStringToComment(int publishmentSystemId, int nodeId, int contentId)
        {
            return PageUtils.GetOpenWindowString("导出数据", PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"NodeID", nodeId.ToString()},
                        {"ContentID", contentId.ToString()},
                        {"ExportType", ExportTypeComment}
                    }), Width, Height, true);
        }

        public static string GetOpenWindowStringToGatherRule(int publishmentSystemId, string checkBoxId, string alertString)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("导出数据", PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"ExportType", ExportTypeGatherRule}
                    }), checkBoxId, alertString, Width, Height, true);
        }

        public static string GetOpenWindowStringToInput(int publishmentSystemId, int inputId)
        {
            return PageUtils.GetOpenWindowString("导出数据", PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"InputID", inputId.ToString()},
                        {"ExportType", ExportTypeInput}
                    }), Width, Height, true);
        }

        public static string GetOpenWindowStringToTagStyle(int publishmentSystemId, int styleId)
        {
            return PageUtils.GetOpenWindowString("导出数据", PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"StyleID", styleId.ToString()},
                        {"ExportType", ExportTypeTagStyle}
                    }), Width, Height, true);
        }

        public static string GetOpenWindowStringToChannel(int publishmentSystemId, string checkBoxId, string alertString)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("导出数据", PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"ExportType", ExportTypeChannel}
                    }), checkBoxId, alertString, Width, Height, true);
        }

        public static string GetOpenWindowStringToSingleTableStyle(ETableStyle tableStyle, string tableName, int publishmentSystemId, int relatedIdentity)
        {
            return PageUtils.GetOpenWindowString("导出数据", PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"TableStyle", ETableStyleUtils.GetValue(tableStyle)},
                        {"TableName", tableName},
                        {"ExportType", ExportTypeSingleTableStyle},
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"RelatedIdentity", relatedIdentity.ToString()}
                    }), Width, Height, true);
        }

        public static string GetOpenWindowStringToRelatedField(int publishmentSystemId, int relatedFieldId)
        {
            return PageUtils.GetOpenWindowString("导出数据", PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"RelatedFieldID", relatedFieldId.ToString()},
                        {"ExportType", ExportTypeRelatedField}
                    }), Width, Height, true);
        }

        public static string GetOpenWindowStringToExport(int publishmentSystemId, string exportType)
        {
            return PageUtils.GetOpenWindowString("导出数据", PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"ExportType", exportType}
                    }), Width, Height, true);
        }

        public static string GetRedirectUrlStringToExportTracker(string startDateString, string endDateString, int publishmentSystemId, int nodeId, int contentId, int totalNum, bool isDelete)
        {
            return PageUtils.GetCmsUrl(nameof(ModalExportMessage), new NameValueCollection
                    {
                        {"ExportType", ExportTypeTrackerContent},
                        {"StartDateString", startDateString},
                        {"EndDateString", endDateString},
                        {"PublishmentSystemID", publishmentSystemId.ToString()},
                        {"NodeID", nodeId.ToString()},
                        {"ContentID", contentId.ToString()},
                        {"TotalNum", totalNum.ToString()},
                        {"IsDelete", isDelete.ToString()}
                    });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _exportType = Body.GetQueryString("ExportType");

            if (!IsPostBack)
            {
                var isExport = true;
                var fileName = string.Empty;
                try
                {
                    if (_exportType == ExportTypeTrackerHour)
                    {
                        fileName = ExportTrackingHours();
                    }
                    else if (_exportType == ExportTypeTrackerDay)
                    {
                        fileName = ExportTrackingDays();
                    }
                    else if (_exportType == ExportTypeTrackerMonth)
                    {
                        fileName = ExportTrackingMonths();
                    }
                    else if (_exportType == ExportTypeTrackerYear)
                    {
                        fileName = ExportTrackingYears();
                    }
                    else if (_exportType == ExportTypeTrackerContent)
                    {
                        var startDateString = Body.GetQueryString("StartDateString");
                        var endDateString = Body.GetQueryString("EndDateString");
                        var nodeId = Body.GetQueryInt("NodeID");
                        var contentId = Body.GetQueryInt("ContentID");
                        var totalNum = Body.GetQueryInt("TotalNum");
                        var isDelete = Body.GetQueryBool("IsDelete");
                        fileName = ExportTrackingContents(startDateString, endDateString, nodeId, contentId, totalNum, isDelete);
                    }
                    else if (_exportType == ExportTypeInputContent)
                    {
                        var inputId = Body.GetQueryInt("InputID");
                        fileName = ExportInputContent(inputId);
                    }
                    else if (_exportType == ExportTypeComment)
                    {
                        var nodeId = Body.GetQueryInt("NodeID");
                        var contentId = Body.GetQueryInt("ContentID");
                        fileName = ExportComment(nodeId, contentId);
                    }
                    else if (_exportType == ExportTypeGatherRule)
                    {
                        var gatherRuleNameArrayList = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("GatherRuleNameCollection"));
                        fileName = ExportGatherRule(gatherRuleNameArrayList);
                    }
                    else if (_exportType == ExportTypeInput)
                    {
                        var inputId = Body.GetQueryInt("InputID");
                        fileName = ExportInput(inputId);
                    }
                    else if (_exportType == ExportTypeRelatedField)
                    {
                        var relatedFieldId = Body.GetQueryInt("RelatedFieldID");
                        fileName = ExportRelatedField(relatedFieldId);
                    }
                    else if (_exportType == ExportTypeTagStyle)
                    {
                        var styleId = Body.GetQueryInt("StyleID");
                        fileName = ExportTagStyle(styleId);
                    }
                    else if (_exportType == ExportTypeContentZip)
                    {
                        var nodeId = Body.GetQueryInt("NodeID");
                        var contentIdCollection = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
                        var isPeriods = Body.GetQueryBool("isPeriods");
                        var startDate = Body.GetQueryString("startDate");
                        var endDate = Body.GetQueryString("endDate");
                        var checkedState = ETriStateUtils.GetEnumType(Body.GetQueryString("checkedState"));
                        isExport = ExportContentZip(nodeId, contentIdCollection, isPeriods, startDate, endDate, checkedState, out fileName);
                    }
                    else if (_exportType == ExportTypeContentAccess)
                    {
                        var nodeId = Body.GetQueryInt("NodeID");
                        var contentIdCollection = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
                        var displayAttributes = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("DisplayAttributes"));
                        var isPeriods = Body.GetQueryBool("isPeriods");
                        var startDate = Body.GetQueryString("startDate");
                        var endDate = Body.GetQueryString("endDate");
                        var checkedState = ETriStateUtils.GetEnumType(Body.GetQueryString("checkedState"));
                        isExport = ExportContentAccess(nodeId, contentIdCollection, displayAttributes, isPeriods, startDate, endDate, checkedState, out fileName);
                    }
                    else if (_exportType == ExportTypeContentExcel)
                    {
                        var nodeId = Body.GetQueryInt("NodeID");
                        var contentIdCollection = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
                        var displayAttributes = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("DisplayAttributes"));
                        var isPeriods = Body.GetQueryBool("isPeriods");
                        var startDate = Body.GetQueryString("startDate");
                        var endDate = Body.GetQueryString("endDate");
                        var checkedState = ETriStateUtils.GetEnumType(Body.GetQueryString("checkedState"));
                        ExportContentExcel(nodeId, contentIdCollection, displayAttributes, isPeriods, startDate, endDate, checkedState, out fileName);
                    }
                    else if (_exportType == ExportTypeChannel)
                    {
                        var nodeIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ChannelIDCollection"));
                        fileName = ExportChannel(nodeIdList);
                    }
                    else if (_exportType == ExportTypeSingleTableStyle)
                    {
                        var tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("TableStyle"));
                        var tableName = Body.GetQueryString("TableName");
                        var relatedIdentity = Body.GetQueryInt("RelatedIdentity");
                        fileName = ExportSingleTableStyle(tableStyle, tableName, relatedIdentity);
                    }

                    if (isExport)
                    {
                        var link = new HyperLink();
                        var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                        link.NavigateUrl = ActionsDownload.GetUrl(PublishmentSystemInfo.Additional.ApiUrl, filePath);
                        link.Text = "下载";
                        var successMessage = "成功导出文件！&nbsp;&nbsp;" + ControlUtils.GetControlRenderHtml(link);
                        SuccessMessage(successMessage);
                    }
                    else
                    {
                        FailMessage("导出失败，所选条件没有匹配内容，请重新选择条件导出内容");
                    }
                }
                catch (Exception ex)
                {
                    var failedMessage = "文件导出失败！<br/><br/>原因为：" + ex.Message;
                    FailMessage(ex, failedMessage);
                }
            }
        }

        private string ExportTrackingHours()
        {
            var docFileName = "24小时统计.csv";
            var filePath = PathUtils.GetTemporaryFilesPath(docFileName);
            ExcelObject.CreateExcelFileForTrackingHours(filePath, PublishmentSystemId);

            return docFileName;
        }

        private string ExportTrackingDays()
        {
            var docFileName = "天统计.csv";
            var filePath = PathUtils.GetTemporaryFilesPath(docFileName);
            ExcelObject.CreateExcelFileForTrackingDays(filePath, PublishmentSystemId);

            return docFileName;
        }

        private string ExportTrackingMonths()
        {
            var docFileName = "月统计.csv";
            var filePath = PathUtils.GetTemporaryFilesPath(docFileName);
            ExcelObject.CreateExcelFileForTrackingMonths(filePath, PublishmentSystemId);

            return docFileName;
        }

        private string ExportTrackingYears()
        {
            var docFileName = "年统计.csv";
            var filePath = PathUtils.GetTemporaryFilesPath(docFileName);
            ExcelObject.CreateExcelFileForTrackingYears(filePath, PublishmentSystemId);

            return docFileName;
        }

        private string ExportTrackingContents(string startDateString, string endDateString, int nodeId, int contentId, int totalNum, bool isDelete)
        {
            var docFileName = "内容统计.csv";
            var filePath = PathUtils.GetTemporaryFilesPath(docFileName);
            ExcelObject.CreateExcelFileForTrackingContents(filePath, startDateString, endDateString, PublishmentSystemInfo, nodeId, contentId, totalNum, isDelete);

            return docFileName;
        }

        private string ExportInputContent(int inputId)
        {
            var inputInfo = DataProvider.InputDao.GetInputInfo(inputId);

            string docFileName = $"{inputInfo.InputName}.csv";
            var filePath = PathUtils.GetTemporaryFilesPath(docFileName);
            ExcelObject.CreateExcelFileForInputContents(filePath, PublishmentSystemInfo, inputInfo);

            return docFileName;
        }

        private string ExportComment(int nodeId, int contentId)
        {
            var docFileName = "评论.csv";
            var filePath = PathUtils.GetTemporaryFilesPath(docFileName);
            ExcelObject.CreateExcelFileForComments(filePath, PublishmentSystemInfo, nodeId, contentId);

            return docFileName;
        }

        private string ExportGatherRule(List<string> gatherRuleNameArrayList)
        {
            var docFileName = "GatherRule.xml";
            var filePath = PathUtils.GetTemporaryFilesPath(docFileName);

            var exportObject = new ExportObject(PublishmentSystemId);
            var gatherRuleInfoArrayList = new ArrayList();
            foreach (string gatherRuleName in gatherRuleNameArrayList)
            {
                gatherRuleInfoArrayList.Add(DataProvider.GatherRuleDao.GetGatherRuleInfo(gatherRuleName, PublishmentSystemId));
            }

            exportObject.ExportGatherRule(filePath, gatherRuleInfoArrayList);

            return docFileName;
        }

        private string ExportInput(int inputId)
        {
            var exportObject = new ExportObject(PublishmentSystemId);
            return exportObject.ExportInput(inputId);
        }

        private string ExportRelatedField(int relatedFieldId)
        {
            var exportObject = new ExportObject(PublishmentSystemId);
            return exportObject.ExportRelatedField(relatedFieldId);
        }

        private string ExportTagStyle(int styleId)
        {
            var styleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(styleId);

            var exportObject = new ExportObject(PublishmentSystemId);
            return exportObject.ExportTagStyle(styleInfo);
        }

        private bool ExportContentZip(int nodeId, List<int> contentIdArrayList, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState, out string fileName)
        {
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            fileName = $"{nodeInfo.NodeName}.zip";
            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            var exportObject = new ExportObject(PublishmentSystemId);
            return exportObject.ExportContents(filePath, nodeId, contentIdArrayList, isPeriods, dateFrom, dateTo, checkedState);
        }

        private bool ExportContentAccess(int nodeId, List<int> contentIdArrayList, List<string> displayAttributes, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState, out string fileName)
        {
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            fileName = $"{nodeInfo.NodeName}.mdb";
            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            return AccessObject.CreateAccessFileForContents(filePath, PublishmentSystemInfo, nodeInfo, contentIdArrayList, displayAttributes, isPeriods, dateFrom, dateTo, checkedState);
        }

        private void ExportContentExcel(int nodeId, List<int> contentIdList, List<string> displayAttributes, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState, out string fileName)
        {
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

            fileName = $"{nodeInfo.NodeName}.csv";
            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            ExcelObject.CreateExcelFileForContents(filePath, PublishmentSystemInfo, nodeInfo, contentIdList, displayAttributes, isPeriods, dateFrom, dateTo, checkedState);
        }

        private string ExportChannel(List<int> nodeIdList)
        {
            var exportObject = new ExportObject(PublishmentSystemId);
            return exportObject.ExportChannels(nodeIdList);
        }

        private string ExportSingleTableStyle(ETableStyle tableStyle, string tableName, int relatedIdentity)
        {
            var exportObject = new ExportObject(PublishmentSystemId);
            return exportObject.ExportSingleTableStyle(tableStyle, tableName, relatedIdentity);
        }
    }
}
