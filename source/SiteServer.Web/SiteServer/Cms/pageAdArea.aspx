<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageAdArea" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form id="Form1" class="form-inline" runat="server">
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts ID="Alerts1" runat="server" />
  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          广告位名称：
         <asp:TextBox ID="AdAreaName"  runat="server"></asp:TextBox> &nbsp; <asp:Button ID="Seach" runat="server" OnClick="ReFresh" Text="查询" />
        </td>
      </tr>
    </table>
  </div>

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="AdAreaName" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn
        HeaderText="广告位名称">
        <ItemTemplate> <%#DataBinder.Eval(Container.DataItem,"AdAreaName")%> </ItemTemplate>
        <ItemStyle Width="220" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="广告位描述">
        <ItemTemplate> <%#DataBinder.Eval(Container.DataItem,"Summary")%> </ItemTemplate>
      </asp:TemplateColumn>
      <asp:BoundColumn
        HeaderText="添加日期"
        DataField="AddDate"
        DataFormatString="{0:yyyy-MM-dd}"
        ReadOnly="true">
        <ItemStyle Width="70" cssClass="center" />
      </asp:BoundColumn>
        <asp:TemplateColumn HeaderText="是否有效">
        <ItemTemplate> <%#GetIsEnabled((string)DataBinder.Eval(Container.DataItem,"IsEnabled"))%> </ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn >
        <ItemTemplate> <a href="pageAdAreaAdd.aspx?PublishmentSystemID=<%=Request.QueryString["PublishmentSystemID"]%>&AdAreaName=<%# DataBinder.Eval(Container.DataItem,"AdAreaName")%>">编辑</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn >
        <ItemTemplate> <a href="pageAdv.aspx?PublishmentSystemID=<%=Request.QueryString["PublishmentSystemID"]%>&AdAreaID=<%# DataBinder.Eval(Container.DataItem,"AdAreaID")%>">广告</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn >
        <ItemTemplate> <a href="pageAdArea.aspx?PublishmentSystemID=<%=Request.QueryString["PublishmentSystemID"]%>&AdAreaName=<%# DataBinder.Eval(Container.DataItem,"AdAreaName")%>&Delete=True" onClick="javascript:return confirm('此操作将删除广告位“<%# DataBinder.Eval(Container.DataItem,"AdAreaName")%>”，确认吗？');">删除</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="<input type=&quot;checkbox&quot; onclick=&quot;_checkFormAll(this.checked)&quot;>">
        <ItemTemplate>
          <input type="checkbox" name="AdAreaNameCollection" value='<%#DataBinder.Eval(Container.DataItem, "AdAreaName")%>' />
        </ItemTemplate>
        <ItemStyle Width="20" cssClass="center"/>
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddAdArea" OnClick="AddAdArea_OnClick" Text="添加广告位" runat="server" />
    &nbsp;
    <asp:Button class="btn" id="Delete" OnClick="Delete_OnClick" Text="删除广告位" runat="server" />
  </ul>

</form>
</body>
</html>
