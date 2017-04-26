<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentTidyUp" Trace="false" %>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts text="根据添加日期(或内容ID)重新排序(不可逆,请慎重)。" runat="server"></bairong:alerts>

    <table class="table table-hover table-noborder">
    <tr>
        <td width="120">
            <bairong:Help HelpText="请选择排序字段" Text="排序字段：" runat="server">
            </bairong:Help>
        </td>
        <td>
            <asp:RadioButtonList ID="rblAttributeName" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td width="120">
            <bairong:Help HelpText="请选择排序方向" Text="排序方向：" runat="server">
            </bairong:Help>
        </td>
        <td>
            <asp:RadioButtonList ID="rblIsDesc" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
        </td>
    </tr>
    </table>

</form>
</body>
</html>
