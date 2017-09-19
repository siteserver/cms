<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageIntegrationPayWeixin" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server" class="container">

        <div class="row">
          <div class="card-box">
            <div class="row">
              <div class="col-lg-10">
                <h4 class="m-t-0 header-title"><b>微信公众号支付设置</b></h4>
                <p class="text-muted font-13 m-b-30">
                  在此设置微信公众号支付配置
                </p>
              </div>
            </div>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">是否启用微信公众号支付</label>
                <div class="col-sm-3">
                  <asp:DropDownList id="DdlIsEnabled" AutoPostBack="true" OnSelectedIndexChanged="DdlIsEnabled_SelectedIndexChanged" class="form-control"
                    runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6">
                  <span class="help-block"></span>
                </div>
              </div>

              <asp:PlaceHolder id="PhSettings" runat="server">

                <div class="form-group">
                  <label class="col-sm-3 control-label">公众号 AppID</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbAppId" placeholder="微信公众号的 AppID" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">微信公众平台 - 开发 - 基本配置 - 开发者 ID - AppID</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbAppId" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAppId" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">公众号 AppSecret</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbAppSecret" placeholder="微信公众号的 AppSecret" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">微信公众平台 - 开发 - 基本配置 - 开发者 ID - AppSecret</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbAppSecret" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAppSecret" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">微信支付商户号</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbMchId" placeholder="微信支付商户号" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">在微信支付申请完成后微信支付商户平台的通知邮件中获取，请确保邮件中的公众号 AppID 与本次填写的公众号 AppID 一致</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbMchId" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbMchId" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red"
                      Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">API 密钥</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbKey" placeholder="API 密钥（32位）" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">微信支付商户平台 - 账户中心 - 账户设置 - API 安全 - API 密钥 - 设置密钥</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbKey" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red"
                      Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">API 证书</label>
                  <div class="col-sm-8">
                    <asp:TextBox id="TbClientCert" placeholder="apiclient_cert（pem 格式）" TextMode="MultiLine" Rows="6" class="form-control" runat="server"></asp:TextBox>
                    <span class="help-block">微信支付商户平台 - 账户中心 - 账户设置 - API 安全 - API 证书 - 下载证书</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbClientCert" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbClientCert" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">API 证书密钥</label>
                  <div class="col-sm-8">
                    <asp:TextBox id="TbClientKey" placeholder="apiclient_key（pem 格式）" TextMode="MultiLine" Rows="15" class="form-control" runat="server"></asp:TextBox>
                    <span class="help-block">微信支付商户平台 - 账户中心 - 账户设置 - API 安全 - API 证书 - 下载证书</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbClientKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbClientKey" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>

              </asp:PlaceHolder>

              <div class="form-group m-b-0">
                <div class="col-sm-offset-3 col-sm-9">
                  <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                  <input type="button" value="返 回" class="btn m-l-10" onclick="location.href = 'pageIntegrationPay.aspx'" />
                </div>
              </div>

            </div>
          </div>
        </div>

      </form>
    </body>

    </html>