<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalAdminPassword" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">用户名</label>
          <div class="col-5 form-control-plaintext">
            <asp:Label id="LbUserName" runat="server" />
          </div>
          <div class="col-4 help-block"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">新密码</label>
          <div class="col-5">
            <asp:TextBox TextMode="Password" id="TbPassword" class="form-control" runat="server" />
          </div>
          <div class="col-4">
            <small class="form-text text-muted">输入需要设置的新密码</small>
            <asp:RequiredFieldValidator ControlToValidate="TbPassword" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
            />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">再次输入新密码</label>
          <div class="col-5">
            <asp:TextBox TextMode="Password" id="TbConfirmPassword" class="form-control" runat="server" />
          </div>
          <div class="col-4 help-block">
            <asp:RequiredFieldValidator ControlToValidate="TbConfirmPassword" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
            />
            <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="TbPassword" ControlToValidate="TbConfirmPassword"
              Display="Dynamic" ErrorMessage=" 两次输入的密码不一致！" foreColor="red"></asp:CompareValidator>
          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->