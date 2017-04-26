<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.PageUserConfigWriting" %>
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
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" Text="用户的投稿范围为稿件发布者所属角色下有权限的站点及栏目；站点栏目的审核规则为稿件的审核规则，通过设置站点栏目的审核规则来控制稿件的审核" />

        <div class="popover popover-static">
            <h3 class="popover-title">投稿启用设置</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                        <td style="width: 200px;vertical-align:middle">是否启用用户中心投稿：</td>
                        <td>
                            <asp:DropDownList ID="DdlIsWritingEnabled" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlIsWritingEnabled_SelectedIndexChanged"></asp:DropDownList>
                            <span>关闭后，用户中心将不显示投稿菜单。</span>
                        </td>
                    </tr>
                </table>

                <asp:PlaceHolder ID="PhWriting" runat="server">
                <hr />
                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td>用户组</td>
                        <td>投稿设置</td>
                    </tr>
                    <asp:Repeater id="RptGroup" runat="server">
                      <ItemTemplate>
                        <tr>
                            <td style="width: 200px; vertical-align:middle" class="center">
                              <asp:Literal id="ltlGroupName" runat="server"></asp:Literal>
                              <asp:HiddenField id="hfGroupID" runat="server"></asp:HiddenField>
                            </td>
                            <td>
                              <table class="table noborder">
                                  <tr>
                                    <td style="width: 180px;border-left: none; vertical-align:middle">是否启用用户中心投稿：</td>
                                    <td style="width: 180px;border-left: none; vertical-align:middle">
                                      <asp:DropDownList ID="ddlIsGroupWritingEnabled" OnSelectedIndexChanged="DdlIsGroupWritingEnabled_SelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
                                      <br />
                                      <span>关闭后，用户中心将不显示投稿菜单。</span>
                                    </td>
                                    <asp:PlaceHolder id="phIsGroupWritingEnabled" runat="server">
                                    <td style="width: 180px;border-left: none; vertical-align:middle">稿件关联管理员账号：</td>
                                    <td style="border-left: none; vertical-align:middle">
                                        <asp:TextBox MaxLength="50" ID="tbGroupWritingAdminUserName" runat="server" />
                                        <asp:RequiredFieldValidator
                                            ControlToValidate="tbGroupWritingAdminUserName"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic"
                                            runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <br />
                                        <span>在此设置投稿关联的管理员，用户的稿件以此管理员的权限为依据</span><br />
                                        <span>同时用户的投稿范围为关联管理员所属角色下有权限的站点及栏目</span>
                                    </td>
                                    </asp:PlaceHolder>
                                    <td style="border-left: none; vertical-align:middle"></td>
                                  </tr>
                              </table>
                            </td>
                        </tr>
                      </ItemTemplate>
                    </asp:Repeater>
                </table>
                </asp:PlaceHolder>

                <hr />
                <table class="table noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>
