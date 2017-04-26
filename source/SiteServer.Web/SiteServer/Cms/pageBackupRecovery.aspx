<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageBackupRecovery" %>
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
    <h3 class="popover-title">数据恢复</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="160">需要恢复的备份类型：</td>
          <td>
            <asp:DropDownList ID="BackupType" AutoPostBack="true" OnSelectedIndexChanged="Options_SelectedIndexChanged" runat="server"></asp:DropDownList>
          </td>
        </tr>
        <asp:PlaceHolder ID="PlaceHolder_Delete" runat="server">
          <tr>
            <td>清除站点栏目及内容：</td>
            <td>
              <asp:RadioButtonList ID="IsDeleteChannels" runat="server" RepeatDirection="Horizontal" class="noborder">
                <asp:ListItem Text="清除站点栏目及内容" Value="True"></asp:ListItem>
                <asp:ListItem Text="保留站点栏目及内容" Value="False" Selected="true"></asp:ListItem>
              </asp:RadioButtonList>
            </td>
          </tr>
          <tr>
            <td>清除站点显示模板：</td>
            <td>
              <asp:RadioButtonList ID="IsDeleteTemplates" runat="server" RepeatDirection="Horizontal" class="noborder">
                <asp:ListItem Text="清除站点显示模板" Value="True"></asp:ListItem>
                <asp:ListItem Text="保留站点显示模板" Value="False" Selected="true"></asp:ListItem>
              </asp:RadioButtonList>
            </td>
          </tr>
          <tr>
            <td>清除站点文件：</td>
            <td>
              <asp:RadioButtonList ID="IsDeleteFiles" runat="server" RepeatDirection="Horizontal" class="noborder">
                <asp:ListItem Text="清除站点文件" Value="True"></asp:ListItem>
                <asp:ListItem Text="保留站点文件" Value="False" Selected="true"></asp:ListItem>
              </asp:RadioButtonList>
            </td>
          </tr>
        </asp:PlaceHolder>
        <tr>
          <td>是否覆盖同名数据：</td>
          <td>
            <asp:RadioButtonList ID="IsOverride" runat="server" RepeatDirection="Horizontal" class="noborder">
              <asp:ListItem Text="覆盖同名数据" Value="True" Selected="true"></asp:ListItem>
              <asp:ListItem Text="不覆盖同名数据" Value="False"></asp:ListItem>
            </asp:RadioButtonList>
          </td>
        </tr>
        <tr style="display:none">
          <td>恢复方式：</td>
          <td>
            <asp:RadioButtonList ID="IsRecoveryByUpload" AutoPostBack="true" OnSelectedIndexChanged="Options_SelectedIndexChanged" runat="server"></asp:RadioButtonList>
          </td>
        </tr>
        <asp:PlaceHolder ID="PlaceHolderByUpload" runat="server">
          <tr>
            <td>选择备份文件：</td>
            <td>
              <input type=file  id=myFile size="35" runat="server"/>
              <asp:RequiredFieldValidator ControlToValidate="myFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            </td>
          </tr>
        </asp:PlaceHolder>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="RecoveryButton" text="开始恢复" onclick="RecoveryButton_OnClick" runat="server" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
