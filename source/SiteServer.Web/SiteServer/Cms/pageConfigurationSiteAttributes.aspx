<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationSiteAttributes" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form id="myForm" class="form-inline" runat="server">
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />
  <script type="text/javascript" charset="utf-8" src="../assets/validate.js"></script>

  <div class="popover popover-static">
    <h3 class="popover-title">站点属性设置</h3>
    <div class="popover-content">
      <table class="table noborder table-hover">
          <tr>
            <td width="220"></td>
            <td class="right"><asp:Literal id="LtlSettings" runat="server" /></td>
          </tr>
          <tr>
            <td>站点名称：</td>
            <td>
              <asp:TextBox Columns="25" MaxLength="50" id="TbPublishmentSystemName" runat="server" class="input-xlarge" />
              <asp:RequiredFieldValidator ControlToValidate="TbPublishmentSystemName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPublishmentSystemName" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
            </td>
          </tr>
          <bairong:AuxiliaryControl ID="AcAttributes" runat="server"/>
        </table>

        <hr />
        <table class="table noborder">
          <tr>
            <td class="center">
              <asp:Button class="btn btn-primary" id="BtnSubmit" text="确 定" onclick="Submit_OnClick" runat="server" />
            </td>
          </tr>
        </table>
    </div>
  </div>


</form>
</body>
</html>
