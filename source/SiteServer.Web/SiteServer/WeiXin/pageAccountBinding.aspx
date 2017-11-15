<%@ Page Language="C#" Inherits="SiteServer.WeiXin.BackgroundPages.ConsoleAccountBinding" %>
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
        <bairong:Alerts Text="绑定的微信公众账号必须处于“开发模式并开启”状态" runat="server" />
        <script type="text/javascript" src="js/jquery.zclip.min.js"></script>

        <style type="text/css">
            i.step {
                font-weight: bold;
                font-size: 16px;
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

            div.activateapp_picture0 {
                display: inline-block;
                margin: 10px 20px;
                width: 300px;
                height: 173px;
                background: transparent url("images/weixin-activate0.png") no-repeat;
            }

            div.activateapp_picture {
                display: inline-block;
                margin: 10px 20px;
                width: 300px;
                height: 173px;
                background: transparent url("images/weixin-activate.png") 0 -273px no-repeat;
            }
        </style>

        <div class="popover popover-static">
            <h3 class="popover-title">绑定微信公共账号</h3>
            <div class="popover-content">

                <asp:PlaceHolder ID="PhStep1" runat="server">

                    <div class="container-fluid" id="weixinactivate">
                        <div class="row-fluid">

                            <div class="Span6">

                                <div>
                                    <i class="step">第一步：</i>
                                    <span class="activate_title">选择需要绑定的微信公众号类型</span>
                                </div>

                                <table class="table noborder table-hover">
                                    <tr>
                                        <td>公众帐号类型：</td>
                                        <td>
                                            <asp:RadioButtonList class="radiobuttonlist" ID="RblWXAccountType" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>微信号：</td>
                                        <td>
                                            <asp:TextBox ID="TbWhchatID" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </div>

                            <div class="Span6">
                                <div class="intro-grid">
                                    <p class="activate_desc">注：请确认您的公众账号类型</p>
                                    <div class="activateapp_picture0"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="PhStep2" Visible="false" runat="server">
                    <div class="container-fluid" id="weixinactivate">
                        <div class="row-fluid">
                            <div class="Span12">
                                <div>
                                    <i class="step">第二步：</i>
                                    <span class="activate_title">进入微信公众平台并设置接口</span>
                                </div>
                            </div>
                        </div>
                        <div class="row-fluid">
                            <div class="Span4">
                                <i class="step">1：
                                </i>
                                <span class="activate_title">进入微信公共平台
                                </span>
                                
                                <a class="weixin_link" target="_blank" href="http://mp.weixin.qq.com/">http://mp.weixin.qq.com
                                </a>
                                <div class="step_one">
                                </div>
                                <p class="activate_desc">
                                    登录微信公众平台，点击进入【开发者中心】
                                </p>
                            </div>
                            <div class="line">
                            </div>
                            <div class="Span4">
                                <i class="step">2：
                                </i>
                                <span class="activate_title">修改服务器配置
                                </span>
                                <div class="step_two">
                                </div>
                                <p class="activate_desc">
                                    在服务器配置处，点击【修改配置】
                                </p>
                            </div>
                            <div class="line">
                            </div>
                            <div class="Span4">
                                <i class="step">3：
                                </i>
                                <span class="activate_title">配置微信接口
                                </span>
                                <div class="step_three">
                                </div>
                                <p class="activate_desc">
                                    分别将下方的信息复制到【接口配置信息】的输入框
                                </p>
                                <h6>URL:
                                </h6>
                                <div class="row-fluid">
                                    <div class="Span10">
                                        <code id="text-url">
                                            <asp:Literal ID="LtlURL" runat="server" /></code>
                                    </div>
                                    <div class="Span2">
                                        <a href="javascript:;" id="url-copy" class="btn btn-small btn-success">复制</a>
                                    </div>
                                </div>
                                <h6>Token:
                                </h6>
                                <div class="row-fluid">
                                    <div class="Span10">
                                        <code id="text-token">
                                            <asp:Literal ID="LtlToken" runat="server" /></code>
                                    </div>
                                    <div class="Span2">
                                        <a href="javascript:;" id="token-copy" class="btn btn-small btn-success">复制</a>
                                    </div>
                                    <script type="text/javascript">
                                        $(document).ready(function () {
                                            $('a#url-copy').zclip({
                                                path: 'js/ZeroClipboard.swf',
                                                copy: $('#text-url').text(),
                                                afterCopy: function () {
                                                    toastr.success('URL已复制，请粘贴到微信公众平台', '操作成功');
                                                }
                                            });
                                            $('a#token-copy').zclip({
                                                path: 'js/ZeroClipboard.swf',
                                                copy: $('#text-token').text(),
                                                afterCopy: function () {
                                                    toastr.success('Token已复制，请粘贴到微信公众平台', '操作成功');
                                                }
                                            });
                                        });
                                    </script>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="PhStep3" Visible="false" runat="server">

                    <div class="container-fluid" id="weixinactivate">
                        <div class="row-fluid">

                            <div class="Span6">

                                <div>
                                    <i class="step">第三步：</i>
                                    <span class="activate_title">配置微信开发者凭据</span>
                                </div>

                                <table class="table noborder table-hover">
                                    <tr>
                                        <td>请输入开发者凭据（AppID）：</td>
                                        <td>
                                            <asp:TextBox class="input-large" ID="TbAppID" runat="server" />
                                            <asp:RequiredFieldValidator
                                                ControlToValidate="TbAppID"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic"
                                                runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbAppID"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>请输入开发者凭据（AppSecret）：</td>
                                        <td>
                                            <asp:TextBox class="input-large" ID="TbAppSecret" runat="server" />
                                            <asp:RequiredFieldValidator
                                                ControlToValidate="TbAppSecret"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic"
                                                runat="server" />
                                            <asp:RegularExpressionValidator
                                                runat="server"
                                                ControlToValidate="TbAppSecret"
                                                ValidationExpression="[^']+"
                                                ErrorMessage=" *" ForeColor="red"
                                                Display="Dynamic" />
                                        </td>
                                    </tr>
                                </table>
                            </div>

                            <div class="Span6">
                                <div class="intro-grid">
                                    <p class="activate_desc">注：AppID以及Appsecret来自于您申请开发接口时提供的账号和密码 消息加解密方式选择【明文模式】或者【兼容模式】</p>
                                    <div class="activateapp_picture"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <hr />
                <table class="table noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" Text="下一步" OnClick="Submit_OnClick" runat="server" />
                            <asp:Button class="btn" Text="返 回" OnClick="Return_OnClick" runat="server" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>

