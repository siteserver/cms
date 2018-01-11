<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalAdminView" %>
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
          <label class="col-3 text-right col-form-label">账号</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlUserName" runat="server" />
          </div>
          <div class="col-1 help-block"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">姓名</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlDisplayName" runat="server" />
          </div>
          <div class="col-1 help-block"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">创建时间</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlCreationDate" runat="server" />
          </div>
          <div class="col-1 help-block"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">最后登录时间</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlLastActivityDate" runat="server" />
          </div>
          <div class="col-1 help-block"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">电子邮箱</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlEmail" runat="server" />
          </div>
          <div class="col-1 help-block"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">手机号码</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlMobile" runat="server" />
          </div>
          <div class="col-1 help-block"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">角色</label>
          <div class="col-8 form-control-plaintext">
            <asp:Literal id="LtlRoles" runat="server" />
          </div>
          <div class="col-1 help-block"></div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">关 闭</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->