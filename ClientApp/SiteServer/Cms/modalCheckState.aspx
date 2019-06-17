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

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">内容标题</label>
          <div class="col-10 form-control-plaintext">
            <asp:Literal ID="LtlTitle" runat="server"></asp:Literal>
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">审核状态</label>
          <div class="col-4 form-control-plaintext">
            <asp:Literal ID="LtlState" runat="server"></asp:Literal>
          </div>
        </div>

        <asp:PlaceHolder ID="PhCheckReasons" runat="server" Visible="false">
          <table class="table tablesaw table-hover m-0">
            <thead>
              <tr class="thead">
                <th>审核人</th>
                <th>审核时间</th>
                <th>原因</th>
              </tr>
            </thead>
            <tbody>
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
            </tbody>
          </table>
        </asp:PlaceHolder>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" ID="BtnCheck" Text="审 核" OnClick="Submit_OnClick" runat="server" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->