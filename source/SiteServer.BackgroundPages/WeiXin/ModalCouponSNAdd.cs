using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.IO;
using SiteServer.CMS.WeiXin.Data;
using System.Collections.Generic;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalCouponSnAdd : BasePageCms
    {
        public TextBox TbTotalNum;
        public HtmlInputFile HifUpload;

        private int _couponId;
        private int _flg;

        public static string GetOpenWindowString(int publishmentSystemId, int couponId, int flg)
        {
            return PageUtils.GetOpenWindowString("新增优惠劵数量",
                PageUtils.GetWeiXinUrl(nameof(ModalCouponSnAdd), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"couponId", couponId.ToString()},
                    {"flg", flg.ToString()}
                }), 400, 300);
        }

        public static string GetOpenUploadWindowString(int publishmentSystemId, int couponId, int flg)
        {
            return PageUtils.GetOpenWindowString("上传优惠劵",
                PageUtils.GetWeiXinUrl(nameof(ModalCouponSnAdd), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"couponId", couponId.ToString()},
                    {"flg", flg.ToString()}
                }), 400, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _couponId = Body.GetQueryInt("couponID");
            _flg = Body.GetQueryInt("flg");

            if (!IsPostBack)
            {
                if (_flg == 0)
                {
                    TbTotalNum.ReadOnly = false;
                   
                }
                if (_flg == 1)
                {
                    TbTotalNum.ReadOnly = true;
                   
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                if (_flg == 0)
                {
                    var totalNum = TranslateUtils.ToInt(TbTotalNum.Text);
                    if (totalNum > 1000)
                    {
                        FailMessage("新增失败，一次最多只能新增1000张优惠劵");
                    }
                    else
                    {
                        DataProviderWx.CouponSnDao.Insert(PublishmentSystemId, _couponId, totalNum);

                        Body.AddSiteLog(PublishmentSystemId, "新增优惠劵数量", $"数量:{totalNum}");
                        isChanged = true;
                    }
                }
                if (_flg == 1)
                {
                    //string filehou = this.hifUpload.PostedFile.FileName.Split('.')[1].ToString();

                    //if (filehou != "xls" || filehou != "xlsx")
                    //{
                    //    base.FailMessage("请检查上传文件的类型.必须为EXCEL文件.");
                    //}

                    var filePath = PathUtils.GetTemporaryFilesPath("coupon_sn_upload.csv");
                    FileUtils.DeleteFileIfExists(filePath);

                    HifUpload.PostedFile.SaveAs(filePath);

                    try
                    {
                        List<List<string>> snList;
                        List<string> head;
                        CsvUtils.Import(filePath, out head, out snList);

                        if (snList.Count > 0)
                        {
                            //DataProviderWx.CouponSnDao.Insert(PublishmentSystemId, _couponId, snList);
                            Body.AddSiteLog(PublishmentSystemId, "新增优惠劵数量", $"数量:{snList.Count}");
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
                PageUtils.CloseModalPage(Page);
            }
        }
    }
}
