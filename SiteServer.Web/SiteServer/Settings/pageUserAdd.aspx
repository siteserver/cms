<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUserAdd" %>
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
            <li class="nav-item active">
              <a class="nav-link" href="pageUser.aspx">用户管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUserCheck.aspx">审核新用户</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUserConfiguration.aspx">用户设置</a>
            </li>
          </ul>
        </div>

        <div class="card-box">
          <div class="m-t-0 header-title">
            <asp:Literal ID="LtlPageTitle" runat="server" />
          </div>

          <div class="form-group">
            <label class="col-form-label">账号
              <asp:RequiredFieldValidator ControlToValidate="TbUserName" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
            </label>
            <asp:TextBox ID="TbUserName" cssClass="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">帐号用于登录系统，由字母、数字组成</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">姓名
              <asp:RequiredFieldValidator ControlToValidate="TbDisplayName" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
            </label>
            <asp:TextBox ID="TbDisplayName" class="form-control" runat="server"></asp:TextBox>
          </div>

          <asp:PlaceHolder ID="PhPassword" runat="server">
            <div class="form-group">
              <label class="col-form-label">密码
                <asp:RequiredFieldValidator ControlToValidate="TbPassword" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                />
              </label>
              <asp:TextBox ID="TbPassword" TextMode="Password" class="form-control" runat="server"></asp:TextBox>
              <small class="form-text text-muted">
                <asp:Literal ID="LtlPasswordTips" runat="server" />
              </small>
            </div>

            <div class="form-group">
              <label class="col-form-label">确认密码
                <asp:RequiredFieldValidator ControlToValidate="tbConfirmPassword" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                />
                <asp:CompareValidator ID="tbNewPasswordCompare" runat="server" ControlToCompare="TbPassword" ControlToValidate="tbConfirmPassword"
                  Display="Dynamic" ErrorMessage=" 两次输入的密码不一致！请再输入一遍您上面填写的密码。" ForeColor="red"></asp:CompareValidator>
              </label>
              <asp:TextBox ID="tbConfirmPassword" TextMode="Password" class="form-control" runat="server"></asp:TextBox>
            </div>
          </asp:PlaceHolder>

          <div class="form-group">
            <label class="col-form-label">电子邮箱
              <asp:RegularExpressionValidator ControlToValidate="TbEmail" ValidationExpression="(\w[0-9a-zA-Z_-]*@(\w[0-9a-zA-Z_-]*\.)+\w{2,})"
                ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
            </label>
            <asp:TextBox ID="TbEmail" class="form-control" runat="server"></asp:TextBox>
          </div>

          <div class="form-group">
            <label class="col-form-label">手机号码
              <asp:RegularExpressionValidator ControlToValidate="TbMobile" ValidationExpression="^(13|15|18)\d{9}$" ErrorMessage=" *" ForeColor="red"
                Display="Dynamic" runat="server" />
            </label>
            <asp:TextBox ID="TbMobile" class="form-control" runat="server"></asp:TextBox>
          </div>

          <hr />

          <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />
          <asp:Button id="BtnReturn" class="btn" text="返 回" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->