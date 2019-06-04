<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentTidyUp" Trace="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts text="根据添加日期(或内容ID)重新排序(不可逆,请慎重)。" runat="server" />

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">排序字段</label>
          <div class="col-8">
            <asp:DropDownList ID="DdlAttributeName" class="form-control" runat="server"></asp:DropDownList>
          </div>
          <div class="col-1"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">排序方向</label>
          <div class="col-8">
            <asp:DropDownList ID="DdlIsDesc" class="form-control" runat="server"></asp:DropDownList>
          </div>
          <div class="col-1"></div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->