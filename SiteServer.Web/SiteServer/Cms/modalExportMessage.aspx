<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalExportMessage" %>
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

        <div class="form-horizontal">

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <button type="button" class="btn btn-default" onclick="window.parent.layer.closeAll()">关 闭</button>
            </div>
            <div class="col-xs-1"></div>
          </div>
        </div>

      </form>
    </body>

    </html>