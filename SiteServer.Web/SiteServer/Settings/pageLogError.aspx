<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageLogError" %>
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
              <a class="nav-link" href="pageLogSite.aspx">站点日志</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageLogAdmin.aspx">管理员日志</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageLogUser.aspx">用户日志</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageLogError.aspx">系统错误日志</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageLogConfiguration.aspx">日志设置</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="form-inline">
            <div class="form-group">
              <label class="col-form-label m-r-10">来源</label>
              <asp:DropDownList id="DdlPluginId" class="form-control" runat="server" />
            </div>

            <div class="form-group m-l-10">
              <label class="col-form-label m-r-10">时间：从</label>
              <ctrl:DateTimeTextBox id="TbDateFrom" class="form-control" runat="server" />
            </div>

            <div class="form-group m-l-10">
              <label class="col-form-label m-r-10">到</label>
              <ctrl:DateTimeTextBox id="TbDateTo" class="form-control" runat="server" />
            </div>

            <div class="form-group m-l-10">
              <label class="col-form-label m-r-10">关键字</label>
              <asp:TextBox ID="TbKeyword" class="form-control" runat="server" />
            </div>

            <asp:Button class="btn btn-success m-l-10 btn-md" OnClick="Search_OnClick" ID="Search" Text="搜 索" runat="server" />
          </div>

          <div class="panel panel-default m-t-20">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th width="30" class="text-center text-nowrap">Id</th>
                      <th>错误摘要</th>
                      <th>错误消息</th>
                      <th width="150" class="text-center text-nowrap">日期</th>
                      <th width="30">
                        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td class="text-center text-nowrap">
                            <asp:Literal ID="ltlId" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlSummary" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlMessage" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center text-nowrap">
                            <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center text-nowrap">
                            <input type="checkbox" name="IDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
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

          <asp:Literal ID="LtlState" runat="server"></asp:Literal>

          <asp:Button class="btn m-r-5" id="BtnDelete" Text="删 除" runat="server" />
          <asp:Button class="btn m-r-5" id="BtnDeleteAll" Text="删除全部" runat="server" />
          <asp:Button class="btn m-r-5" ID="BtnSetting" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->