<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateLog" %>
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
            修订历史
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="panel panel-default m-t-10">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th class="text-center" width="60">序号</th>
                      <th>修订人</th>
                      <th>修订时间</th>
                      <th>字符数</th>
                      <th class="text-center" width="80"></th>
                      <th class="text-center" width="30">
                        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td class="text-center">
                            <asp:Literal ID="ltlIndex" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlAddUserName" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlContentLength" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlView" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <input type="checkbox" name="IDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
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

          <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->