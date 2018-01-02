<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalCheckState" Trace="false" %>
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
              <asp:Literal ID="LtlTitle" runat="server"></asp:Literal>
            </div>
            <div class="col-xs-1"></div>
          </div>

          <div class="form-group">
            <label class="col-xs-3 control-label text-right">审核状态</label>
            <div class="col-xs-8">
              <asp:Literal ID="LtlState" runat="server"></asp:Literal>
            </div>
            <div class="col-xs-1"></div>
          </div>

          <asp:PlaceHolder ID="PhCheckReasons" runat="server" Visible="false">
            <table class="table table-hover">
              <tr class="head info">
                <td>审核人</td>
                <td>审核时间</td>
                <td>原因</td>
              </tr>
              <asp:Repeater ID="RptContents" runat="server">
                <ItemTemplate>
                  <tr>
                    <td>
                      <asp:Literal ID="ltlUserName" runat="server"></asp:Literal>
                    </td>
                    <td>
                      <asp:Literal ID="ltlCheckDate" runat="server"></asp:Literal>
                    </td>
                    <td>
                      <asp:Literal ID="ltlReasons" runat="server"></asp:Literal>
                    </td>
                  </tr>
                </ItemTemplate>
              </asp:Repeater>
            </table>
          </asp:PlaceHolder>

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