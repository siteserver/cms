<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalConfigurationCreateChannel" Trace="false"%>
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
            <label class="col-xs-3 control-label text-right">当内容变动时是否生成本栏目</label>
            <div class="col-xs-8">
              <asp:DropDownList ID="DdlIsCreateChannelIfContentChanged" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-1"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">选择内容变动时需要生成的栏目</label>
            <div class="col-xs-8">
              <asp:ListBox ID="LbNodeId" class="form-control" SelectionMode="Multiple" Rows="13" runat="server"></asp:ListBox>
            </div>
            <div class="col-xs-1"></div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" ID="BtnCheck" Text="审 核" OnClick="Submit_OnClick" runat="server" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>