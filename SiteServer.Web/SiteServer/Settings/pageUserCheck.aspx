<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUserCheck" %>
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
              <a class="nav-link" href="pageUser.aspx">用户管理</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageUserCheck.aspx">审核新用户</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUserConfiguration.aspx">用户设置</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="panel panel-default">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table id="contents" class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th>账号</th>
                      <th>姓名</th>
                      <th>注册时间</th>
                      <th width="60">&nbsp;</th>
                      <th width="30">
                        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlUserName" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlCreateDate" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:HyperLink ID="hlEditLink" Text="编辑" runat="server"></asp:HyperLink>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlSelect" runat="server"></asp:Literal>
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

          <hr />

          <asp:Button class="btn btn-primary m-r-5" id="BtnCheck" Text="审核通过" runat="server" />
          <asp:Button class="btn  m-r-5" id="BtnDelete" Text="删 除" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->