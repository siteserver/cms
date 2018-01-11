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

        <asp:PlaceHolder ID="PhExport" runat="server">
          <div class="form-group form-row">
            <label class="col-3 text-right col-form-label">用户类型</label>
            <div class="col-7">
              <asp:DropDownList ID="DdlCheckedState" runat="server" class="form-control"></asp:DropDownList>
            </div>
            <div class="col-2">

            </div>
          </div>
        </asp:PlaceHolder>

        <hr />

        <div class="text-right mr-1">
          <asp:Button id="BtnSubmit" class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->