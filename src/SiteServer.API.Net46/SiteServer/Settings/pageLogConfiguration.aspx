<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageLogConfiguration" %>
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
          <a class="nav-link" href="pageLogSite.aspx">站点日志</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageLogAdmin.aspx">管理员日志</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageLogUser.aspx">用户日志</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageLogError.aspx">系统错误日志</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="pageLogConfiguration.aspx">日志设置</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">是否启用定时删除日志功能</label>
        <asp:RadioButtonList ID="RblIsTimeThreshold" class="radio radio-primary" AutoPostBack="true"
          OnSelectedIndexChanged="RblIsTimeThreshold_SelectedIndexChanged" RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
        <small class="form-text text-muted">启用后系统将定时自动删除日志，以节省数据库存储空间</small>
      </div>

      <asp:PlaceHolder ID="PhTimeThreshold" runat="server">
        <div class="form-group">
          <label class="col-form-label">日志保留天数
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTime" ValidationExpression="[^']+"
              ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
          </label>
          <asp:TextBox ID="TbTime" class="form-control" runat="server" />
          <small class="form-text text-muted">设置为60天，则默认只保留60天的日志，60天之前的日志将被系统自动删除</small>
        </div>
      </asp:PlaceHolder>

      <hr />

      <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->