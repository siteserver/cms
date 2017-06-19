<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageMenuAdd" %>
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
        <bairong:Alerts runat="server" />

        <script type="text/javascript">
            var contentSelect = function (title, nodeID, contentID, pageUrl) {
                $('#titles').show().html('内容页：' + title + '&nbsp;<a href="' + pageUrl + '" target="blank">查看</a>');
                $('#idsCollection').val(nodeID + '_' + contentID);
            };
            var selectChannel = function (nodeNames, nodeID, pageUrl) {
                $('#titles').show().html('栏目页：' + nodeNames + '&nbsp;<a href="' + pageUrl + '" target="blank">查看</a>');
                $('#idsCollection').val(nodeID + '_0');
            };
            var selectKeyword = function (keyword) {
                $('#TbKeyword').val(keyword);
            };
        </script>

        <div class="popover popover-static">
            <h3 class="popover-title">
                <asp:Literal ID="LtlPageTitle" runat="server" />
            </h3>
            <div class="popover-content">

                <table class="table noborder">
                    <tr>
                        <td width="100">菜单名称：</td>
                        <td>
                            <asp:TextBox ID="TbMenuName" runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="TbMenuName" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbMenuName" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="PhMenuType" runat="server">
                        <tr>
                            <td>点击菜单触发：</td>
                            <td>
                                <asp:DropDownList ID="DdlMenuType" AutoPostBack="true" OnSelectedIndexChanged="ddlMenuType_OnSelectedIndexChanged" runat="server"></asp:DropDownList></td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="PhKeyword" Visible="false" runat="server">
                        <tr>
                            <td>关键词：</td>
                            <td>
                                <asp:TextBox ID="TbKeyword" runat="server" />
                                <asp:RequiredFieldValidator ControlToValidate="TbKeyword" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbKeyword" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                                &nbsp;<asp:Button id="BtnKeywordSelect" class="btn btn-info" Text="选择" runat="server" />
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="PhUrl" Visible="false" runat="server">
                        <tr>
                            <td>网址：</td>
                            <td>
                                <asp:TextBox ID="TbUrl" runat="server" Style="width: 300px;" />
                                <asp:RequiredFieldValidator ControlToValidate="TbUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="PhSite" Visible="false" runat="server">
                        <tr>
                            <td>微网站页面：</td>
                            <td>
                                <div id="titles" class="well well-small" style="display: none"></div>
                                <asp:Button id="BtnContentSelect" class="btn btn-info" Text="选择内容页" runat="server" />
                                <asp:Button id="BtnChannelSelect" class="btn btn-info" Text="选择栏目页" runat="server" />
                                <input id="idsCollection" name="idsCollection" type="hidden" value="" />
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

        <asp:Literal ID="LtlScript" runat="server" />

    </form>
</body>
</html>

