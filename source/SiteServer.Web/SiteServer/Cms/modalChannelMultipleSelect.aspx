<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelMultipleSelect" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server" text="点击栏目名称进行选择"></bairong:alerts>

  <asp:PlaceHolder id="PhPublishmentSystemId" runat="server">
    <div class="well well-small">
      选择站点：
      <asp:DropDownList ID="DdlPublishmentSystemId" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlPublishmentSystemId_OnSelectedIndexChanged"> </asp:DropDownList>
    </div>
  </asp:PlaceHolder>

  <table class="table table-bordered table-hover">
    <tr treeItemLevel="0">
      <td>
        <img align="absmiddle" src="../assets/icons/tree/minus.gif" />
        <img align="absmiddle" border="0" src="../assets/icons/tree/folder.gif" />
        <asp:Literal ID="LtlChannelName" runat="server"></asp:Literal>
      </td>
    </tr>
    <asp:Repeater ID="RptChannel" runat="server">
      <itemtemplate>
        <asp:Literal id="ltlHtml" runat="server" />
      </itemtemplate>
    </asp:Repeater>
  </table>

</form>
</body>
</html>
