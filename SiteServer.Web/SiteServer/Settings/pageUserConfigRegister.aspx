<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUserConfigRegister" %>
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
            <li class="nav-item active">
              <a class="nav-link" href="pageUserConfigRegister.aspx">用户注册设置</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUserConfigLogin.aspx">用户登录设置</a>
            </li>
          </ul>
        </div>

        <div class="card-box">

          <div class="form-group">
            <label class="col-form-label">允许新用户注册</label>
            <asp:RadioButtonList ID="RblIsRegisterAllowed" class="radio radio-primary" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList>
            </asp:RadioButtonList>
            <small class="form-text text-muted">选择否将禁止新用户注册, 但不影响过去已注册的会员的使用</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">注册密码最小长度</label>
            <asp:TextBox ID="TbRegisterPasswordMinLength" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">0代表不限制</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">注册密码规则限制</label>
            <asp:DropDownList ID="DdlRegisterPasswordRestriction" class="form-control" runat="server"></asp:DropDownList>
          </div>

          <div class="form-group">
            <label class="col-form-label">新用户注册验证</label>
            <asp:DropDownList ID="DdlRegisterVerifyType" class="form-control" OnSelectedIndexChanged="DdlRegisterVerifyType_SelectedIndexChanged"
              runat="server" AutoPostBack="true"></asp:DropDownList>
            <small class="form-text text-muted">选择短信验证将向用户发送短信验证码以确认手机号码，此选项需要开启短信发送功能</small>
          </div>

          <asp:PlaceHolder ID="PhRegisterSms" runat="server">
            <div class="form-group">
              <label class="col-form-label">发送验证码短信模板Id
                <asp:RequiredFieldValidator ControlToValidate="TbRegisterSmsTplId" runat="server" ErrorMessage="*"
                  foreColor="Red"></asp:RequiredFieldValidator>
              </label>
              <asp:TextBox ID="TbRegisterSmsTplId" class="form-control" runat="server"></asp:TextBox>
              <small class="form-text text-muted">需进入短信供应商模板管理界面，添加验证码类短信模板并获取模板Id</small>
            </div>
          </asp:PlaceHolder>

          <div class="form-group">
            <label class="col-form-label">同一IP注册间隔限制
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbRegisterMinMinutesOfIpAddress" ValidationExpression="[^']+"
                errorMessage=" *" foreColor="red" display="Dynamic" />
            </label>
            <asp:TextBox ID="TbRegisterMinMinutesOfIpAddress" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">同一IP在本时间间隔内将只能注册一个帐号，0 为不限制</small>
          </div>

          <hr />

          <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />

        </div>

      </form>
    </body>

    </html>