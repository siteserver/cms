<%@ Page Language="c#" Inherits="SiteServer.BackgroundPages.PageLogin" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管理员登录</title>
    <bairong:Code Type="JQuery" runat="server" />
    <bairong:Code Type="bootstrap" runat="server" />
    <bairong:Code Type="html5shiv" runat="server" />
    <script language="JavaScript">
        if (window.top != self) {
            window.top.location = self.location;
        }
        $(document).ready(function () { $('#TbAccount').focus(); });
    </script>
    <link href="css/login.css" rel="stylesheet" type="text/css" />
</head>
<body class="yunBg">
    <form class="form-inline" runat="server" autocomplete="off">
        <div class="yunMain">
            <div class="yunTop">
                <a class="yunLogo" href="http://www.siteserver.cn"><img src="pic/login/siteserver.png" /></a>
            </div>
            <div class="yunItmName">
                <img src="pic/login/yun_ico1.jpg" width="31" height="32" /><span class="yunItmS">管理员登录</span>
            </div>
            <div class="yunBox">
                <div style="width: auto 0; margin: 0 100px"><asp:Literal ID="LtlMessage" runat="server" /></div>
                <div class="yun_u1">
                    <ul>
                        <li><span class="yun_s1">账号：</span>

                            <asp:TextBox class="yun_int1" ID="TbAccount" runat="server" />
                            <asp:RequiredFieldValidator
                                ControlToValidate="TbAccount"
                                ErrorMessage=" *" ForeColor="red"
                                Display="Dynamic"
                                runat="server" />
                            <asp:RegularExpressionValidator
                                runat="server"
                                ControlToValidate="TbAccount"
                                ValidationExpression="[^']+"
                                ErrorMessage=" *" ForeColor="red"
                                Display="Dynamic" />
                            请输入注册的手机号/邮箱/用户名
                        </li>
                        <li><span class="yun_s1">密码：</span>
                            <input type="password" style="display:none" />
                            <asp:TextBox class="yun_int1" ID="TbPassword" TextMode="Password" runat="server" />
                            <asp:RequiredFieldValidator
                                ControlToValidate="TbPassword"
                                ErrorMessage=" *" ForeColor="red"
                                Display="Dynamic"
                                runat="server" />
                            <asp:RegularExpressionValidator
                                runat="server"
                                ControlToValidate="TbPassword"
                                ValidationExpression="[^']+"
                                ErrorMessage=" *" ForeColor="red"
                                Display="Dynamic" />
                        </li>
                        <asp:PlaceHolder ID="PhValidateCode" runat="server">
                            <li><span class="yun_s1">验证码：</span>
                                <asp:TextBox class="yun_int1 yun_int2" ID="TbValidateCode" runat="server" />
                                <asp:Literal ID="LtlValidateCodeImage" runat="server"></asp:Literal>
                                <asp:RequiredFieldValidator
                                    ControlToValidate="TbValidateCode"
                                    ErrorMessage=" *" ForeColor="red"
                                    Display="Dynamic"
                                    runat="server" />
                            </li>
                        </asp:PlaceHolder>
                        <li><span class="yun_s1">&nbsp;</span>
                            <label class="checkbox">
                                <asp:CheckBox ID="CbRememberMe" Checked="true" runat="server"></asp:CheckBox>
                                记住用户名
                            </label>
                        </li>
                        <li><span class="yun_s1">&nbsp;</span>
                            <asp:Button class="yun_submit" ID="LoginSubmit" Style="width: 101px" OnClick="Submit_OnClick" runat="server" />
                            &nbsp;
                            <a href="findPwd.aspx">找回密码？</a>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="yunFooter">北京百容千域软件技术开发有限公司 版权所有 Copyright © 2003-<script>document.write(new Date().getFullYear());</script></div>
        </div>
    </form>
</body>
</html>

<!--#include file="./inc/scripts.aspx"-->