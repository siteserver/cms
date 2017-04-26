<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageGatherRule" %>
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

        <asp:DataGrid ID="dgContents" ShowHeader="true" AutoGenerateColumns="false" DataKeyField="GatherRuleName" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" GridLines="none" runat="server">
            <Columns>
                <asp:TemplateColumn
                    HeaderText="采集规则名称">
                    <ItemTemplate>
                        <asp:Literal ID="ltlGatherRuleName" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="180" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn
                    HeaderText="采集网址">
                    <ItemTemplate>
                        <asp:Literal ID="ltlGatherUrl" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" />
                </asp:TemplateColumn>
                <asp:TemplateColumn
                    HeaderText="最近一次采集时间">
                    <ItemTemplate>
                        <asp:Literal ID="ltlLastGatherDate" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="150" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn
                    HeaderText="是否自动生成">
                    <ItemTemplate>
                        <asp:Literal ID="ltlIsAutoCreate" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="150" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <asp:Literal ID="ltlTestGatherUrl" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="70" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <img style="vertical-align: middle;" src="../Pic/Icon/gather.gif">
                        <asp:Literal ID="ltlStartGatherUrl" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="100" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <asp:Literal ID="ltlEditLink" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <asp:Literal ID="ltlCopyLink" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <asp:Literal ID="ltlDeleteLink" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn
                    HeaderText="<input type='checkbox' onClick='_checkFormAll(this.checked)'>">
                    <ItemTemplate>
                        <input type="checkbox" name="GatherRuleNameCollection" value='<%#DataBinder.Eval(Container.DataItem, "GatherRuleName")%>' />
                    </ItemTemplate>
                    <ItemStyle Width="20" CssClass="center" />
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>

        <ul class="breadcrumb breadcrumb-button">
            <input type="button" class="btn btn-success" onclick="location.href='pageGatherRuleAdd.aspx?PublishmentSystemID=<%=PublishmentSystemId%>';" value="添加采集规则" />
            <input type="button" class="btn" onclick="<%=GetImportClickString()%>" value="导入采集规则" />
            <asp:Button class="btn" ID="Export" Text="导出采集规则" runat="server" />
            <asp:Button class="btn" ID="Start" Text="开始采集" runat="server" />
            <input type="button" class="btn" onclick="<%=GetAutoCreateClickString()%>" value="打开自动生成" />
            <input type="button" class="btn" onclick="<%=GetNoAutoCreateClickString()%>" value="关闭自动生成" />
        </ul>

    </form>
</body>
</html>

