<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteKeyword" %>
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
        <li class="nav-item active">
          <a class="nav-link" href="pageSiteKeyword.aspx">敏感词管理</a>
        </li>
        <li class="nav-item">
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
            <table class="table tablesaw table-hover m-0">
              <thead>
                <tr class="thead">
                  <th>敏感词</th>
                  <th>替换为</th>
                  <th class="text-center" width="120">等级</th>
                  <th width="120"></th>
                  <th width="120"></th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <tr>
                      <td>
                        <asp:Literal id="ltlKeyword" runat="server" />
                      </td>
                      <td>
                        <asp:Literal id="ltlAlternative" runat="server" />
                      </td>
                      <td class="text-center">
                        <asp:Literal id="ltlGrade" runat="server" />
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
      <ctrl:SqlPager ID="SpContents" runat="server" class="table table-pager" />
      <hr />
      <asp:Button class="btn btn-primary" ID="BtnAdd" Text="添加敏感词" runat="server" />
      <asp:Button class="btn" ID="BtnImport" Text="导入词库" runat="server" />
      <asp:Button class="btn" Text="导出词库" runat="server" OnClick="ExportWord_Click" />
    </div>
  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->