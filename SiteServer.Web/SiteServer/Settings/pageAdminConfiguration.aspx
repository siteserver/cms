<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAdminConfiguration" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">

        <div class="card-box">
          <ul class="nav nav-pills">
            <li class="nav-item">
              <a class="nav-link" href="pageAdministrator.aspx">管理员管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageAdminRole.aspx">角色管理</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageAdminConfiguration.aspx">管理员设置</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageAdminDepartment.aspx">所属部门管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageAdminArea.aspx">所在区域管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageAdminAccessTokens.cshtml">API密钥管理</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="form-group">
            <label class="col-form-label">管理员用户名最小长度</label>
            <asp:TextBox ID="TbLoginUserNameMinLength" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">0代表不限制</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">管理员密码最小长度</label>
            <asp:TextBox ID="TbLoginPasswordMinLength" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">0代表不限制</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">管理员密码规则限制</label>
            <asp:DropDownList ID="DdlLoginPasswordRestriction" class="form-control" runat="server"></asp:DropDownList>
          </div>

          <div class="form-group">
            <label class="col-form-label">是否开启登录失败锁定</label>
            <asp:RadioButtonList ID="RblIsLoginFailToLock" OnSelectedIndexChanged="RblIsLoginFailToLock_SelectedIndexChanged" class="radio radio-primary"
              runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
            </asp:RadioButtonList>
          </div>

          <asp:PlaceHolder ID="PhFailToLock" runat="server">
            <div class="form-group">
              <label class="col-form-label">登录失败次数锁定（次）
                <asp:RequiredFieldValidator ControlToValidate="TbLoginFailToLockCount" runat="server" ErrorMessage="*" foreColor="Red"></asp:RequiredFieldValidator>
              </label>
              <asp:TextBox ID="TbLoginFailToLockCount" class="form-control" runat="server"></asp:TextBox>
              <small class="form-text text-muted">一旦登录失败达到指定次数之后管理员就会被锁定</small>
            </div>

            <div class="form-group">
              <label class="col-form-label">管理员锁定类型</label>
              <asp:DropDownList ID="DdlLoginLockingType" OnSelectedIndexChanged="DdlLoginLockingType_SelectedIndexChanged" class="form-control"
                runat="server" AutoPostBack="true"></asp:DropDownList>
            </div>


            <asp:PlaceHolder ID="PhLoginLockingHours" runat="server">
              <div class="form-group">
                <label class="col-form-label">管理员锁定时间（小时）
                  <asp:RequiredFieldValidator ControlToValidate="TbLoginLockingHours" runat="server" ErrorMessage="*" foreColor="Red"></asp:RequiredFieldValidator>
                </label>
                <asp:TextBox ID="TbLoginLockingHours" class="form-control" runat="server"></asp:TextBox>
              </div>
            </asp:PlaceHolder>
          </asp:PlaceHolder>

          <div class="form-group">
            <label class="col-form-label">是否开启管理员找回密码功能</label>
            <asp:RadioButtonList ID="RblIsFindPassword" class="radio radio-primary" OnSelectedIndexChanged="RblIsFindPassword_SelectedIndexChanged"
              runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
            </asp:RadioButtonList>
          </div>

          <asp:PlaceHolder ID="PhFindPassword" runat="server">
            <div class="form-group">
              <label class="col-form-label">发送验证码短信模板Id
                <asp:RequiredFieldValidator ControlToValidate="TbFindPasswordSmsTplId" runat="server" ErrorMessage="*" foreColor="Red"></asp:RequiredFieldValidator>
              </label>
              <asp:TextBox ID="TbFindPasswordSmsTplId" class="form-control" runat="server"></asp:TextBox>
              <small class="form-text text-muted">需进入短信供应商模板管理界面，添加验证码类短信模板并获取模板Id</small>
            </div>
          </asp:PlaceHolder>

          <div class="form-group">
            <label class="col-form-label">管理员是否可以查看其他人添加的内容</label>
            <asp:RadioButtonList ID="RblIsViewContentOnlySelf" runat="server" class="radio radio-primary" RepeatDirection="Horizontal"></asp:RadioButtonList>
            <small class="form-text text-muted">注意：超级管理员、站点管理员、具有审核权限的管理员，此设置无效。</small>
          </div>

          <hr />

          <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->