<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteUrlAssets" enableViewState = "false" %>
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
              <a class="nav-link" href="pageSite.aspx">系统站点管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteUrlWeb.aspx">Web访问地址</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageSiteUrlAssets.aspx">资源文件访问地址</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteUrlApi.aspx">API访问地址</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteAuxiliaryTable.aspx">内容表管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteKeyword.aspx">敏感词管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteTemplate.aspx">站点模板管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteTemplateOnline.aspx">在线站点模板</a>
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
                      <th>站点名称</th>
                      <th>文件夹</th>
                      <th>资源文件存储文件夹</th>
                      <th>资源文件访问地址</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <ItemTemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlName" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlDir" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlAssetsDir" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlAssetsUrl" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
                          </td>
                        </tr>
                      </ItemTemplate>
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
    <!--#include file="../inc/foot.html"-->