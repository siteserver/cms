<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Settings.ModalImportZip" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">导入方式</label>
          <div class="col-8">
            <asp:DropDownList ID="DdlImportType" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="RblImportType_SelectedIndexChanged"
              runat="server"></asp:DropDownList>
          </div>
          <div class="col-1 help-block"></div>
        </div>

        <asp:PlaceHolder ID="PhUpload" runat="server">
          <div class="form-group form-row">
            <label class="col-3 text-right col-form-label">选择压缩包</label>
            <div class="col-8">
              <input type="file" id="HifFile" class="form-control" runat="server" />
            </div>
            <div class="col-1 help-block">
              <asp:RequiredFieldValidator ControlToValidate="HifFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
            </div>
          </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="PhDownload" runat="server">
          <div class="form-group form-row">
            <label class="col-3 text-right col-form-label">压缩包下载地址</label>
            <div class="col-8">
              <asp:TextBox id="TbDownloadUrl" class="form-control" runat="server" />
            </div>
            <div class="col-1 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbDownloadUrl" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
            </div>
          </div>
        </asp:PlaceHolder>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->