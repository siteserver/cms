<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUtilityAccessTokens" %>
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
              <a class="nav-link" href="pageUtilityCache.aspx">系统缓存</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUtilityParameter.aspx">系统参数查看</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageUtilityAccessTokens.aspx">API密钥</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUtilityEncrypt.aspx">加密字符串</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUtilityJsMin.aspx">JS脚本压缩</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUtilityDbLogDelete.aspx">清空数据库日志</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server">
          API密钥可以用于访问 SiteServer REST API <a href="https://docs.siteserver.cn/api/" target="_blank">阅读更多</a>
        </ctrl:alerts>

        <div class="card-box">
          <p class="text-muted font-13 m-b-25">
            <a class="btn btn-success m-l-5" href="pageUtilityAccessTokensAdd.aspx">添加新密钥</a>
          </p>

          <div class="panel panel-default">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table id="contents" class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th>名称</th>
                      <th>授权范围</th>
                      <th class="text-center">操作</th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater id="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td style="word-break: break-all;">
                            <span>
                              <asp:Literal id="ltlTitle" runat="server"></asp:Literal>
                            </span>
                          </td>
                          <td style="word-break: break-all;">
                            <span>
                              <asp:Literal id="ltlScopes" runat="server"></asp:Literal>
                            </span>
                          </td>
                          <td class="text-center">
                              <asp:Literal id="ltlActions" runat="server"></asp:Literal>
                          </td>
                        </tr>
                      </itemtemplate>
                    </asp:Repeater>
                  </tbody>
                </table>

              </div>
            </div>
          </div>

          <ctrl:sqlPager id="SpContents" runat="server" class="table table-pager" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->