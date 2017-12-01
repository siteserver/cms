<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageConfig" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/header.aspx"-->
</head>

<body>
  <!--#include file="../inc/openWindow.html"-->
  <form class="form-inline" runat="server">
    <asp:Literal id="LtlBreadCrumb" runat="server" />
    <bairong:alerts runat="server" />

    <!-- <ul class="nav nav-pills">
  <li class="active"><a href="pageUserProfile.aspx">数据库连接</a></li>
  <li><a href="pageUserPassword.aspx">更改密码</a></li>
</ul> -->

    <div class="popover popover-static">
      <h3 class="popover-title">数据库连接</h3>
      <div class="popover-content">

        <table class="table noborder table-hover">
          <tr>
            <td width="160">数据库连接：</td>
            <td>
              <asp:DropDownList ID="DdlIsDefault" AutoPostBack="true" OnSelectedIndexChanged="DdlIsDefault_SelectedIndexChanged" runat="server"></asp:DropDownList>
            </td>
          </tr>

          <asp:PlaceHolder id="PhCustom" runat="server">
            <tr>
              <td width="160">数据库类型：</td>
              <td>
                <asp:DropDownList ID="DdlSqlDatabaseType" AutoPostBack="true" OnSelectedIndexChanged="DdlSqlDatabaseType_SelectedIndexChanged" runat="server"></asp:DropDownList>
              </td>
            </tr>

            <asp:PlaceHolder ID="PhSql1" runat="server">
              <tr>
                <td>数据库主机：</td>
                <td>
                  <asp:TextBox style="width:285px" ID="TbSqlServer" runat="server" />
                  <asp:RequiredFieldValidator runat="server" ControlToValidate="TbSqlServer" ErrorMessage="数据库主机为必填项。" foreColor="red" ToolTip="数据库主机为必填项。"
                    Display="Dynamic"></asp:RequiredFieldValidator>
                  <span>IP地址或者服务器名</span>
                </td>
              </tr>
              <tr>
                <td>身份验证：</td>
                <td>
                  <asp:DropDownList ID="DdlIsTrustedConnection" AutoPostBack="true" OnSelectedIndexChanged="DdlIsTrustedConnection_SelectedIndexChanged"
                    runat="server"></asp:DropDownList>
                </td>
              </tr>

              <asp:PlaceHolder id="PhSqlUserNamePassword" runat="server">
                <tr>
                  <td>数据库用户：</td>
                  <td>
                    <asp:TextBox style="width:285px" ID="TbSqlUserName" runat="server" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TbSqlUserName" ErrorMessage="数据库用户为必填项。" foreColor="red" ToolTip="数据库用户为必填项。"
                      Display="Dynamic"></asp:RequiredFieldValidator>
                    <span>连接数据库的用户名</span>
                  </td>
                </tr>
                <tr>
                  <td>数据库密码：</td>
                  <td>
                    <asp:TextBox style="width:285px" ID="TbSqlPassword" TextMode="Password" runat="server" />
                    <input type="hidden" runat="server" id="HihSqlHiddenPassword" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TbSqlPassword" ErrorMessage="数据库密码为必填项。" foreColor="red" ToolTip="数据库密码为必填项。"
                      Display="Dynamic"></asp:RequiredFieldValidator>
                    <span>连接数据库的密码</span>
                  </td>
                </tr>
              </asp:PlaceHolder>

              <tr>
                  <td></td>
                  <td>
                    <asp:Button id="BtnConnect" class="btn btn-succss" OnClick="Connect_Click" runat="server" Text="连接数据库" />
                    <span>点击按钮获取可用的数据库</span>
                  </td>
              </tr>

            </asp:PlaceHolder>
          </asp:PlaceHolder>
        </table>

        <asp:PlaceHolder ID="PhSql2" runat="server" Visible="false">
          <hr />
          <table class="table noborder table-hover">
            <tr>
              <td>选择数据库：</td>
              <td>
                <asp:DropDownList ID="DdlSqlDatabaseName" runat="server"></asp:DropDownList>
              </td>
            </tr>
          </table>
        </asp:PlaceHolder>

        <hr />

        <table class="table noborder">
          <tr>
            <td class="center">
              <asp:Button class="btn btn-primary" OnClick="Submit_Click" runat="server" Text="修 改" />
              <input class="btn" type="button" onClick="location.href='pageManagement.aspx';return false;" value="返 回" />
            </td>
          </tr>
        </table>

      </div>
    </div>

  </form>
</body>

</html>