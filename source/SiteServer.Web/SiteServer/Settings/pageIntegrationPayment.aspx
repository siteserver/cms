<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageIntegrationPayment" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/header.aspx"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->
      <form class="form-inline" runat="server">
        <asp:Literal ID="LtlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <div class="popover popover-static">
          <h3 class="popover-title">支付设置</h3>
          <div class="popover-content">

            <table class="table noborder table-hover">
              <tr>
                <td width="180">聚合支付服务商：</td>
                <td>
                  <asp:DropDownList ID="DdlProviderType" AutoPostBack="true" OnSelectedIndexChanged="DdlProviderType_SelectedIndexChanged"
                    runat="server"></asp:DropDownList>
                  <asp:Literal ID="LtlType" runat="server" />
                </td>
              </tr>
              <asp:PlaceHolder id="PhPingxx" runat="server">
                <tr>
                  <td>应用 ID：</td>
                  <td>
                    <asp:TextBox ID="TbPingxxAppId" width="320" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TbPingxxAppId" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPingxxAppId" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                      Ping++ 系统中标识你的应用标识
                  </td>
                </tr>
                <tr>
                  <td>Secret Key：</td>
                  <td>
                    <asp:TextBox ID="TbPingxxSecretKey" width="320" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TbPingxxSecretKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPingxxSecretKey" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                      Server 端交易密钥
                  </td>
                </tr>
                <tr>
                  <td>已开通支付渠道：</td>
                  <td>
                    <asp:CheckBoxList ID="CblPingxxChannels" runat="server" />
                  </td>
                </tr>
              </asp:PlaceHolder>

            </table>

            <hr />
            <table class="table noborder">
              <tr>
                <td class="center">
                  <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                </td>
              </tr>
            </table>
          </div>
        </div>
      </form>
    </body>

    </html>