<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.ModalTableMetadataView" %>
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

  <table class="table table-bordered table-striped">
    <asp:PlaceHolder ID="phAttribute" runat="server">
      <tr>
        <td width="140"><bairong:help HelpText="需要添加的字段名称" Text="字段名：" runat="server" ></bairong:help></td>
        <td><asp:Label id="lblAttributeName" runat="server" /></td>
      </tr>
      <tr>
        <td width="140"><bairong:help HelpText="此辅助表的名称" Text="辅助表名：" runat="server" ></bairong:help></td>
        <td><asp:Label id="AuxiliaryTableENName" runat="server" /></td>
      </tr>
      <tr>
        <td width="140"><bairong:help HelpText="此字段的数据类型" Text="数据类型：" runat="server" ></bairong:help></td>
        <td><asp:Label id="DataType" runat="server" /></td>
      </tr>
      <tr>
        <td width="140"><bairong:help HelpText="此字段的数据长度" Text="数据长度：" runat="server" ></bairong:help></td>
        <td><asp:Label id="DataLength" runat="server" /></td>
      </tr>
      <tr>
        <td width="140"><bairong:help HelpText="显示在表单界面中的名称" Text="显示名称：" runat="server" ></bairong:help></td>
        <td><asp:Label id="DisplayName" runat="server" /></td>
      </tr>
      <tr>
        <td width="140"><bairong:help HelpText="显示在表单界面中的帮助提示信息" Text="显示帮助提示：" runat="server" ></bairong:help></td>
        <td><asp:Label id="HelpText" runat="server" /></td>
      </tr>
      <tr>
        <td width="140"><bairong:help HelpText="是否在表单界面中显示此字段。" Text="是否显示：" runat="server" ></bairong:help></td>
        <td><asp:Label id="IsVisible" runat="server" /></td>
      </tr>
      <tr>
        <td width="140"><bairong:help HelpText="设置是否对此字段启用表单验证" Text="需要验证：" runat="server" ></bairong:help></td>
        <td><asp:Label id="IsValidate" runat="server" /></td>
      </tr>
      <tr>
        <td width="140"><bairong:help HelpText="在表单界面中此字段的表单提交类型。" Text="表单提交类型：" runat="server" ></bairong:help></td>
        <td><asp:Label id="InputType" runat="server" /></td>
      </tr>
      <tr id="RowDefaultValue" runat="server">
        <td width="140"><bairong:help HelpText="在表单界面中此项默认显示的值" Text="默认显示值：" runat="server" ></bairong:help></td>
        <td><asp:Label id="DefaultValue" runat="server" /></td>
      </tr>
      <tr id="RowIsHorizontal" runat="server">
        <td width="140"><bairong:help HelpText="各项的排列方向。" Text="排列方向：" runat="server" ></bairong:help></td>
        <td><asp:Label id="IsHorizontal" runat="server" /></td>
      </tr>
      <tr id="RowSetItems" runat="server">
        <td colspan="2" class="center"><asp:Repeater ID="MyRepeater" runat="server">
            <itemtemplate>
              <table width="100%" border="0" cellspacing="2" cellpadding="2">
                <tr>
                  <td class="center" style="width:20px;"><strong><%# Container.ItemIndex + 1 %></strong></td>
                  <td><table width="100%" border="0" cellspacing="3" cellpadding="3">
                      <tr>
                        <td width="120"><bairong:help HelpText="选项的标题。" Text="选项标题：" runat="server" ></bairong:help></td>
                        <td colspan="3"><asp:Label id="ItemTitle" runat="server" /></td>
                      </tr>
                      <tr>
                        <td width="120"><bairong:help HelpText="选项的值。" Text="选项值：" runat="server" ></bairong:help></td>
                        <td colspan="3"><asp:Label id="ItemValue" runat="server" /></td>
                      </tr>
                      <tr>
                        <td width="120"><bairong:help HelpText="是否初始化时选定此项。" Text="初始化时选定：" runat="server" ></bairong:help></td>
                        <td colspan="3"><asp:Label id="IsSelected" runat="server" /></td>
                      </tr>
                    </table></td>
                </tr>
              </table>
            </itemtemplate>
            <SeparatorTemplate>
              <table width="100%" class="center" cellspacing="0" cellpadding="0">
                <tr>
                  <td class="mframe-b-mid">&nbsp;</td>
                </tr>
              </table>
            </SeparatorTemplate>
          </asp:Repeater></td>
      </tr>
    </asp:PlaceHolder>
  </table>

</form>
</body>
</html>
