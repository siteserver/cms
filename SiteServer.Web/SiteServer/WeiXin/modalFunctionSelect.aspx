<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalFunctionSelect" Trace="false"%>
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
<asp:Button id="BtnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server" />

  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          <asp:DropDownList ID="DdlKeywordType" runat="server" OnSelectedIndexChanged="ReFresh" AutoPostBack="true"></asp:DropDownList>
         </td>
      </tr>
    </table>
  </div>

  <asp:PlaceHolder id="PhFunction" runat="server" visible="false">

  <table id="contents" class="table table-bordered">
    <tr class="info thead">
      <td>点击选择</td>
    </tr>
    <tr><td>
      <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
        <asp:Literal ID="LtlTitle" runat="server"></asp:Literal>
      </itemtemplate>
    </asp:Repeater>
    </td></tr>
  </table>

  <bairong:sqlPager id="SpContents" runat="server" class="table table-pager" />

  </asp:PlaceHolder>

</form>
</body>
</html>
