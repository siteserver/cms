<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageTagStylePreview" %>
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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">模板标签样式</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="155"> 样式名称： </td>
          <td><asp:Literal ID="ltlStyleName" runat="server"></asp:Literal></td>
        </tr>
        <tr>
          <td width="155"> 调用标签： </td>
          <td><asp:Literal ID="ltlElement" runat="server"></asp:Literal></td>
        </tr>
        <tr>
          <td width="155"> 修改样式： </td>
          <td>
            <input type=button class="btn" onClick="<%=GetTemplateUrl()%>" value="修改模板" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>

    <hr />

    <div style="margin:0 10px 0 10px;">
    <div class="popover popover-static">
      <h3 class="popover-title">预览</h3>
      <div class="popover-content">
        <br>
        <asp:Literal ID="ltlForm" runat="server"></asp:Literal>
      </div>
    </div>
    </div>

</body>
</html>
