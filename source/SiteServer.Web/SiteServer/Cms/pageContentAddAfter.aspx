<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentAddAfter" %>
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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">后续操作</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="160">请选择后续操作：</td>
          <td>
            <asp:RadioButtonList ID="Operation" runat="server" AutoPostBack="true" cssClass="radiobuttonlist" RepeatDirection="Vertical" OnSelectedIndexChanged="Operation_SelectedIndexChanged"></asp:RadioButtonList>
          </td>
        </tr>
      </table>

      <asp:PlaceHolder id="phPublishmentSystemID" runat="server">
      <table class="table noborder table-hover">
        <tr>
          <td width="160">选择站点：</td>
          <td><asp:DropDownList ID="PublishmentSystemIDDropDownList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PublishmentSystemID_SelectedIndexChanged"></asp:DropDownList></td>
        </tr>
        <tr>
          <td>投稿到：</td>
          <td>
            <asp:ListBox ID="NodeIDListBox" SelectionMode="Multiple" Height="320" width="400" runat="server"></asp:ListBox>
            <asp:RequiredFieldValidator
              ControlToValidate="NodeIDListBox"
              ErrorMessage=" *" foreColor="red"
              Display="Dynamic"
              runat="server"
              />
            </td>
        </tr>
      </table>
      </asp:PlaceHolder>
  
      <asp:PlaceHolder id="phSubmit" runat="server">
        <hr />
        <table class="table noborder">
          <tr>
            <td class="center">
              <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
            </td>
          </tr>
        </table>
      </asp:PlaceHolder>

    </div>
  </div>

</form>
</body>
</html>
