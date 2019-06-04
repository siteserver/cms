<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationSiteAttributes" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
  <script type="text/javascript" charset="utf-8" src="../assets/validate.js"></script>
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">

    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationSite.aspx?siteId=<%=SiteId%>">站点设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationContent.aspx?siteId=<%=SiteId%>">内容设置</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="javascript:;">站点属性</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">站点名称
          <asp:RequiredFieldValidator ControlToValidate="TbSiteName" errorMessage=" *" foreColor="red" display="Dynamic"
            runat="server" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSiteName" ValidationExpression="[^']+"
            errorMessage=" *" foreColor="red" display="Dynamic" />
        </label>
        <asp:TextBox MaxLength="50" id="TbSiteName" runat="server" class="form-control" />
      </div>

      <asp:Literal ID="LtlAttributes" runat="server" />

      <hr />

      <asp:Button class="btn btn-primary" id="BtnSubmit" text="确 定" onclick="Submit_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->