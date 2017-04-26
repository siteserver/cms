<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.ModalUserView" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <table class="table table-striped">
    <tr>
      <td width="100" height="25">用户ID：</td>
      <td><asp:Literal id="ltlUserID" runat="server" /></td>
      <td width="100" height="25">用户名：</td>
      <td><asp:Literal id="ltlUserName" runat="server" /></td>
    </tr>
    <tr>
      <td>姓名：</td>
      <td><asp:Literal id="ltlDisplayName" runat="server" /></td>
      <td>用户组：</td>
      <td><asp:Literal id="ltlGroup" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">电子邮箱：</td>
      <td><asp:Literal id="ltlEmail" runat="server" /></td>
      <td>手机号码：</td>
      <td><asp:Literal ID="ltlMobile" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">登录次数：</td>
      <td><asp:Literal id="ltlLoginCount" runat="server" /></td>
      <td>投稿数量：</td>
      <td><asp:Literal ID="ltlWritingCount" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">单位：</td>
      <td><asp:Literal id="ltlOrganization" runat="server" /></td>
      <td>部门：</td>
      <td><asp:Literal ID="ltlDepartment" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">职位：</td>
      <td colspan="3"><asp:Literal id="ltlPosition" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">出生日期：</td>
      <td><asp:Literal id="ltlBirthday" runat="server" /></td>
      <td>性别：</td>
      <td><asp:Literal id="ltlGender" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">毕业院校：</td>
      <td><asp:Literal id="ltlGraduation" runat="server" /></td>
      <td>学历：</td>
      <td><asp:Literal id="ltlEducation" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">地址：</td>
      <td colspan="3"><asp:Literal id="ltlAddress" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">微信：</td>
      <td><asp:Literal id="ltlWeiXin" runat="server" /></td>
      <td>QQ：</td>
      <td><asp:Literal id="ltlQQ" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">微博：</td>
      <td colspan="3"><asp:Literal id="ltlWeiBo" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">兴趣：</td>
      <td colspan="3"><asp:Literal id="ltlInterests" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">签名：</td>
      <td colspan="3"><asp:Literal id="ltlSignature" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">注册时间：</td>
      <td><asp:Literal id="ltlCreateDate" runat="server" /></td>
      <td width="160">最后修改密码时间：</td>
      <td><asp:Literal ID="ltlLastResetPasswordDate" runat="server" /></td>
    </tr>
    <tr>
      <td height="25">最后登录时间：</td>
      <td colspan="3"><asp:Literal ID="ltlLastActivityDate" runat="server" /></td>
    </tr>
  </table>

</form>
</body>
</html>
