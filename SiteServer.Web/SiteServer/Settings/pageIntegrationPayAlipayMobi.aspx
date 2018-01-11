<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageIntegrationPayAlipayMobi" %>
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
            支付宝手机网站支付集成
          </div>
          <p class="text-muted font-13 m-b-25">
            在此设置支付宝手机网站支付
          </p>

          <div class="form-group">
            <label class="col-form-label">是否启用支付宝手机网站支付</label>
            <asp:DropDownList id="DdlIsEnabled" AutoPostBack="true" OnSelectedIndexChanged="DdlIsEnabled_SelectedIndexChanged" class="form-control"
              runat="server"></asp:DropDownList>
          </div>

          <asp:PlaceHolder id="PhSettings" runat="server">

            <div class="form-group">
              <label class="col-form-label">接口类型</label>
              <asp:DropDownList id="DdlIsMApi" AutoPostBack="true" OnSelectedIndexChanged="DdlIsMApi_SelectedIndexChanged" class="form-control"
                runat="server"></asp:DropDownList>
            </div>

            <asp:PlaceHolder id="PhMApi" runat="server">

              <div class="form-group">
                <label class="col-form-label">合作伙伴身份（PID）
                  <asp:RequiredFieldValidator ControlToValidate="TbPid" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                  />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPid" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red"
                    Display="Dynamic" />
                </label>
                <asp:TextBox id="TbPid" class="form-control" runat="server"></asp:TextBox>
                <small class="form-text text-muted">
                  <a href="https://open.alipay.com" target="_blank">支付宝开放平台</a> - 密钥管理 - mapi网关产品密钥 - 合作伙伴身份（PID）</small>
              </div>

              <div class="form-group">
                <label class="col-form-label">MD5 密钥
                  <asp:RequiredFieldValidator ControlToValidate="TbMd5" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                  />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbMd5" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red"
                    Display="Dynamic" />
                </label>
                <asp:TextBox id="TbMd5" class="form-control" runat="server"></asp:TextBox>
                <small class="form-text text-muted">
                  <a href="https://open.alipay.com" target="_blank">支付宝开放平台</a> - 密钥管理 - mapi网关产品密钥 - MD5密钥</small>
              </div>

            </asp:PlaceHolder>
            <asp:PlaceHolder id="PhOpenApi" runat="server">

              <div class="form-group">
                <label class="col-form-label">APPID
                  <asp:RequiredFieldValidator ControlToValidate="TbAppId" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                  />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAppId" ValidationExpression="[^']+" ErrorMessage=" *"
                    ForeColor="red" Display="Dynamic" />
                </label>
                <asp:TextBox id="TbAppId" class="form-control" runat="server"></asp:TextBox>
                <small class="form-text text-muted">
                  <a href="https://open.alipay.com" target="_blank">支付宝开放平台</a> - 应用 - APPID
                </small>
              </div>

              <div class="form-group">
                <label class="col-form-label">支付宝公钥
                  <asp:RequiredFieldValidator ControlToValidate="TbPublicKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                  />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPublicKey" ValidationExpression="[^']+" ErrorMessage=" *"
                    ForeColor="red" Display="Dynamic" />
                </label>
                <asp:TextBox id="TbPublicKey" TextMode="MultiLine" Rows="6" class="form-control" runat="server"></asp:TextBox>
                <small class="form-text text-muted">
                  <a href="https://open.alipay.com" target="_blank">支付宝开放平台</a> - 应用 - RSA2(SHA256)密钥 - 查看支付宝公钥
                </small>
              </div>

              <div class="form-group">
                <label class="col-form-label">应用私钥
                  <asp:RequiredFieldValidator ControlToValidate="TbPrivateKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                  />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPrivateKey" ValidationExpression="[^']+" ErrorMessage=" *"
                    ForeColor="red" Display="Dynamic" />
                </label>
                <asp:TextBox id="TbPrivateKey" TextMode="MultiLine" Rows="15" class="form-control" runat="server"></asp:TextBox>
                <small class="form-text text-muted">
                  与“
                  <a href="https://open.alipay.com" target="_blank">支付宝开放平台</a> - 应用 - RSA2(SHA256)密钥” 中设置的应用公钥对应的应用私钥，非 PKCS8 编码
                </small>
              </div>

            </asp:PlaceHolder>

          </asp:PlaceHolder>

          <hr />

          <div class="text-center">
            <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
            <input type="button" value="返 回" class="btn m-l-10" onclick="location.href = 'pageIntegrationPay.aspx'" />
          </div>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->