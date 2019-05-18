<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Account.PageProfile" %>
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
        <li class="nav-item active">
          <a class="nav-link" href="pageProfile.aspx">修改资料</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pagePassword.aspx">更改密码</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">账号</label>
        <div class="form-control-plaintext">
          <asp:Literal ID="LtlUserName" runat="server"></asp:Literal>
        </div>
      </div>

      <div class="form-group">
        <label class="col-form-label">
          姓名
          <asp:RequiredFieldValidator runat="server" ControlToValidate="TbDisplayName" ErrorMessage="姓名为必填项" foreColor="red"
            ToolTip="姓名为必填项" Display="Dynamic"></asp:RequiredFieldValidator>
        </label>
        <asp:TextBox ID="TbDisplayName" class="form-control" runat="server"></asp:TextBox>
      </div>

      <div class="form-group">
        <label class="col-form-label">
          电子邮件
          <asp:RegularExpressionValidator ControlToValidate="TbEmail" ValidationExpression="(\w[0-9a-zA-Z_-]*@(\w[0-9a-zA-Z-]*\.)+\w{2,})"
            ErrorMessage="邮件格式不正确" foreColor="red" Display="Dynamic" runat="server" />
        </label>
        <asp:TextBox ID="TbEmail" class="form-control" runat="server"></asp:TextBox>
      </div>

      <div class="form-group">
        <label class="col-form-label">
          手机号码
          <asp:RegularExpressionValidator ControlToValidate="TbMobile" ValidationExpression="^0?\d{11}$" ErrorMessage="手机号码格式不正确"
            foreColor="red" Display="Dynamic" runat="server" />
        </label>
        <asp:TextBox ID="TbMobile" class="form-control" runat="server"></asp:TextBox>
      </div>

      <hr />

      <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->