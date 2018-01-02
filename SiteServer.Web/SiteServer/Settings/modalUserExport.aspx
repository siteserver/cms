<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Settings.ModalUserExport" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts text="在此导出用户数据至Excel中" runat="server" />

        <div class="form-horizontal">

          <asp:PlaceHolder ID="PhExport" runat="server">
            <div class="form-group">
              <label class="col-xs-3 text-right control-label">用户类型</label>
              <div class="col-xs-7">
                <asp:DropDownList ID="DdlCheckedState" runat="server" class="form-control"></asp:DropDownList>
              </div>
              <div class="col-xs-2">

              </div>
            </div>
          </asp:PlaceHolder>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button id="BtnSubmit" class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>