using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalSpecialAdd : BasePageCms
    {
        public TextBox TbTitle;
        public TextBox TbUrl;
        public PlaceHolder PhUpload;
        public HtmlInputFile HifUpload;

        private SpecialInfo _specialInfo;

        public static string GetOpenWindowString(int siteId)
        {
            return LayerUtils.GetOpenScript("添加专题", PageUtils.GetCmsUrl(siteId, nameof(ModalSpecialAdd), null), 500, 400);
        }

        public static string GetOpenWindowString(int siteId, int specialId)
        {
            return LayerUtils.GetOpenScript("编辑专题", PageUtils.GetCmsUrl(siteId, nameof(ModalSpecialAdd), new NameValueCollection
            {
                {"specialId", specialId.ToString()}
            }), 500, 400);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            var specialId = AuthRequest.GetQueryInt("specialId");
            if (specialId > 0)
            {
                _specialInfo = SpecialManager.GetSpecialInfo(SiteId, specialId);
            }

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Template);

            if (_specialInfo != null)
            {
                TbTitle.Text = _specialInfo.Title;
                TbUrl.Text = _specialInfo.Url;
                PhUpload.Visible = false;
            }
            else
            {
                TbUrl.Text = $"@/special/{DateTime.Now:yyyy/MM/dd}/";
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var title = TbTitle.Text;
            var url = PathUtility.AddVirtualToPath(TbUrl.Text);

            if (_specialInfo != null)
            {
                var oldDirectoryPath = string.Empty;
                var newDirectoryPath = string.Empty;

                if (_specialInfo.Title != title && DataProvider.SpecialDao.IsTitleExists(SiteId, title))
                {
                    FailMessage("专题修改失败，专题名称已存在！");
                    return;
                }
                if (_specialInfo.Url != url)
                {
                    if (DataProvider.SpecialDao.IsUrlExists(SiteId, url))
                    {
                        FailMessage("专题修改失败，专题访问地址已存在！");
                        return;
                    }

                    oldDirectoryPath = SpecialManager.GetSpecialDirectoryPath(SiteInfo, _specialInfo.Url);
                    newDirectoryPath = SpecialManager.GetSpecialDirectoryPath(SiteInfo, url);
                }

                _specialInfo.Title = title;
                _specialInfo.Url = url;
                DataProvider.SpecialDao.Update(_specialInfo);

                if (oldDirectoryPath != newDirectoryPath)
                {
                    DirectoryUtils.MoveDirectory(oldDirectoryPath, newDirectoryPath, true);
                }
            }
            else
            {
                if (HifUpload.PostedFile == null || "" == HifUpload.PostedFile.FileName)
                {
                    FailMessage("专题添加失败，请选择ZIP文件上传！");
                    return;
                }

                var filePath = HifUpload.PostedFile.FileName;
                if (!StringUtils.EqualsIgnoreCase(Path.GetExtension(filePath), ".zip"))
                {
                    FailMessage("专题添加失败，必须上传ZIP文件！");
                    return;
                }

                if (DataProvider.SpecialDao.IsTitleExists(SiteId, title))
                {
                    FailMessage("专题添加失败，专题名称已存在！");
                    return;
                }
                if (DataProvider.SpecialDao.IsUrlExists(SiteId, url))
                {
                    FailMessage("专题添加失败，专题访问地址已存在！");
                    return;
                }

                var directoryPath = SpecialManager.GetSpecialDirectoryPath(SiteInfo, url);
                DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

                var zipFilePath = SpecialManager.GetSpecialZipFilePath(directoryPath);

                HifUpload.PostedFile.SaveAs(zipFilePath);
                var srcDirectoryPath = SpecialManager.GetSpecialSrcDirectoryPath(directoryPath);
                ZipUtils.ExtractZip(zipFilePath, srcDirectoryPath);

                DirectoryUtils.Copy(srcDirectoryPath, directoryPath, true);
                //var htmlFiles = Directory.GetFiles(srcDirectoryPath, "*.html", SearchOption.AllDirectories);
                //foreach (var htmlFile in htmlFiles)
                //{
                //    CreateManager.CreateFile();
                //}

                var specialInfo = new SpecialInfo
                {
                    Title = title,
                    Url = url,
                    SiteId = SiteId,
                    AddDate = DateTime.Now
                };

                DataProvider.SpecialDao.Insert(specialInfo);
            }

            LayerUtils.Close(Page);
        }
    }
}