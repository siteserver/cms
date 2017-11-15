<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageNodeGroup" %>
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
              <b>栏目组管理</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此管理栏目组
            </p>

            <ul class="nav nav-pills m-b-30">
              <li class="active">
                <a href="javascript:;">栏目组管理</a>
              </li>
              <li class="">
                <a href="pageContentGroup.aspx?publishmentSystemId=<%=PublishmentSystemId%>">内容组管理</a>
              </li>
              <li class="">
                <a href="pageContentTags.aspx?publishmentSystemId=<%=PublishmentSystemId%>">内容标签管理</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="NodeGroupName" HeaderStyle-CssClass="info thead text-center"
                CssClass="table table table-hover m-0" gridlines="none" runat="server">
                <Columns>
                  <asp:BoundColumn HeaderText="栏目组名称" DataField="NodeGroupName">
                    <ItemStyle Width="160" cssClass="text-center" />
                  </asp:BoundColumn>
                  <asp:BoundColumn HeaderText="栏目组简介" DataField="Description"> </asp:BoundColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <asp:HyperLink ID="UpLinkButton" runat="server">
                        <img src="../Pic/icon/up.gif" border="0" alt="上升" />
                      </asp:HyperLink>
                    </ItemTemplate>
                    <ItemStyle Width="50" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <asp:HyperLink ID="DownLinkButton" runat="server">
                        <img src="../Pic/icon/down.gif" border="0" alt="下降" />
                      </asp:HyperLink>
                    </ItemTemplate>
                    <ItemStyle Width="50" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <%# GetChannelHtml((string)DataBinder.Eval(Container.DataItem,"NodeGroupName"))%>
                    </ItemTemplate>
                    <ItemStyle Width="80" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <%# GetEditHtml((string)DataBinder.Eval(Container.DataItem,"NodeGroupName"))%>
                    </ItemTemplate>
                    <ItemStyle Width="80" cssClass="text-center" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn>
                    <ItemTemplate>
                      <%# GetDeleteHtml((string)DataBinder.Eval(Container.DataItem,"NodeGroupName"))%>
                    </ItemTemplate>
                    <ItemStyle Width="80" cssClass="text-center" />
                  </asp:TemplateColumn>
                </Columns>
              </asp:dataGrid>

              <hr />

              <div class="form-group m-b-0">
                <asp:Button id="BtnAddGroup" class="btn btn-primary m-l-15" text="添加栏目组" runat="server" />
              </div>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>