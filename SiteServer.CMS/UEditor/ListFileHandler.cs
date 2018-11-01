using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.UEditor
{
    /// <summary>
    /// FileManager 的摘要说明
    /// </summary>
    public class ListFileManager : Handler
    {
        enum ResultState
        {
            Success,
            InvalidParam,
            AuthorizError,
            IOError,
            PathNotFound
        }

        private int Start;
        private int Size;
        private int Total;
        private ResultState State;
        private string PathToList;
        private string[] FileList;
        private string[] SearchExtensions;

        public int SiteId { get; }
        public EUploadType UploadType { get; }

        public ListFileManager(HttpContext context, string pathToList, string[] searchExtensions, int siteId, EUploadType uploadType)
            : base(context)
        {
            SearchExtensions = searchExtensions.Select(x => x.ToLower()).ToArray();
            PathToList = pathToList;
            SiteId = siteId;
            UploadType = uploadType;
        }

        public override void Process()
        {
            try
            {
                Start = string.IsNullOrEmpty(Request["start"]) ? 0 : Convert.ToInt32(Request["start"]);
                Size = string.IsNullOrEmpty(Request["size"]) ? Config.GetInt("imageManagerListSize") : Convert.ToInt32(Request["size"]);
            }
            catch (FormatException)
            {
                State = ResultState.InvalidParam;
                WriteResult();
                return;
            }
            var buildingList = new List<String>();
            try
            {
                var siteInfo = SiteManager.GetSiteInfo(SiteId);
                var sitePath = PathUtility.GetSitePath(siteInfo); // 本站点物理路径
                var applicationPath = WebConfigUtils.PhysicalApplicationPath.ToLower().Trim(' ', '/', '\\'); // 系统物理路径
                if (UploadType == EUploadType.Image)
                {
                    PathToList = siteInfo.Additional.ImageUploadDirectoryName; 
                }
                else if(UploadType == EUploadType.File)
                {
                    PathToList = siteInfo.Additional.FileUploadDirectoryName;
                }

                //var localPath = Server.MapPath(PathToList);
                var localPath = PathUtils.Combine(sitePath, PathToList);

                buildingList.AddRange(Directory.GetFiles(localPath, "*", SearchOption.AllDirectories)
                    .Where(x => SearchExtensions.Contains(Path.GetExtension(x).ToLower()))
                    .Select(x => x.Substring(applicationPath.Length).Replace("\\", "/")));
                Total = buildingList.Count;
                FileList = buildingList.OrderBy(x => x).Skip(Start).Take(Size).ToArray();
            }
            catch (UnauthorizedAccessException)
            {
                State = ResultState.AuthorizError;
            }
            catch (DirectoryNotFoundException)
            {
                State = ResultState.PathNotFound;
            }
            catch (IOException)
            {
                State = ResultState.IOError;
            }
            finally
            {
                WriteResult();
            }
        }

        private void WriteResult()
        {
            WriteJson(new
            {
                state = GetStateString(),
                list = FileList == null ? null : FileList.Select(x => new { url = x }),
                start = Start,
                size = Size,
                total = Total
            });
        }

        private string GetStateString()
        {
            switch (State)
            {
                case ResultState.Success:
                    return "SUCCESS";
                case ResultState.InvalidParam:
                    return "参数不正确";
                case ResultState.PathNotFound:
                    return "路径不存在";
                case ResultState.AuthorizError:
                    return "文件系统权限不足";
                case ResultState.IOError:
                    return "文件系统读取错误";
            }
            return "未知错误";
        }
    }
}