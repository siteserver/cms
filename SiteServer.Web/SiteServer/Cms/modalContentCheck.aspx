<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentCheck" Trace="false" %>
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
            <label class="col-3 col-form-label text-right">内容标题</label>
            <div class="col-8">
              <h6><asp:Literal ID="LtlTitles" runat="server"></asp:Literal></h6>
            </div>
            <div class="col-1"></div>
          </div>

          <div class="form-group form-row">
            <label class="col-3 col-form-label text-right">审核状态</label>
            <div class="col-8">
              <asp:DropDownList ID="DdlCheckType" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-1"></div>
          </div>

          <div class="form-group form-row">
            <label class="col-3 col-form-label text-right">转移到栏目</label>
            <div class="col-8">
              <asp:DropDownList ID="DdlTranslateChannelId" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-1"></div>
          </div>

          <div class="form-group form-row">
            <label class="col-3 col-form-label text-right">审核原因</label>
            <div class="col-8">
              <asp:TextBox ID="TbCheckReasons" class="form-control" TextMode="MultiLine" Rows="3" runat="server" />
            </div>
            <div class="col-1"></div>
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