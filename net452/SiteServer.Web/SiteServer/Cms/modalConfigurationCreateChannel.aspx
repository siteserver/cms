<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalConfigurationCreateChannel" Trace="false"%>
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
          <label class="col-3 col-form-label text-right">当内容变动时是否生成本栏目</label>
          <div class="col-9">
            <asp:DropDownList ID="DdlIsCreateChannelIfContentChanged" class="form-control" runat="server"></asp:DropDownList>
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 col-form-label text-right">选择内容变动时需要生成的栏目</label>
          <div class="col-9">
            <asp:ListBox ID="LbChannelId" class="form-control" SelectionMode="Multiple" Rows="13" runat="server"></asp:ListBox>
          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" ID="BtnCheck" Text="确 定" OnClick="Submit_OnClick" runat="server" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->