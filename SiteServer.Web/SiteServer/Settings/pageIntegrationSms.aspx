<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageIntegrationSms" %>
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
            <li class="nav-item active">
              <a class="nav-link" href="pageIntegrationSms.aspx">短信集成</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageIntegrationPay.aspx">支付集成</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="form-group">
            <label class="col-form-label">短信服务商</label>
            <asp:DropDownList ID="DdlProviderType" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlProviderType_SelectedIndexChanged"
              runat="server"></asp:DropDownList>
            <small class="form-text text-muted">
              <asp:Literal ID="LtlType" runat="server" />
            </small>
          </div>

          <asp:PlaceHolder id="PhSettings" runat="server">
            <div class="form-group">
              <label class="col-form-label">App Key
                <asp:RequiredFieldValidator ControlToValidate="TbAppKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAppKey" ValidationExpression="[^']+" ErrorMessage=" *"
                  ForeColor="red" Display="Dynamic" />
              </label>
              <asp:TextBox ID="TbAppKey" class="form-control" runat="server" />
            </div>
          </asp:PlaceHolder>

          <hr />

          <div class="text-center">
            <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
          </div>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->