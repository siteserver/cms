<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalSelectFile" Trace="false" %>
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

        <div class="row">
          <div class="col-6">
            <div class="btn-group">
              <asp:Button class="btn" ID="BtnUpload" runat="server" text="上传"></asp:Button>
              <asp:Button class="btn" text="后退" runat="server" CommandName="NavigationBar" CommandArgument="Back" OnCommand="LinkButton_Command"></asp:Button>
              <asp:Button class="btn" text="向上" runat="server" CommandName="NavigationBar" CommandArgument="Up" OnCommand="LinkButton_Command"></asp:Button>
              <asp:Button class="btn" text="刷新" runat="server" CommandName="NavigationBar" CommandArgument="Reload" OnCommand="LinkButton_Command"></asp:Button>
            </div>
          </div>
          <div class="col-6">
            <asp:DropDownList ID="DdlListType" class="form-control" runat="server" OnSelectedIndexChanged="ddlListType_SelectedIndexChanged"
              AutoPostBack="true"></asp:DropDownList>
          </div>
        </div>

        <hr />

        <div class="row">
          <div class="col-12">
            <asp:Literal id="LtlCurrentDirectory" runat="server" />
          </div>
        </div>

        <hr />

        <asp:Literal id="LtlFileSystems" runat="server" enableViewState="false" />

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->