<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUserConfigLogin" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <ul class="nav nav-pills">
            <li class="nav-item">
              <a class="nav-link" href="pageUser.aspx">用户管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUserCheck.aspx">审核新用户</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUserConfigRegister.aspx">用户注册设置</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageUserConfigLogin.aspx">用户登录设置</a>
            </li>
          </ul>
        </div>

        <div class="card-box">

          <div class="form-group">
            <label class="col-form-label">是否开启失败锁定</label>
            <asp:RadioButtonList ID="RblIsFailToLock" class="radio radio-primary" OnSelectedIndexChanged="RblIsFailToLock_SelectedIndexChanged"
              runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
            </asp:RadioButtonList>
          </div>

          <asp:PlaceHolder ID="PhFailToLock" runat="server">
            <div class="form-group">
              <label class="col-form-label">失败次数锁定（次）
                <asp:RequiredFieldValidator ControlToValidate="TbLoginFailCount" runat="server" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
              </label>
              <asp:TextBox ID="TbLoginFailCount" class="form-control" runat="server"></asp:TextBox>
              <small class="form-text text-muted">一旦登录失败达到指定次数之后用户就会被锁定</small>
            </div>

            <div class="form-group">
              <label class="col-form-label">用户锁定类型</label>
              <asp:DropDownList ID="DdlLockType" class="form-control" OnSelectedIndexChanged="DdlLockType_SelectedIndexChanged" runat="server"
                AutoPostBack="true"></asp:DropDownList>
            </div>

            <asp:PlaceHolder ID="PhLockingTime" runat="server">
              <div class="form-group">
                <label class="col-form-label">锁定时间（小时）
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
              <label class="col-form-label">发送验证码短信模板Id
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