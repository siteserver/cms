<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageRecord" %>
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

        <div class="card-box">
          <div class="m-t-0 header-title">
            系统调试日志
          </div>
          <p class="text-muted font-13 m-b-25">
            此页面仅供开发人员调试系统使用
          </p>

          <div class="form-inline">
            <div class="form-group">
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
                <table id="contents" class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th>内容</th>
                      <th>描述</th>
                      <th>来源</th>
                      <th width="150">日期</th>
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
                            <asp:Literal ID="ltlText" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlSummary" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlSource" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
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

          <asp:Button class="btn m-r-5" id="BtnDelete" Text="删 除" runat="server" />
          <asp:Button class="btn m-r-5" id="BtnDeleteAll" Text="删除全部" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->