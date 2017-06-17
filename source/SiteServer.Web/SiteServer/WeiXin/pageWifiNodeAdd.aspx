<%@ Page Language="C#" Inherits="SSiteServer.BackgroundPages.WeiXin.PageifiNodeAdd" %>
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
        <asp:Literal ID="LtlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <style type="text/css">
            div.step {
                font-weight: bold;
                font-size: 16px;
                margin-bottom: 10px;
            }

            span.activate_title {
                line-height: 34px;
                font-size: 16px;
                color: #333;
            }

            p.activate_desc {
                width: 100%;
                margin-left: 32px;
                font-size: 13px;
                font-weight: bold;
            }

            div.step_one, div.step_two, div.step_three {
                display: inline-block;
                margin-left: 30px;
                width: 280px;
                height: 190px;
                background: transparent url("images/weixin-activate.png") no-repeat;
            }

            div.step_two, div.step_three {
                margin-top: 20px;
            }

            div.step_one {
                background-position: -40px -48px;
            }

            div.step_two {
                background-position: -395px -48px;
            }

            div.step_three {
                background-position: -760px -48px;
            }
        </style>

        <bairong:Code Type="ajaxupload" runat="server" />
        <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/swfupload.js"></script>
        <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/handlers.js"></script>
        <div class="popover popover-static operation-area">
            <h3 class="popover-title">
                <asp:Literal ID="LtlPageTitle" runat="server" />
            </h3>
            <div class="popover-content">
                <div class="container-fluid" id="weixinactivate">
                    <asp:PlaceHolder ID="PhStep1" runat="server">
                        <div class="row-fluid">
                            <div class="Span6">
                                <div class="step">配置路由器数据信息</div>
                                <table class="table noborder table-hover">
                                    <tr>
                                        <td width="120">路由ID：</td>
                                        <td>
                                            <asp:TextBox class="input-xlarge" ID="TbNodeID" runat="server" />
                                            <br />
                                            <span class="gray">连接成功打开http://192.168.1.1 查看右下角路由唯一ID.</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>路由名称：</td>
                                        <td>
                                            <asp:TextBox class="input-xlarge" ID="TbNodeName" runat="server" />
                                            <br />
                                            <span class="gray">用户连接WIFI认证的时候查看的名称.</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>微信号：</td>
                                        <td>
                                            <asp:TextBox class="input-xlarge" ID="TbWechatID" runat="server" ReadOnly="true" />
                                            <br />
                                            <span class="gray">配置公共账号填写微信名称.用户关注此账号上网.</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>是否绑定：</td>
                                        <td class="checkbox">
                                            <asp:CheckBox ID="CbIsWxBindType" runat="server" Checked="true" Text="绑定微信号" />
                                            <br />
                                            <span class="gray">绑定微信号,用户关注触发关键字上网.</span>
                                        </td>
                                    </tr>
                                </table>
                            </div>

                            <div class="Span6">
                                <div class="intro-grid">
                                    <p><strong>绑定的微信图片信息：</strong></p>
                                    <asp:Literal ID="LtlImageUrl" runat="server" />
                                    <a id="js_imageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
                                </div>
                            </div>

                        </div>

                        <script type="text/javascript">
                            new AjaxUpload('js_imageUrl', {
                                action: '<%=GetUploadUrl()%>',
                                name: "Upload",
                                data: {},
                                onSubmit: function (file, ext) {
                                    var reg = /^(jpg|jpeg|png|gif)$/i;
                                    if (ext && reg.test(ext)) {
                                        //$('#img_upload_txt_').text('上传中... ');
                                    } else {
                                        //$('#img_upload_txt_').text('只允许上传JPG,PNG,GIF图片');
                                        alert('只允许上传JPG,PNG,GIF图片');
                                        return false;
                                    }
                                },
                                onComplete: function (file, response) {
                                    if (response) {
                                        response = eval("(" + response + ")");
                                        if (response.success == 'true') {
                                            $('#preview_imageUrl').attr('src', response.url);
                                            $('#imageUrl').val(response.virtualUrl);
                                        } else {
                                            alert(response.message);
                                        }
                                    }
                                }
                            });
                        </script>
                    </asp:PlaceHolder>
                </div>
                <input id="imageUrl" name="imageUrl" type="hidden" runat="server" />
                <hr />
                <table class="table table-noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" ID="btnSubmit" Text="确定" OnClick="Submit_OnClick" runat="server" />
                            <asp:Button class="btn" ID="btnReturn" Text="返 回" runat="server" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>
