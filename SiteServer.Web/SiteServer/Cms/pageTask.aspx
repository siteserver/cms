<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTask" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <!--#include file="../inc/head.html"-->
  </head>

  <body>
    <form class="container" runat="server">
        <bairong:alerts text="启用定时任务需要在服务器中安装SiteServer Service服务组件" runat="server" />

      <div class="raw">
        <div class="card-box">
          <h4 class="m-t-0 header-title">
            <b><%=ServiceType%>任务管理</b>
          </h4>
          <p class="text-muted font-13 m-b-25"></p>

          <ul class="nav nav-pills m-b-30">
            <li class="<%=(ServiceType == "定时生成" ? "active" : "")%>">
              <a href="pageTask.aspx?serviceType=Create&publishmentSystemId=<%=PublishmentSystemId%>">定时生成</a>
            </li>
            <li class="<%=(ServiceType == "定时采集" ? "active" : "")%>">
              <a href="pageTask.aspx?serviceType=Gather&publishmentSystemId=<%=PublishmentSystemId%>">定时采集</a>
            </li>
            <li class="<%=(ServiceType == "定时备份" ? "active" : "")%>">
              <a href="pageTask.aspx?serviceType=Backup&publishmentSystemId=<%=PublishmentSystemId%>">定时备份</a>
            </li>
          </ul>

          <div class="form-horizontal">

            <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead text-center"
              CssClass="table table-hover m-0" gridlines="none" runat="server">
              <Columns>
                <asp:TemplateColumn HeaderText="所属站点">
                  <ItemTemplate>
                    <asp:Literal ID="ltlSite" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="160" HorizontalAlign="left" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="任务名称">
                  <ItemTemplate>
                    <asp:Literal ID="ltlTaskName" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle HorizontalAlign="left" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="执行周期">
                  <ItemTemplate>
                    <asp:Literal ID="ltlFrequencyType" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="80" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="最近一次执行时间">
                  <ItemTemplate>
                    <asp:Literal ID="ltlLastExecuteDate" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="150" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="是否启用">
                  <ItemTemplate>
                    <asp:Literal ID="ltlIsEnabled" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="80" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                  <ItemTemplate>
                    <asp:Literal ID="ltlEditHtml" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="80" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                  <ItemTemplate>
                    <asp:Literal ID="ltlEnabledHtml" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="80" cssClass="text-center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                  <ItemTemplate>
                    <asp:Literal ID="ltlDeleteHtml" runat="server"></asp:Literal>
                  </ItemTemplate>
                  <ItemStyle Width="80" cssClass="text-center" />
                </asp:TemplateColumn>
              </Columns>
            </asp:dataGrid>

            <hr />

            <div class="form-group m-b-0">
              <asp:Button class="btn btn-primary m-l-15" id="AddTask" Text="添加任务" runat="server" />
            </div>

          </div>

        </div>
      </div>

    </form>
  </body>

  </html>