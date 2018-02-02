<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageChannel" enableViewState="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            栏目管理
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="panel panel-default">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table class="tablesaw table table-hover m-b-0 tablesaw-stack">
                  <thead>
                    <tr>
                      <th>栏目名</th>
                      <th class"text-nowrap">所属栏目组</th>
                      <th class="text-nowrap">栏目索引</th>
                      <th width="60">上升</th>
                      <th width="60">下降</th>
                      <th width="60">&nbsp;</th>
                      <th width="20"></th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <asp:Literal id="ltlHtml" runat="server" />
                      </itemtemplate>
                    </asp:Repeater>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <hr />

          <asp:PlaceHolder id="PhAddChannel" runat="server">
            <asp:Button class="btn m-r-5" id="BtnAddChannel1" Text="快速添加" runat="server" />
            <asp:Button class="btn m-r-5" id="BtnAddChannel2" Text="添加栏目" runat="server" />
          </asp:PlaceHolder>
          <asp:PlaceHolder id="PhChannelEdit" runat="server">
            <asp:Button class="btn m-r-5" id="BtnAddToGroup" Text="设置栏目组" runat="server" />
            <asp:Button class="btn m-r-5" id="BtnSelectEditColumns" Text="编辑项" runat="server" />
          </asp:PlaceHolder>
          <asp:PlaceHolder id="PhTranslate" runat="server">
            <asp:Button class="btn m-r-5" id="BtnTranslate" Text="转 移" runat="server" />
          </asp:PlaceHolder>
          <asp:PlaceHolder id="PhImport" runat="server">
            <asp:Button class="btn m-r-5" id="BtnImport" Text="导 入" runat="server" />
          </asp:PlaceHolder>
          <asp:Button class="btn m-r-5" id="BtnExport" Text="导 出" runat="server" />
          <asp:PlaceHolder id="PhDelete" runat="server">
            <asp:Button class="btn m-r-5" id="BtnDelete" Text="删 除" runat="server" />
          </asp:PlaceHolder>
          <asp:PlaceHolder id="PhCreate" runat="server">
            <asp:Button class="btn m-r-5" id="BtnCreate" Text="生 成" runat="server" />
          </asp:PlaceHolder>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->