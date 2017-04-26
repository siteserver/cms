<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelImport" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" enctype="multipart/form-data" method="post" runat="server">
<%--<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />--%>
<bairong:alerts runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td>栏目文件：</td>
      <td><input type=file  id=myFile size="35" runat="server"/>
        <asp:RequiredFieldValidator ControlToValidate="myFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /></td>
    </tr>
    <tr>
      <td>父栏目：</td>
      <td><asp:DropDownList ID="ParentNodeID" runat="server"></asp:DropDownList></td>
    </tr>
    <tr>
      <td>是否覆盖同名栏目：</td>
      <td><asp:RadioButtonList ID="IsOverride" runat="server" RepeatDirection="Horizontal" class="noborder">
          <asp:ListItem Text="覆盖" Value="True" Selected="true"></asp:ListItem>
          <asp:ListItem Text="不覆盖" Value="False"></asp:ListItem>
        </asp:RadioButtonList></td>
    </tr>
  </table>

    <table class="table table-noborder">
        <tr>
          <td class="center">
              <asp:Button class="btn btn-primary" id="btnSubmit" text="确定" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" />
          </td>
        </tr>
      </table>
</form>
</body>
</html>
