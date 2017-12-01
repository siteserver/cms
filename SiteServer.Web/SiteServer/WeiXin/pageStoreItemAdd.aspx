<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageStoreItemAdd" %>
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
        <script type="text/javascript" src="http://api.map.baidu.com/api?v=1.4"></script>
        <div class="popover popover-static operation-area">
            <h3 class="popover-title">
                <asp:Literal ID="LtlPageTitle" runat="server" />
            </h3>
            <div class="popover-content">
                <div class="container-fluid" id="weixinactivate">
                    <asp:PlaceHolder ID="PhStep1" runat="server">
                        <div class="row-fluid">
                            <div class="Span6">
                                <div class="step">微门店信息维护</div>
                                <table class="table noborder table-hover">

                                    <tr>
                                        <td>名称：</td>
                                        <td>
                                            <asp:TextBox ID="TbStoreName" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>属性：</td>
                                        <td>
                                            <asp:DropDownList ID="DdlStoreCategoryName" class="input-medium" runat="server"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>电话：</td>
                                        <td>
                                            <asp:TextBox ID="TbStoreTel" runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbStoreTel"
                                                ValidationExpression="^(0\d{2,3}-\d{7,8})$|([0-9]{4}-[0-9]{3}-[0-9]{3})"
                                                ErrorMessage="电话格式不正确！" Display="Dynamic" ForeColor="#CC0000"> 
                                            </asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>手机：</td>
                                        <td>
                                            <asp:TextBox ID="TbStoreMobile" runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbStoreMobile"
                                                ValidationExpression="^(1[3584]\d{9})$"
                                                ErrorMessage="手机格式不正确！" Display="Dynamic" ForeColor="#CC0000"> 
                                            </asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>地址：</td>
                                        <td>
                                            <asp:TextBox ID="TbStoreAddress" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>图片：</td>
                                        <td>
                                            <asp:Literal ID="LtlImageUrl" runat="server" />
                                            <a id="js_storeImageUrl" href="javascript:;" style="margin-top: 15px;" onclick="return false;" class="btn btn-success">上传</a>
                                        </td>
                                        <script type="text/javascript">
                                            new AjaxUpload('js_storeImageUrl', {
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
                                    </tr>
                                    <tr>
                                        <td>描述：</td>
                                        <td>
                                            <asp:TextBox ID="TbSummary" TextMode="Multiline" class="textarea" Rows="4" Style="width: 95%; padding: 5px;" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="Span6">
                                <script src="js/map.js"></script>
                                <input type="text" id="suggestId" class="input-xlarge" value="请输入门店地址" />
                                <input id="BtnSearchMP" type="button" value="搜索" />
                                <div style="width: 530px; height: 400px; border: 1px solid #000; margin-top: 10px;" id="container"></div>
                                <script type="text/javascript">
                                    $(function () {
                                        baidu_map();
                                        $("#btnSearchMP").click(function () { loadmap(); });
                                    });
                                </script>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </div>
                <input id="imageUrl" name="imageUrl" type="hidden" runat="server" />
                <input id="txtLongitude" name="txtLongitude" type="hidden" runat="server" />
                <input id="txtLatitude" name="txtLatitude" type="hidden" runat="server" />
                <hr />
                <table class="table table-noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" id="BtnSubmit" Text="确定" OnClick="Submit_OnClick" runat="server" />
                            <asp:Button class="btn" id="BtnReturn" Text="返 回" runat="server" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>


