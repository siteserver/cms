<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Analysis.PageLogConfiguration" %>
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
        <bairong:Alerts Text="在此设置日志自动删除阈值" runat="server"></bairong:Alerts>

        <div class="popover popover-static">
            <h3 class="popover-title">日志管理设置</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                        <td width="180">是否启用记录时间阈值：</td>
                        <td>
                            <asp:RadioButtonList ID="rblIsTimeThreshold" AutoPostBack="true" OnSelectedIndexChanged="rblIsTimeThreshold_SelectedIndexChanged" RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="phTimeThreshold" runat="server">
                        <tr>
                            <td>时间：</td>
                            <td>
                                <asp:TextBox ID="tbTime" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="tbTime" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                                <span class="gray">单位（天）
                                    <br />
                                    设置为60天，则默认只保留60天的日志
                                </span>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td>是否启用记录条数阈值：</td>
                        <td>
                            <asp:RadioButtonList ID="rblIsCounterThreshold" AutoPostBack="true" OnSelectedIndexChanged="rblIsCounterThreshold_SelectedIndexChanged" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server"></asp:RadioButtonList>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="phCounterThreshold" runat="server">
                        <tr>
                            <td>条数：</td>
                            <td>
                                <asp:TextBox ID="tbCounter" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="tbCounter" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                                <span class="gray">单位（万）
                                    <br />
                                    设置为3万，则默认只保留3万条日志
                                </span>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </table>

                <hr />
                <table class="table noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>
