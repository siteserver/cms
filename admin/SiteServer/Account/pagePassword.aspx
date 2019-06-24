<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Account.PagePassword" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">

    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="pageProfile.aspx">修改资料</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="pagePassword.aspx">更改密码</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">管理员登录名</label>
        <div class="form-control-plaintext">
          <asp:Literal ID="LtlUserName" runat="server"></asp:Literal>
        </div>
      </div>

      <div class="form-group">
        <label class="col-form-label">
          当前密码
          <asp:RequiredFieldValidator ControlToValidate="TbCurrentPassword" ErrorMessage=" *" ForeColor="red" Display="Dynamic"
            runat="server" />
        </label>
        <!--防止表单的自动填充功能-->
        <input type="password" style="display: none" />
        <!--防止表单的自动填充功能-->
        <asp:TextBox ID="TbCurrentPassword" class="form-control" runat="server" TextMode="Password"></asp:TextBox>
      </div>

      <div class="form-group">
        <label class="col-form-label">
          新密码
          <asp:RequiredFieldValidator ControlToValidate="TbNewPassword" ErrorMessage=" *" ForeColor="red" Display="Dynamic"
            runat="server" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbNewPassword" ValidationExpression="[^']+"
            ErrorMessage="不能输入单引号" foreColor="red" Display="Dynamic" />
        </label>
        <asp:TextBox ID="TbNewPassword" class="form-control" runat="server" TextMode="Password"></asp:TextBox>
      </div>

      <div class="form-group">
        <label class="col-form-label">
          再次输入新密码
          <asp:RequiredFieldValidator ControlToValidate="TbConfirmNewPassword" ErrorMessage=" *" ForeColor="red"
            Display="Dynamic" runat="server" />
          <asp:CompareValidator runat="server" ControlToCompare="TbNewPassword" ControlToValidate="TbConfirmNewPassword"
            Display="Dynamic" ForeColor="red" ErrorMessage=" 两次输入的新密码不一致！请再输入一遍您上面填写的新密码。"></asp:CompareValidator>
        </label>
        <asp:TextBox ID="TbConfirmNewPassword" class="form-control" runat="server" TextMode="Password"></asp:TextBox>
      </div>

      <hr />

      <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->