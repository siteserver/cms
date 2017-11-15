<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageCardAdd" %>
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
            .city {
                width: 94px;
            }

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
        <script type="text/javascript" src="../../SiteFiles/bairong/JQuery/jscolor/jscolor.js"></script>
        <script type="text/javascript" src="../../SiteFiles/Services/WeiXin/Card/scripts/lib/provincesdata.js"></script>
        <script type="text/javascript" src="../../SiteFiles/Services/WeiXin/Card/scripts/lib/jquery.provincesCity.js"></script>
        <script type="text/javascript">
            $(function () {
                $("#province").ProvinceCity();
                $("#province select").attr("class", "city")
                if ($("#shopPosition").val().length > 0) {
                    $("#province select").eq(0).val($("#shopPosition").val().split(',')[0]);
                    $("#province select").eq(1).append('<option value="' + $("#shopPosition").val().split(',')[1] + '" selected="selected">' + $("#shopPosition").val().split(',')[1] + '</option>');
                    $("#province select").eq(2).append('<option value="' + $("#shopPosition").val().split(',')[2] + '" selected="selected">' + $("#shopPosition").val().split(',')[2] + '</option>');
                }
                $("#province select").change(function () {
                    $("#shopPosition").val($("#province select").eq(0).val() + "," + $("#province select").eq(1).val() + "," + $("#province select").eq(2).val());
                });
            });
        </script>
        <div class="popover popover-static operation-area">
            <h3 class="popover-title">
                <asp:Literal ID="LtlPageTitle" runat="server" />
            </h3>
            <div class="popover-content">
                <div class="container-fluid" id="weixinactivate">
                    <asp:PlaceHolder ID="PhStep1" runat="server">
                        <div class="row-fluid">

                            <div class="Span6">
                                <div class="step">第一步：配置会员卡属性</div>
                                <table class="table noborder table-hover">
                                    <tr>
                                        <td width="120">主题：</td>
                                        <td>
                                            <asp:TextBox class="input-xlarge" ID="TbTitle" runat="server" />
                                            <asp:RequiredFieldValidator
                                                ControlToValidate="TbTitle"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic"
                                                runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbTitle"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>摘要：</td>
                                        <td>
                                            <asp:TextBox ID="TbSummary" TextMode="Multiline" class="textarea" Rows="4" Style="width: 95%; padding: 5px;" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>会员卡状态：</td>
                                        <td class="checkbox">
                                            <asp:CheckBox ID="CbIsEnabled" runat="server" Checked="true" Text="启用会员卡" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>触发关键词：</td>
                                        <td>
                                            <asp:TextBox ID="TbKeywords" runat="server" />
                                            <asp:RequiredFieldValidator ControlToValidate="TbKeywords" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbKeywords"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />
                                            <br>
                                            <span class="gray">多个关键词请用空格格开：例如: 微信 腾讯 阁下</span>
                                        </td>
                                    </tr>
                                </table>
                            </div>

                            <div class="Span6">
                                <div class="intro-grid">
                                    <p><strong>会员卡显示图片：</strong></p>
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

                    <asp:PlaceHolder ID="PhStep2" Visible="false" runat="server">
                        <div class="row-fluid">

                            <div class="Span6">
                                <div class="step">第二步：配置会员卡详情页</div>
                                <table class="table noborder table-hover">
                                    <tr>
                                        <td width="120">会员卡名称：</td>
                                        <td>
                                            <asp:TextBox class="input-xlarge" ID="TbCardTitle" runat="server" />
                                            <asp:RequiredFieldValidator
                                                ControlToValidate="TbCardTitle"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic"
                                                runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbCardTitle"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="120">名称颜色值：</td>
                                        <td>
                                            <asp:TextBox class="input-xlarge color" ID="TbCardTitleColor" Width="220px" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="120">卡号颜色值：</td>
                                        <td>
                                            <input type="hidden" id="navImageCssColor" runat="server" />
                                            <asp:TextBox class="input-xlarge color" ID="TbCardSNColor" Width="220px" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </div>

                            <div class="Span6">
                                <div class="intro-grid">
                                    <p><strong>会员卡正面图片：</strong></p>
                                    <asp:Literal ID="LtlContentFrontImageUrl" runat="server" />
                                    <a id="js_contentFrontImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a><br />
                                    <span>规格：596*325</span>
                                </div>

                                <div class="intro-grid">
                                    <p><strong>会员卡背面图片：</strong></p>
                                    <asp:Literal ID="LtlContentBackImageUrl" runat="server" />
                                    <a id="js_contentBackImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a><br />
                                     <span>规格：596*325</span>
                                </div>

                            </div>

                            <script type="text/javascript">
                                new AjaxUpload('js_contentFrontImageUrl', {
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
                            $('#preview_contentFrontImageUrl').attr('src', response.url);
                            $('#contentFrontImageUrl').val(response.virtualUrl);
                        } else {
                            alert(response.message);
                        }
                    }
                }
            });
                            </script>
                            <script type="text/javascript">
                                new AjaxUpload('js_contentBackImageUrl', {
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
                            $('#preview_contentBackImageUrl').attr('src', response.url);
                            $('#contentBackImageUrl').val(response.virtualUrl);
                        } else {
                            alert(response.message);
                        }
                    }
                }
            });
                            </script>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="PhStep3" Visible="false" runat="server">
                        <div class="row-fluid">

                            <div class="Span6">
                                <div class="step">第三步：配置商家信息</div>
                                <table class="table noborder table-hover">
                                    <tr>
                                        <td width="120">商家名称</td>
                                        <td>
                                            <asp:TextBox class="input-xlarge" ID="TbShopName" runat="server" />
                                            <asp:RequiredFieldValidator
                                                ControlToValidate="TbShopName"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic"
                                                runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbShopName"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>商家所在区</td>
                                        <td>
                                            <span id="province"></span>
                                            <input type="hidden" id="shopPosition" runat="server" />
                                         </td>
                                    </tr>
                                    <tr>
                                        <td>商家详细地址</td>
                                        <td>
                                            <asp:TextBox class="input-xlarge" ID="TbShopAddress" runat="server" />
                                            <asp:RequiredFieldValidator
                                                ControlToValidate="TbShopAddress"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic"
                                                runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbShopAddress"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />

                                            <input type="button" id="BtnMap" class="btn" value="查看效果" />
                                            <script type="text/javascript">
                                                $(function () {
                                                    if ($("#tbShopAddress").val().length > 0) {
                                                        $("#mapAddress").show();
                                                    }
                                                })
                                                $("#btnMap").click(function () {
                                                    $("#mapAddress").show();
                                                    $("#map").children().remove();
                                                    var mapUrl = "http://map.baidu.com/mobile/webapp/place/list/qt=s&wd=" + $("#" + "<%=tbShopAddress.ClientID%>").val() + "/vt=map";
                           var iframe = $("<iframe style='width:100%;height:100%;background-color:#ffffff;margin-bottom:15px;' scrolling='auto' frameborder='0' width='100%' height='100%' src='" + mapUrl + "'></iframe>");
                           $("#map").append(iframe);
                       });
                                            </script>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>商家电话：</td>
                                        <td>
                                            <asp:TextBox ID="TbShopTel" class="input-xlarge" runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbShopTel"
                                                ValidationExpression="^((0\d{2,3}-\d{7,8})|(1[3584]\d{9}))$|([0-9]{4}-[0-9]{3}-[0-9]{3})"
                                                ErrorMessage="电话格式不正确！" Display="Dynamic" ForeColor="#CC0000"> 
                                            </asp:RegularExpressionValidator>
                                           

                                        </td>
                                    </tr>
                                    <tr>
                                        <td>商家用户</td>
                                        <td>
                                            <asp:TextBox class="input-xlarge" ID="TbShopManage" runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbShopManage"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>商家密码</td>
                                        <td>
                                            <asp:TextBox class="input-xlarge" ID="TbShopPassword" runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbShopPassword"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="Span6" id="mapAddress" style="display: none">
                                <div class="intro-grid">
                                    <p><strong>商家详细地址</strong></p>
                                    <div style="height: 300px;" id="map">
                                        <asp:Literal ID="LtlMap" runat="server" /></div>
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                </div>

                <input id="imageUrl" name="imageUrl" type="hidden" runat="server" />
                <input id="contentFrontImageUrl" name="contentFrontImageUrl" type="hidden" runat="server" />
                <input id="contentBackImageUrl" name="contentBackImageUrl" type="hidden" runat="server" />

                <hr />
                <table class="table table-noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" id="BtnSubmit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                            <asp:Button class="btn" id="BtnReturn" Text="返 回" runat="server" />
                         </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>

