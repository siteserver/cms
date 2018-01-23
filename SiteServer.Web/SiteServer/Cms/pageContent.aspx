<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContent" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script type="text/javascript">
        $(document).ready(function () {
          loopRows(document.getElementById('contents'), function (cur) {
            cur.onclick = chkSelect;
          });
        });
      </script>
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="btn-toolbar" role="toolbar">
            <div class="btn-group">
              <asp:Literal ID="LtlButtons" runat="server"></asp:Literal>
            </div>

            <div class="btn-group ml-1">
              <button type="button" class="btn btn-primary dropdown-toggle" onclick="$('#dropdown-more').toggle();return false;">
                更多
                <span class="caret"></span>
              </button>
              <div id="dropdown-more" class="dropdown-menu">
                <asp:Literal ID="LtlMoreButtons" runat="server"></asp:Literal>
              </div>
            </div>
          </div>

          <div id="contentSearch" class="m-t-10" style="display:none">
            <div class="form-inline">
              <div class="form-group">
                <label class="col-form-label m-r-10">时间</label>
                <ctrl:DateTimeTextBox ID="TbDateFrom" class="form-control" Columns="12" runat="server" />
              </div>

              <div class="form-group m-l-10">
                <label class="col-form-label m-r-10">目标</label>
                <asp:DropDownList ID="DdlSearchType" class="form-control" runat="server"> </asp:DropDownList>
              </div>

              <div class="form-group m-l-10">
                <label class="col-form-label m-r-10">关键字</label>
                <asp:TextBox class="form-control" ID="TbKeyword" runat="server" />
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
                      <th>内容标题(点击查看) </th>
                      <asp:Literal id="LtlColumnsHead" runat="server"></asp:Literal>
                      <th class="text-center text-nowrap" width="100">操作</th>
                      <th class="text-center text-nowrap" width="100">状态</th>
                      <th width="20" class="text-center text-nowrap">
                        <input type="checkbox" onClick="selectRows(document.getElementById('contents'), this.checked);">
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <asp:Repeater ID="RptContents" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                          </td>
                          <asp:Literal ID="ltlColumns" runat="server"></asp:Literal>
                          <td class="text-center text-nowrap">
                            <asp:Literal ID="ltlCommands" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center text-nowrap">
                            <asp:Literal ID="ltlStatus" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center text-nowrap">
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
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->