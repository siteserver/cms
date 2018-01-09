<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageServiceTask" %>
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
            <li class="nav-item">
              <a class="nav-link" href="pageServiceStatus.aspx">服务组件状态</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageServiceTask.aspx">定时任务管理</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="panel panel-default">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table id="contents" class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th>所属站点</th>
                      <th>任务名称</th>
                      <th>任务类型</th>
                      <th>执行周期</th>
                      <th class="text-center">最近一次执行时间</th>
                      <th class="text-center">是否启用</th>
                      <th class="text-center"></th>
                      <th class="text-center"></th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlPublishmentSystem" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlTaskName" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlServiceType" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlFrequencyType" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlLastExecuteDate" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlIsEnabled" runat="server"></asp:Literal>
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

        </div>

      </form>
    </body>

    </html>