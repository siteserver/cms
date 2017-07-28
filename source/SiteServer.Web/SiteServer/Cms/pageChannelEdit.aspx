<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageChannelEdit" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=7" />
    <!--#include file="../inc/header.aspx"-->
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->
    <form class="form-inline" runat="server">
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <ul class="nav nav-pills">
            <li id="tab1" class="active"><a href="javascript:;" onclick="_toggleTab(1,2);">基本信息</a></li>
            <li id="tab2"><a href="javascript:;" onclick="_toggleTab(2,2);">高级设置</a></li>
        </ul>

        <div id="column1" class="popover popover-static">
            <h3 class="popover-title">基本信息</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                        <td width="150">栏目名称：</td>
                        <td>
                            <asp:TextBox Columns="45" MaxLength="255" ID="NodeName" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator"
                                ControlToValidate="NodeName"
                                ErrorMessage=" *" ForeColor="red"
                                Display="Dynamic"
                                runat="server" /></td>
                    </tr>
                    <tr>
                        <td>栏目索引：</td>
                        <td>
                            <asp:TextBox Columns="45" MaxLength="255" ID="NodeIndexName" runat="server" />
                            <asp:RegularExpressionValidator
                                runat="server"
                                ControlToValidate="NodeIndexName"
                                ValidationExpression="[^']+"
                                ErrorMessage=" *" ForeColor="red"
                                Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                        <td>内容模型：</td>
                        <td>
                            <asp:DropDownList ID="ContentModelID" runat="server"></asp:DropDownList></td>
                    </tr>
                    <tr id="FilePathRow" runat="server">
                        <td>生成页面路径：</td>
                        <td>
                            <asp:TextBox Columns="45" MaxLength="200" ID="FilePath" runat="server" />
                            <asp:RegularExpressionValidator
                                runat="server"
                                ControlToValidate="FilePath"
                                ValidationExpression="[^']+"
                                ErrorMessage=" *" ForeColor="red"
                                Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                        <td>栏目页面命名规则：</td>
                        <td>
                            <asp:TextBox Columns="38" MaxLength="200" ID="ChannelFilePathRule" runat="server" />
                            <asp:Button ID="CreateChannelRule" class="btn" Text="构造" runat="server"></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td>内容页面命名规则：</td>
                        <td>
                            <asp:TextBox Columns="38" MaxLength="200" ID="ContentFilePathRule" runat="server" />
                            <asp:Button ID="CreateContentRule" class="btn" Text="构造" runat="server"></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td>栏目图片地址：</td>
                        <td>
                            <asp:TextBox ID="NavigationPicPath" MaxLength="100" size="45" runat="server" />
                            <asp:Button ID="SelectImage" class="btn" Text="选择" runat="server"></asp:Button>
                            <asp:Button ID="UploadImage" class="btn" Text="上传" runat="server"></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <bairong:TextEditorControl ID="Content" runat="server"></bairong:TextEditorControl>
                        </td>
                    </tr>
                    <tr>
                        <td>关键字列表：</td>
                        <td>
                            <asp:TextBox Rows="3" Width="350" MaxLength="100" TextMode="MultiLine" ID="Keywords" runat="server" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="Keywords"
                                ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                            <=100
                            <br />
                            <span class="gray">注意：各关键词间用英文逗号“,”隔开。</span>
                        </td>
                    </tr>
                    <tr>
                        <td>页面描述：</td>
                        <td>
                            <asp:TextBox Width="350" Rows="4" MaxLength="200" TextMode="MultiLine" ID="Description" runat="server" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="Description"
                                ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                            <=200
                        </td>
                    </tr>
                    <bairong:ChannelAuxiliaryControl ID="ControlForAuxiliary" runat="server" />
                    <tr>
                        <td>栏目组：</td>
                        <td>
                            <asp:CheckBoxList CssClass="checkboxlist" ID="NodeGroupNameCollection" DataTextField="NodeGroupName" DataValueField="NodeGroupName" RepeatDirection="Horizontal" RepeatColumns="5" runat="server" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

        <div id="column2" style="display: none" class="popover popover-static">
            <h3 class="popover-title">高级设置</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr id="LinkURLRow" runat="server">
                        <td width="150">外部链接：</td>
                        <td>
                            <asp:TextBox Columns="45" MaxLength="200" ID="LinkUrl" runat="server" />
                            <asp:RegularExpressionValidator
                                runat="server"
                                ControlToValidate="LinkUrl"
                                ValidationExpression="[^']+"
                                ErrorMessage=" *" ForeColor="red"
                                Display="Dynamic" />
                        </td>
                    </tr>
                    <tr id="LinkTypeRow" runat="server">
                        <td>链接类型：</td>
                        <td>
                            <asp:DropDownList ID="LinkType" runat="server"></asp:DropDownList></td>
                    </tr>
                    <tr id="ChannelTemplateIDRow" runat="server">
                        <td>栏目模版：</td>
                        <td>
                            <asp:DropDownList ID="ChannelTemplateID" DataTextField="TemplateName" DataValueField="TemplateID" runat="server"></asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td>本栏目的内容模版：</td>
                        <td>
                            <asp:DropDownList ID="ContentTemplateID" DataTextField="TemplateName" DataValueField="TemplateID" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>可以添加栏目：</td>
                        <td>
                            <asp:RadioButtonList ID="IsChannelAddable" RepeatDirection="Horizontal" class="noborder" runat="server">
                                <asp:ListItem Text="是" Selected="True" />
                                <asp:ListItem Text="否" />
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td>可以添加内容：</td>
                        <td>
                            <asp:RadioButtonList ID="IsContentAddable" RepeatDirection="Horizontal" class="noborder" runat="server">
                                <asp:ListItem Text="是" Selected="True" />
                                <asp:ListItem Text="否" />
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>

            </div>
        </div>

        <hr />
        <table class="table noborder">
            <tr>
                <td class="center">
                    <asp:Button class="btn btn-primary" ID="Submit" Text="修 改" OnClick="Submit_OnClick" runat="server" />
                    <input class="btn" type="button" onclick="location.href='<%=ReturnUrl%>    ';return false;" value="返 回" />
                </td>
            </tr>
        </table>

    </form>
</body>
</html>
