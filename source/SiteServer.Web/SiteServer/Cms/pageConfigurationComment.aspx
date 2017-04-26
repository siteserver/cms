<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationComment" %>
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

        <div class="popover popover-static">
            <h3 class="popover-title">评论管理设置</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                        <td width="240">是否启用评论功能：</td>
                        <td>
                            <asp:RadioButtonList ID="RblIsCommentable"
                                RepeatDirection="Horizontal" class="noborder"
                                AutoPostBack="true" OnSelectedIndexChanged="RblIsCommentable_OnSelectedIndexChanged"
                                runat="server">
                                <asp:ListItem Text="启用" Value="True" Selected="True" />
                                <asp:ListItem Value="False" Text="禁用" />
                            </asp:RadioButtonList>
                            <span>在此开启或禁用评论功能</span>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="PhComments" runat="server">
                        <tr>
                            <td>新评论是否需要审核：</td>
                            <td>
                                <asp:RadioButtonList ID="RblIsCheckComments"
                                RepeatDirection="Horizontal" class="noborder"
                                runat="server">
                                    <asp:ListItem Text="需要审核" Value="True" Selected="True" />
                                    <asp:ListItem Value="False" Text="不需要审核" />
                                </asp:RadioButtonList>
                                <span>在此设置新评论是否需要审核</span>
                            </td>
                        </tr>
                        <tr>
                            <td>是否启用匿名评论：</td>
                            <td>
                                <asp:RadioButtonList ID="RblIsAnonymousComments"
                                RepeatDirection="Horizontal" class="noborder"
                                runat="server">
                                    <asp:ListItem Value="True" Text="启用" Selected="True" />
                                    <asp:ListItem Value="False" Text="禁用" />
                                </asp:RadioButtonList>
                                <span>在此设置评论是否启用匿名评论</span>
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
