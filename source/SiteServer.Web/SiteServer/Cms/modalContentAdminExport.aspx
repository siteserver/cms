<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalContentAdminExport" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <!--#include file="../inc/header.aspx"-->
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->
    <form class="form-inline" enctype="multipart/form-data" method="post" runat="server">
        <%--<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />--%>
        <bairong:Alerts runat="server"></bairong:Alerts>

        <script type="text/javascript" language="javascript">
            function checkAll(layer, bcheck) {
                for (var i = 0; i < layer.children.length; i++) {
                    if (layer.children[i].children.length > 0) {
                        checkAll(layer.children[i], bcheck);
                    } else {
                        if (layer.children[i].type == "checkbox") {
                            layer.children[i].checked = bcheck;
                        }
                    }
                }
            }
  </script>

        <table class="table table-noborder table-hover">
            <tr>
                <td>时间段选择：</td>
                <td>
                    <span id="periods">&nbsp;&nbsp;
          开始：
         
                        <bairong:DateTimeTextBox ID="tbStartDate" class="input-small" runat="server" />
                        &nbsp;&nbsp;
          结束：
         
                        <bairong:DateTimeTextBox ID="tbEndDate" class="input-small" runat="server" />
                    </span>
                </td>
            </tr>
            <asp:PlaceHolder ID="phDisplayAttributes" runat="server">
                <tr>
                    <td colspan="2">选择需要导出的字段：<input type="checkbox" id="check_groups" onclick="checkAll(document.getElementById('Group'), this.checked);"><label for="check_groups">全选</label>
                        <span id="Group">
                            <asp:CheckBoxList ID="cblDisplayAttributes" RepeatColumns="3" RepeatDirection="Horizontal" class="noborder" Width="100%" runat="server" /></span>
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>

        <table class="table table-noborder">
            <tr>
                <td class="center">
                    <asp:Button class="btn btn-primary" ID="btnSubmit" Text="确定" UseSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>

