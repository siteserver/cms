<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageCommentCheck" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
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
            评论审核
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="panel panel-default m-t-20">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table id="contents" class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th>评论 </th>
                      <th>所属内容 </th>
                      <th width="120" class="text-center">评论时间</th>
                      <th width="50" class="text-center">
                        <input type="checkbox" onClick="selectRows(document.getElementById('contents'), this.checked);">
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlComment" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlContent" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlItemSelect" runat="server"></asp:Literal>
                          </td>
                        </tr>
                      </itemtemplate>
                    </asp:Repeater>
                  </tbody>
                </table>

              </div>
            </div>
          </div>

          <ctrl:sqlPager id="SpContents" runat="server" class="table table-pager" />

          <hr />

          <asp:Button class="btn btn-success" id="BtnCheck" Text="审 核" runat="server" />
          <asp:Button class="btn m-l-5" id="BtnDelete" Text="删 除" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->