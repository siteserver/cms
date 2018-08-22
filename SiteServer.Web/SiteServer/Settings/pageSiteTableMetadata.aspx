<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteTableMetadata" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">

        <div class="card-box">
          <ul class="nav nav-pills">
            <li class="nav-item">
              <a class="nav-link" href="pageSite.aspx">系统站点管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteUrlWeb.aspx">Web访问地址</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteUrlAssets.aspx">资源文件访问地址</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteUrlApi.aspx">API访问地址</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageSiteAuxiliaryTable.aspx">内容表管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteKeyword.aspx">敏感词管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteTemplate.aspx">站点模板管理</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageSiteTemplateOnline.aspx">在线站点模板</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            真实字段管理
          </div>

          <div class="panel panel-default m-t-20">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th>字段名</th>
                      <th>数据类型</th>
                      <th class="text-center">上升</th>
                      <th class="text-center">下降</th>
                      <th></th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal id="ltlAttributeName" runat="server" />
                          </td>
                          <td>
                            <asp:Literal id="ltlDataType" runat="server" />
                          </td>
                          <td class="text-center">
                            <asp:HyperLink ID="hlUp" runat="server">
                              <img src="../Pic/icon/up.gif" />
                            </asp:HyperLink>
                          </td>
                          <td class="text-center">
                            <asp:HyperLink ID="hlDown" runat="server">
                              <img src="../Pic/icon/down.gif" />
                            </asp:HyperLink>
                          </td>
                          <td class="text-center">
                            <asp:Literal id="ltlEditUrl" runat="server" />
                          </td>
                          <td class="text-center">
                            <asp:Literal id="ltlDeleteUrl" runat="server" />
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

          <asp:Button class="btn btn-primary" ID="BtnAdd" Text="添加字段" runat="server"></asp:Button>
          <asp:Button class="btn" ID="BtnCreateDb" Text="创建内容表" runat="server"></asp:Button>
          <asp:Button class="btn" ID="BtnDelete" Text="删除内容表" runat="server"></asp:Button>
          <asp:Button class="btn" ID="BtnReCreateDb" Text="重新创建内容表" runat="server"></asp:Button>
          <asp:Button class="btn" ID="BtnSqlString" Text="显示创建表SQL命令" runat="server"></asp:Button>
          <asp:Button class="btn m-r-5" text="返 回" onclick="Return_OnClick" runat="server" />

        </div>

        <asp:PlaceHolder id="PhSyncTable" runat="server">
          <div class="card-box">
            <div class="m-t-0 header-title">
              同步内容表
            </div>
            <p class="text-muted font-13 m-b-25">
              此内容表在创建后被修改，与数据库中的实际表结构有差别，请同步内容表。
            </p>

            <hr />

            <asp:Button class="btn btn-primary" Text="同步内容表" OnClick="SyncTableButton_OnClick" runat="server"></asp:Button>
          </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder id="PhSqlString" runat="server">
          <div class="card-box">
            <div class="m-t-0 header-title">
              创建内容表SQL命令
            </div>
            <p class="text-muted font-13 m-b-25">
              <asp:Literal ID="LtlSqlString" runat="server" />
            </p>
          </div>
        </asp:PlaceHolder>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->