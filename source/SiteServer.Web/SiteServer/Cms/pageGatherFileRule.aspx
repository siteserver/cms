<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageGatherFileRule" %>
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
                    <ItemTemplate><%#DataBinder.Eval(Container.DataItem,"GatherRuleName")%> </ItemTemplate>
                    <ItemStyle Width="180" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn
                    HeaderText="采集网址">
                    <ItemTemplate><%#GetGatherUrl((string)DataBinder.Eval(Container.DataItem,"GatherUrl"))%> </ItemTemplate>
                    <ItemStyle CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn
                    HeaderText="最近一次采集时间">
                    <ItemTemplate><%#GetLastGatherDate((DateTime)DataBinder.Eval(Container.DataItem,"LastGatherDate"))%> </ItemTemplate>
                    <ItemStyle Width="150" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn
                    HeaderText="是否自动生成">
                    <ItemTemplate><%#GetIsAutoCreate(bool.Parse(DataBinder.Eval(Container.DataItem,"IsAutoCreate").ToString()))%> </ItemTemplate>
                    <ItemStyle Width="150" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate><%#GetTestGatherUrl((string)DataBinder.Eval(Container.DataItem,"GatherRuleName"))%> </ItemTemplate>
                    <ItemStyle Width="70" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <img style="vertical-align: middle;" src="../Pic/Icon/gather.gif"><%#GetStartGatherUrl((string)DataBinder.Eval(Container.DataItem,"GatherRuleName"))%>
                    </ItemTemplate>
                    <ItemStyle Width="100" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate><%#GetEditLink((string)DataBinder.Eval(Container.DataItem,"GatherRuleName"))%> </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate><a href="pageGatherFileRule.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&GatherRuleName=<%# DataBinder.Eval(Container.DataItem,"GatherRuleName")%>&Copy=True">复制</a> </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate><a href="pageGatherFileRule.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&GatherRuleName=<%# DataBinder.Eval(Container.DataItem,"GatherRuleName")%>&Delete=True" onclick="javascript:return confirm('此操作将删除采集规则“<%# DataBinder.Eval(Container.DataItem,"GatherRuleName")%>”，确认吗？');">删除</a> </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn
                    HeaderText="<input type='checkbox' onClick='_checkFormAll(this.checked)'>">
                    <ItemTemplate>
                        <input type="checkbox" name="GatherFileRuleNameCollection" value='<%#DataBinder.Eval(Container.DataItem, "GatherRuleName")%>' />
                    </ItemTemplate>
                    <ItemStyle Width="20" CssClass="center" />
                </asp:TemplateColumn>
            </Columns>

        </asp:DataGrid>

        <ul class="breadcrumb breadcrumb-button">
            <input type="button" class="btn btn-success" onclick="location.href='pageGatherFileRuleAdd.aspx?PublishmentSystemID=<%=PublishmentSystemId%>    ';" value="添加采集规则" />
            <asp:Button class="btn" ID="Start" Text="开始采集" runat="server" />
            <input type="button" class="btn" onclick="<%=GetAutoCreateClickString()%>" value="打开自动生成" />
            <input type="button" class="btn" onclick="<%=GetNoAutoCreateClickString()%>" value="关闭自动生成" />
        </ul>

    </form>
</body>
</html>

