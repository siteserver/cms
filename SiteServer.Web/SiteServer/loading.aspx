<%@ Page Language="c#" Inherits="SiteServer.BackgroundPages.PageLoading" Trace="False" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>载入程序</title>
    <link rel="stylesheet" href="inc/style.css" type="text/css" />
    <bairong:Code Type="jQuery" runat="server" />
    <bairong:Code Type="html5shiv" runat="server" />
</head>
<body>
    <table width="100%" height="380" border="0" cellpadding="4" cellspacing="0">
        <tr>
            <td class="center" valign="middle">
                <img src="pic/loading.gif" />
                <br />
                <span style="margin-top: 10px;">载入中，请稍候...</span>
            </td>
        </tr>
    </table>
</body>
</html>
<script language="javascript">
    //防止xss，update by sessionliang at 20151214
    $(function () {
        var url = encodeURI("<%=GetRedirectUrl()%>");
    if (url && url.length > 0) {
        setTimeout(function () { location.href = decodeURI(url); }, 200);
    }
});
</script>

