<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateReference" enableviewstate="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            STL语言参考
          </div>
          <p class="text-muted font-13 m-b-25">
            STL语言为SiteServer模板语言(SiteServer Template Language)的缩写，是一种和HTML语言类似的服务器端语言。
            <a href="http://stl.siteserver.cn" target="_blank">STL 语言参考手册</a>
          </p>

          <ul class="nav nav-tabs tabs-bordered">
            <asp:Literal ID="LtlElements" runat="server"></asp:Literal>
          </ul>
          <div id="tab-content" class="tab-content">
            <asp:Literal ID="LtlAttributes" runat="server"></asp:Literal>
          </div>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->
    <script>
      $(document).ready(function () {
        $(".nav-link").click(function () {
          $('.nav-item').removeClass('active');
          var attr = $(this).data("attr");
          $(this).parent().addClass('active');
          $('.tab-pane').hide();
          $('#' + attr).show();
        });

        $(".enum-link").click(function () {
          if ($(this).data('show')) {
            $(this).removeClass('ion-ios-arrow-up');
            $(this).addClass('ion-ios-arrow-down');
            $(this).data('show', '');
          } else {
            $(this).removeClass('ion-ios-arrow-down');
            $(this).addClass('ion-ios-arrow-up');
            $(this).data('show', 'true');
          }
          $(this).parent().parent().siblings('div').toggle();
        });
      });
    </script>