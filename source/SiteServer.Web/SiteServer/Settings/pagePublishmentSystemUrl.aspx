<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PagePublishmentSystemUrl" enableViewState = "false" %>
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
            <h4 class="m-t-0 header-title"><b>访问地址管理</b></h4>
            <p class="text-muted font-13 m-b-25">
              在此设置站点的访问地址
            </p>

            <ul class="nav nav-pills m-b-30">
                <li class="active">
                    <a href="javascript:;">站点访问地址</a>
                </li>
                <li class="">
                    <a href="pageConfig.aspx">全局设置</a>
                </li>
            </ul>

            <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover"
            runat="server">
            <Columns>
              <asp:TemplateColumn HeaderText="站点名称">
                <ItemTemplate>
                  <asp:Literal ID="ltlName" runat="server"></asp:Literal>
                </ItemTemplate>
              </asp:TemplateColumn>
              <asp:TemplateColumn HeaderText="文件夹">
                <ItemTemplate>
                  <asp:Literal ID="ltlDir" runat="server"></asp:Literal>
                </ItemTemplate>
              </asp:TemplateColumn>
              <asp:TemplateColumn HeaderText="Web访问地址">
                <ItemTemplate>
                  <asp:Literal ID="ltlWebUrl" runat="server"></asp:Literal>
                </ItemTemplate>
              </asp:TemplateColumn>
              <asp:TemplateColumn>
                <ItemTemplate>
                  <asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
                </ItemTemplate>
                <ItemStyle Width="50" cssClass="center" />
              </asp:TemplateColumn>
            </Columns>
          </asp:dataGrid>

          </div>
        </div>

      </form>
    </body>

    </html>