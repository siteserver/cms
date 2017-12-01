<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageCouponActAdd" %>
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
                                <div class="step">第一步：配置活动开始属性</div>
                                <table class="table noborder table-hover">
                                    <tr>
                                        <td width="120">活动主题：</td>
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
                                        <td>活动摘要：</td>
                                        <td>
                                            <asp:TextBox ID="TbSummary" TextMode="Multiline" class="textarea" Rows="4" Style="width: 95%; padding: 5px;" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>开始时间：</td>
                                        <td>
                                            <bairong:DateTimeTextBox ID="dtbStartDate" Now="true" ShowTime="true" Columns="20" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>结束时间：</td>
                                        <td>
                                            <bairong:DateTimeTextBox ID="dtbEndDate" ShowTime="true" Columns="20" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>活动状态：</td>
                                        <td class="checkbox">
                                            <asp:CheckBox ID="CbIsEnabled" runat="server" Checked="true" Text="启用活动" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>触发关键词：</td>
                                        <td>
                                            <asp:TextBox ID="TbKeywords" runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbKeywords"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />
                                            <br>
                                            <span class="gray">多个关键词请用空格格开：例如: 微信 腾讯</span>
                                        </td>
                                    </tr>
                                </table>
                            </div>

                            <div class="Span6">
                                <div class="intro-grid">
                                    <p><strong>活动进行中显示图片：</strong></p>
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
                                <div class="step">第二步：配置活动获奖详情页</div>
                                <table class="table noborder table-hover">                                   
                                    <tr>
                                        <td width="120">优惠劵使用说明：</td>
                                        <td>
                                            <asp:TextBox ID="TbContentUsage" TextMode="Multiline" class="textarea" Rows="4" Style="width: 95%; padding: 5px;" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>活动规则说明：</td>
                                        <td>
                                            <asp:TextBox ID="TbContentDescription" TextMode="Multiline" class="textarea" Rows="4" Style="width: 95%; padding: 5px;" runat="server" />
                                        </td>
                                    </tr>
                                     <tr>
                                        <td>活动方兑奖密码：</td>
                                        <td>
                                            <asp:TextBox ID="TbAwardCode" runat="server" />
                                            <br>
                                            <span>若不设置密码，中奖确认页面的活动方兑奖区域将不显示</span>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="Span6">
                                <div class="intro-grid">
                                    <p><strong>中奖后显示的头部图片：</strong></p>
                                    <asp:Literal ID="LtlContentImageUrl" runat="server" />
                                    <a id="js_contentImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
                                </div>
                            </div>

                        </div>

                        <script type="text/javascript">
                            new AjaxUpload('js_contentImageUrl', {
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
                                            $('#preview_contentImageUrl').attr('src', response.url);
                                            $('#contentImageUrl').val(response.virtualUrl);
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

                            <div class="step">第三步：配置领取提交表单</div>
                            <table class="table noborder table-hover">
                                <tr>
                                    <td width="160">是否显示姓名字段：</td>
                                    <td class="checkbox">
                                        <asp:CheckBox ID="CbIsFormRealName" runat="server" Checked="true" Text="显示" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>姓名重命名：</td>
                                    <td>
                                        <asp:TextBox class="input-xlarge" ID="TbFormRealNameTitle" Text="姓名" runat="server" />
                                        <asp:RequiredFieldValidator
                                            ControlToValidate="TbFormRealNameTitle"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic"
                                            runat="server" />
                                        <asp:RegularExpressionValidator
                                            runat="server"
                                            ControlToValidate="TbFormRealNameTitle"
                                            ValidationExpression="[^']+"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>是否显示手机字段：</td>
                                    <td class="checkbox">
                                        <asp:CheckBox ID="CbIsFormMobile" runat="server" Checked="true" Text="显示" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>手机重命名：</td>
                                    <td>
                                        <asp:TextBox class="input-xlarge" ID="TbFormMobileTitle" Text="手机" runat="server" />
                                        <asp:RequiredFieldValidator
                                            ControlToValidate="TbFormMobileTitle"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic"
                                            runat="server" />
                                        <asp:RegularExpressionValidator
                                            runat="server"
                                            ControlToValidate="TbFormMobileTitle"
                                            ValidationExpression="[^']+"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>是否显示邮箱字段：</td>
                                    <td class="checkbox">
                                        <asp:CheckBox ID="CbIsFormEmail" runat="server" Checked="true" Text="显示" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>邮箱重命名：</td>
                                    <td>
                                        <asp:TextBox class="input-xlarge" ID="TbFormEmailTitle" Text="邮箱" runat="server" />
                                        <asp:RequiredFieldValidator
                                            ControlToValidate="TbFormEmailTitle"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic"
                                            runat="server" />
                                        <asp:RegularExpressionValidator
                                            runat="server"
                                            ControlToValidate="TbFormEmailTitle"
                                            ValidationExpression="[^']+"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>是否显示地址字段：</td>
                                    <td class="checkbox">
                                        <asp:CheckBox ID="CbIsFormAddress" runat="server" Checked="true" Text="显示" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>地址重命名：</td>
                                    <td>
                                        <asp:TextBox class="input-xlarge" ID="TbFormAddressTitle" Text="地址" runat="server" />
                                        <asp:RequiredFieldValidator
                                            ControlToValidate="TbFormAddressTitle"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic"
                                            runat="server" />
                                        <asp:RegularExpressionValidator
                                            runat="server"
                                            ControlToValidate="TbFormAddressTitle"
                                            ValidationExpression="[^']+"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic" />
                                    </td>
                                </tr>
                            </table>

                        </div>

                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="PhStep4" Visible="false" runat="server">
                        <div class="row-fluid">

                            <div class="Span6">
                                <div class="step">第四步：配置活动结束属性</div>
                                <table class="table noborder table-hover">
                                    <tr>
                                        <td width="120">活动结束主题：</td>
                                        <td>
                                            <asp:TextBox class="input-large" ID="TbEndTitle" runat="server" />
                                            <asp:RequiredFieldValidator
                                                ControlToValidate="TbEndTitle"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic"
                                                runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbEndTitle"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>活动结束摘要：</td>
                                        <td>
                                            <asp:TextBox ID="TbEndSummary" TextMode="Multiline" class="textarea" Rows="4" Style="width: 95%; padding: 5px;" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </div>

                            <div class="Span6">
                                <div class="intro-grid">
                                    <p><strong>活动已结束显示图片：</strong></p>
                                    <asp:Literal ID="LtlEndImageUrl" runat="server" />
                                    <a id="js_endImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
                                </div>
                            </div>

                        </div>

                        <script type="text/javascript">
                            new AjaxUpload('js_endImageUrl', {
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
                                            $('#preview_endImageUrl').attr('src', response.url);
                                            $('#endImageUrl').val(response.virtualUrl);
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
                <input id="contentImageUrl" name="contentImageUrl" type="hidden" runat="server" />
                <input id="endImageUrl" name="endImageUrl" type="hidden" runat="server" />

                <hr />
                <table class="table table-noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" id="BtnSubmit" Text="下一步" OnClick="Submit_OnClick" runat="server" />
                            <asp:Button class="btn" id="BtnReturn" Text="返 回" runat="server" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>

