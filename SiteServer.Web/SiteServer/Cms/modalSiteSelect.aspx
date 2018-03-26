<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalSiteSelect" %>
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

        <div class="row">
          <asp:Literal id="LtlHtml" runat="server" />
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->