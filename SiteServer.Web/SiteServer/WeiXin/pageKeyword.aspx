<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageKeyword" %>
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
  <bairong:alerts runat="server"></bairong:alerts>
  
    <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          关键词类型：
          <asp:DropDownList ID="DdlKeywordType" runat="server"></asp:DropDownList>
          关键字：
          <asp:TextBox id="TbKeyword" MaxLength="500" runat="server"/>
          <asp:Button class="btn" OnClick="Search_OnClick" id="Search" text="搜 索"  runat="server"/>
        </td>
      </tr>
    </table>
  </div>

  <table id="contents" class="table table-bordered">
    <tr class="info thead">
      <td>所有关键词</td>
    </tr>
    <tr><td>
      <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
        <asp:Literal ID="LtlKeyword" runat="server"></asp:Literal>
      </itemtemplate>
    </asp:Repeater>
    </td></tr>
  </table>

  <bairong:sqlPager id="SpContents" runat="server" class="table table-pager" />

</form>
</body>
</html>
