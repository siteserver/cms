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