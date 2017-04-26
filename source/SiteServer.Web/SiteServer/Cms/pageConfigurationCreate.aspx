<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationCreate" %>
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
        <bairong:Alerts Text="在此对生成页面进行详细设置" runat="server"></bairong:Alerts>

        <div class="popover popover-static">
            <h3 class="popover-title">页面生成设置</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                        <td width="260">当内容变动时是否生成本页：</td>
                        <td><asp:RadioButtonList ID="IsCreateContentIfContentChanged" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td>当栏目变动时是否生成本页：</td>
                        <td><asp:RadioButtonList ID="IsCreateChannelIfChannelChanged" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td>生成页面中是否显示相关信息：</td>
                        <td><asp:RadioButtonList ID="IsCreateShowPageInfo" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td>是否设置meta标签强制IE8兼容：</td>
                        <td><asp:RadioButtonList ID="IsCreateIE8Compatible" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td>是否设置meta标签强制浏览器清除缓存：</td>
                        <td><asp:RadioButtonList ID="IsCreateBrowserNoCache" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td>是否设置包含JS容错代码：</td>
                        <td><asp:RadioButtonList ID="IsCreateJsIgnoreError" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td>内容列表及搜索是否可包含重复标题：</td>
                        <td><asp:RadioButtonList ID="IsCreateSearchDuplicate" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td>是否将stl:include转换为SSI动态包含：</td>
                        <td>
                            <asp:RadioButtonList ID="IsCreateIncludeToSSI" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
                            <span>需要IIS启用服务器端包含功能，同时需要将生成页面后缀设置为“.shtml”</span>
                        </td>
                    </tr>
                    <tr>
                        <td>是否生成页面中包含JQuery脚本引用：</td>
                        <td><asp:RadioButtonList ID="IsCreateWithJQuery" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td>是否启用双击生成页面：</td>
                        <td>
                            <asp:RadioButtonList ID="IsCreateDoubleClick" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
                            <span>此功能通常用于制作调试期间，网站开发期间建议启用</span>
                        </td>
                    </tr>
                    <tr>
                        <td>翻页中生成的静态页面最大数：</td>
                        <td>
                            <asp:TextBox ID="tbCreateStaticMaxPage" class="input-mini" runat="server" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="tbCreateStaticMaxPage" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />页<br />
                            <span>设置翻页中生成的静态页面最大数，剩余页面将动态获取；设置为0代表将静态页面全部生成</span>
                            <br />
                            <span>也可以在模板的翻页标签中设置maxPage属性进行设置</span>
                        </td>
                    </tr>
                    <tr>
                        <td>根据添加日期限制是否生成内容：</td>
                        <td>
                            <asp:RadioButtonList ID="IsCreateStaticContentByAddDate" RepeatDirection="Horizontal" class="noborder" AutoPostBack="true" OnSelectedIndexChanged="IsCreateStaticContentByAddDate_SelectedIndexChanged" runat="server"></asp:RadioButtonList>
                            <span>若启用此选项，系统将不再生成所选添加时间之前的内容页</span>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="phIsCreateStaticContentByAddDate" runat="server">
                        <tr>
                            <td>生成内容添加日期限制：</td>
                            <td>
                                <bairong:DateTimeTextBox id="tbCreateStaticContentAddDate" class="input-medium" runat="server" />
                                <br />
                                <span>在此设置内容添加日期，此日期之前的内容页将不再生成</span>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </table>

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
