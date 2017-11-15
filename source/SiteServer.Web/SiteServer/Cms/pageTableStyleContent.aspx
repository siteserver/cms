<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTableStyleContent" %>
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
              <b>内容字段管理</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此管理内容字段
            </p>

            <ul class="nav nav-pills m-b-30">
              <li class="active">
                <a href="javascript:;">内容字段管理</a>
              </li>
              <li class="">
                <a href="pageTableStyleChannel.aspx?publishmentSystemId=<%=PublishmentSystemId%>">栏目字段管理</a>
              </li>
              <li class="">
                <a href="pageRelatedField.aspx?publishmentSystemId=<%=PublishmentSystemId%>">联动字段设置</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-1 control-label">栏目</label>
                <div class="col-sm-6">
                  <asp:DropDownList ID="DdlNodeId" class="form-control" OnSelectedIndexChanged="Redirect" AutoPostBack="true" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-5"></div>
              </div>

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
                  <asp:TemplateColumn HeaderText="是否启用">
                    <ItemTemplate>
                      <asp:Literal ID="ltlIsVisible" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="80" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="验证规则">
                    <ItemTemplate>
                      <asp:Literal ID="ltlValidate" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="100" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <asp:HyperLink ID="UpLinkButton" runat="server">
                        <img src="../Pic/icon/up.gif" border="0" alt="上升" />
                      </asp:HyperLink>
                    </ItemTemplate>
                    <ItemStyle Width="40" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <asp:HyperLink ID="DownLinkButton" runat="server">
                        <img src="../Pic/icon/down.gif" border="0" alt="下降" />
                      </asp:HyperLink>
                    </ItemTemplate>
                    <ItemStyle Width="40" cssClass="text-center" />
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

              <hr />

              <div class="form-group m-b-0">
                <asp:Button class="btn btn-primary m-l-15" id="BtnAddStyle" Text="新增虚拟字段" runat="server" />
                <asp:Button class="btn btn-primary m-l-15" id="BtnAddStyles" Text="批量新增虚拟字段" runat="server" />
                <asp:Button class="btn btn-primary m-l-15" id="BtnImport" Text="导 入" runat="server" />
                <asp:Button class="btn btn-primary m-l-15" id="BtnExport" Text="导 出" runat="server" />
              </div>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>