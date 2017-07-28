<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovPublicCreateIdentifier" %>
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
  <bairong:alerts text="系统将重新生成所选栏目下的指定类型信息的索引号。" runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">重新生成索引号</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="200">生成索引号栏目：</td>
          <td><asp:DropDownList ID="ddlNodeID" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
          <td>生成索引号信息类型：</td>
          <td><asp:RadioButtonList id="rblCreateType" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="生 成" onclick="Submit_OnClick" runat="server" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
