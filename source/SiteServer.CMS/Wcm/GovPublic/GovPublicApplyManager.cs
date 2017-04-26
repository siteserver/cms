using System;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.GovPublic
{
	public class GovPublicApplyManager
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

        public static string GetApplyRemark(int applyID)
        {
            var remarkBuilder = new StringBuilder();
            var remarkInfoArrayList = DataProvider.GovPublicApplyRemarkDao.GetRemarkInfoArrayList(applyID);
            foreach (GovPublicApplyRemarkInfo remarkInfo in remarkInfoArrayList)
            {
                if (!string.IsNullOrEmpty(remarkInfo.Remark))
                {
                    remarkBuilder.Append(
                        $@"<span style=""color:gray;"">{DepartmentManager.GetDepartmentName(remarkInfo.DepartmentID)}({remarkInfo
                            .UserName}){EGovPublicApplyRemarkTypeUtils.GetText(remarkInfo.RemarkType)}意见: </span><br />{StringUtils
                            .MaxLengthText(remarkInfo.Remark, 25)}<br />");
                }
            }
            if (remarkBuilder.Length > 0) remarkBuilder.Length -= 6;
            return remarkBuilder.ToString();
        }

        public static void LogNew(int publishmentSystemID, int applyID, string fromName, string toDepartmentName)
        {
            var logInfo = new GovPublicApplyLogInfo(0, publishmentSystemID, applyID, 0, string.Empty, EGovPublicApplyLogType.New, PageUtils.GetIpAddress(), DateTime.Now,
                $"前台{fromName}提交申请{toDepartmentName}");
            DataProvider.GovPublicApplyLogDao.Insert(logInfo);
        }

        public static void LogSwitchTo(int publishmentSystemID, int applyID, string switchToDepartmentName, string administratorName, int departmentId)
        {
            var logInfo = new GovPublicApplyLogInfo(0, publishmentSystemID, applyID, departmentId, administratorName, EGovPublicApplyLogType.SwitchTo, PageUtils.GetIpAddress(), DateTime.Now,
                $"{DepartmentManager.GetDepartmentName(departmentId)}({administratorName})转办申请至{switchToDepartmentName} ");
            DataProvider.GovPublicApplyLogDao.Insert(logInfo);
        }

        public static void Log(int publishmentSystemID, int applyID, EGovPublicApplyLogType logType, string administratorName, int departmentId)
        {
            var logInfo = new GovPublicApplyLogInfo(0, publishmentSystemID, applyID, departmentId, administratorName, logType, PageUtils.GetIpAddress(), DateTime.Now, string.Empty);

            var departmentName = DepartmentManager.GetDepartmentName(departmentId);

            if (logType == EGovPublicApplyLogType.Accept)
            {
                logInfo.Summary = $"{departmentName}({administratorName})受理申请";
            }
            else if (logType == EGovPublicApplyLogType.Deny)
            {
                logInfo.Summary = $"{departmentName}({administratorName})拒绝受理申请";
            }
            else if (logType == EGovPublicApplyLogType.Reply)
            {
                logInfo.Summary = $"{departmentName}({administratorName})回复申请";
            }
            else if (logType == EGovPublicApplyLogType.Comment)
            {
                logInfo.Summary = $"{departmentName}({administratorName})批示申请";
            }
            else if (logType == EGovPublicApplyLogType.Redo)
            {
                logInfo.Summary = $"{departmentName}({administratorName})要求返工";
            }
            else if (logType == EGovPublicApplyLogType.Check)
            {
                logInfo.Summary = $"{departmentName}({administratorName})审核通过";
            }
            DataProvider.GovPublicApplyLogDao.Insert(logInfo);
        }

        public static EGovPublicApplyLimitType GetLimitType(PublishmentSystemInfo publishmentSystemInfo, GovPublicApplyInfo applyInfo)
        {
            var ts = new TimeSpan(DateTime.Now.Ticks - applyInfo.AddDate.Ticks);

            var alert = publishmentSystemInfo.Additional.GovPublicApplyDateLimit + publishmentSystemInfo.Additional.GovPublicApplyAlertDate;
            var yellow = alert + publishmentSystemInfo.Additional.GovPublicApplyYellowAlertDate;
            var red = yellow + publishmentSystemInfo.Additional.GovPublicApplyRedAlertDate;

            if (ts.Days >= red)
            {
                return EGovPublicApplyLimitType.Red;
            }
            else if (ts.Days >= yellow)
            {
                return EGovPublicApplyLimitType.Yellow;
            }
            else if (ts.Days >= alert)
            {
                return EGovPublicApplyLimitType.Alert;
            }
           
            return EGovPublicApplyLimitType.Normal;
        }
	}
}
