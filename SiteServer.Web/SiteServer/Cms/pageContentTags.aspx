<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentTags" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <style type="text/css">
        .tag_popularity_1 {
          FONT-SIZE: 12px;
          font-weight: normal !important;
          COLOR: #104d6c;
        }

        .tag_popularity_2 {
          FONT-WEIGHT: bold;
          COLOR: #104d6c;
        }

        .tag_popularity_3 {
          FONT-WEIGHT: bold;
          COLOR: #ff0f6f;
          font-size: 14px !important;
        }

        .tag_popularity_4 {
          FONT-WEIGHT: bold;
          font-size: 16px !important;
          COLOR: #ff0f6f !important
        }
      </style>
    </head>

    <body>
      <form class="container" runat="server">
        <bairong:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>内容标签管理</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此管理内容标签
            </p>

            <ul class="nav nav-pills m-b-30">
              <li class="">
                <a href="pageNodeGroup.aspx?publishmentSystemId=<%=PublishmentSystemId%>">栏目组管理</a>
              </li>
              <li class="">
                <a href="pageContentGroup.aspx?publishmentSystemId=<%=PublishmentSystemId%>">内容组管理</a>
              </li>
              <li class="active">
                <a href="javascript:;">内容标签管理</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" AllowPaging="true" OnPageIndexChanged="MyDataGrid_Page"
                HeaderStyle-CssClass="info thead text-center" CssClass="table table-hover m-0" gridlines="none" runat="server">
                <PagerStyle Mode="NumericPages" PageButtonCount="10" HorizontalAlign="Right" />
                <Columns>
                  <asp:TemplateColumn HeaderText="标签">
                    <ItemTemplate>
                      <asp:Literal ID="ltlTagName" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="使用次数">
                    <ItemTemplate>
                      <asp:Literal ID="ltlCount" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="80" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate> &nbsp;
                      <asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="80" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="80" cssClass="text-center" />
                  </asp:TemplateColumn>
                </Columns>
              </asp:dataGrid>

              <br />

              <table class="table m-0">
                <tr>
                  <td>
                    <asp:LinkButton ID="LbPageFirst" OnClick="NavigationButtonClick" CommandName="FIRST" Runat="server">首页</asp:LinkButton>
                    |
                    <asp:LinkButton ID="LbPagePrevious" OnClick="NavigationButtonClick" CommandName="PREVIOUS" Runat="server">前页</asp:LinkButton>
                    |
                    <asp:LinkButton ID="LbPageNext" OnClick="NavigationButtonClick" CommandName="NEXT" Runat="server">后页</asp:LinkButton>
                    |
                    <asp:LinkButton ID="LbPageLast" OnClick="NavigationButtonClick" CommandName="LAST" Runat="server">尾页</asp:LinkButton>
                  </td>
                  <td class="text-right"> 分页
                    <asp:Literal runat="server" ID="LtlCurrentPage" />
                  </td>
                </tr>
              </table>

              <hr />

              <div class="form-group m-b-0">
                <asp:Button class="btn btn-primary m-l-15" id="AddTag" Text="添加标签" runat="server" />
              </div>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>