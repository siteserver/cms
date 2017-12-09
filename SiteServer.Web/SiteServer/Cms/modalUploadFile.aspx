<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalUploadFile" Trace="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">选择上传的附件</label>
            <div class="col-xs-8">
              <input type="file" id="HifUpload" class="form-control" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:RequiredFieldValidator ControlToValidate="HifUpload" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
              />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">是否重命名</label>
            <div class="col-xs-8">
              <asp:DropDownList id="DdlIsFileUploadChangeFileName" class="form-control" runat="server" />
            </div>
            <div class="col-xs-1"></div>
          </div>

          <asp:Literal id="LtlScript" runat="server"></asp:Literal>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>
        </div>

      </form>
    </body>

    </html>