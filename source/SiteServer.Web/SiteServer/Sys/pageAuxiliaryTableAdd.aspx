<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PageAuxiliaryTableAdd" %>
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
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server" /></h3>
    <div class="popover-content">

      <table class="table noborder table-hover">
        <tr>
          <td width="140">辅助表标识：</td>
          <td>
            <asp:TextBox Columns="35" MaxLength="22" id="TableENName" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="TableENName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TableENName" ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage=" 只允许包含字母、数字以及下划线" foreColor="red" Display="Dynamic" />
            <br>
            <span>辅助表的唯一标识，当在数据库中创建辅助表时此标识作为被创建表的名称</span>
          </td>
        </tr>
        <tr>
          <td>辅助表名称：</td>
          <td>
            <asp:TextBox Columns="35" MaxLength="50" id="TableCNName" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="TableCNName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TableCNName" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
        </tr>
        <tr>
          <td>辅助表简介：</td>
          <td>
            <asp:TextBox Columns="45" Rows="4" TextMode="MultiLine" id="Description" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="Description" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
        </tr>
        <tr>
          <td>辅助表类型：</td>
          <td>
            <asp:RadioButtonList id="AuxiliaryTableType" RepeatDirection="Horizontal" class="noborder" RepeatColumns="4" runat="server"> </asp:RadioButtonList>
            <span>辅助表类型与站点中栏目的内容模型相对应</span>
          </td>
        </tr>
      </table>

      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
            <input type="button" class="btn" value="返 回" onClick="javascript:location.href='pageAuxiliaryTable.aspx';" />
          </td>
        </tr>
      </table>

    </div>
  </div>

</form>
</body>
</html>
