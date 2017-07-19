<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalChangePublishmentSystemUrl" Trace="false" %>
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
        <bairong:Alerts Text="此操作将修改站点的访问地址，修改前请先确认此地址能够被访问。" runat="server"></bairong:Alerts>

        <table class="table table-noborder table-hover">
            <tr>
                <td width="160">生成页面URL前缀</td>
                <td>
                    <asp:TextBox ID="TbPublishmentSystemUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TbPublishmentSystemUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic"
                        runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPublishmentSystemUrl" ValidationExpression="[^']+" ErrorMessage=" *"
                        ForeColor="red" Display="Dynamic" />
                    <br />
                    <span class="gray">页面所有地址将保留此前缀，可以设置绝对路径（域名）或者相对路径（如：“/”）</span>
                </td>
            </tr>
            <tr>
                <td>网站部署方式：</td>
                <td>
                    <asp:DropDownList ID="DdlIsMultiDeployment" AutoPostBack="true" OnSelectedIndexChanged="DdlIsMultiDeployment_SelectedIndexChanged"
                        runat="server"></asp:DropDownList>
                    <br />
                    <span>如果是多服务器部署，请选择“内外网分离部署”</span>
                </td>
            </tr>
            <asp:PlaceHolder ID="PhSingle" runat="server">
                <tr>
                    <td>站点访问地址：</td>
                    <td>
                        <asp:TextBox ID="TbSiteUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="TbSiteUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                        />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSiteUrl" ValidationExpression="[^']+" ErrorMessage=" *"
                            ForeColor="red" Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td>API访问地址：</td>
                    <td>
                        <asp:TextBox ID="TbApiUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="TbApiUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                        />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="TbApiUrl" ValidationExpression="[^']+" ErrorMessage=" *"
                            ForeColor="red" Display="Dynamic" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="PhMulti" runat="server">
                <tr>
                    <td>外部站点访问地址：</td>
                    <td>
                        <asp:TextBox ID="TbOuterSiteUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="TbOuterSiteUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="TbOuterSiteUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td>内部站点访问地址：</td>
                    <td>
                        <asp:TextBox ID="TbInnerSiteUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="TbInnerSiteUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="TbInnerSiteUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td>外部API访问地址：</td>
                    <td>
                        <asp:TextBox ID="TbOuterApiUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="TbOuterApiUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="TbOuterApiUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td>内部API访问地址：</td>
                    <td>
                        <asp:TextBox ID="TbInnerApiUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="TbInnerApiUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="TbInnerApiUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td>用户中心访问地址：</td>
                <td>
                    <asp:TextBox ID="TbHomeUrl" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="TbHomeUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbHomeUrl" ValidationExpression="[^']+" ErrorMessage=" *"
                        ForeColor="red" Display="Dynamic" />
                </td>
            </tr>
        </table>

    </form>
</body>

</html>