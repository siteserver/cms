<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAdministratorAdd" %>
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
          <a class="nav-link" href="pageAdministrator.aspx">管理员管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminRole.aspx">角色管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminConfiguration.aspx">管理员设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminDepartment.aspx">所属部门管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminArea.aspx">所在区域管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="adminAccessTokens.cshtml">API密钥管理</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="m-t-0 header-title">
        <asp:Literal id="LtlPageTitle" runat="server" />
      </div>
      <p class="text-muted font-13 m-b-25"></p>

      <div class="form-group">
        <label class="col-form-label">账号
          <asp:RequiredFieldValidator ControlToValidate="TbUserName" ErrorMessage=" *" foreColor="red" Display="Dynamic"
            runat="server" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbUserName" ValidationExpression="[^']+"
            ErrorMessage="不能用单引号" foreColor="red" Display="Dynamic" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbUserName" ValidationExpression="[^<]+"
            ErrorMessage="不能用&lt;注册" foreColor="red" Display="Dynamic" />
        </label>
        <asp:TextBox ID="TbUserName" cssClass="form-control" runat="server" />
        <small class="form-text text-muted">帐号用于登录系统，由字母、数字组成</small>
      </div>

      <div class="form-group">
        <label class="col-form-label">姓名
          <asp:RequiredFieldValidator ControlToValidate="TbDisplayName" ErrorMessage=" *" foreColor="red" Display="Dynamic"
            runat="server" />
        </label>
        <asp:TextBox ID="TbDisplayName" class="form-control" runat="server" />
      </div>

      <asp:PlaceHolder ID="PhPassword" runat="server">
        <div class="form-group">
          <label class="col-form-label">密码
            <asp:RequiredFieldValidator ControlToValidate="TbPassword" ErrorMessage=" *" foreColor="red" Display="Dynamic"
              runat="server" />
          </label>
          <asp:TextBox TextMode="Password" ID="TbPassword" class="form-control" runat="server" />
        </div>
        <div class="form-group">
          <label class="col-form-label">确认密码
            <asp:RequiredFieldValidator ControlToValidate="TbConfirmPassword" ErrorMessage=" *" foreColor="red" Display="Dynamic"
              runat="server" />
            <asp:CompareValidator runat="server" ControlToCompare="TbPassword" ControlToValidate="TbConfirmPassword"
              Display="Dynamic" foreColor="red" ErrorMessage=" 两次输入的密码不一致！请再输入一遍您上面填写的密码。"></asp:CompareValidator>
          </label>
          <asp:TextBox TextMode="Password" ID="TbConfirmPassword" class="form-control" runat="server" />
        </div>
      </asp:PlaceHolder>

      <div class="form-group">
        <label class="col-form-label">所属部门</label>
        <asp:DropDownList ID="DdlDepartmentId" class="form-control" runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">所在区域</label>
        <asp:DropDownList ID="DdlAreaId" class="form-control" runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">电子邮箱
          <asp:RegularExpressionValidator ControlToValidate="TbEmail" ValidationExpression="(\w[0-9a-zA-Z_-]*@(\w[0-9a-zA-Z-]*\.)+\w{2,})"
            ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
        </label>
        <asp:TextBox ID="TbEmail" runat="server" class="form-control"></asp:TextBox>
      </div>

      <div class="form-group">
        <label class="col-form-label">手机号码
          <asp:RegularExpressionValidator ControlToValidate="TbMobile" ValidationExpression="^0?\d{11}$" ErrorMessage=" *"
            foreColor="red" Display="Dynamic" runat="server" />
        </label>
        <asp:TextBox ID="TbMobile" runat="server" class="form-control"></asp:TextBox>
      </div>

      <hr />

      <asp:Button class="btn btn-primary" OnClick="Submit_OnClick" Text="确 定" runat="server" />
      <asp:Button class="btn ml-1" ID="BtnReturn" Text="返 回" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->