<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalAdminAccessToken" %>
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
          <label class="col-3 text-right col-form-label">名称</label>
          <div class="col-5 form-control-plaintext">
            <asp:Literal id="LtlTitle" runat="server" />
          </div>
          <div class="col-4 help-block"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">API密钥</label>
          <div class="col-5 form-control-plaintext">
            <code style="font-size: 16px"><asp:Literal id="LtlToken" runat="server" /></code>
          </div>
          <div class="col-4 help-block">
            <asp:Button id="BtnRegenerate" class="btn btn-success m-l-5" text="重 设" runat="server" onClick="Regenerate_OnClick"
            />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label"></label>
          <div class="col-5 form-control-plaintext">
            添加时间：
            <asp:Literal id="LtlAddDate" runat="server" /> 更新时间：
            <asp:Literal id="LtlUpdatedDate" runat="server" />
          </div>
          <div class="col-4 help-block"></div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">关 闭</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->