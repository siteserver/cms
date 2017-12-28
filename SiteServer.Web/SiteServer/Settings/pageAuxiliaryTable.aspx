<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAuxiliaryTable" %>
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

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>辅助表管理</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此管理内容辅助表
            </p>

            <div class="form-horizontal">

              <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead text-center"
                CssClass="table table-hover m-0" gridlines="none" runat="server">
                <Columns>
                  <asp:TemplateColumn HeaderText="辅助表标识">
                    <ItemTemplate>
                      <asp:Literal id="ltlTableName" runat="server" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="辅助表名称">
                    <ItemTemplate>
                      <asp:Literal id="ltlTableCnName" runat="server" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="是否被使用">
                    <ItemTemplate>
                      <asp:Literal id="ltlIsUsed" runat="server" />
                      </span>
                    </ItemTemplate>
                    <ItemStyle Width="120" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="是否存在">
                    <ItemTemplate>
                      <asp:Literal id="ltlIsCreatedInDb" runat="server" />
                      </span>
                    </ItemTemplate>
                    <ItemStyle Width="120" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="创建后修改">
                    <ItemTemplate>
                      <asp:Literal id="ltlIsChangedAfterCreatedInDb" runat="server" />
                      </span>
                    </ItemTemplate>
                    <ItemStyle Width="120" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <asp:Literal id="ltlMetadataEdit" runat="server" />
                    </ItemTemplate>
                    <ItemStyle Width="120" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <asp:Literal id="ltlStyleEdit" runat="server" />
                    </ItemTemplate>
                    <ItemStyle Width="120" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <asp:Literal id="ltlEdit" runat="server" />
                    </ItemTemplate>
                    <ItemStyle Width="120" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <asp:Literal id="ltlDelete" runat="server" />
                    </ItemTemplate>
                    <ItemStyle Width="120" cssClass="text-center" />
                  </asp:TemplateColumn>
                </Columns>
              </asp:dataGrid>

            </div>

            <hr />

            <div class="form-group m-b-0">
              <asp:Button class="btn btn-success" id="BtnAdd" Text="新增辅助表" runat="server" />
            </div>

          </div>
        </div>

      </form>
    </body>

    </html>