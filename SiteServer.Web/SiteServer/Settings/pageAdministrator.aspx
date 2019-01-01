<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAdministrator" %>
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
        <li class="nav-item active">
          <a class="nav-link" href="pageAdministrator.aspx">管理员管理</a>
        </li>
        <li class="nav-item">
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
      <div class="m-t-10">
        <div class="form-inline">
          <div class="form-group">
            <label class="col-form-label m-r-10">角色</label>
            <asp:DropDownList ID="DdlRoleName" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server"></asp:DropDownList>
          </div>
          <div class="form-group m-l-10">
            <label class="col-form-label m-r-10">每页显示条数</label>
            <asp:DropDownList ID="DdlPageNum" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server">
              <asp:ListItem Text="30" Value="30"></asp:ListItem>
              <asp:ListItem Text="50" Value="50"></asp:ListItem>
              <asp:ListItem Text="100" Value="100"></asp:ListItem>
              <asp:ListItem Text="200" Value="200"></asp:ListItem>
              <asp:ListItem Text="300" Value="300"></asp:ListItem>
            </asp:DropDownList>
          </div>
          <div class="form-group m-l-10">
            <label class="col-form-label m-r-10">排序</label>
            <asp:DropDownList ID="DdlOrder" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server">
              <asp:ListItem Text="登录名称" Value="UserName" Selected="true"></asp:ListItem>
              <asp:ListItem Text="最后登录日期" Value="LastActivityDate"></asp:ListItem>
              <asp:ListItem Text="创建日期" Value="CreationDate"></asp:ListItem>
              <asp:ListItem Text="登录次数" Value="CountOfLogin"></asp:ListItem>
            </asp:DropDownList>
          </div>
          <div class="form-group m-l-10">
            <label class="col-form-label m-r-10">最后登录日期</label>
            <asp:DropDownList ID="DdlLastActivityDate" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server">
              <asp:ListItem Text="全部时间" Value="0" Selected="true"></asp:ListItem>
              <asp:ListItem Text="1天内" Value="1"></asp:ListItem>
              <asp:ListItem Text="2天内" Value="2"></asp:ListItem>
              <asp:ListItem Text="3天内" Value="3"></asp:ListItem>
              <asp:ListItem Text="1周内" Value="7"></asp:ListItem>
              <asp:ListItem Text="1个月内" Value="30"></asp:ListItem>
              <asp:ListItem Text="3个月内" Value="90"></asp:ListItem>
              <asp:ListItem Text="半年内" Value="180"></asp:ListItem>
              <asp:ListItem Text="1年内" Value="365"></asp:ListItem>
            </asp:DropDownList>
          </div>
        </div>
        <div class="form-inline m-t-10">
          <div class="form-group">
            <label class="col-form-label m-r-10">所属部门</label>
            <asp:DropDownList ID="DdlDepartmentId" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server"></asp:DropDownList>
          </div>
          <div class="form-group m-l-10">
            <label class="col-form-label m-r-10">所在区域</label>
            <asp:DropDownList ID="DdlAreaId" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server"></asp:DropDownList>
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
                  <th>头像</th>
                  <th>账号</th>
                  <th>姓名</th>
                  <th>手机</th>
                  <th>部门</th>
                  <th>区域</th>
                  <th>最后登录</th>
                  <th>登录次数</th>
                  <th>角色</th>
                  <th>操作</th>
                  <th width="20">
                    <input onclick="_checkFormAll(this.checked)" type="checkbox" />
                  </th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <itemtemplate>
                    <tr>
                      <td>
                        <asp:Literal ID="ltlAvatar" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlUserName" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlMobile" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlDepartment" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlArea" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlLastActivityDate" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlCountOfLogin" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlRoles" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center text-nowrap">
                        <asp:Literal ID="ltlActions" runat="server"></asp:Literal>
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
      <ctrl:pager id="PgContents" runat="server" />
      <hr />
      <a href="adminProfile.cshtml" class="btn btn-primary m-r-5">新 增</a>
      <asp:Button class="btn m-r-5" id="BtnLock" Text="锁 定" runat="server" />
      <asp:Button class="btn m-r-5" id="BtnUnLock" Text="解除锁定" runat="server" />
      <asp:Button class="btn m-r-5" id="BtnDelete" Text="删 除" runat="server" />
    </div>
  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->