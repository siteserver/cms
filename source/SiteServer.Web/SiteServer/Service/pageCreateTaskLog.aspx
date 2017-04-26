<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Service.PageCreateTaskLog" %>
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
          类型：
          <asp:DropDownList ID="DdlIsSuccess" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server"></asp:DropDownList>
          时间：从
          <bairong:DateTimeTextBox id="TbDateFrom" class="input-small" runat="server" />
          &nbsp;到&nbsp;
          <bairong:DateTimeTextBox id="TbDateTo" class="input-small" runat="server" />
          关键字：
          <asp:TextBox ID="TbKeyword" MaxLength="500" size="37" runat="server"/>
          <asp:Button class="btn" OnClick="Search_OnClick" ID="Search" Text="搜 索"  runat="server"/>
        </td>
      </tr>
    </table>
  </div>

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td class="center">&nbsp;所属站点</td>
      <td class="center">&nbsp;生成页面名称</td>
      <td class="center">&nbsp;生成页面类型</td>
      <td class="center">生成日期</td>
      <td class="center">&nbsp;用时</td>
      <td class="center" width="80">&nbsp;是否成功</td>
      <td class="center">&nbsp;失败原因</td>
      <td width="20">
        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
      </td>
    </tr>
    <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
        <tr>
          <td class="center">
            <asp:Literal ID="ltlPublishmentSystem" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ltlTaskName" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ltlCreateType" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ltlTimeSpan" runat="server"></asp:Literal>
          </td>
          <td class="center">&nbsp;
            <asp:Literal ID="ltlIsSuccess" runat="server"></asp:Literal></td>
          <td>&nbsp;
            <asp:Literal ID="ltlErrorMessage" runat="server"></asp:Literal>
          </td>
          <td class="center"><input type="checkbox" name="IDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' /></td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="SpContents" runat="server" class="table table-pager" />

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />
    <asp:Button class="btn" id="BtnDeleteAll" Text="删除全部" runat="server" />
  </ul>

</form>
</body>
</html>
