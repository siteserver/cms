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

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">内容标题</label>
            <div class="col-xs-8">
              <asp:Literal ID="LtlTitles" runat="server"></asp:Literal>
            </div>
            <div class="col-xs-1"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">审核状态</label>
            <div class="col-xs-8">
              <asp:DropDownList ID="DdlCheckType" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-1"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">转移到栏目</label>
            <div class="col-xs-8">
              <asp:DropDownList ID="DdlTranslateNodeId" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-1"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">审核原因</label>
            <div class="col-xs-8">
              <asp:TextBox ID="TbCheckReasons" class="form-control" TextMode="MultiLine" Rows="3" runat="server" />
            </div>
            <div class="col-xs-1"></div>
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