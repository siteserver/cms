<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUserConfiguration" %>
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
              <a class="nav-link" href="pageUser.aspx">用户管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUserCheck.aspx">审核新用户</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageUserConfiguration.aspx">用户设置</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="form-group">
            <label class="col-form-label">允许新用户注册</label>
            <asp:RadioButtonList ID="RblIsRegisterAllowed" class="radio radio-primary" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList>
            </asp:RadioButtonList>
            <small class="form-text text-muted">选择否将禁止新用户注册, 但不影响过去已注册的会员的使用</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">密码最小长度</label>
            <asp:TextBox ID="TbRegisterPasswordMinLength" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">0代表不限制</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">密码规则限制</label>
            <asp:DropDownList ID="DdlRegisterPasswordRestriction" class="form-control" runat="server"></asp:DropDownList>
          </div>

          <div class="form-group">
            <label class="col-form-label">同一IP注册间隔限制
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbRegisterMinMinutesOfIpAddress" ValidationExpression="[^']+"
                errorMessage=" *" foreColor="red" display="Dynamic" />
            </label>
            <asp:TextBox ID="TbRegisterMinMinutesOfIpAddress" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">同一IP在本时间间隔内将只能注册一个帐号，0 为不限制</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">是否开启登录失败锁定</label>
            <asp:RadioButtonList ID="RblIsFailToLock" class="radio radio-primary" OnSelectedIndexChanged="RblIsFailToLock_SelectedIndexChanged"
              runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
            </asp:RadioButtonList>
          </div>

          <asp:PlaceHolder ID="PhFailToLock" runat="server">
            <div class="form-group">
              <label class="col-form-label">登录失败次数锁定（次）
                <asp:RequiredFieldValidator ControlToValidate="TbLoginFailCount" runat="server" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
              </label>
              <asp:TextBox ID="TbLoginFailCount" class="form-control" runat="server"></asp:TextBox>
              <small class="form-text text-muted">一旦登录失败达到指定次数之后用户就会被锁定</small>
            </div>

            <div class="form-group">
              <label class="col-form-label">用户登录锁定类型</label>
              <asp:DropDownList ID="DdlLockType" class="form-control" OnSelectedIndexChanged="DdlLockType_SelectedIndexChanged" runat="server"
                AutoPostBack="true"></asp:DropDownList>
            </div>

            <asp:PlaceHolder ID="PhLockingTime" runat="server">
              <div class="form-group">
                <label class="col-form-label">登录锁定时间（小时）
                  <asp:RequiredFieldValidator ControlToValidate="TbLockingTime" runat="server" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                </label>
                <asp:TextBox ID="TbLockingTime" class="form-control" runat="server"></asp:TextBox>
              </div>
            </asp:PlaceHolder>
          </asp:PlaceHolder>

          <div class="form-group">
            <label class="col-form-label">是否开启找回密码功能</label>
            <asp:RadioButtonList ID="RblIsFindPassword" class="radio radio-primary" OnSelectedIndexChanged="RblIsFindPassword_SelectedIndexChanged"
              runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
            </asp:RadioButtonList>
          </div>

          <asp:PlaceHolder ID="PhFindPassword" runat="server">
            <div class="form-group">
              <label class="col-form-label">找回密码发送验证码短信模板Id
                <asp:RequiredFieldValidator ControlToValidate="TbFindPasswordSmsTplId" runat="server" ErrorMessage="*" foreColor="Red"></asp:RequiredFieldValidator>
              </label>
              <asp:TextBox ID="TbFindPasswordSmsTplId" class="form-control" runat="server"></asp:TextBox>
              <small class="form-text text-muted">需进入短信供应商模板管理界面，添加验证码类短信模板并获取模板Id</small>
            </div>
          </asp:PlaceHolder>

          <hr />

          <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->