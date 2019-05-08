<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalUserView" Trace="false"%>
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
      <label class="col-2 text-right col-form-label">用户Id</label>
      <div class="col-4 form-control-plaintext">
        <asp:Literal id="LtlUserId" runat="server" />
      </div>
      <label class="col-2 text-right col-form-label">用户名</label>
      <div class="col-4 form-control-plaintext">
        <asp:Literal id="LtlUserName" runat="server" />
      </div>
    </div>

    <div class="form-group form-row">
      <label class="col-2 text-right col-form-label">注册时间</label>
      <div class="col-4 form-control-plaintext">
        <asp:Literal id="LtlCreateDate" runat="server" />
      </div>
      <label class="col-2 text-right col-form-label">登录次数</label>
      <div class="col-4 form-control-plaintext">
        <asp:Literal id="LtlLoginCount" runat="server" />
      </div>
    </div>

    <div class="form-group form-row">
      <label class="col-2 text-right col-form-label">最后登录时间</label>
      <div class="col-4 form-control-plaintext">
        <asp:Literal id="LtlLastActivityDate" runat="server" />
      </div>
      <label class="col-2 text-right col-form-label">最后修改密码时间</label>
      <div class="col-4 form-control-plaintext">
        <asp:Literal id="LtlLastResetPasswordDate" runat="server" />
      </div>
    </div>

    <asp:Literal id="LtlAttributes" runat="server" />

    <hr />

    <div class="text-right mr-1">
      <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">关 闭</button>
    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->