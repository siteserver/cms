<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentCheck" %>
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

        <div class="raw">
          <div class="card-box">

            <div class="row m-t-5 m-b-10">
              <div class="col-lg-12">
                <div class="form-inline" role="form" _lpchecked="1">
                  <div class="form-group">
                    <label>栏目</label>
                    <asp:DropDownList ID="DdlChannelId" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" class="form-control" runat="server"></asp:DropDownList>
                  </div>
                  <div class="form-group">
                    <label>状态</label>
                    <asp:DropDownList ID="DdlState" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
                      runat="server"></asp:DropDownList>
                  </div>
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
                        <th>栏目 </th>
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
                            <td>
                              <asp:Literal ID="ltlChannel" runat="server"></asp:Literal>
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

            <hr />

            <div class="form-group m-b-0">
              <asp:Button class="btn btn-success" id="BtnCheck" Text="审 核" runat="server" />
              <asp:Button class="btn m-l-5" id="BtnDelete" Text="删 除" runat="server" />
            </div>

          </div>
        </div>

      </form>
    </body>

    </html>