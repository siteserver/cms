<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Analysis.PageLogSite" %>
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

  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          站点：
          <asp:DropDownList ID="PublishmentSystem" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server"></asp:DropDownList>
          类型：
          <asp:DropDownList ID="LogType" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server"></asp:DropDownList>
        </td>
      </tr>
      <tr>
        <td>
          时间：从
          <bairong:DateTimeTextBox id="DateFrom" class="input-small" runat="server" />
          &nbsp;到&nbsp;
          <bairong:DateTimeTextBox id="DateTo" class="input-small" runat="server" />
          管理员：
          <asp:TextBox ID="UserName" MaxLength="500" size="20" runat="server"/>
          关键字：
          <asp:TextBox ID="Keyword" MaxLength="500" size="37" runat="server"/>
          <asp:Button class="btn" OnClick="Search_OnClick" ID="Search" Text="搜 索"  runat="server"/>
        </td>
      </tr>
    </table>
  </div>

  <table id="contents" class="table table-bordered table-hover">
    <tr class="info thead">
      <asp:Literal ID="ltlPublishmentSystem" runat="server"></asp:Literal>
      <td class="center" width="100">&nbsp;管理员</td>
      <td class="center" width="100">IP地址</td>
      <td class="center" width="150">日期</td>
      <td class="center" width="150">&nbsp;动作</td>
      <td>&nbsp;描述</td>
      <td width="20">
        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
      </td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <tr>
          <asp:Literal ID="ltlPublishmentSystem" runat="server"></asp:Literal>
          <td class="center" width="100"><asp:Literal ID="ltlUserName" runat="server"></asp:Literal></td>
          <td class="center" width="100"><asp:Literal ID="ltlIPAddress" runat="server"></asp:Literal></td>
          <td class="center" width="150"><asp:Literal ID="ltlAddDate" runat="server"></asp:Literal></td>
          <td class="center" width="150">&nbsp;
            <asp:Literal ID="ltlAction" runat="server"></asp:Literal></td>
          <td>&nbsp;
            <asp:Literal ID="ltlSummary" runat="server"></asp:Literal></td>
          <td class="center" width="20"><input type="checkbox" name="IDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' /></td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="spContents" runat="server" class="table table-pager" />

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn" id="Delete" Text="删 除" runat="server" />
    <asp:Button class="btn" id="DeleteAll" Text="删除全部" runat="server" />
  </ul>

</form>
</body>
</html>
