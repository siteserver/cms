<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageIntegrationPayAlipayPc" %>
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
                <h4 class="m-t-0 header-title"><b>支付宝电脑网站支付设置</b></h4>
                <p class="text-muted font-13 m-b-30">
                  在此设置支付宝电脑网站支付配置
                </p>
              </div>
            </div>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">是否启用支付宝电脑网站支付</label>
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
                  <label class="col-sm-3 control-label">支付宝账户</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbAccount" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">支付宝企业账户（邮箱）</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbAccount" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAccount" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">APPID</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbAppId" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">支付宝开放平台 - PID 和公钥管理 - 开放平台密钥 - APPID</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbAppId" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAppId" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">合作伙伴身份（PID）</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbPid" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">支付宝开放平台 - PID 和公钥管理 - mapi 网关产品密钥 - 合作伙伴身份（PID）</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbPid" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPid" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red"
                      Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">MD5 密钥</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbMd5" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">支付宝开放平台 - PID 和公钥管理 - mapi 网关产品密钥 - 合作伙伴密钥 - 安全校验码（key） - MD5 密钥</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbMd5" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbMd5" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red"
                      Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">支付宝公钥</label>
                  <div class="col-sm-8">
                    <asp:TextBox id="TbPublicKey" TextMode="MultiLine" Rows="6" class="form-control" runat="server"></asp:TextBox>
                    <span class="help-block">支付宝开放平台 - PID 和公钥管理 - mapi 网关产品密钥 - 安全校验码（key）- RSA (SHA1) 密钥 - 查看支付宝公钥</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbPublicKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPublicKey" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">应用私钥</label>
                  <div class="col-sm-8">
                    <asp:TextBox id="TbPrivateKey" TextMode="MultiLine" Rows="15" class="form-control" runat="server"></asp:TextBox>
                    <span class="help-block">非 PKCS8 编码，与“支付宝开放平台 - PID 和公钥管理 - mapi 网关产品密钥 - 安全校验码（key）- RSA (SHA1) 密钥 ”中设置的应用公钥对应的应用私钥</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbPrivateKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPrivateKey" ValidationExpression="[^']+" ErrorMessage=" *"
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