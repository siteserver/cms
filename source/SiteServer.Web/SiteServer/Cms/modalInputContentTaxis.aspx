<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalInputContentTaxis" Trace="false" %>
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

        <table class="table table-noborder table-hover">
            <tr>
                <td width="120">
                    <bairong:Help HelpText="对所选内容的方向" Text="排序方向：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:RadioButtonList ID="TaxisType" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
            </tr>
            <tr>
                <td width="120">
                    <bairong:Help HelpText="对所选内容的数目" Text="移动数目：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:TextBox class="input-mini" Text="1" MaxLength="50" ID="TaxisNum" runat="server" />
                    <asp:RequiredFieldValidator
                        ControlToValidate="TaxisNum"
                        ErrorMessage=" *" ForeColor="red"
                        Display="Dynamic"
                        runat="server" />
                    <asp:RegularExpressionValidator
                        runat="server"
                        ControlToValidate="TaxisNum"
                        ValidationExpression="^([1-9]|[1-9][0-9]{1,})$"
                        ErrorMessage=" *" ForeColor="red"
                        Display="Dynamic" /></td>
            </tr>
        </table>

    </form>
</body>
</html>

