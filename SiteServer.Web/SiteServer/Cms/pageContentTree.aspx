<%@ Page Language="C#" Trace="false" EnableViewState="false" Inherits="SiteServer.BackgroundPages.BasePageCms" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html style="background-color: #eeeeee;">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script type="text/javascript">
        $(document).ready(function () {
          $('body').height($(window).height());
          $('body').addClass('scroll');
        });
      </script>
    </head>

    <body style="margin: 0; padding: 0; background-color: #eeeeee;">
      <form class="m-0" runat="server">
        <div class="list-group mail-list">
          <div onclick="location.reload(true);" style="cursor: pointer;background-color: #dddddd;" class="list-group-item b-0">
            栏目列表
          </div>
        </div>
        <table class="table table-sm table-hover table-tree">
          <tbody>
            <ctrl:ChannelTree runat="server"></ctrl:ChannelTree>
          </tbody>
        </table>
      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->