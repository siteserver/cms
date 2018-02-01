<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Plugins.ModalManualInstall" %>
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
            插件安装方式
          </label>
          <asp:RadioButtonList ID="RblInstallType" class="radio radio-primary" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="RblInstallType_SelectedIndexChanged"
            runat="server"></asp:RadioButtonList>
        </div>

        <asp:PlaceHolder id="PhFile" runat="server">
          <div class="form-group m-l-15 m-r-15">
            <label class="col-form-label">
              上传插件包
              <asp:RequiredFieldValidator ControlToValidate="HifFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
            </label>
            <input type="file" id="HifFile" class="form-control" runat="server" />
            <small class="form-text text-muted">插件包是后缀为.nupkg的文件</small>
          </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder id="PhIdAndVersion" visible="false" runat="server">
          <div class="form-group m-l-15 m-r-15">
            <label class="col-form-label">
              插件标识符
              <asp:RequiredFieldValidator ControlToValidate="TbPluginId" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
            </label>
            <asp:TextBox id="TbPluginId" class="form-control" runat="server" />
            <small class="form-text text-muted">插件标识符在插件库中必须是唯一的</small>
          </div>
          <div class="form-group m-l-15 m-r-15">
            <label class="col-form-label">
              插件版本号
              <asp:RequiredFieldValidator ControlToValidate="TbVersion" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
            </label>
            <asp:TextBox id="TbVersion" class="form-control" runat="server" />
            <small class="form-text text-muted">插件版本号用以确定需要安装的插件版本</small>
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