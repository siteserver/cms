<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.User.ModalUserSelect" %>
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
        <asp:Button ID="btnSubmit" UseSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" Style="display: none" />
        <bairong:Alerts runat="server"></bairong:Alerts>

        <div class="well well-small">
            <table class="table table-noborder">
                <tr>
                    <td>会员组：
          <asp:DropDownList ID="ddlLevelID" class="input-medium" runat="server" OnSelectedIndexChanged="Search_OnClick" AutoPostBack="true"></asp:DropDownList>
                        注册时间：
          <asp:DropDownList ID="CreateDate" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server">
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
                        最后活动时间：
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
                    </td>
                </tr>
                <tr>
                    <td>
                        <%-- 所属部门：
          <asp:DropDownList ID="ddlDepartmentID" class="input-medium" runat="server" OnSelectedIndexChanged="Search_OnClick" AutoPostBack="true" ></asp:DropDownList>
          所在区域：
          <asp:DropDownList ID="ddlAreaID" class="input-medium" runat="server" OnSelectedIndexChanged="Search_OnClick" AutoPostBack="true" ></asp:DropDownList>--%>
          关键字：
          <asp:TextBox ID="Keyword" MaxLength="500" Size="45" runat="server" />
                        <asp:Button class="btn" OnClick="Search_OnClick" ID="Search" Text="搜 索" runat="server" />
                    </td>
                </tr>
            </table>
        </div>

        <table class="table table-bordered table-hover">
            <tr class="info thead">
                <td>账号</td>
                <td>姓名</td>
                <td>会员组</td>
                <%--      <td>所属部门</td>
      <td>所在区域</td>--%>
                <td>注册时间</td>
                <td>注册 IP</td>
                <td>最后活动时间</td>
                <td width="20">
                    <input onclick="_checkFormAll(this.checked)" type="checkbox" />
                </td>
            </tr>
            <asp:Repeater ID="rptContents" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Literal ID="ltlUserName" runat="server"></asp:Literal></td>
                        <td>
                            <asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal></td>
                        <td>
                            <asp:Literal ID="ltLevel" runat="server"></asp:Literal></td>
                        <%--            <td><asp:Literal ID="ltlDepartmentID" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlAreaID" runat="server"></asp:Literal></td>--%>
                        <td>
                            <asp:Literal ID="ltlCreateDate" runat="server"></asp:Literal></td>
                        <td>
                            <asp:Literal ID="ltlCreateIPAddress" runat="server"></asp:Literal></td>
                        <td>
                            <asp:Literal ID="ltlLastActivityDate" runat="server"></asp:Literal></td>
                        <td class="center">
                            <asp:Literal ID="ltlSelect" runat="server"></asp:Literal></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>

        <bairong:SqlPager ID="spContents" runat="server" class="table table-pager" />

    </form>
</body>
</html>

