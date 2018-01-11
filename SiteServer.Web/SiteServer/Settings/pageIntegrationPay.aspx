<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageIntegrationPay" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>

    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">

        <div class="card-box">
          <ul class="nav nav-pills">
            <li class="nav-item">
              <a class="nav-link" href="pageIntegrationSms.aspx">短信集成</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageIntegrationPay.aspx">支付集成</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            支付集成
          </div>
          <p class="text-muted font-13 m-b-25">
            如已有渠道参数可直接进行参数填写，如尚未获得参数可交由我们代为申请。
          </p>

          <table class="table table-hover m-0">
            <thead>
              <tr>
                <th>支付渠道</th>
                <th>应用场景</th>
                <th>状态</th>
              </tr>
            </thead>
            <tbody>
              <tr style="cursor: pointer" onclick="location.href='pageIntegrationPayAlipayPc.aspx'">
                <td>
                  <div>
                    <img src="../images/channel_alipay.gif">支付宝电脑网站支付</div>
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
              <tr style="cursor: pointer" onclick="location.href='pageIntegrationPayAlipayMobi.aspx'">
                <td>
                  <div>
                    <img src="../images/channel_alipay.gif">支付宝手机网站支付</div>
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
                  <div>
                    <img src="../images/channel_weixin.gif">微信公众号支付</div>
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
              <tr style="cursor: pointer" onclick="location.href='pageIntegrationPayJdpay.aspx'">
                <td>
                  <div>
                    <img src="../images/channel_jdpay.gif">京东支付</div>
                </td>
                <td>
                  <div class="m-t-15">PC 端网页/移动网页</div>
                </td>
                <td>
                  <div class="m-t-15">
                    <asp:Literal id="LtlJdpay" runat="server" />
                  </div>
                </td>
              </tr>
            </tbody>
          </table>


        </div>

        <asp:Literal id="LtlScript" runat="server" />
      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->