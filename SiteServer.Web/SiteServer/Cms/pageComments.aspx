<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageComments" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <%@ Register TagPrefix="stl" Namespace="SiteServer.CMS.StlControls" Assembly="SiteServer.CMS" %>
      <!DOCTYPE html>
      <html>

      <head>
        <meta charset="utf-8">
        <!--#include file="../inc/head.html"-->
        <script type="text/javascript">
          $(document).ready(function () {
            loopRows(document.getElementById('contents'), function (cur) {
              cur.onclick = chkSelect;
            });
          });
        </script>
      </head>

      <body>
        <form class="m-l-15 m-r-15" runat="server">
          <ctrl:alerts runat="server" />

          <div class="card-box">
            <div class="m-t-0 header-title">
              评论管理
            </div>
            <p class="text-muted font-13 m-b-25"></p>

            <!--#include file="../inc/scripts.aspx"-->
            <ctrl:Script src="~/sitefiles/assets/components/js.cookie.js" runat="server" />
            <stl:commentInput id="StlCommentInput" IsAnonymous="true" PageNum="20" runat="server" />

            <ctrl:sqlPager id="SpContents" runat="server" class="table table-pager" />

            <hr />

            <asp:Button class="btn" id="BtnExport" runat="server" Text="导出Excel"></asp:Button>
            <asp:Button class="btn m-l-5" CausesValidation="false" OnClick="Return_OnClick" Text="返 回" runat="server" />

          </div>

        </form>
      </body>

      </html>
      <!--#include file="../inc/foot.html"-->