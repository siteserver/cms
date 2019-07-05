<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteTableStyle" %>
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
        <li class="nav-item active">
          <a class="nav-link" href="siteTables.cshtml">内容表管理</a>
        </li>
        <li class="nav-item">
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
      <div class="m-t-0 header-title"> 虚拟字段管理 </div>
      <div class="panel panel-default m-t-20">
        <div class="panel-body p-0">
          <div class="table-responsive">
            <table class="table tablesaw table-hover m-0">
              <thead>
                <tr class="thead">
                  <th>字段名</th>
                  <th>显示名称</th>
                  <th class="text-center">表单提交类型</th>
                  <th class="text-center">字段类型</th>
                  <th class="text-center">验证规则</th>
                  <th class="text-center">排序</th>
                  <th class="text-center">显示样式</th>
                  <th class="text-center">表单验证</th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <tr>
                      <td>
                        <asp:Literal ID="ltlAttributeName" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlInputType" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlFieldType" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlValidate" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlTaxis" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlEditStyle" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlEditValidate" runat="server"></asp:Literal>
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
      <asp:Button class="btn btn-primary m-r-5" id="BtnAddStyle" Text="新增虚拟字段" runat="server" />
      <asp:Button class="btn m-r-5" id="BtnAddStyles" Text="批量新增虚拟字段" runat="server" />
      <asp:Button class="btn m-r-5" id="BtnImport" Text="导 入" runat="server" />
      <asp:Button class="btn m-r-5" id="BtnExport" Text="导 出" runat="server" />
      <asp:Button class="btn m-r-5" text="返 回" onclick="Return_OnClick" runat="server" />
    </div>
  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->