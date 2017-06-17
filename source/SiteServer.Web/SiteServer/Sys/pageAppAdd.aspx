<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PageAppAdd" EnableViewState="false" %>
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
        <bairong:alerts text="在此选择需要创建的应用类型，不同的类型代表不同的场景，请选择合适的类型创建应用" runat="server" />

        <style type="text/css">
            .app-div {
                margin: 0 auto;
                width: 92%;
                vertical-align: middle;
            }

            .icon-span {
                text-align: center;
                padding: 30px;
                height: 200px;
            }

            .icon-span:hover {
                background-color: #eee;
            }

            .icon-span a {
                font-size: 14px;
            }

            .icon-span .icon-5 {
                font-size: 8em;
            }

            .icon-span h5 {
                margin-top: 15px;
            }

            .icon-span .notavaliable {
                opacity: 0.6;
            }
        </style>

        <div class="popover popover-static">
            <h3 class="popover-title">创建新应用</h3>
            <div class="popover-content">

                <div class="app-div">
                    <asp:Repeater ID="RptContents" runat="server">
                        <itemtemplate>
                            <asp:Literal id="ltlHtml" runat="server" />
                        </itemtemplate>
                    </asp:Repeater>
                </div>

                <div style="clear:both"></div>

            </div>
        </div>

    </form>
</body>

</html>
<!-- check for 3.6 html permissions -->