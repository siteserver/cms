<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageIntegrationPayJdpay" %>
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
                <h4 class="m-t-0 header-title">
                  <b>京东支付设置</b>
                </h4>
                <p class="text-muted font-13 m-b-30">
                  在此设置京东支付配置
                </p>
              </div>
            </div>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">是否启用京东支付</label>
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
                  <label class="col-sm-3 control-label">商户号</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbMerchant" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">
                      <a href="https://biz.jd.com" target="_blank">京东金融</a> - 商户号</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbMerchant" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbMerchant" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red"
                      Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">用户名</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbUserId" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">
                      <a href="https://biz.jd.com" target="_blank">京东金融</a> - 安全中心 - 账户设置</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbUserId" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbUserId" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red"
                      Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">MD5 密钥</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbMd5Key" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">
                      <a href="https://biz.jd.com" target="_blank">京东金融</a> - 安全中心 - 密钥设置 - 京东支付密钥设置 - MD5 密钥</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbMd5Key" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbMd5Key" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red"
                      Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">DES密钥</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbDesKey" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block">
                      <a href="https://biz.jd.com" target="_blank">京东金融</a> - 安全中心 - 密钥设置 - 京东支付密钥设置 - DES密钥</span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbDesKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDesKey" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red"
                      Display="Dynamic" />
                  </div>
                </div>

                <div class="form-group">
                  <label class="col-sm-3 control-label">商户RSA公钥</label>
                  <div class="col-sm-8">
                    <asp:TextBox id="TbPublicKey" TextMode="MultiLine" Rows="6" class="form-control" runat="server"></asp:TextBox>
                    <span class="help-block">
                      <a href="https://biz.jd.com" target="_blank">京东金融</a> - 安全中心 - 密钥设置 - 京东支付密钥设置 - 商户RSA公钥</span>
                    </span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbPublicKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPublicKey" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">商户RSA私钥</label>
                  <div class="col-sm-8">
                    <asp:TextBox id="TbPrivateKey" TextMode="MultiLine" Rows="15" class="form-control" runat="server"></asp:TextBox>
                    <span class="help-block">
                      <a href="https://biz.jd.com" target="_blank">京东金融</a> - 安全中心 - 密钥设置 - 京东支付密钥设置 - 商户RSA私钥</span>
                    </span>
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