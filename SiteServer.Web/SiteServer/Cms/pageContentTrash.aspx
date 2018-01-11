<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentTrash" %>
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
          <div class="m-t-0 header-title">
            内容回收站
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="m-t-10">
            <div class="form-inline">
              <div class="form-group">
                <label class="col-form-label m-r-10">栏目</label>
                <asp:DropDownList ID="DdlChannelId" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" class="form-control" runat="server"></asp:DropDownList>
              </div>

              <div class="form-group m-l-10">
                <label class="col-form-label m-r-10">每页显示条数</label>
                <asp:DropDownList ID="DdlPageNum" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server">
                  <asp:ListItem Text="默认" Value="0" Selected="true"></asp:ListItem>
                  <asp:ListItem Text="10" Value="10"></asp:ListItem>
                  <asp:ListItem Text="16" Value="16"></asp:ListItem>
                  <asp:ListItem Text="20" Value="20"></asp:ListItem>
                  <asp:ListItem Text="30" Value="30"></asp:ListItem>
                  <asp:ListItem Text="50" Value="50"></asp:ListItem>
                  <asp:ListItem Text="100" Value="100"></asp:ListItem>
                  <asp:ListItem Text="200" Value="200"></asp:ListItem>
                  <asp:ListItem Text="300" Value="300"></asp:ListItem>
                </asp:DropDownList>
              </div>
            </div>

            <div class="form-inline m-t-10">
              <div class="form-group">
                <label class="col-form-label m-r-10">时间：从</label>
                <ctrl:DateTimeTextBox id="TbDateFrom" class="form-control" Columns="12" runat="server" />
              </div>

              <div class="form-group m-l-10">
                <label class="col-form-label m-r-10">到</label>
                <ctrl:DateTimeTextBox id="TbDateTo" class="form-control" Columns="12" runat="server" />
              </div>

              <div class="form-group m-l-10">
                <label class="col-form-label m-r-10">目标</label>
                <asp:DropDownList ID="DdlSearchType" class="form-control" runat="server"></asp:DropDownList>
              </div>

              <div class="form-group m-l-10">
                <label class="col-form-label m-r-10">关键字</label>
                <asp:TextBox id="TbKeyword" class="form-control" runat="server" />
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
                      <th>原位置 </th>
                      <th width="150">删除时间</th>
                      <th width="60">&nbsp;</th>
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
                            <asp:Literal ID="ltlItemTitle" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlChannel" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlDeleteDate" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlItemEditUrl" runat="server"></asp:Literal>
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

          <asp:Button class="btn m-r-5 btn-success" id="BtnRestore" Text="还 原" runat="server" />
          <asp:Button class="btn m-r-5" id="BtnRestoreAll" Text="全部还原" runat="server" />
          <asp:Button class="btn m-r-5" id="BtnDelete" Text="删 除" runat="server" />
          <asp:Button class="btn m-r-5" id="BtnDeleteAll" Text="清空回收站" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->