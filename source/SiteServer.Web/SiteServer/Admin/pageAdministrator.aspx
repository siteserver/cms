<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.PageAdministrator" %>
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
          角色：
          <asp:DropDownList ID="RoleName" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server"></asp:DropDownList>
          每页显示条数：
          <asp:DropDownList ID="PageNum" class="input-small" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server">
            <asp:ListItem Text="默认" Value="0" Selected="true"></asp:ListItem>
            <asp:ListItem Text="30" Value="30"></asp:ListItem>
            <asp:ListItem Text="50" Value="50"></asp:ListItem>
            <asp:ListItem Text="100" Value="100"></asp:ListItem>
            <asp:ListItem Text="200" Value="200"></asp:ListItem>
            <asp:ListItem Text="300" Value="300"></asp:ListItem>
          </asp:DropDownList>
          排序：
          <asp:DropDownList ID="Order" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server">
            <asp:ListItem Text="登录名称" Value="UserName" Selected="true"></asp:ListItem>
            <asp:ListItem Text="最后登录日期" Value="LastActivityDate"></asp:ListItem>
            <asp:ListItem Text="创建日期" Value="CreationDate"></asp:ListItem>
            <asp:ListItem Text="登录次数" Value="CountOfLogin"></asp:ListItem>
          </asp:DropDownList></td>
        </tr>
        <tr>
          <td>
            所在区域：
            <asp:DropDownList ID="ddlAreaID" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server"></asp:DropDownList>
            最后登录日期：
            <asp:DropDownList ID="LastActivityDate" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server">
              <asp:ListItem Text="全部时间" Value="0" Selected="true"></asp:ListItem>
              <asp:ListItem Text="1天内" Value="1"></asp:ListItem>
              <asp:ListItem Text="2天内" Value="2"></asp:ListItem>
              <asp:ListItem Text="3天内" Value="3"></asp:ListItem>
              <asp:ListItem Text="1周内" Value="7"></asp:ListItem>
              <asp:ListItem Text="1个月内" Value="30"></asp:ListItem>
              <asp:ListItem Text="3个月内" Value="90"></asp:ListItem>
              <asp:ListItem Text="半年内" Value="180"></asp:ListItem>
              <asp:ListItem Text="1年内" Value="365"></asp:ListItem>
            </asp:DropDownList>
            关键字：
            <asp:TextBox id="Keyword" MaxLength="500" Size="45" runat="server"/>
            <asp:Button class="btn" OnClick="Search_OnClick" id="Search" text="搜 索"  runat="server"/></td>
        </tr>
    </table>
  </div>

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td width="100">账号</td>
      <td width="100">姓名</td>
      <td width="100">手机</td>
      <td width="120">部门</td>
      <td width="120">区域</td>
      <td width="80">最后登录</td>
      <td width="60">登录次数</td>
      <td>角色</td>
      <td width="50">&nbsp;</td>
      <td width="50">&nbsp;</td>
      <td width="50">&nbsp;</td>
      <td width="20">
        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
      </td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
          <tr>
            <td><asp:Literal ID="ltlUserName" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlMobile" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlDepartment" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlArea" runat="server"></asp:Literal></td>
            <td class="center"><%# GetDateTime((DateTime)DataBinder.Eval(Container.DataItem,"LastActivityDate"))%></td>
            <td class="center"><%# DataBinder.Eval(Container.DataItem,"CountOfLogin")%></td>
            <td><%# GetRolesHtml(DataBinder.Eval(Container.DataItem,"UserName").ToString())%></td>
            <td class="center"><asp:Literal ID="ltlEdit" runat="server"></asp:Literal></td>
            <td class="center"><asp:Literal ID="ltlRole" runat="server"></asp:Literal></td>
            <td class="center"><asp:HyperLink NavigateUrl="javascript:;" ID="hlChangePassword" Text="重设密码" runat="server"></asp:HyperLink></td>
            <td class="center"><asp:Literal ID="ltlSelect" runat="server"></asp:Literal></td>
          </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="spContents" runat="server" class="table table-pager" />

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddButton" Text="新 增" runat="server" />
    <asp:Button class="btn" id="Lock" Text="锁 定" runat="server" />
    <asp:Button class="btn" id="UnLock" Text="解除锁定" runat="server" />
    <asp:Button class="btn" id="Delete" Text="删 除" runat="server" />
  </ul>

</form>
</body>
</html>
