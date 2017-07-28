<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.PageUserConfigDisplay" %>
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
        <bairong:Alerts runat="server" />

        <bairong:Code Type="ajaxUpload" runat="server" />

        <div class="popover popover-static">
            <h3 class="popover-title">基本配置</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                        <td width="150">是否启用用户中心：</td>
                        <td>
                            <asp:DropDownList ID="DdlIsEnable" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlIsEnable_SelectedIndexChanged"></asp:DropDownList>
                            <span>关闭后，用户中心将不能使用。</span>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="PhOpen" runat="server">
                        <tr>
                            <td width="150">系统名称：</td>
                            <td>
                                <asp:TextBox ID="TbTitle" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator
                                    ControlToValidate="TbTitle"
                                    ErrorMessage=" *" ForeColor="red"
                                    Display="Dynamic"
                                    runat="server" /></td>
                        </tr>
                        <tr>
                            <td width="150">版权信息：</td>
                            <td><asp:TextBox ID="TbCopyright" width="450" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td width="150">备案号：</td>
                            <td><asp:TextBox ID="TbBeianNo" width="450" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td width="120">LOGO：</td>
                            <td><asp:Literal ID="LtlLogoUrl" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <td width="120">选择图片：</td>
                            <td>
                                <div id="uploadFile" class="btn btn-success">选 择</div>
                                尺寸：168×48
                                <span id="img_upload_txt" style="clear: both; font-size: 12px; color: #FF3737;"></span></td>
                        </tr>
                        <script type="text/javascript" language="javascript">
                            $(document).ready(function () {
                              new AjaxUpload('uploadFile', {
                                action: "pageUserConfigDisplay.aspx?uploadLogo=true",
                                name: "Filedata",
                                data: {},
                                onSubmit: function (file, ext) {
                                  var reg = /^(gif|jpg|jpeg|png)$/i;
                                  if (ext && reg.test(ext)) {
                                    $('#img_upload_txt').text('上传中... ');
                                  } else {
                                    $('#img_upload_txt').text('系统不允许上传指定的格式');
                                    return false;
                                  }
                                },
                                onComplete: function (file, response) {
                                    console.log(response);
                                  if (response) {
                                    response = JSON.parse(response)
                                    if (response.success === 'true') {
                                        location.reload()
                                    } else {
                                        $('#img_upload_txt').text(response.message);
                                    }
                                  }
                                }
                              });
                            });
                        </script>
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
