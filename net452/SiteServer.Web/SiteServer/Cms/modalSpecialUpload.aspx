<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalSpecialUpload" Trace="false"%>
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

        <div class="form-group">
          <label class="col-md-3 control-label">专题名称</label>
          <div class="col-md-6 help-block">
            <asp:Literal id="LtlTitle" runat="server" />
          </div>
          <div class="col-md-3">
          </div>
        </div>

        <div class="form-group">
          <label class="col-md-3 control-label">专题文件（zip压缩包）</label>
          <div class="col-md-6">
            <input type="file" id="HifUpload" class="form-control" runat="server" />
          </div>
          <div class="col-md-3">
            <span class="help-block"></span>
          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->