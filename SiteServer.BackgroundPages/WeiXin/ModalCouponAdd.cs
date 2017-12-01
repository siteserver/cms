using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalCouponAdd : BasePageCms
    {
        public TextBox TbTitle;
        public TextBox TbTotalNum;
        public CheckBox CbIsEnabled;

        private int _couponId;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加优惠劵",
                PageUtils.GetWeiXinUrl(nameof(ModalCouponAdd), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()}
                }), 400, 300);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int couponId)
        {
            return PageUtils.GetOpenWindowString("编辑优惠劵",
                PageUtils.GetWeiXinUrl(nameof(ModalCouponAdd), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"couponId", couponId.ToString()}
                }), 400, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _couponId = Body.GetQueryInt("couponID");

            if (!IsPostBack)
            {
                if (_couponId > 0)
                {
                    var couponInfo = DataProviderWx.CouponDao.GetCouponInfo(_couponId);

                    TbTitle.Text = couponInfo.Title;
                    TbTotalNum.Text = couponInfo.TotalNum.ToString();
                    TbTotalNum.Enabled = false;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                if (this._couponId == 0)
                {
                    var totalNum = TranslateUtils.ToInt(TbTotalNum.Text);

                    if (totalNum > 1000)
                    {
                        FailMessage("添加失败，一次最多只能新增1000张优惠劵");
                    }
                    else
                    {
                        var couponInfo = new CouponInfo { Id = 0, PublishmentSystemId = PublishmentSystemId, ActId = 0, Title = TbTitle.Text, TotalNum = totalNum, AddDate = DateTime.Now };

                        var couponId = DataProviderWx.CouponDao.Insert(couponInfo);

                        if (CbIsEnabled.Checked == false)
                        {
                            DataProviderWx.CouponSnDao.Insert(PublishmentSystemId, couponId, totalNum);
                        }

                        Body.AddSiteLog(PublishmentSystemId, "添加优惠劵", $"优惠劵:{TbTitle.Text}");

                        isChanged = true;
                    }
                }
                else
                {
                    var couponInfo = DataProviderWx.CouponDao.GetCouponInfo(_couponId);
                    couponInfo.Title = TbTitle.Text;

                    DataProviderWx.CouponDao.Update(couponInfo);

                    Body.AddSiteLog(PublishmentSystemId, "编辑优惠劵", $"优惠劵:{TbTitle.Text}");

                    isChanged = true;
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
