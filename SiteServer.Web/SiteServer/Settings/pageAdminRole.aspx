<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAdminRole" %>
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
        <li class="nav-item active">
          <a class="nav-link" href="pageAdminRole.aspx">角色管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminConfiguration.aspx">管理员设置</a>
        </li>
        <li class="nav-item">
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
                  <th>角色名称</th>
                  <th>备注</th>
                  <th width="80">&nbsp;</th>
                  <th width="80">&nbsp;</th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <tr>
                      <td>
                        <asp:Literal id="ltlRoleName" runat="server" />
                      </td>
                      <td>
                        <asp:Literal id="ltlDescription" runat="server" />
                      </td>
                      <td>
                        <asp:Literal id="ltlEdit" runat="server" />
                      </td>
                      <td>
                        <asp:Literal id="ltlDelete" runat="server" />
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

      <asp:Button id="BtnAdd" class="btn btn-primary" Text="创建角色" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->