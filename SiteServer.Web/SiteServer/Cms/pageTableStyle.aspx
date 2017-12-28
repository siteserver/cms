<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTableStyle" %>
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
            <b>站点属性管理</b>
          </h4>
          <p class="text-muted font-13 m-b-25">
            在此管理站点属性字段
          </p>

          <div class="form-horizontal">

            <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead text-center"
              CssClass="table table-hover m-0" gridlines="none" runat="server">
              <Columns>
                <asp:TemplateColumn HeaderText="字段名">
                  <ItemTemplate>
                    <asp:Literal ID="ltlAttributeName" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="140" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="显示名称">
                  <ItemTemplate>
                    <asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="140" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="表单提交类型">
                  <ItemTemplate>
                    <asp:Literal ID="ltlInputType" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="120" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="字段类型">
                  <ItemTemplate>
                    <asp:Literal ID="ltlFieldType" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="120" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="验证规则">
                  <ItemTemplate>
                    <asp:Literal ID="ltlValidate" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="100" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="排序">
                  <ItemTemplate>
                    <asp:Literal ID="ltlTaxis" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="80" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="显示样式">
                  <ItemTemplate>
                    <asp:Literal ID="ltlEditStyle" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="120" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="表单验证">
                  <ItemTemplate>
                    <asp:Literal ID="ltlEditValidate" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="80" cssClass="text-center" />
                </asp:TemplateColumn>
              </Columns>
            </asp:dataGrid>

          </div>

          <hr />

          <div class="form-group m-b-0">
            <asp:Button class="btn m-r-5" id="BtnAddStyle" Text="新增虚拟字段" runat="server" />
            <asp:Button class="btn m-r-5" id="BtnAddStyles" Text="批量新增虚拟字段" runat="server" />
            <asp:Button class="btn m-r-5" id="BtnImport" Text="导 入" runat="server" />
            <asp:Button class="btn m-r-5" id="BtnExport" Text="导 出" runat="server" />
            <asp:Button class="btn" id="BtnReturn" Text="返 回" runat="server" />
          </div>

        </div>
      </div>

    </form>
  </body>

  </html>
