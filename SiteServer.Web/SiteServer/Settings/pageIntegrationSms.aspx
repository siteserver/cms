<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageIntegrationSms" %>
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
          <h3 class="popover-title">短信设置</h3>
          <div class="popover-content">

            <table class="table noborder table-hover">
              <tr>
                <td width="180">短信服务商：</td>
                <td>
                  <asp:DropDownList ID="DdlProviderType" AutoPostBack="true" OnSelectedIndexChanged="DdlProviderType_SelectedIndexChanged"
                    runat="server"></asp:DropDownList>
                  <asp:Literal ID="LtlType" runat="server" />
                </td>
              </tr>
              <asp:PlaceHolder id="PhSettings" runat="server">
                <tr>
                  <td>App Key：</td>
                  <td>
                    <asp:TextBox ID="TbAppKey" width="320" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TbAppKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAppKey" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
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