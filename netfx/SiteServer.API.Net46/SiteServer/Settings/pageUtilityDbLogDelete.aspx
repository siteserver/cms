<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUtilityDbLogDelete" %>
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
          <a class="nav-link" href="pageUtilityCache.aspx">系统缓存</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityParameter.aspx">系统参数查看</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityEncrypt.aspx">加密字符串</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageUtilityJsMin.aspx">JS脚本压缩</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="pageUtilityDbLogDelete.aspx">清空数据库日志</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <p class="text-muted font-13 m-b-25">
        上一次清空日志时间：
        <asp:Literal id="LtlLastExecuteDate" runat="server" />
      </p>

      <hr />

      <asp:Button class="btn btn-primary" ID="Submit" Text="清空数据库日志" OnClick="Submit_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->