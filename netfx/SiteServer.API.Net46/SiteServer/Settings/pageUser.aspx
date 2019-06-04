<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUser" %>
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
          <a class="nav-link" href="pageUser.aspx">用户管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="userGroup.cshtml">用户组管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="userStyle.cshtml">用户字段</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="userConfig.cshtml">用户设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="userHome.cshtml">用户中心设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="userMenu.cshtml">用户中心菜单</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="m-t-10">
        <div class="form-inline">
          <div class="form-group">
            <label class="col-form-label m-r-10">用户组</label>
            <asp:DropDownList ID="DdlGroupId" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server"></asp:DropDownList>
          </div>

          <div class="form-group m-l-10">
            <label class="col-form-label m-r-10">登录次数</label>
            <asp:DropDownList ID="DdlLoginCount" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server">
              <asp:ListItem Text="<全部>" Value="0" Selected="true"></asp:ListItem>
              <asp:ListItem Text=">30" Value="30"></asp:ListItem>
              <asp:ListItem Text=">50" Value="50"></asp:ListItem>
              <asp:ListItem Text=">100" Value="100"></asp:ListItem>
              <asp:ListItem Text=">200" Value="200"></asp:ListItem>
              <asp:ListItem Text=">300" Value="300"></asp:ListItem>
            </asp:DropDownList>
          </div>

          <div class="form-group m-l-10">
            <label class="col-form-label m-r-10">注册时间</label>
            <asp:DropDownList ID="DdlCreationDate" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server">
              <asp:ListItem Text="<全部时间>" Value="0" Selected="true"></asp:ListItem>
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

          <div class="form-group m-l-10">
            <label class="col-form-label m-r-10">最后活动时间</label>
            <asp:DropDownList ID="DdlLastActivityDate" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server">
              <asp:ListItem Text="<全部时间>" Value="0" Selected="true"></asp:ListItem>
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

          <div class="form-group m-l-10">
            <label class="col-form-label m-r-10">每页显示条数</label>
            <asp:DropDownList ID="DdlPageNum" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"
              runat="server">
              <asp:ListItem Text="<默认>" Value="0" Selected="true"></asp:ListItem>
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
                  <th>账号</th>
                  <th>邮箱</th>
                  <th>手机</th>
                  <th class="text-nowrap text-center">注册时间</th>
                  <th class="text-nowrap text-center">用户组</th>
                  <th class="text-nowrap text-center">登录次数</th>
                  <th class="text-nowrap text-center" width="120">&nbsp;</th>
                  <th width="20">
                    <input onclick="_checkFormAll(this.checked)" type="checkbox" />
                  </th>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <ItemTemplate>
                    <tr>
                      <td>
                        <asp:Literal ID="ltlUserName" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlEmail" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlMobile" runat="server"></asp:Literal>
                      </td>
                      <td class="text-nowrap text-center">
                        <asp:Literal ID="ltlCreationDate" runat="server"></asp:Literal>
                      </td>
                      <td class="text-nowrap text-center">
                        <asp:Literal ID="ltlGroupName" runat="server"></asp:Literal>
                      </td>
                      <td class="text-nowrap text-center">
                        <asp:Literal ID="ltlLoginCount" runat="server"></asp:Literal>
                      </td>
                      <td class="text-nowrap text-center">
                        <asp:HyperLink NavigateUrl="javascript:;" ID="hlChangePassword" Text="重设密码" runat="server"></asp:HyperLink>
                        <asp:HyperLink ID="hlEditLink" class="ml-2" Text="编辑" runat="server"></asp:HyperLink>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlSelect" runat="server"></asp:Literal>
                      </td>
                    </tr>
                  </ItemTemplate>
                </asp:Repeater>
              </tbody>
            </table>

          </div>
        </div>
      </div>

      <ctrl:sqlPager id="SpContents" runat="server" class="table table-pager" />

      <hr />

      <asp:Button class="btn btn-primary m-r-5" id="BtnCheck" Text="审核通过" runat="server" />
      <asp:Button class="btn m-r-5" ID="BtnAdd" Text="添加用户" runat="server" />
      <asp:Button class="btn m-r-5" ID="BtnLock" Text="锁定用户" runat="server" />
      <asp:Button class="btn m-r-5" ID="BtnUnLock" Text="解除锁定" runat="server" />
      <asp:Button class="btn m-r-5" ID="BtnDelete" Text="删 除" runat="server" />
      <asp:Button class="btn m-r-5" ID="BtnExport" Text="导出Excel" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->