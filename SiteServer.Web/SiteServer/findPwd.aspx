<%@ Page Language="c#" Inherits="SiteServer.BackgroundPages.PageFindPwd" %>
	<!DOCTYPE html>
	<html>

	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
		<title>找回密码</title>
		<script language="javascript" src="assets/jquery/jquery-1.9.1.min.js"></script>
		<link rel="stylesheet" type="text/css" href="assets/bootstrap/css/bootstrap.min.css">
		<script language="JavaScript">
			if (window.top != self) {
				window.top.location = self.location;
			}
			$(document).ready(function () {
				$('#TbAccount').focus();
			});
		</script>
		<link href="css/login.css" rel="stylesheet" type="text/css" />
	</head>

	<body class="yunBg">
		<form runat="server" autocomplete="off">
			<div class="yunMain">
				<div class="yunTop">
					<a class="yunLogo" href="http://www.siteserver.cn">
						<img src="pic/login/siteserver.png" />
					</a>
				</div>
				<div class="yunItmName">
					<img src="pic/login/yun_ico2.jpg" width="31" height="32" />
					<span class="yunItmS">
						<asp:Literal ID="LtlPageTitle" runat="server" />
					</span>
				</div>
				<div class="yunBox">
					<div style="width: auto 0; margin: 10px 100px">
						<asp:Literal ID="LtlMessage" runat="server" />
					</div>
					<div class="yun_u1">
						<asp:PlaceHolder ID="PhStepAccount" runat="server">
							<ul>
								<li>
									<span class="yun_s1">账号：</span>
									<asp:TextBox class="yun_int1" ID="TbAccount" runat="server" />
									<asp:RequiredFieldValidator ControlToValidate="TbAccount" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
									/>
									<asp:RegularExpressionValidator runat="server" ControlToValidate="TbAccount" ValidationExpression="[^']+" ErrorMessage=" *"
									  ForeColor="red" Display="Dynamic" /> 请输入注册的手机号/邮箱/用户名
								</li>
								<li>
									<span class="yun_s1">验证码：</span>
									<asp:TextBox class="yun_int1 yun_int2" ID="TbValidateCode" runat="server" />
									<asp:Literal ID="LtlValidateCodeImage" runat="server"></asp:Literal>
									<asp:RequiredFieldValidator ControlToValidate="TbValidateCode" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
									/>
								</li>
								<li>
									<span class="yun_s1">&nbsp;</span>

								</li>
								<li>
									<span class="yun_s1">&nbsp;</span>
									<asp:Button class="yun_submit2" Style="width: 101px" OnClick="Account_OnClick" runat="server" /> &nbsp;
									<a href="login.aspx">返回登录</a>
								</li>
							</ul>
						</asp:PlaceHolder>
						<asp:PlaceHolder ID="PhStepSmsCode" visible="false" runat="server">
							<ul>
								<li>
									<span class="yun_s1">短信验证码：</span>
									<asp:TextBox class="yun_int1 yun_int2" ID="TbSmsCode" runat="server" />
									<asp:RequiredFieldValidator ControlToValidate="TbSmsCode" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
									/>
								</li>
								<li>
									<span class="yun_s1">&nbsp;</span>

								</li>
								<li>
									<span class="yun_s1">&nbsp;</span>
									<asp:Button class="yun_submit2" Style="width: 101px" OnClick="SmsCode_OnClick" runat="server" /> &nbsp;
									<a href="login.aspx">返回登录</a>
								</li>
							</ul>
						</asp:PlaceHolder>
						<asp:PlaceHolder ID="PhStepChangePassword" visible="false" runat="server">
							<ul>
								<li>
									<span class="yun_s1">新密码：</span>
									<asp:TextBox class="yun_int1" TextMode="Password" ID="TbPassword" runat="server" />
									<asp:RequiredFieldValidator ControlToValidate="TbPassword" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
									/>
									<asp:RegularExpressionValidator runat="server" ControlToValidate="TbPassword" ValidationExpression="[^']+" ErrorMessage=" *"
									  ForeColor="red" Display="Dynamic" />
								</li>
								<li>
									<span class="yun_s1">确认新密码：</span>
									<asp:TextBox class="yun_int1" TextMode="Password" ID="TbConfirmPassword" runat="server" />
									<asp:RequiredFieldValidator ControlToValidate="TbConfirmPassword" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
									/>
									<asp:RegularExpressionValidator runat="server" ControlToValidate="TbConfirmPassword" ValidationExpression="[^']+" ErrorMessage=" *"
									  ForeColor="red" Display="Dynamic" />
								</li>
								<li>
									<span class="yun_s1">&nbsp;</span>

								</li>
								<li>
									<span class="yun_s1">&nbsp;</span>
									<asp:Button class="yun_submit2" Style="width: 101px" OnClick="ChangePassword_OnClick" runat="server" /> &nbsp;
									<a href="login.aspx">返回登录</a>
								</li>
							</ul>
						</asp:PlaceHolder>
					</div>
				</div>
				<div class="yunFooter">北京百容千域软件技术开发有限公司 版权所有 Copyright © 2003-
					<script>
						document.write(new Date().getFullYear());
					</script>
				</div>
			</div>
		</form>
	</body>

	</html>
	<!--#include file="inc/foot.html"-->