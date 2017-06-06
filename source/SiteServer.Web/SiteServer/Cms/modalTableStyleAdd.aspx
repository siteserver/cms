<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTableStyleAdd" Trace="false" %>
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

    <table class="table table-noborder table-hover">
      <tr>
        <td>
          <bairong:help HelpText="保存在数据库中的字段名称" Text="字段名称：" runat="server"></bairong:help>
        </td>
        <td colspan="3">
          <asp:TextBox Columns="25" MaxLength="50" id="TbAttributeName" runat="server" />
          <asp:RequiredFieldValidator ControlToValidate="TbAttributeName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAttributeName" ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage="字段名称只允许包含字母、数字以及下划线"
            foreColor="red" Display="Dynamic" />
        </td>
      </tr>
      <tr>
        <td>
          <bairong:help HelpText="显示在表单界面中的名称" Text="显示名称：" runat="server"></bairong:help>
        </td>
        <td colspan="3">
          <asp:TextBox Columns="25" MaxLength="50" id="TbDisplayName" runat="server" />
          <asp:RequiredFieldValidator ControlToValidate="TbDisplayName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDisplayName" ValidationExpression="[^']+" errorMessage=" *"
            foreColor="red" display="Dynamic" />
        </td>
      </tr>
      <tr>
        <td>
          <bairong:help HelpText="显示在表单界面中的帮助提示信息" Text="显示帮助提示：" runat="server"></bairong:help>
        </td>
        <td colspan="3">
          <asp:TextBox Columns="60" id="TbHelpText" runat="server" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbHelpText" ValidationExpression="[^']+" errorMessage=" *"
            foreColor="red" display="Dynamic" />
        </td>
      </tr>
      <tr>
        <td>
          <bairong:help HelpText="是否启用本样式。" Text="是否启用：" runat="server"></bairong:help>
        </td>
        <td>
          <asp:RadioButtonList id="RblIsVisible" RepeatDirection="Horizontal" class="noborder" runat="server">
            <asp:ListItem Text="是" Selected="True" />
            <asp:ListItem Text="否" />
          </asp:RadioButtonList>
        </td>
        <td>
          <bairong:help HelpText="是否在表单中采用单行显示。" Text="是否单行显示：" runat="server"></bairong:help>
        </td>
        <td width="30%">
          <asp:RadioButtonList id="RblIsSingleLine" RepeatDirection="Horizontal" class="noborder" runat="server">
            <asp:ListItem Text="是" />
            <asp:ListItem Text="否" Selected="True" />
          </asp:RadioButtonList>
        </td>
      </tr>
      <tr>
        <td>
          <bairong:help HelpText="在表单界面中此字段的表单提交类型。" Text="表单提交类型：" runat="server"></bairong:help>
        </td>
        <td colspan="3">
          <asp:DropDownList ID="DdlInputType" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server"></asp:DropDownList>
        </td>
      </tr>
      <asp:PlaceHolder ID="PhIsFormatString" Visible="false" runat="server">
        <tr>
          <td>
            <bairong:help HelpText="是否可设置此字段的文字显示格式。" Text="可否设置格式：" runat="server"></bairong:help>
          </td>
          <td colspan="3">
            <asp:RadioButtonList id="RblIsFormatString" RepeatDirection="Horizontal" class="noborder" runat="server">
              <asp:ListItem Text="可设置" />
              <asp:ListItem Text="不可设置" Selected="True" />
            </asp:RadioButtonList>
          </td>
        </tr>
      </asp:PlaceHolder>
      <tr id="TrRelatedField" runat="server">
        <td>
          <bairong:help HelpText="联动字段" Text="联动字段：" runat="server"></bairong:help>
        </td>
        <td>
          <asp:DropDownList id="DdlRelatedFieldId" runat="server" />
        </td>
        <td>
          <bairong:help HelpText="显示方式" Text="显示方式：" runat="server"></bairong:help>
        </td>
        <td>
          <asp:DropDownList id="DdlRelatedFieldStyle" class="input-medium" runat="server" />
        </td>
      </tr>
      <tr id="TrHeightAndWidth" runat="server">
        <td>
          <bairong:help HelpText="显示宽度" Text="显示宽度：" runat="server"></bairong:help>
        </td>
        <td>
          <asp:TextBox class="input-mini" MaxLength="50" Text="0" id="TbWidth" runat="server" /> px
          <asp:RegularExpressionValidator ControlToValidate="TbWidth" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
            foreColor="red" runat="server" />
          <span class="gray">（0代表默认）</span>
        </td>
        <td>
          <bairong:help HelpText="显示高度" Text="显示高度：" runat="server"></bairong:help>
        </td>
        <td>
          <asp:TextBox class="input-mini" MaxLength="50" Text="0" id="TbHeight" runat="server" /> px
          <asp:RegularExpressionValidator ControlToValidate="TbHeight" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
            foreColor="red" runat="server" />
          <span class="gray">（0代表默认）</span>
        </td>
      </tr>
      <tr id="TrRepeat" runat="server">
        <td>
          <bairong:help HelpText="各项的排列方向。" Text="排列方向：" runat="server"></bairong:help>
        </td>
        <td>
          <asp:DropDownList id="DdlIsHorizontal" class="input-small" runat="server">
            <asp:ListItem Text="水平" Selected="True" />
            <asp:ListItem Text="垂直" />
          </asp:DropDownList>
        </td>
        <td>
          <bairong:help HelpText="项显示的列数。" Text="列数：" runat="server"></bairong:help>
        </td>
        <td>
          <asp:TextBox class="input-mini" MaxLength="50" Text="0" id="TbColumns" runat="server" />
          <asp:RegularExpressionValidator ControlToValidate="TbColumns" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
            foreColor="red" runat="server" />
          <span class="gray">（0代表未设置此属性）</span>
        </td>
      </tr>
      <tr>
        <td>默认显示值：</td>
        <td colspan="3">
          <asp:TextBox Columns="60" id="TbDefaultValue" runat="server" />
          <span id="SpanDateTip" runat="server">
            <br>
            {Current}代表当前日期/日期时间
          </span>
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDefaultValue" ValidationExpression="[^']+" errorMessage=" *"
            foreColor="red" display="Dynamic" />
        </td>
      </tr>
      <tr id="TrItemsType" runat="server">
        <td>
          <bairong:help HelpText="设置选择项需要的项数，设置完项数后需要设置每一项的标题和值。" Text="设置选项：" runat="server"></bairong:help>
        </td>
        <td colspan="3">
          <asp:DropDownList ID="DdlItemType" class="input-medium" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server">
            <asp:ListItem Text="快速设置" Value="True" Selected="True" />
            <asp:ListItem Text="详细设置" Value="False" />
          </asp:DropDownList>&nbsp;&nbsp;
          <asp:PlaceHolder ID="PhItemCount" runat="server">
            共有
            <asp:TextBox class="input-mini" id="TbItemCount" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="TbItemCount" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            /> 个选项&nbsp;
            <asp:Button class="btn" style="margin-bottom:0px;" id="SetCount" text="设 置" onclick="SetCount_OnClick" CausesValidation="false"
              runat="server" />
            <asp:RegularExpressionValidator ControlToValidate="TbItemCount" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="此项必须为数字"
              foreColor="red" runat="server" />
          </asp:PlaceHolder>
        </td>
      </tr>
      <tr id="TrItemsRapid" runat="server">
        <td>
          <bairong:help HelpText="设置选项可选值。" Text="选项可选值：" runat="server"></bairong:help>
        </td>
        <td colspan="3">
          <asp:TextBox Columns="60" id="TbItemValues" runat="server" />
          <asp:RequiredFieldValidator ControlToValidate="TbItemValues" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
          /><span class="grey">英文","分隔</span>
        </td>
      </tr>
      <tr id="TrItems" runat="server">
        <td colspan="4" class="center">
          <asp:Repeater ID="RptItems" runat="server">
            <itemtemplate>
              <table width="100%" border="0" cellspacing="2" cellpadding="2">
                <tr>
                  <td class="center" style="width:20px;"><strong><%# Container.ItemIndex + 1 %></strong></td>
                  <td>
                    <table width="100%" border="0" cellspacing="3" cellpadding="3">
                      <tr>
                        <td width="120">
                          <bairong:help HelpText="设置选项的标题。" Text="选项标题：" runat="server"></bairong:help>
                        </td>
                        <td colspan="3">
                          <asp:TextBox ID="ItemTitle" Columns="40" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ItemTitle") %>'></asp:TextBox>
                          <asp:RequiredFieldValidator ControlToValidate="ItemTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td width="120">
                          <bairong:help HelpText="设置选项的值。" Text="选项值：" runat="server"></bairong:help>
                        </td>
                        <td colspan="3">
                          <asp:TextBox ID="ItemValue" Columns="40" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ItemValue") %>'></asp:TextBox>
                          <asp:RequiredFieldValidator ControlToValidate="ItemValue" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td width="120">
                          <bairong:help HelpText="设置是否初始化时选定此项。" Text="初始化时选定：" runat="server"></bairong:help>
                        </td>
                        <td colspan="3">
                          <asp:CheckBox ID="IsSelected" runat="server" Checked="False" Text="选定"></asp:CheckBox>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </itemtemplate>
          </asp:Repeater>
        </td>
      </tr>
    </table>

  </form>
</body>

</html>