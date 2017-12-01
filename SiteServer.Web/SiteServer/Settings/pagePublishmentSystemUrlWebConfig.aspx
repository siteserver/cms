<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PagePublishmentSystemUrlWebConfig" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form class="container" runat="server">
        <bairong:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title"><b>Web 访问地址设置</b></h4>
            <p class="text-muted font-13 m-b-25">
              在此设置Web的部署方式
            </p>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">站点名称</label>
                <div class="col-sm-9 help-block">
                  <asp:Literal id="LtlPublishmentSystemName" runat="server"></asp:Literal>
                </div>
              </div>

              <div class="form-group">
                <label class="col-sm-3 control-label">Web部署方式</label>
                <div class="col-sm-3">
                  <asp:DropDownList id="DdlIsSeparatedWeb" AutoPostBack="true" OnSelectedIndexChanged="DdlIsSeparatedWeb_SelectedIndexChanged"
                    class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6">
                  <span class="help-block">设置网站页面部署方式</span>
                </div>
              </div>

              <asp:PlaceHolder ID="PhSeparatedWeb" runat="server">
                <div class="form-group">
                  <label class="col-sm-3 control-label">独立部署Web访问地址</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbSeparatedWebUrl" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block"></span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbSeparatedWebUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSeparatedWebUrl" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
              </asp:PlaceHolder>

              <hr />

              <div class="form-group m-b-0">
                <div class="col-sm-offset-3 col-sm-9">
                  <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                  <input type="button" value="返 回" class="btn btn-default m-l-10" onclick="location.href = 'pagePublishmentSystemUrlWeb.aspx'">
                </div>
              </div>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>