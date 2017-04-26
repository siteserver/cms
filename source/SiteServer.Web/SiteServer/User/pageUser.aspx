<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.PageUser" %>

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
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <div class="well well-small">
            <table class="table table-noborder">
                <tr>
                    <td>用户组：
                        <asp:DropDownList ID="DdlGroup" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server"></asp:DropDownList>
                        每页显示条数：
                        <asp:DropDownList ID="DdlPageNum" class="input-small" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server">
                            <asp:ListItem Text="默认" Value="0" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="30" Value="30"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                            <asp:ListItem Text="100" Value="100"></asp:ListItem>
                            <asp:ListItem Text="200" Value="200"></asp:ListItem>
                            <asp:ListItem Text="300" Value="300"></asp:ListItem>
                        </asp:DropDownList>
                        登录次数：
                        <asp:DropDownList ID="DdlLoginCount" class="input-small" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server">
                            <asp:ListItem Text="全部" Value="0" Selected="true"></asp:ListItem>
                            <asp:ListItem Text=">30" Value="30"></asp:ListItem>
                            <asp:ListItem Text=">50" Value="50"></asp:ListItem>
                            <asp:ListItem Text=">100" Value="100"></asp:ListItem>
                            <asp:ListItem Text=">200" Value="200"></asp:ListItem>
                            <asp:ListItem Text=">300" Value="300"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>目标：
                        <asp:DropDownList ID="DdlSearchType" class="input-medium" runat="server"></asp:DropDownList>
                        关键字：
                        <asp:TextBox ID="TbKeyword" MaxLength="500" Size="45" runat="server" /> 注册时间：
                        <asp:DropDownList ID="DdlCreationDate" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server">
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
                        <asp:DropDownList ID="DdlLastActivityDate" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server">
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
                        <asp:Button class="btn" OnClick="Search_OnClick" ID="Search" Text="搜 索" runat="server" />
                    </td>
                </tr>
            </table>
        </div>

        <table class="table table-bordered table-hover">
            <tr class="info thead">
                <td>登录名</td>
                <td>显示名</td>
                <td>邮箱</td>
                <td>手机</td>
                <td>注册时间</td>
                <td>最后活动时间</td>
                <td>登录次数</td>
                <td>用户组</td>
                <td class="center" width="60">投稿数量</td>
                <td class="center" width="60">&nbsp;</td>
                <td class="center" width="60">&nbsp;</td>
                <td width="20"><input onclick="_checkFormAll(this.checked)" type="checkbox" /></td>
            </tr>
            <asp:Repeater ID="RptContents" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Literal ID="ltlUserName" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <asp:Literal ID="ltlEmail" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <asp:Literal ID="ltlMobile" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlCreationDate" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlLastActivityDate" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlLoginCount" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlGroupName" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlWritingCount" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:HyperLink NavigateUrl="javascript:;" ID="hlChangePassword" Text="重设密码" runat="server"></asp:HyperLink>
                        </td>
                        <td class="center">
                            <asp:HyperLink ID="hlEditLink" Text="编辑" runat="server"></asp:HyperLink>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlSelect" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>

        <bairong:SqlCountPager ID="SpContents" runat="server" class="table table-pager" />

        <ul class="breadcrumb breadcrumb-button">
            <asp:Button class="btn btn-success" ID="BtnAdd" Text="添加用户" runat="server" />
            <asp:Button class="btn" ID="BtnAddToGroup" Text="设置用户组" runat="server" />
            <asp:Button class="btn" ID="BtnLock" Text="锁定用户" runat="server" />
            <asp:Button class="btn" ID="BtnUnLock" Text="解除锁定" runat="server" />
            <asp:Button class="btn" ID="BtnDelete" Text="删 除" runat="server" />
            <asp:Button class="btn" ID="BtnExport" Text="导出Excel" runat="server" />
            <asp:Button class="btn" ID="BtnImport" Text="导入Excel" runat="server" />
        </ul>

    </form>
</body>

</html>
