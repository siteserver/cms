<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Settings.ModalManualUpdateSystem" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form enctype="multipart/form-data" method="post" runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group m-l-15 m-r-15">
          <label class="col-form-label">
            升级方式
          </label>
          <asp:RadioButtonList ID="RblInstallType" class="radio radio-primary" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="RblInstallType_SelectedIndexChanged"
            runat="server"></asp:RadioButtonList>
          <small class="form-text text-muted">手动升级 SiteServer CMS 可以下载升级包并手动上传，也可以设置指定的版本号通过系统自动下载并升级</small>
        </div>

        <asp:PlaceHolder id="PhUpload" runat="server">
          <div class="form-group m-l-15 m-r-15">
            <label class="col-form-label">
              上传 SiteServer CMS 升级包
              <asp:RequiredFieldValidator ControlToValidate="HifFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
            </label>
            <input type="file" id="HifFile" class="form-control" runat="server" />
            <small class="form-text text-muted">SiteServer CMS 升级包为 SS.CMS.&lt;version&gt;.nupkg的文件</small>
          </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder id="PhVersion" visible="false" runat="server">
          <div class="form-group m-l-15 m-r-15">
            <label class="col-form-label">
              指定需要升级的SiteServer CMS 版本号
              <asp:RequiredFieldValidator ControlToValidate="TbVersion" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
            </label>
            <asp:TextBox id="TbVersion" class="form-control" runat="server" />
            <small class="form-text text-muted">用以确定需要升级到具体的 SiteServer CMS 版本</small>
          </div>
        </asp:PlaceHolder>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" ID="BtnCheck" Text="确 定" OnClick="Submit_OnClick" runat="server" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->