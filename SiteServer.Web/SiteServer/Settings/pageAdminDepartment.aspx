<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAdminDepartment" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">

    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="pageAdministrator.aspx">管理员管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminRole.aspx">角色管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminConfiguration.aspx">管理员设置</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="pageAdminDepartment.aspx">所属部门管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminArea.aspx">所在区域管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="adminAccessTokens.cshtml">API密钥管理</a>
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
                  <th>部门名称</th>
                  <th>部门编号</th>
                  <th width="100">管理员人数</th>
                  <th width="60">上升</th>
                  <th width="60">下降</th>
                  <th width="80">&nbsp;</th>
                  <th width="20">
                    <input onclick="_checkFormAll(this.checked)" type="checkbox" />
                  </th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <tr>
                      <asp:Literal id="ltlHtml" runat="server" />
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

      <asp:Button class="btn btn-primary m-r-5" id="BtnAdd" Text="新 增" runat="server" />
      <asp:Button class="btn m-r-5" id="BtnDelete" Text="删 除" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->