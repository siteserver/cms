<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageChannel" enableViewState="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form class="container" runat="server">
        <ctrl:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>栏目管理</b>
            </h4>
            <p class="text-muted font-13 m-b-25"></p>

            <div class="form-horizontal">

              <table class="table table-hover m-0">
                <tr class="info thead text-center">
                  <td>栏目名</td>
                  <td width="300">所属栏目组</td>
                  <td width="100">栏目索引</td>
                  <td width="50">上升</td>
                  <td width="50">下降</td>
                  <td width="50">&nbsp;</td>
                  <td width="20"></td>
                </tr>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <asp:Literal id="ltlHtml" runat="server" />
                  </itemtemplate>
                </asp:Repeater>
              </table>



              <hr />

              <div class="form-group m-b-0">
                <asp:PlaceHolder id="PhAddChannel" runat="server">
                  <asp:Button class="btn btn-default m-l-10" id="BtnAddChannel1" Text="快速添加" runat="server" />
                  <asp:Button class="btn btn-default m-l-10" id="BtnAddChannel2" Text="添加栏目" runat="server" />
                </asp:PlaceHolder>
                <asp:PlaceHolder id="PhChannelEdit" runat="server">
                  <asp:Button class="btn btn-default m-l-10" id="BtnAddToGroup" Text="设置栏目组" runat="server" />
                  <asp:Button class="btn btn-default m-l-10" id="BtnSelectEditColumns" Text="编辑项" runat="server" />
                </asp:PlaceHolder>
                <asp:PlaceHolder id="PhTranslate" runat="server">
                  <asp:Button class="btn btn-default m-l-10" id="BtnTranslate" Text="转 移" runat="server" />
                </asp:PlaceHolder>
                <asp:PlaceHolder id="PhImport" runat="server">
                  <asp:Button class="btn btn-default m-l-10" id="BtnImport" Text="导 入" runat="server" />
                </asp:PlaceHolder>
                <asp:Button class="btn btn-default m-l-10" id="BtnExport" Text="导 出" runat="server" />
                <asp:PlaceHolder id="PhDelete" runat="server">
                  <asp:Button class="btn btn-default m-l-10" id="BtnDelete" Text="删 除" runat="server" />
                </asp:PlaceHolder>
                <asp:PlaceHolder id="PhCreate" runat="server">
                  <asp:Button class="btn btn-default m-l-10" id="BtnCreate" Text="生 成" runat="server" />
                </asp:PlaceHolder>
              </div>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>