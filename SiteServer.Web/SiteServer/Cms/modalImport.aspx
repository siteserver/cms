<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalImport" %>
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

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">需要导入的文件</label>
            <div class="col-xs-8">
              <input type=file id="HifMyFile" class="form-control" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:RequiredFieldValidator ControlToValidate="HifMyFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">是否覆盖同名规则</label>
            <div class="col-xs-8">
              <asp:DropDownList ID="DdlIsOverride" runat="server" class="form-control">
                <asp:ListItem Text="覆盖" Value="True" Selected="true"></asp:ListItem>
                <asp:ListItem Text="不覆盖" Value="False"></asp:ListItem>
              </asp:DropDownList>
            </div>
            <div class="col-xs-1"></div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" ID="BtnCheck" Text="确 定" OnClick="Submit_OnClick" runat="server" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>