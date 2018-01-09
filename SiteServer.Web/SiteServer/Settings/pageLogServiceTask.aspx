<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageLogServiceTask" %>
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
              <a class="nav-link" href="pageLogServiceTask.aspx">服务组件任务日志</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageLogServiceCreateTask.aspx">服务组件生成日志</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageLogError.aspx">系统错误日志</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageLogConfiguration.aspx">日志设置</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-10">
            <div class="form-inline">
              <div class="form-group">
                <label class="col-form-label m-r-10">类型</label>
                <asp:DropDownList ID="DdlIsSuccess" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server"></asp:DropDownList>
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
          </div>

          <div class="panel panel-default m-t-20">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table id="contents" class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th class="text-center">所属站点</th>
                      <th class="text-center">任务名称</th>
                      <th class="text-center">任务类型</th>
                      <th class="text-center">执行日期</th>
                      <th class="text-center">是否成功</th>
                      <th>失败原因</th>
                      <th class="text-center" width="30">
                        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td class="text-center">
                            <asp:Literal ID="ltlPublishmentSystem" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlTaskName" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlServiceType" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlIsSuccess" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlErrorMessage" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
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

          <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />
          <asp:Button class="btn" id="BtnDeleteAll" Text="删除全部" runat="server" />
          <asp:Button class="btn" ID="BtnSetting" runat="server" />

        </div>

      </form>
    </body>

    </html>