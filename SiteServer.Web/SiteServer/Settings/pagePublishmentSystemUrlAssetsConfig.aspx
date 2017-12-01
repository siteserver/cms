<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PagePublishmentSystemUrlAssetsConfig" %>
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
            <h4 class="m-t-0 header-title">
              <b>资源文件访问地址设置</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此设置资源文件的部署方式
            </p>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">站点名称</label>
                <div class="col-sm-9 help-block">
                  <asp:Literal id="LtlPublishmentSystemName" runat="server"></asp:Literal>
                </div>
              </div>

              <div class="form-group">
                <label class="col-sm-3 control-label">资源文件部署方式</label>
                <div class="col-sm-3">
                  <asp:DropDownList id="DdlIsSeparatedAssets" AutoPostBack="true" OnSelectedIndexChanged="DdlIsSeparatedAssets_SelectedIndexChanged"
                    class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6">
                  <span class="help-block">资源文件包含图片、视频、附件等除了HTML页面之外的所有文件</span>
                </div>
              </div>

              <asp:PlaceHolder ID="PhSeparatedAssets" runat="server">
                <div class="form-group">
                  <label class="col-sm-3 control-label">资源文件存储文件夹</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbAssetsDir" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block"></span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAssetsDir" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">独立部署资源文件访问地址</label>
                  <div class="col-sm-3">
                    <asp:TextBox id="TbSeparatedAssetsUrl" class="form-control" runat="server"></asp:TextBox>
                  </div>
                  <div class="col-sm-5">
                    <span class="help-block"></span>
                  </div>
                  <div class="col-sm-1">
                    <asp:RequiredFieldValidator ControlToValidate="TbSeparatedAssetsUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic"
                      runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSeparatedAssetsUrl" ValidationExpression="[^']+" ErrorMessage=" *"
                      ForeColor="red" Display="Dynamic" />
                  </div>
                </div>
              </asp:PlaceHolder>

              <hr />

              <div class="form-group m-b-0">
                <div class="col-sm-offset-3 col-sm-9">
                  <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                  <input type="button" value="返 回" class="btn btn-default m-l-10" onclick="location.href = 'pagePublishmentSystemAssetsUrl.aspx'">
                </div>
              </div>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>