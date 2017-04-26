<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.ModalPermissionsSet" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <table class="table table-noborder">
    <tr>
      <td width="140">管理员级别：</td>
      <td>
        <asp:DropDownList ID="DdlPredefinedRole" AutoPostBack="true" OnSelectedIndexChanged="DdlPredefinedRole_SelectedIndexChanged" runat="server"></asp:DropDownList>
      </td>
    </tr>
    <asp:PlaceHolder id="PhPublishmentSystemId" runat="server">
    <tr>
      <td>可以管理的站点：</td>
      <td>
        <asp:CheckBoxList ID="CblPublishmentSystemId" class="checkboxlist" repeatColumns="2" runat="server"></asp:CheckBoxList>
      </td>
    </tr>
    </asp:PlaceHolder>
    <tr id="TrRolesRow" runat="server">
      <td colspan="2">
        <table class="table">
          <tr>
            <td class="pull-right">可用的角色：</td>
            <td width="50">&nbsp;</td>
            <td>用户拥有的角色：</td>
          </tr>
          <tr>
            <td class="pull-right">
              <asp:ListBox ID="LbAvailableRoles" runat="server" SelectionMode="Multiple" Rows="14" class="RolesListBox"></asp:ListBox>
            </td>
            <td height="100%"><table height="100%" cols="1" cellpadding="0" width="100%">
                <tr>
                  <td class="center" valign="middle"><p>
                      <asp:Button class="btn" text=" -> " onclick="AddRole_OnClick" runat="server" />
                    </p>
                    <p>
                      <asp:Button class="btn" text=" <- " onclick="DeleteRole_OnClick" runat="server" />
                    </p>
                    <p>
                      <asp:Button class="btn" text=" >> " onclick="AddRoles_OnClick" runat="server" />
                    </p>
                    <p>
                      <asp:Button class="btn" text=" << " onclick="DeleteRoles_OnClick" runat="server" />
                    </p></td>
                </tr>
              </table></td>
            <td valign="top" ><asp:ListBox ID="LbAssignedRoles" runat="server" SelectionMode="Multiple" Rows="14" class="RolesListBox"></asp:ListBox></td>
          </tr>
        </table></td>
    </tr>
  </table>

</form>
</body>
</html>
