<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationCrossSiteTrans" %>
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
          <a class="nav-link" href="javascript:;">跨站转发审核设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationCrossSiteTransChannels.aspx?siteId=<%=SiteId%>">跨站转发栏目设置</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts text="选择当栏目未设置跨站转发类型时采用的默认设置，同时设置跨站转发到本站点的内容是否需要审核" runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">跨站转发到本站点的内容是否需要审核</label>
        <asp:RadioButtonList ID="RblIsCrossSiteTransChecked" RepeatDirection="Horizontal" class="radio radio-primary "
          runat="server">
        </asp:RadioButtonList>
      </div>

      <hr />

      <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->