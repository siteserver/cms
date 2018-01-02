<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PagePublishmentSystemUrlApi" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="container" runat="server">
        <bairong:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>API访问地址</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此设置API的访问地址
            </p>

            <ul class="nav nav-pills m-b-30">
              <li>
                <a href="pagePublishmentSystemUrlWeb.aspx">Web访问地址</a>
              </li>
              <li>
                <a href="pagePublishmentSystemUrlAssets.aspx">资源文件访问地址</a>
              </li>
              <li class="active">
                <a href="javascript:;">API访问地址</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">API部署方式</label>
                <div class="col-sm-3">
                  <asp:DropDownList id="DdlIsSeparatedApi" AutoPostBack="true" OnSelectedIndexChanged="DdlIsSeparatedApi_SelectedIndexChanged"
                    class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6">
                  <span class="help-block">设置API服务器部署方式</span>
                </div>
              </div>

              <asp:PlaceHolder ID="PhSeparatedApi" runat="server">
                <div class="form-group">
                  <label class="col-sm-3 control-label">独立部署API访问地址</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbSeparatedApiUrl" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block"></span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbSeparatedApiUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSeparatedApiUrl" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
              </asp:PlaceHolder>

              <hr />

              <div class="form-group m-b-0">
                <div class="col-sm-offset-3 col-sm-9">
                  <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                </div>
              </div>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>