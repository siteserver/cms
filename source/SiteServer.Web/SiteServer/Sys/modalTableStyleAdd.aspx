<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.ModalTableStyleAdd" Trace="false" %>
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
      <td><bairong:help HelpText="保存在数据库中的字段名称" Text="字段名称：" runat="server" ></bairong:help></td>
      <td colspan="3"><asp:TextBox Columns="25" MaxLength="50" id="tbAttributeName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbAttributeName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbAttributeName"
						ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage="字段名称只允许包含字母、数字以及下划线" foreColor="red" Display="Dynamic" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="显示在表单界面中的名称" Text="显示名称：" runat="server" ></bairong:help></td>
      <td colspan="3"><asp:TextBox Columns="25" MaxLength="50" id="tbDisplayName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbDisplayName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbDisplayName"
							ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="显示在表单界面中的帮助提示信息" Text="显示帮助提示：" runat="server" ></bairong:help></td>
      <td colspan="3"><asp:TextBox Columns="60" id="tbHelpText" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbHelpText"
							ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="是否启用本样式。" Text="是否启用：" runat="server" ></bairong:help></td>
      <td><asp:RadioButtonList id="rblIsVisible" RepeatDirection="Horizontal" class="noborder" runat="server">
          <asp:ListItem Text="是" Selected="True" />
          <asp:ListItem Text="否" />
        </asp:RadioButtonList></td>
      <td><bairong:help HelpText="是否在表单中采用单行显示。" Text="是否单行显示：" runat="server" ></bairong:help></td>
      <td width="30%"><asp:RadioButtonList id="rblIsSingleLine" RepeatDirection="Horizontal" class="noborder" runat="server">
          <asp:ListItem Text="是" />
          <asp:ListItem Text="否" Selected="True" />
        </asp:RadioButtonList></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="在表单界面中此字段的表单提交类型。" Text="表单提交类型：" runat="server" ></bairong:help></td>
      <td colspan="3"><asp:DropDownList ID="ddlInputType" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server"></asp:DropDownList></td>
    </tr>
    <asp:PlaceHolder ID="phIsFormatString" Visible="false" runat="server">
    <tr>
      <td><bairong:help HelpText="是否可设置此字段的文字显示格式。" Text="可否设置格式：" runat="server" ></bairong:help></td>
      <td colspan="3"><asp:RadioButtonList id="rblIsFormatString" RepeatDirection="Horizontal" class="noborder" runat="server">
          <asp:ListItem Text="可设置" />
          <asp:ListItem Text="不可设置" Selected="True" />
        </asp:RadioButtonList></td>
    </tr>
    </asp:PlaceHolder>
    <tr id="rowRelatedField" runat="server">
      <td><bairong:help HelpText="联动字段" Text="联动字段：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList id="ddlRelatedFieldID" runat="server" /></td>
      <td><bairong:help HelpText="显示方式" Text="显示方式：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList id="ddlRelatedFieldStyle" class="input-medium" runat="server" /></td>
    </tr>
    <tr id="rowHeightAndWidth" runat="server">
      <td><bairong:help HelpText="显示宽度" Text="显示宽度：" runat="server" ></bairong:help></td>
      <td>
        <asp:TextBox class="input-mini" MaxLength="50" Text="0" id="tbWidth" runat="server" />
        px
        <asp:RegularExpressionValidator
					ControlToValidate="tbWidth"
					ValidationExpression="\d+"
					Display="Dynamic"
					errorMessage=" *" foreColor="red"
					runat="server"/>
        <span class="gray">（0代表默认）</span>
      </td>
      <td><bairong:help HelpText="显示高度" Text="显示高度：" runat="server" ></bairong:help></td>
      <td>
        <asp:TextBox class="input-mini" MaxLength="50" Text="0" id="tbHeight" runat="server" />
        px
        <asp:RegularExpressionValidator
					ControlToValidate="tbHeight"
					ValidationExpression="\d+"
					Display="Dynamic"
					errorMessage=" *" foreColor="red"
					runat="server"/>
        <span class="gray">（0代表默认）</span>
      </td>
    </tr>
    <tr id="rowRepeat" runat="server">
      <td><bairong:help HelpText="各项的排列方向。" Text="排列方向：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList id="ddlIsHorizontal" class="input-small" runat="server">
          <asp:ListItem Text="水平" Selected="True"/>
          <asp:ListItem Text="垂直"  />
        </asp:DropDownList></td>
      <td><bairong:help HelpText="项显示的列数。" Text="列数：" runat="server" ></bairong:help></td>
      <td>
        <asp:TextBox class="input-mini" MaxLength="50" Text="0" id="tbColumns" runat="server" />
        <asp:RegularExpressionValidator
					ControlToValidate="tbColumns"
					ValidationExpression="\d+"
					Display="Dynamic"
					errorMessage=" *" foreColor="red"
					runat="server"/>
        <span class="gray">（0代表未设置此属性）</span>
      </td>
    </tr>
    <tr>
      <td>默认显示值：</td>
      <td colspan="3"><asp:TextBox Columns="60" id="tbDefaultValue" runat="server" />
        <span id="DateTip" runat="server"><br>
        {Current}代表当前日期/日期时间</span>
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbDefaultValue"
              ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr id="rowItemsType" runat="server">
      <td><bairong:help HelpText="设置选择项需要的项数，设置完项数后需要设置每一项的标题和值。" Text="设置选项：" runat="server" ></bairong:help></td>
      <td colspan="3">
      <asp:DropDownList ID="ddlItemType" class="input-medium" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server">
        <asp:ListItem Text="快速设置" Value="True" Selected="True"/>
        <asp:ListItem Text="详细设置" Value="False" />
      </asp:DropDownList>&nbsp;&nbsp;
      <asp:PlaceHolder ID="phItemCount" runat="server">
      共有
      <asp:TextBox class="input-mini" id="tbItemCount" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbItemCount" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /> 个选项&nbsp;
        <asp:Button class="btn" style="margin-bottom:0px;" id="SetCount" text="设 置" onclick="SetCount_OnClick" CausesValidation="false" runat="server" />
        <asp:RegularExpressionValidator ControlToValidate="tbItemCount" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="此项必须为数字" foreColor="red" runat="server" />
      </asp:PlaceHolder>
      </td>
    </tr>
    <tr id="rowItemsRapid" runat="server">
      <td><bairong:help HelpText="设置选项可选值。" Text="选项可选值：" runat="server" ></bairong:help></td>
      <td colspan="3">
      <asp:TextBox Columns="60" id="tbItemValues" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbItemValues" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /><span class="grey">英文","分隔</span>
      </td>
    </tr>
    <tr id="rowItems" runat="server">
      <td colspan="4" class="center"><asp:Repeater ID="MyRepeater" runat="server">
          <itemtemplate>
            <table width="100%" border="0" cellspacing="2" cellpadding="2">
              <tr>
                <td class="center" style="width:20px;"><strong><%# Container.ItemIndex + 1 %></strong></td>
                <td><table width="100%" border="0" cellspacing="3" cellpadding="3">
                    <tr>
                      <td width="120"><bairong:help HelpText="设置选项的标题。" Text="选项标题：" runat="server" ></bairong:help></td>
                      <td colspan="3"><asp:TextBox ID="ItemTitle" Columns="40" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ItemTitle") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="ItemTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /></td>
                    </tr>
                    <tr>
                      <td width="120"><bairong:help HelpText="设置选项的值。" Text="选项值：" runat="server" ></bairong:help></td>
                      <td colspan="3"><asp:TextBox ID="ItemValue" Columns="40" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ItemValue") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="ItemValue" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /></td>
                    </tr>
                    <tr>
                      <td width="120"><bairong:help HelpText="设置是否初始化时选定此项。" Text="初始化时选定：" runat="server" ></bairong:help></td>
                      <td colspan="3"><asp:CheckBox ID="IsSelected" runat="server" Checked="False" Text="选定"></asp:CheckBox></td>
                    </tr>
                  </table></td>
              </tr>
            </table>
          </itemtemplate>
        </asp:Repeater></td>
    </tr>
  </table>

</form>
</body>
</html>
