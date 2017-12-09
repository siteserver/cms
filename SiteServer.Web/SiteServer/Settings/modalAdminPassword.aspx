<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalAdminPassword" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">用户名</label>
            <div class="col-xs-5">
              <asp:Label id="LbUserName" runat="server" />
            </div>
            <div class="col-xs-4 help-block"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">新密码</label>
            <div class="col-xs-5">
              <asp:TextBox TextMode="Password" id="TbPassword" class="form-control" runat="server" />
            </div>
            <div class="col-xs-4 help-block">
              输入需要设置的新密码
              <asp:RequiredFieldValidator ControlToValidate="TbPassword" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
              />
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">再次输入新密码</label>
            <div class="col-xs-5">
              <asp:TextBox TextMode="Password" id="TbConfirmPassword" class="form-control" runat="server" />
            </div>
            <div class="col-xs-4 help-block">
              <asp:RequiredFieldValidator ControlToValidate="TbConfirmPassword" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
              />
              <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="TbPassword" ControlToValidate="TbConfirmPassword"
                Display="Dynamic" ErrorMessage=" 两次输入的密码不一致！" foreColor="red"></asp:CompareValidator>
            </div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>