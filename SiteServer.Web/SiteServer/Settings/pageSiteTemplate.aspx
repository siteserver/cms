<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteTemplate" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">
    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="pageSite.aspx">系统站点管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageSiteUrlWeb.aspx">Web地址</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageSiteUrlAssets.aspx">文件地址</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageSiteUrlApi.aspx">API地址</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="siteTables.cshtml">内容表管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageSiteKeyword.aspx">敏感词管理</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="pageSiteTemplate.aspx">站点模板管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="siteTemplateOnline.cshtml">在线站点模板</a>
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
                  <th>站点模板名称</th>
                  <th>站点模板文件夹</th>
                  <th>站点模板介绍</th>
                  <th class="text-center">创建日期</th>
                  <th class="text-center"></th>
                  <th class="text-center"></th>
                  <th class="text-center"></th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptDirectories" runat="server">
                  <ItemTemplate>
                    <tr>
                      <td>
                        <asp:Literal ID="ltlTemplateName" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlDirectoryName" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlCreationDate" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlDownloadUrl" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlCreateUrl" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
                      </td>
                    </tr>
                  </ItemTemplate>
                </asp:Repeater>
              </tbody>
            </table>
          </div>
        </div>
      </div>
      <hr />
      <div class="panel panel-default">
        <div class="panel-body p-0">
          <div class="table-responsive">
            <table id="contents" class="table tablesaw table-hover m-0">
              <thead>
                <tr class="thead">
                  <th>未解压站点模板</th>
                  <th class="text-center">创建日期</th>
                  <th class="text-center"></th>
                  <th class="text-center"></th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptZipFiles" runat="server">
                  <ItemTemplate>
                    <tr>
                      <td>
                        <asp:Literal ID="ltlFileName" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlCreationDate" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlDownloadUrl" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
                      </td>
                    </tr>
                  </ItemTemplate>
                </asp:Repeater>
              </tbody>
            </table>
          </div>
        </div>
      </div>
      <hr />
      <asp:Button class="btn btn-primary" id="BtnImport" Text="导入站点模板" runat="server" />
    </div>
  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->