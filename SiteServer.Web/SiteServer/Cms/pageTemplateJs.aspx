<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateJs" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts text="脚本文件在站点目录 js 中，模板中使用 &amp;lt;script type=&quot;text/javascript&quot; src=&quot;{stl.siteurl}/js/脚本文件.js&quot;&gt;&amp;lt;/script&gt; 引用。"
          runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            脚本文件管理
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="panel panel-default m-t-10">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table class="table tablesaw table-hover m-0">
                  <thead>
                    <tr>
                      <th>文件名称 </th>
                      <th>文件编码 </th>
                      <th width="80" class="text-center"></th>
                      <th width="80" class="text-center"></th>
                      <th width="80" class="text-center"></th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlFileName" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlCharset" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlView" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlEdit" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlDelete" runat="server"></asp:Literal>
                          </td>
                        </tr>
                      </itemtemplate>
                    </asp:Repeater>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <hr />

          <asp:Button class="btn btn-primary" id="BtnAdd" Text="新增文件" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->