<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTaskAdd" Trace="false" %>
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
        <asp:Button ID="btnSubmit" UseSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" Style="display: none" />
        <bairong:Alerts runat="server"></bairong:Alerts>

        <script type="text/javascript" language="javascript">
            function selectAll(isChecked) {
                for (var i = 0; i < document.getElementById('<%=CreateChannelIDCollection.ClientID%>').options.length; i++) {
                    document.getElementById('<%=CreateChannelIDCollection.ClientID%>').options[i].selected = isChecked;
                }
            }
        </script>

        <table class="table table-noborder table-hover">
            <tr>
                <td width="120">
                    <bairong:Help HelpText="任务名称" Text="任务名称：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:TextBox Columns="35" MaxLength="200" ID="TaskName" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TaskName" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TaskName"
                        ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" /></td>
            </tr>
            <tr>
                <td>
                    <bairong:Help HelpText="任务执行的频率" Text="任务执行频率：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:DropDownList ID="FrequencyType" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="FrequencyType_SelectedIndexChanged" runat="server"></asp:DropDownList>
                    <asp:PlaceHolder ID="PlaceHolder_PeriodIntervalMinute" Visible="false" runat="server">&nbsp;周期：&nbsp;每
          <asp:TextBox class="input-mini" MaxLength="50" Text="30" ID="PeriodInterval" runat="server" />
                        &nbsp;
          <asp:DropDownList ID="PeriodIntervalType" class="input-small" runat="server"></asp:DropDownList>
                        <asp:RequiredFieldValidator ControlToValidate="PeriodInterval" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                        <asp:RegularExpressionValidator
                            ControlToValidate="PeriodInterval"
                            ValidationExpression="\d+"
                            Display="Dynamic"
                            ErrorMessage="必须为大于零的整数"
                            foreColor="red"
                            runat="server" />
                        <asp:CompareValidator
                            ControlToValidate="PeriodInterval"
                            Operator="GreaterThan"
                            ValueToCompare="0"
                            Display="Dynamic"
                            ErrorMessage="必须为大于零的整数"
                            foreColor="red"
                            runat="server" />
                    </asp:PlaceHolder>
                </td>
            </tr>
            <asp:PlaceHolder ID="PlaceHolder_NotPeriod" runat="server">
                <tr>
                    <td>
                        <bairong:Help HelpText="任务执行的开始时刻" Text="任务开始时刻：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <table width="100%" border="0" cellspacing="1" cellpadding="1">
                            <tr>
                                <td>日期：</td>
                                <td>
                                    <asp:DropDownList ID="StartDay" CssClass="input-small" runat="server"></asp:DropDownList></td>
                                <td>星期：</td>
                                <td>
                                    <asp:DropDownList ID="StartWeekday" CssClass="input-small" runat="server"></asp:DropDownList></td>
                                <td>小时：</td>
                                <td>
                                    <asp:DropDownList ID="StartHour" CssClass="input-small" runat="server"></asp:DropDownList></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td width="120">
                    <bairong:Help HelpText="任务描述" Text="任务描述：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:TextBox Columns="55" TextMode="MultiLine" Rows="2" MaxLength="200" ID="Description" runat="server" /></td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <asp:PlaceHolder ID="PlaceHolder_Create" Visible="false" runat="server">
                <tr>
                    <td>
                        <bairong:Help HelpText="选择需要生成的对象" Text="需要生成的对象：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:ListBox ID="CreateChannelIDCollection" SelectionMode="Multiple" Rows="13" runat="server"></asp:ListBox>
                        &nbsp;<asp:CheckBox ID="CreateIsCreateAll" class="checkbox inline" Text="生成全部" runat="server"></asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <bairong:Help HelpText="生成文件的类型" Text="生成类型：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:CheckBoxList ID="CreateCreateTypes" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:CheckBoxList></td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="PlaceHolder_Gather" Visible="false" runat="server">
                <tr>
                    <td>
                        <bairong:Help ID="GatherHelp" Text="需要定时采集的的对象：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:ListBox ID="GatherListBox" SelectionMode="Multiple" Rows="10" runat="server"></asp:ListBox>
                        <asp:RequiredFieldValidator ControlToValidate="GatherListBox" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" /></td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="PlaceHolder_Backup" Visible="false" runat="server">
                <tr>
                    <td>
                        <bairong:Help HelpText="选择备份类型" Text="选择备份类型：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:DropDownList ID="BackupType" runat="server"></asp:DropDownList></td>
                </tr>
                <asp:PlaceHolder ID="PlaceHolder_Backup_PublishmentSystem" Visible="false" runat="server">
                    <tr>
                        <td>
                            <bairong:Help HelpText="选择需要备份的站点" Text="需要备份的站点：" runat="server"></bairong:Help>
                        </td>
                        <td>
                            <asp:ListBox ID="BackupPublishmentSystemIDCollection" SelectionMode="Multiple" Rows="8" runat="server"></asp:ListBox>
                            &nbsp;&nbsp;
                            <asp:CheckBox ID="BackupIsBackupAll" runat="server" onClick="selectAll(this.checked);" Text="全部"></asp:CheckBox></td>
                    </tr>
                </asp:PlaceHolder>
            </asp:PlaceHolder>
        </table>

    </form>
</body>
</html>
