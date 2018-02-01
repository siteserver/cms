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

        <div class="form-group form-row">
          <label class="col-3 col-form-label text-right">需要导入的文件</label>
          <div class="col-8">
            <input type=file id="HifMyFile" class="form-control" runat="server" />
          </div>
          <div class="col-1">
            <asp:RequiredFieldValidator ControlToValidate="HifMyFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 col-form-label text-right">是否覆盖同名规则</label>
          <div class="col-8">
            <asp:DropDownList ID="DdlIsOverride" runat="server" class="form-control">
              <asp:ListItem Text="覆盖" Value="True" Selected="true"></asp:ListItem>
              <asp:ListItem Text="不覆盖" Value="False"></asp:ListItem>
            </asp:DropDownList>
          </div>
          <div class="col-1"></div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" ID="BtnCheck" Text="确 定" OnClick="Submit_OnClick" runat="server" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->