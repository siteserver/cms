<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PagePublishmentSystemUrlAssets" enableViewState = "false" %>
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
              <b>访问地址管理</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此设置站点资源文件的访问地址
            </p>

            <ul class="nav nav-pills m-b-30">
              <li>
                <a href="pagePublishmentSystemUrlWeb.aspx">Web访问地址</a>
              </li>
              <li class="active">
                <a href="javascript:;">资源文件访问地址</a>
              </li>
              <li>
                <a href="pagePublishmentSystemUrlApi.aspx">API访问地址</a>
              </li>
            </ul>

            <div class="form-horizontal">
              <asp:dataGrid id="DgContents" borderWidth="0" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead"
                CssClass="table table-hover m-0" runat="server">
                <Columns>
                  <asp:TemplateColumn HeaderText="站点名称">
                    <ItemTemplate>
                      <asp:Literal ID="ltlName" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle borderWidth="0" />
                    <HeaderStyle borderWidth="0" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="文件夹">
                    <ItemTemplate>
                      <asp:Literal ID="ltlDir" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle borderWidth="0" />
                    <HeaderStyle borderWidth="0" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="资源文件存储文件夹">
                    <ItemTemplate>
                      <asp:Literal ID="ltlAssetsDir" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle borderWidth="0" />
                    <HeaderStyle borderWidth="0" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="资源文件访问地址">
                    <ItemTemplate>
                      <asp:Literal ID="ltlAssetsUrl" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle borderWidth="0" />
                    <HeaderStyle borderWidth="0" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="100" borderWidth="0" cssClass="center" />
                    <HeaderStyle borderWidth="0" />
                  </asp:TemplateColumn>
                </Columns>
              </asp:dataGrid>
            </div>

          </div>
        </div>

      </form>
    </body>

    </html>