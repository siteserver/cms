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

        <div class="form-group form-row">
          <label class="col-3 col-form-label text-right">选择站点</label>
          <div class="col-8">
            <asp:DropDownList ID="DdlSiteId" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlSiteId_SelectedIndexChanged"></asp:DropDownList>
          </div>
          <div class="col-1"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 col-form-label text-right">转发到</label>
          <div class="col-8">
            <asp:ListBox ID="LbChannelId" class="form-control" style="height:200px;" SelectionMode="Multiple" runat="server"></asp:ListBox>
          </div>
          <div class="col-1">
            <asp:RequiredFieldValidator ControlToValidate="LbChannelId" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
            />
          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" Text="确 定" OnClick="Submit_OnClick" runat="server" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->