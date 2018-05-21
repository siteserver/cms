<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteAuxiliaryTable" %>
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
            <li class="nav-item">
              <a class="nav-link" href="pageSiteUrlAssets.aspx">资源文件访问地址</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteUrlApi.aspx">API访问地址</a>
            </li>
            <li class="nav-item active">
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
                <table class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th>内容表标识</th>
                      <th>内容表名称</th>
                      <th class="text-center" width="120">是否被使用</th>
                      <th class="text-center" width="120">是否存在</th>
                      <th class="text-center" width="120">创建后修改</th>
                      <th class="text-center" width="120"></th>
                      <th width="120"></th>
                      <th width="120"></th>
                      <th width="120"></th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal id="ltlTableName" runat="server" />
                          </td>
                          <td>
                            <asp:Literal id="ltlDisplayName" runat="server" />
                          </td>
                          <td class="text-center">
                            <asp:Literal id="ltlIsUsed" runat="server" />
                          </td>
                          <td class="text-center">
                            <asp:Literal id="ltlIsCreatedInDb" runat="server" />
                          </td>
                          <td class="text-center">
                            <asp:Literal id="ltlIsChangedAfterCreatedInDb" runat="server" />
                          </td>
                          <td class="text-center">
                            <asp:Literal id="ltlMetadataEdit" runat="server" />
                          </td>
                          <td class="text-center">
                            <asp:Literal id="ltlStyleEdit" runat="server" />
                          </td>
                          <td class="text-center">
                            <asp:Literal id="ltlEdit" runat="server" />
                          </td>
                          <td class="text-center">
                            <asp:Literal id="ltlDelete" runat="server" />
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

          <asp:Button class="btn btn-primary" id="BtnAdd" Text="新增内容表" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->