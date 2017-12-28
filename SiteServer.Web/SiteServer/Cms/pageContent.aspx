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
      <script type="text/javascript" src="../assets/bootstrap/bootstrap-3.3.6.min.js"></script>
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <div class="row">
              <div class="col-lg-12">
                <div class="btn-toolbar" role="toolbar">

                  <div class="btn-group">
                    <asp:Literal ID="LtlButtons" runat="server"></asp:Literal>
                  </div>

                  <div class="btn-group">
                    <button type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                      更多
                      <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu">
                      <asp:Literal ID="LtlMoreButtons" runat="server"></asp:Literal>
                    </ul>
                  </div>
                </div>
              </div>
            </div>

            <div id="contentSearch" class="row m-t-10" style="display:none">
              <div class="col-lg-12">
                <div class="form-inline" role="form" _lpchecked="1">
                  <div class="form-group">
                    <label>时间</label>
                    <ctrl:DateTimeTextBox ID="TbDateFrom" class="form-control" Columns="12" runat="server" />
                  </div>

                  <div class="form-group m-l-10">
                    <label>目标</label>
                    <asp:DropDownList ID="DdlSearchType" class="form-control" runat="server"> </asp:DropDownList>
                  </div>

                  <div class="form-group m-l-10">
                    <label>关键字</label>
                    <asp:TextBox class="form-control" ID="TbKeyword" runat="server" />
                  </div>

                  <asp:Button class="btn btn-success m-l-10 btn-md" OnClick="Search_OnClick" ID="Search" Text="搜 索" runat="server" />
                </div>
              </div>
            </div>

            <div class="panel panel-default m-t-20">
              <div class="panel-body p-0">
                <div class="table-responsive">
                  <table id="contents" class="table tablesaw table-hover mails m-0">
                    <thead>
                      <tr class="thead">
                        <th>内容标题(点击查看) </th>
                        <asp:Literal id="LtlColumnsHead" runat="server"></asp:Literal>
                        <asp:Literal id="LtlCommandsHead" runat="server"></asp:Literal>
                        <th width="80" class="text-center">状态</th>
                        <th width="20" class="text-center">
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
                            <td class="text-center">
                              <asp:Literal ID="ltlCommands" runat="server"></asp:Literal>
                            </td>
                            <td class="text-center">
                              <asp:Literal ID="ltlStatus" runat="server"></asp:Literal>
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
          </div>
        </div>

      </form>
    </body>

    </html>