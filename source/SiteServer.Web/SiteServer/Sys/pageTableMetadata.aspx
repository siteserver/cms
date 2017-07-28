<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PageTableMetadata" %>
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

        <asp:DataGrid ID="DgContents" ShowHeader="true" AutoGenerateColumns="false" DataKeyField="TableMetadataID" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" GridLines="none" runat="server">
            <Columns>
                <asp:TemplateColumn
                    HeaderText="字段名">
                    <ItemTemplate>
                        <asp:Literal ID="ltlAttributeName" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="140" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="显示名称">
                    <ItemTemplate>
                        <asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="140" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="数据类型">
                    <ItemTemplate>
                        <asp:Literal ID="ltlDataType" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="100" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="表单提交类型">
                    <ItemTemplate>
                        <asp:Literal ID="ltlInputType" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="120" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="是否显示">
                    <ItemTemplate>
                        <asp:Literal ID="ltlIsVisible" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="70" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="验证规则">
                    <ItemTemplate>
                        <asp:Literal ID="ltlValidate" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="130" HorizontalAlign="left" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="上升">
                    <ItemTemplate>
                        <asp:HyperLink ID="UpLinkButton" CommandName="UP" runat="server"><img src="../Pic/icon/up.gif" border="0" alt="上升" /></asp:HyperLink>
                    </ItemTemplate>
                    <ItemStyle Width="40" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="下降">
                    <ItemTemplate>
                        <asp:HyperLink ID="DownLinkButton" CommandName="DOWN" runat="server"><img src="../Pic/icon/down.gif" border="0" alt="下降" /></asp:HyperLink>
                    </ItemTemplate>
                    <ItemStyle Width="40" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="显示样式">
                    <ItemTemplate>
                        <asp:Literal ID="ltlStyle" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="80" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="表单验证">
                    <ItemTemplate>
                        <asp:Literal ID="ltlEditValidate" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="70" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="50" CssClass="center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="60" CssClass="center" />
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>

        <br>

        <div id="DivSyncTable" runat="server" class="popover popover-static">
            <h3 class="popover-title">同步辅助表</h3>
            <div class="popover-content">

                <table class="table noborder">
                    <tr>
                        <td>此辅助表在创建后被修改，与数据库中的实际表结构有差别，请同步辅助表。
                        </td>
                    </tr>
                </table>

                <hr />
                <table class="table noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" ID="SyncTableButton" Text="同步辅助表" OnClick="SyncTableButton_OnClick" runat="server"></asp:Button>
                        </td>
                    </tr>
                </table>

            </div>
        </div>

        <div class="popover popover-static" style="<%=GetSqlTableStyle()%>">
            <h3 class="popover-title">创建辅助表SQL命令</h3>
            <div class="popover-content">

                <table class="table noborder">
                    <tr>
                        <td>
                            <asp:Literal ID="LtlSqlString" runat="server" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

        <ul class="breadcrumb breadcrumb-button">
            <asp:Button class="btn btn-success" ID="BtnAddColumn" Text="添加字段" runat="server"></asp:Button>
            <asp:Button class="btn btn-success" ID="BtnBatchAddColumn" Text="批量添加字段" runat="server"></asp:Button>
            <span style="<%=GetCreateDbCommandElementStyle()%>">
                <input type="button" class="btn" onclick="location.href='pageTableMetadata.aspx?CreateDB=True&ENName=<%=Request.QueryString["ENName"]%>    &TableType=<%=Request.QueryString["TableType"]%>    ';" value="创建辅助表" />
            </span>
            <span style="<%=GetDeleteDbCommandElementStyle()%>">
                <input type="button" class="btn" onclick="if (confirm('此操作将删除辅助表“<%=Request.QueryString["ENName"]%>”，确认吗？'))location.href='pageTableMetadata.aspx?DeleteDB=True&ENName=<%=Request.QueryString["ENName"]%>    &TableType=<%=Request.QueryString["TableType"]%>    ';" value="删除辅助表" />
            </span>
            <span style="<%=GetReCreateDbCommandElementStyle()%>">
                <input type="button" class="btn" onclick="if (confirm('此操作将覆盖已建立的辅助表，表中已存数据将丢失，确认吗？'))location.href='pageTableMetadata.aspx?ReCreateDB=True&ENName=<%=Request.QueryString["ENName"]%>    &TableType=<%=Request.QueryString["TableType"]%>    ';" value="重新创建辅助表" />
            </span>
            <span style="<%=GetShowCommandElementStyle()%>">
                <input type="button" class="btn" onclick="location.href='pageTableMetadata.aspx?ShowCrateDBCommand=True&ENName=<%=Request.QueryString["ENName"]%>    &TableType=<%=Request.QueryString["TableType"]%>    ';" value="显示创建表SQL命令" />
            </span>
            <input type="button" class="btn" onclick="location.href='<%=GetReturnUrl() %>    ';" value="返 回" />
        </ul>

    </form>
</body>
</html>
