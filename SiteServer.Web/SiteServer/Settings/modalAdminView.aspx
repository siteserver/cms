<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalAdminView" %>
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
            <label class="col-xs-3 text-right control-label">账号</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlUserName" runat="server" />
            </div>
            <div class="col-xs-1 help-block"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">姓名</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlDisplayName" runat="server" />
            </div>
            <div class="col-xs-1 help-block"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">创建时间</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlCreationDate" runat="server" />
            </div>
            <div class="col-xs-1 help-block"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">最后登录时间</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlLastActivityDate" runat="server" />
            </div>
            <div class="col-xs-1 help-block"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">电子邮箱</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlEmail" runat="server" />
            </div>
            <div class="col-xs-1 help-block"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">手机号码</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlMobile" runat="server" />
            </div>
            <div class="col-xs-1 help-block"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">角色</label>
            <div class="col-xs-8">
              <asp:Literal id="LtlRoles" runat="server" />
            </div>
            <div class="col-xs-1 help-block"></div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">关 闭</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>