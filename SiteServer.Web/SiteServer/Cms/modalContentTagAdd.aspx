<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentTagAdd" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
  <!DOCTYPE html>
  <html class="modalPage">

  <head>
    <meta charset="utf-8">
    <!--#include file="../inc/head.html"-->
  </head>

  <body>
    <!--#include file="../inc/openWindow.html"-->

    <form runat="server">
      <bairong:alerts runat="server" />

      <div class="form-horizontal">

        <div class="form-group">
          <label class="col-xs-3 text-right control-label">标签</label>
          <div class="col-xs-8">
              <asp:TextBox class="form-control" Columns="35" Rows="4" TextMode="MultiLine" id="TbTags" runat="server" />
              <div class="help-block">
                  多个标签请用英文逗号（,）分开
              </div>
          </div>
          <div class="col-xs-1 help-block">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTags"
              ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
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