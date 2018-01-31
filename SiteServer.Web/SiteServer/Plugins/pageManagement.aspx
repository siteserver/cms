<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageManagement" %>
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
          <div class="row">
            <div class="col-8">
              <ul class="nav nav-pills">
                <asp:Literal id="LtlNav" runat="server" />
              </ul>
            </div>
            <div class="col-4">
              <asp:Button onClick="BtnReload_Click" class="btn btn-primary float-right btn-md" Text="重新加载所有插件" runat="server" />
            </div>
          </div>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">

          <asp:PlaceHolder id="PhRunnable" runat="server">
            <div class="panel panel-default">
              <div class="panel-body p-0">
                <div class="table-responsive">
                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-nowrap">LOGO</th>
                        <th class="text-nowrap">插件Id</th>
                        <th class="text-nowrap">插件名称</th>
                        <th class="text-nowrap">版本号</th>
                        <th class="text-nowrap">作者</th>
                        <th>插件介绍</th>
                        <th class="text-center text-nowrap">载入时间</th>
                        <th class="text-nowrap"></th>
                      </tr>
                    </thead>
                    <tbody>
                      <asp:Repeater ID="RptRunnable" runat="server">
                        <itemtemplate>
                          <tr>
                            <td class="text-center align-middle text-nowrap">
                              <asp:Literal ID="ltlLogo" runat="server"></asp:Literal>
                            </td>
                            <td class="align-middle text-nowrap">
                              <asp:Literal ID="ltlPluginId" runat="server"></asp:Literal>
                            </td>
                            <td class="align-middle text-nowrap">
                              <asp:Literal ID="ltlPluginName" runat="server"></asp:Literal>
                            </td>
                            <td class="align-middle text-nowrap">
                              <asp:Literal ID="ltlVersion" runat="server"></asp:Literal>
                            </td>
                            <td class="align-middle text-nowrap">
                              <asp:Literal ID="ltlOwners" runat="server"></asp:Literal>
                            </td>
                            <td class="align-middle">
                              <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
                            </td>
                            <td class="text-center align-middle text-nowrap">
                              <asp:Literal ID="ltlInitTime" runat="server"></asp:Literal>
                            </td>
                            <td class="text-center align-middle text-nowrap">
                              <asp:Literal ID="ltlCmd" runat="server"></asp:Literal>
                            </td>
                          </tr>
                        </itemtemplate>
                      </asp:Repeater>
                    </tbody>
                  </table>

                </div>
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhNotRunnable" runat="server">
            <div class="panel panel-default">
              <div class="panel-body p-0">
                <div class="table-responsive">
                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th>插件Id</th>
                        <th>错误详情</th>
                        <th></th>
                      </tr>
                    </thead>
                    <tbody>
                      <asp:Repeater ID="RptNotRunnable" runat="server">
                        <itemtemplate>
                          <tr>
                            <td>
                              <asp:Literal ID="ltlPluginId" runat="server"></asp:Literal>
                            </td>
                            <td>
                              <asp:Literal ID="ltlErrorMessage" runat="server"></asp:Literal>
                            </td>
                            <td class="text-center">
                              <asp:Literal ID="ltlCmd" runat="server"></asp:Literal>
                            </td>
                          </tr>
                        </itemtemplate>
                      </asp:Repeater>
                    </tbody>
                  </table>

                </div>
              </div>
            </div>
          </asp:PlaceHolder>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->