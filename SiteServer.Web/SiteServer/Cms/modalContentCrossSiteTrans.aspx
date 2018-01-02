<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentCrossSiteTrans" Trace="false"%>
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

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">选择站点</label>
            <div class="col-xs-8">
              <asp:DropDownList ID="DdlPublishmentSystemId" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlPublishmentSystemId_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-xs-1"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">转发到</label>
            <div class="col-xs-8">
              <asp:ListBox ID="LbNodeId" class="form-control" style="height:200px;" SelectionMode="Multiple" runat="server"></asp:ListBox>
            </div>
            <div class="col-xs-1">
              <asp:RequiredFieldValidator ControlToValidate="LbNodeId" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
              />
            </div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" Text="确 定" OnClick="Submit_OnClick" runat="server" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>