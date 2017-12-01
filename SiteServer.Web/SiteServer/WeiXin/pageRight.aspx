<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageRight" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <link href="css/add.css" rel="stylesheet" />
    <!--#include file="../inc/header.aspx"-->
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->
    <form class="form-inline" runat="server">
        <asp:Literal ID="LtlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />
        <div class="well well-small">
            <table class="table table-noborder">
                <tr>
                    <td>
                        <div>
                            <h4 class="heading-inline">
                                <asp:Literal ID="LtlWelcome" runat="server"></asp:Literal>
                                &nbsp;&nbsp;<small></small>
                            </h4>
                        </div>

                    </td>
                </tr>
            </table>
        </div>
        <div class="popover popover-static">
            <h3 class="popover-title">
                <lan>微信公众号</lan>
            </h3>
            <div class="popover-content">
                <table class="table noborder table-hover">
                    <tr>
                        <td width="150">URL：</td>
                        <td>
                            <asp:Literal ID="LtlUrl" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>Token：</td>
                        <td>
                            <asp:Literal ID="LtlToken" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </table>
                <asp:Literal ID="LtlBinding" runat="server"></asp:Literal>
                <asp:Literal ID="LtlDelete" runat="server" />
            </div>
        </div>         
    </form>
</body>
</html>

