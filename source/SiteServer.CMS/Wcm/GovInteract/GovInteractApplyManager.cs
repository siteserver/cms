using System;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.GovInteract
{
	public class GovInteractApplyManager
	{       
        public static string GetQueryCode()
        {
            long i = 1;
            foreach (var b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return $"{i - DateTime.Now.Ticks:x}".ToUpper();
        }

        public static string GetApplyRemark(int publishmentSystemID, int contentID)
        {
            var remarkBuilder = new StringBuilder();
            var remarkInfoArrayList = DataProvider.GovInteractRemarkDao.GetRemarkInfoArrayList(publishmentSystemID, contentID);
            foreach (GovInteractRemarkInfo remarkInfo in remarkInfoArrayList)
            {
                if (!string.IsNullOrEmpty(remarkInfo.Remark))
                {
                    if (remarkBuilder.Length > 0) remarkBuilder.Append("<br />");
                    remarkBuilder.Append(
                        $@"<span style=""color:gray;"">{EGovInteractRemarkTypeUtils.GetText(remarkInfo.RemarkType)}意见: </span>{StringUtils
                            .MaxLengthText(remarkInfo.Remark, 25)}");
                }
            }
            return remarkBuilder.ToString();
        }

        public static void LogNew(int publishmentSystemID, int nodeID, int contentID, string realName, string toDepartmentName)
        {
            var logInfo = new GovInteractLogInfo(0, publishmentSystemID, nodeID, contentID, 0, string.Empty, EGovInteractLogType.New, PageUtils.GetIpAddress(), DateTime.Now,
                $"前台{realName}提交办件{toDepartmentName}");
            DataProvider.GovInteractLogDao.Insert(logInfo);
        }

        public static void LogSwitchTo(int publishmentSystemID, int nodeID, int contentID, string switchToDepartmentName, string administratorName, int departmentId)
        {
            var logInfo = new GovInteractLogInfo(0, publishmentSystemID, nodeID, contentID, departmentId, administratorName, EGovInteractLogType.SwitchTo, PageUtils.GetIpAddress(), DateTime.Now,
                $"{DepartmentManager.GetDepartmentName(departmentId)}({administratorName})转办办件至{switchToDepartmentName} ");
            DataProvider.GovInteractLogDao.Insert(logInfo);
        }

        public static void LogTranslate(int publishmentSystemID, int nodeID, int contentID, string nodeName, string administratorName, int departmentId)
        {
            var logInfo = new GovInteractLogInfo(0, publishmentSystemID, nodeID, contentID, departmentId, administratorName, EGovInteractLogType.Translate, PageUtils.GetIpAddress(), DateTime.Now,
                $"{DepartmentManager.GetDepartmentName(departmentId)}({administratorName})从分类“{nodeName}”转移办件至此 ");
            DataProvider.GovInteractLogDao.Insert(logInfo);
        }

        public static void Log(int publishmentSystemID, int nodeID, int contentID, EGovInteractLogType logType, string administratorName, int departmentId)
        {
            var logInfo = new GovInteractLogInfo(0, publishmentSystemID, nodeID, contentID, departmentId, administratorName, logType, PageUtils.GetIpAddress(), DateTime.Now, string.Empty);

            var departmentName = DepartmentManager.GetDepartmentName(departmentId);

            if (logType == EGovInteractLogType.Accept)
            {
                logInfo.Summary = $"{departmentName}({administratorName})受理办件";
            }
            else if (logType == EGovInteractLogType.Deny)
            {
                logInfo.Summary = $"{departmentName}({administratorName})拒绝受理办件";
            }
            else if (logType == EGovInteractLogType.Reply)
            {
                logInfo.Summary = $"{departmentName}({administratorName})回复办件";
            }
            else if (logType == EGovInteractLogType.Comment)
            {
                logInfo.Summary = $"{departmentName}({administratorName})批示办件";
            }
            else if (logType == EGovInteractLogType.Redo)
            {
                logInfo.Summary = $"{departmentName}({administratorName})要求返工";
            }
            else if (logType == EGovInteractLogType.Check)
            {
                logInfo.Summary = $"{departmentName}({administratorName})审核通过";
            }
            DataProvider.GovInteractLogDao.Insert(logInfo);
        }

        public static EGovInteractLimitType GetLimitType(PublishmentSystemInfo publishmentSystemInfo, GovInteractContentInfo contentInfo)
        {
            var ts = new TimeSpan(DateTime.Now.Ticks - contentInfo.AddDate.Ticks);

            var alert = publishmentSystemInfo.Additional.GovInteractApplyDateLimit + publishmentSystemInfo.Additional.GovInteractApplyAlertDate;
            var yellow = alert + publishmentSystemInfo.Additional.GovInteractApplyYellowAlertDate;
            var red = yellow + publishmentSystemInfo.Additional.GovInteractApplyRedAlertDate;

            if (ts.Days >= red)
            {
                return EGovInteractLimitType.Red;
            }
            else if (ts.Days >= yellow)
            {
                return EGovInteractLimitType.Yellow;
            }
            else if (ts.Days >= alert)
            {
                return EGovInteractLimitType.Alert;
            }
           
            return EGovInteractLimitType.Normal;
        }
	}
}
