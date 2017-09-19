<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageIntegrationPay" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>

    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <div class="container" runat="server">

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title"><b>支付设置</b></h4>
            <p class="text-muted font-13 m-b-25">
              如已有渠道参数可直接进行参数填写，如尚未获得参数可交由我们代为申请。
            </p>

            <table class="table table-hover m-0">
              <thead>
                <tr style="cursor: pointer" onclick="location.href='pageIntegrationPayConfiguration.aspx'">
                  <th>支付渠道</th>
                  <th>应用场景</th>
                  <th>状态</th>
                </tr>
              </thead>
              <tbody>
                <tr style="cursor: pointer" onclick="location.href='pageIntegrationPayAlipayPc.aspx'">
                  <td>
                    <div><img src="../images/channel_alipay.gif">支付宝电脑网站支付</div>
                  </td>
                  <td>
                    <div class="m-t-15">PC 端网页</div>
                  </td>
                  <td>
                    <div class="m-t-15">
                      <asp:Literal id="LtlAlipayPc" runat="server" />
                    </div>
                  </td>
                </tr>
                <tr style="cursor: pointer" onclick="location.href='pageIntegrationPayConfiguration.aspx'">
                  <td>
                    <div><img src="../images/channel_alipay.gif">支付宝手机网站支付</div>
                  </td>
                  <td>
                    <div class="m-t-15">移动网页</div>
                  </td>
                  <td>
                    <div class="m-t-15">
                      <asp:Literal id="LtlAlipayMobi" runat="server" />
                    </div>
                  </td>
                </tr>
              </tbody>
              <tbody>
                <tr style="cursor: pointer" onclick="location.href='pageIntegrationPayWeixin.aspx'">
                  <td>
                    <div><img src="../images/channel_weixin.gif">微信公众号支付</div>
                  </td>
                  <td>
                    <div class="m-t-15">PC 端网页/移动网页</div>
                  </td>
                  <td>
                    <div class="m-t-15">
                      <asp:Literal id="LtlWeixin" runat="server" />
                    </div>
                  </td>
                </tr>
              </tbody>
              <tbody>
                <tr style="cursor: pointer" onclick="location.href='pageIntegrationPayConfiguration.aspx'">
                  <td>
                    <div><img src="../images/channel_unionpay.gif">银联网关支付</div>
                  </td>
                  <td>
                    <div class="m-t-15">PC 端网页</div>
                  </td>
                  <td>
                    <div class="m-t-15">
                      <asp:Literal id="LtlUnionpayPc" runat="server" />
                    </div>
                  </td>
                </tr>
                <tr style="cursor: pointer" onclick="location.href='pageIntegrationPayConfiguration.aspx'">
                  <td>
                    <div><img src="../images/channel_unionpay.gif">银联手机支付</div>
                  </td>
                  <td>
                    <div class="m-t-15">移动网页</div>
                  </td>
                  <td>
                    <div class="m-t-15">
                      <asp:Literal id="LtlUnionpayMobi" runat="server" />
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>


          </div>
        </div>

      </div>
      <asp:Literal id="LtlScript" runat="server" />
    </body>

    </html>