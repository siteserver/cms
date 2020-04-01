<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalUserPassword" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts text="" runat="server" />

        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">用户名</label>
          <div class="col-6 form-control-plaintext">
            <asp:Literal id="LtlUserName" runat="server" />
          </div>
          <div class="col-2">

          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">新密码</label>
          <div class="col-6">
            <asp:TextBox TextMode="Password" id="TbPassword" class="form-control" runat="server" />
          </div>
          <div class="col-2">
            <asp:RequiredFieldValidator ControlToValidate="TbPassword" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
            />
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-4 text-right col-form-label">再次输入新密码</label>
          <div class="col-6">
            <asp:TextBox TextMode="Password" id="TbConfirmPassword" class="form-control" runat="server" />
          </div>
          <div class="col-2">
            <asp:RequiredFieldValidator ControlToValidate="TbConfirmPassword" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
            />
            <asp:CompareValidator runat="server" ControlToCompare="TbPassword" ControlToValidate="TbConfirmPassword" Display="Dynamic"
              ErrorMessage=" *" foreColor="red"></asp:CompareValidator>
          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button id="BtnSubmit" class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->