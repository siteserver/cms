<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageGatherDatabaseRuleAdd" %>
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
        <bairong:Alerts runat="server" />

        <div class="popover popover-static">
            <h3 class="popover-title">
                <asp:Literal ID="ltlPageTitle" runat="server" /></h3>
            <div class="popover-content">

                <asp:PlaceHolder ID="GatherRuleBase" runat="server" Visible="false">
                    <table class="table noborder table-hover">
                        <tr>
                            <td width="170">采集规则名称：</td>
                            <td>
                                <asp:TextBox Columns="25" MaxLength="50" ID="GatherRuleName" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>采集到栏目：</td>
                            <td>
                                <asp:DropDownList ID="NodeIDDropDownList" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>采集内容数：</td>
                            <td>
                                <asp:TextBox class="input-mini" Columns="4" MaxLength="4" ID="GatherNum" Style="text-align: right" Text="0" runat="server" />
                                <asp:RequiredFieldValidator
                                    ControlToValidate="GatherNum"
                                    ErrorMessage=" *" ForeColor="red"
                                    Display="Dynamic"
                                    runat="server" />
                                <asp:RegularExpressionValidator
                                    ControlToValidate="GatherNum"
                                    ValidationExpression="\d+"
                                    ErrorMessage="采集数只能是数字"
                                    foreColor="red"
                                    Display="Dynamic"
                                    runat="server" />
                                <br>
                                <span>需要采集的内容数，0代表采集所有内容</span>
                            </td>
                        </tr>
                        <tr>
                            <td>不经过审核：</td>
                            <td>
                                <asp:RadioButtonList ID="IsChecked" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server">
                                    <asp:ListItem Value="True" Selected="true">是</asp:ListItem>
                                    <asp:ListItem Value="False">否</asp:ListItem>
                                </asp:RadioButtonList>
                                <span>采集的内容是否不经过审核直接添加到栏目中</span>
                            </td>
                        </tr>
                        <tr>
                            <td>自动生成：</td>
                            <td>
                                <asp:RadioButtonList ID="IsAutoCreate" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server">
                                    <asp:ListItem Value="True" Selected="true">是</asp:ListItem>
                                    <asp:ListItem Value="False">否</asp:ListItem>
                                </asp:RadioButtonList>
                                <span>采集的内容是否自动生成</span>
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="GatherDatabaseLogin" runat="server" Visible="false">
                    <table class="table noborder table-hover">
                        <tr>
                            <td width="170">数据库类型：</td>
                            <td>
                                <asp:DropDownList ID="DatabaseType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DatabaseType_Changed"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="DatabaseServerRow" runat="server">
                            <td>IP地址或者服务器名：</td>
                            <td>
                                <asp:TextBox ID="DatabaseServer" runat="server" Columns="40" MaxLength="200" value="(local)"></asp:TextBox>
                                <br>
                                <span>数据库所在的IP地址或者服务器名</span>
                            </td>
                        </tr>
                        <tr id="DatabaseFilePathRow" runat="server">
                            <td>数据库文件路径：</td>
                            <td>
                                <asp:TextBox ID="DatabaseFilePath" runat="server" Columns="40" MaxLength="200"></asp:TextBox>
                                <br />
                                <span>以“~/”开头代表系统根目录，以“@/”开头代表站点根目录</span>
                            </td>
                        </tr>
                        <tr>
                            <td>登录账号：</td>
                            <td>
                                <asp:TextBox ID="UserName" Style="width: 150px;" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>登录密码：</td>
                            <td>
                                <asp:TextBox ID="Password" Style="width: 150px;" runat="server" TextMode="Password"></asp:TextBox>
                                <input type="hidden" runat="server" id="PasswordHidden" />
                                <input type="hidden" runat="server" id="DatabaseNameHidden" />
                                <input type="hidden" runat="server" id="RelatedTableNameHidden" />
                                <input type="hidden" runat="server" id="RelatedIdentityHidden" />
                                <input type="hidden" runat="server" id="RelatedOrderByHidden" />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="GatherRelatedTable" runat="server" Visible="false">
                    <table class="table noborder table-hover">
                        <tr id="DatabaseNameRow" runat="server">
                            <td width="170">采集数据库名称：</td>
                            <td>
                                <asp:DropDownList ID="DatabaseName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DatabaseName_Changed"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>采集表名称：</td>
                            <td>
                                <asp:DropDownList ID="RelatedTableName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RelatedTable_Changed"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>主键字段名称：</td>
                            <td>
                                <asp:DropDownList ID="RelatedIdentity" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>排序字段名称：</td>
                            <td>
                                <asp:DropDownList ID="RelatedOrderBy" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>倒序采集：</td>
                            <td>
                                <asp:RadioButtonList ID="IsOrderByDesc" RepeatDirection="Horizontal" class="noborder" runat="server">
                                    <asp:ListItem Value="True" Selected="true">是</asp:ListItem>
                                    <asp:ListItem Value="False">否</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td>Where条件语句：</td>
                            <td>
                                <asp:TextBox ID="WhereString" runat="server" TextMode="MultiLine" Rows="4" Columns="60"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="GatherTableMatch" runat="server" Visible="false">

                    <input type="hidden" id="TableMatchIDHidden" runat="server" />
                    <table class="table noborder table-hover">
                        <tr>
                            <td class="align-right">采集数据库：<asp:Literal ID="TableName" runat="server"></asp:Literal>
                            </td>
                            <td width="50"></td>
                            <td>内容数据库：<asp:Literal ID="TableNameToMatch" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td class="align-right">
                                <asp:ListBox ID="Columns" runat="server" SelectionMode="Single" Rows="14" Style="width: auto"></asp:ListBox>
                            </td>
                            <td>
                                <p>
                                    <asp:Button class="btn" ID="Add" Text=" <- " OnClick="Add_OnClick" runat="server" /></p>
                                <p>
                                    <asp:Button class="btn" ID="Delete" Text=" -> " OnClick="Delete_OnClick" runat="server" /></p>
                            </td>
                            <td>
                                <asp:ListBox ID="ColumnsToMatch" runat="server" SelectionMode="Single" Rows="14" Style="width: auto"></asp:ListBox>
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="Done" runat="server" Visible="false">

                    <blockquote>
                        <p>完成!</p>
                        <small>操作成功。</small>
                    </blockquote>

                </asp:PlaceHolder>
                <asp:PlaceHolder ID="OperatingError" runat="server" Visible="false">

                    <blockquote>
                        <p>发生错误</p>
                        <small>执行向导过程中出错</small>
                    </blockquote>

                    <div class="alert alert-error">
                        <h4>
                            <asp:Label ID="ErrorLabel" runat="server"></asp:Label></h4>
                    </div>

                </asp:PlaceHolder>

                <hr />
                <table class="table noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn" ID="Previous" OnClick="PreviousPanel" runat="server" Text="< 上一步"></asp:Button>
                            <asp:Button class="btn btn-primary" ID="Next" OnClick="NextPanel" runat="server" Text="下一步 >"></asp:Button>
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>

