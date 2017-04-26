<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PagePublishmentSystem" EnableViewState="false" %>
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

        <asp:DataGrid ID="dgContents" ShowHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" GridLines="none" runat="server">
            <Columns>
                <asp:TemplateColumn HeaderText="站点名称">
                    <ItemTemplate>
                        <asp:Literal ID="ltlPublishmentSystemName" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="站点类型">
                    <ItemTemplate>
                        <asp:Literal ID="ltlPublishmentSystemType" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="110" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="文件夹">
                    <ItemTemplate>
                        <asp:Literal ID="ltlPublishmentSystemDir" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="创建日期">
                    <ItemTemplate>
                        <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="70" HorizontalAlign="left" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="保存为站点模板">
                    <ItemTemplate>
                        <nobr><a href="pageSiteTemplateSave.aspx?PublishmentSystemID=<%# Container.DataItem%>">保存</a></nobr>
                    </ItemTemplate>
                    <ItemStyle Width="100" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="修改属性">
                    <ItemTemplate>
                        <nobr><a href="pagePublishmentSystemEdit.aspx?PublishmentSystemID=<%# Container.DataItem%>">修改</a></nobr>
                    </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="站点路径转移">
                    <ItemTemplate>
                        <asp:Literal ID="ltlChangeType" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="90" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="整站替换">
                    <ItemTemplate>
                        <nobr><a href="pagePublishmentSystemReplace.aspx?PublishmentSystemID=<%# Container.DataItem%>">替换</a></nobr>
                    </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="整站删除">
                    <ItemTemplate>
                        <asp:Literal ID="ltlDelete" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="上升">
                    <ItemTemplate>
                        <asp:Literal ID="ltUpLink" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="下降">
                    <ItemTemplate>
                        <asp:Literal ID="ltDownLink" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>

    </form>
</body>
</html>
