<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentCheck" %>
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
            内容审核
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="m-t-10">
            <div class="form-inline">
              <div class="form-group">
                <label class="col-form-label m-r-10">栏目</label>
                <asp:DropDownList ID="DdlChannelId" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" class="form-control" runat="server"></asp:DropDownList>
              </div>

              <div class="form-group m-l-10">
                <label class="col-form-label m-r-10">状态</label>
                <asp:DropDownList ID="DdlState" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server"></asp:DropDownList>
              </div>
            </div>
          </div>

          <div class="panel panel-default m-t-20">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table id="contents" class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th>内容标题(点击查看) </th>
                      <th class="text-nowrap">栏目 </th>
                      <asp:Literal id="LtlColumnsHead" runat="server"></asp:Literal>
                      <th class="text-center text-nowrap" width="100">操作</th>
                      <th class="text-center text-nowrap" width="100">状态</th>
                      <th width="20" class="text-center text-nowrap">
                        <input type="checkbox" onClick="selectRows(document.getElementById('contents'), this.checked);">
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                          </td>
                          <td class="text-nowrap">
                            <asp:Literal ID="ltlChannel" runat="server"></asp:Literal>
                          </td>
                          <asp:Literal ID="ltlColumns" runat="server"></asp:Literal>
                          <td class="text-center text-nowrap">
                            <asp:Literal ID="ltlCommands" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center text-nowrap">
                            <asp:Literal ID="ltlStatus" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center text-nowrap">
                            <asp:Literal ID="ltlSelect" runat="server"></asp:Literal>
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