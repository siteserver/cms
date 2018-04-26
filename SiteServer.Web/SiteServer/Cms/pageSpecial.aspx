<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageSpecial" %>
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
            专题管理
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="form-inline">
            <div class="form-group">
              <label class="col-form-label m-r-10">专题名称/访问地址</label>
              <asp:TextBox ID="TbKeyword" class="form-control m-r-10" runat="server"></asp:TextBox>
              <asp:Button class="btn btn-success" onclick="Search_OnClick" runat="server" Text="搜 索"></asp:Button>
            </div>
          </div>

          <div class="panel panel-default m-t-10">
            <div class="panel-body p-0">
              <div class="table-responsive">
                <table id="contents" class="table tablesaw table-hover m-0">
                  <thead>
                    <tr class="thead">
                      <th>专题名称</th>
                      <th>访问地址</th>
                      <th>添加时间</th>
                      <th>操作</th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlUrl" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlActions" runat="server"></asp:Literal>
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

          <asp:Button class="btn btn-primary" id="BtnAdd" Text="新建专题" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->