<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Plugins.PageInputPreview" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" runat="server">
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">提交表单</h3>
    <div class="popover-content">
    
      <table class="table noborder">
        <tr>
          <td width="155">提交表单名称：</td>
          <td>
            <asp:Literal ID="LtlInputName" runat="server"></asp:Literal>
          </td>
        </tr>
        <tr>
          <td>调用标签：</td>
          <td>
            <asp:Literal ID="LtlInputCode" runat="server"></asp:Literal>
          </td>
        </tr>
        <tr>
          <td>预览：</td>
          <td>
            <asp:Literal ID="LtlForm" runat="server"></asp:Literal>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

  <ul class="breadcrumb breadcrumb-button">
    <input type=button class="btn" onClick="location.href='pageInput.aspx?PublishmentSystemID=<%=PublishmentSystemId%>';" value="返 回" />
  </ul>

</form>

</body>
</html>
