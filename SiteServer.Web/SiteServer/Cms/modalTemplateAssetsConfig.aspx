<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTemplateAssetsConfig" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group">
          <label class="col-form-label">文件夹相对路径</label>
          <asp:TextBox cssClass="form-control" id="TbDirectoryPath" runat="server" />
          <small class="form-text text-muted">
            文件夹相对路径指相对于站点的路径，如<code>dir</code>或者<code>assets/dir</code>
          </small>
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