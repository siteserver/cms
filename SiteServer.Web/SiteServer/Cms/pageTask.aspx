<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTask" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">

        <div class="card-box">
          <ul class="nav nav-pills">
            <asp:Literal id="LtlNavItems" runat="server" />
          </ul>
        </div>

        <ctrl:alerts text="启用定时任务需要在服务器中安装SiteServer Service服务组件" runat="server" />

        <div class="card-box">
          <div class="panel panel-default m-t-10">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table class="table tablesaw table-hover m-0">
                  <thead>
                    <tr>
                      <th width="160">所属站点 </th>
                      <th>任务名称 </th>
                      <th width="90" class="text-center">执行周期</th>
                      <th width="160" class="text-center">最近一次执行时间</th>
                      <th width="100" class="text-center">是否启用</th>
                      <th width="80" class="text-center"></th>
                      <th width="80" class="text-center"></th>
                      <th width="80" class="text-center"></th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlSite" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlTaskName" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlFrequencyType" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlLastExecuteDate" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlIsEnabled" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlEditHtml" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlEnabledHtml" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlDeleteHtml" runat="server"></asp:Literal>
                          </td>
                        </tr>
                      </itemtemplate>
                    </asp:Repeater>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <hr />

          <asp:Button class="btn btn-primary" id="BtnAdd" Text="添加任务" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->