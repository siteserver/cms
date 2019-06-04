<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteDelete" %>
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
        <li class="nav-item active">
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
      <div class="m-t-0 header-title"> 删除站点 </div>
      <p class="text-muted font-13 m-b-25"> 站点名称： <asp:Literal id="LtlSiteName" runat="server"></asp:Literal>
      </p>
      <div class="form-group">
        <label class="col-form-label">是否保留文件</label>
        <asp:RadioButtonList ID="RblRetainFiles" runat="server" RepeatDirection="Horizontal" class="radio radio-primary">
          <asp:ListItem Text="保留文件" Value="true"></asp:ListItem>
          <asp:ListItem Text="删除文件" Value="false" Selected="true"></asp:ListItem>
        </asp:RadioButtonList>
        <small class="form-text text-muted">选择保留文件删除操作将仅在数据库中删除此站点</small>
      </div>
      <hr />
      <asp:Button class="btn btn-danger" id="Submit" text="删 除" OnClick="Submit_OnClick" runat="server" />
      <asp:Button class="btn" text="返 回" onclick="Return_OnClick" runat="server" />
    </div>
  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->