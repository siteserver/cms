using System;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.CMS.Core;
using System.Web.UI.HtmlControls;
using BaiRong.Core.IO;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class CouponSNAdd : BackgroundBasePage
    {
        public TextBox tbTotalNum;
        public HtmlInputFile hifUpload;

        private int couponID;
        private int flg;

        public static string GetOpenWindowString(int publishmentSystemID, int couponID, int flg)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("couponID", couponID.ToString());
            arguments.Add("flg", flg.ToString());
            return PageUtilityWX.GetOpenWindowString("新增优惠劵数量", "modal_couponSNAdd.aspx", arguments, 400, 300);
        }

        public static string GetOpenUploadWindowString(int publishmentSystemID, int couponID, int flg)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("couponID", couponID.ToString());
            arguments.Add("flg", flg.ToString());
            return PageUtilityWX.GetOpenWindowString("上传优惠劵", "modal_couponSNUpload.aspx", arguments, 400, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            couponID = TranslateUtils.ToInt(GetQueryString("couponID"));
            flg = TranslateUtils.ToInt(GetQueryString("flg"));
            if (!IsPostBack)
            {
                if (flg == 0)
                {
                    tbTotalNum.ReadOnly = false;
                   
                }
                if (flg == 1)
                {
                    tbTotalNum.ReadOnly = true;
                   
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                if (flg == 0)
                {
                    var totalNum = TranslateUtils.ToInt(tbTotalNum.Text);
                    if (totalNum > 1000)
                    {
                        FailMessage("新增失败，一次最多只能新增1000张优惠劵");
                    }
                    else
                    {
                        DataProviderWX.CouponSNDAO.Insert(PublishmentSystemID, couponID, totalNum);

                        StringUtility.AddLog(PublishmentSystemID, "新增优惠劵数量", $"数量:{totalNum}");
                        isChanged = true;
                    }
                }
                if (flg == 1)
                {
                    //string filehou = this.hifUpload.PostedFile.FileName.Split('.')[1].ToString();

                    //if (filehou != "xls" || filehou != "xlsx")
                    //{
                    //    base.FailMessage("请检查上传文件的类型.必须为EXCEL文件.");
                    //}

                    var filePath = PathUtils.GetTemporaryFilesPath("coupon_sn_upload.csv");
                    FileUtils.DeleteFileIfExists(filePath);

                    hifUpload.PostedFile.SaveAs(filePath);

                    try
                    {
                        var snList = CSVUtils.Import(filePath);

                        if (snList.Count > 0)
                        {
                            DataProviderWX.CouponSNDAO.Insert(PublishmentSystemID, couponID, snList);
                            StringUtility.AddLog(PublishmentSystemID, "新增优惠劵数量", $"数量:{snList.Count}");
                            isChanged = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "失败：" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }

            if (isChanged)
            {
                JsUtils.OpenWindow.CloseModalPage(Page);
            }
        }
    }
}
