<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteUrlApi" %>
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
        <li class="nav-item active">
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
      <div class="form-group">
        <label class="col-form-label">API部署方式</label>
        <asp:RadioButtonList id="RblIsSeparatedApi" RepeatDirection="Horizontal" AutoPostBack="true"
          OnSelectedIndexChanged="RblIsSeparatedApi_SelectedIndexChanged" class="radio radio-primary" runat="server"></asp:RadioButtonList>
        <small class="form-text text-muted">设置API服务器部署方式</small>
      </div>
      <asp:PlaceHolder ID="PhSeparatedApi" runat="server">
        <div class="form-group">
          <label class="col-form-label">独立部署API访问地址
            <asp:RequiredFieldValidator ControlToValidate="TbSeparatedApiUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic"
              runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSeparatedApiUrl" ValidationExpression="[^']+"
              ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
          </label>
          <asp:TextBox id="TbSeparatedApiUrl" class="form-control" runat="server"></asp:TextBox>
        </div>
      </asp:PlaceHolder>
      <hr />
      <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />
    </div>
  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->