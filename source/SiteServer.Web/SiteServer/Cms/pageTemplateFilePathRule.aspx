<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateFilePathRule" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form class="container" runat="server">
        <bairong:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>页面命名规则</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此可设置各栏目的生成页面命名规则，或者指定对应栏目的生成页面地址
            </p>

            <ul class="nav nav-pills m-b-30">
              <li class="active">
                <a href="javascript:;">页面命名规则</a>
              </li>
              <li class="">
                <a href="pageConfigurationCreate.aspx?publishmentSystemId=<%=PublishmentSystemId%>">页面生成设置</a>
              </li>
              <li class="">
                <a href="pageConfigurationCreateTrigger.aspx?publishmentSystemId=<%=PublishmentSystemId%>">页面生成触发器</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <table class="table table-hover m-0">
                <tr class="info thead">
                  <td>栏目名</td>
                  <td>页面路径</td>
                  <td class="center" style="width:50px;">&nbsp;</td>
                </tr>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <asp:Literal id="ltlHtml" runat="server" />
                  </itemtemplate>
                </asp:Repeater>
              </table>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>