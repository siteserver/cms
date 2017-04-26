<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageAd" %>
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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          固定广告类型：
          <asp:DropDownList ID="AdType" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="ReFresh" runat="server"></asp:DropDownList>
        </td>
      </tr>
    </table>
  </div>

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="AdName" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn
        HeaderText="广告名称">
        <ItemTemplate> <%#DataBinder.Eval(Container.DataItem,"AdName")%> </ItemTemplate>
        <ItemStyle cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="广告类型">
        <ItemTemplate> <%#GetAdType((string)DataBinder.Eval(Container.DataItem,"AdType"))%> </ItemTemplate>
        <ItemStyle Width="140" cssClass="center" />
      </asp:TemplateColumn>
      <asp:BoundColumn
        HeaderText="生效时间"
        DataField="StartDate"
        DataFormatString="{0:yyyy-MM-dd}"
        ReadOnly="true">
        <ItemStyle Width="70" cssClass="center" />
      </asp:BoundColumn>
      <asp:TemplateColumn HeaderText="是否有效">
        <ItemTemplate> <%#GetIsEnabled((string)DataBinder.Eval(Container.DataItem,"IsEnabled"))%> </ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn >
        <ItemTemplate> <a href="pageAdAdd.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&AdName=<%# DataBinder.Eval(Container.DataItem,"AdName")%>">编辑</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn >
        <ItemTemplate> <a href="pageAd.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&AdName=<%# DataBinder.Eval(Container.DataItem,"AdName")%>&Delete=True" onClick="javascript:return confirm('此操作将删除广告“<%# DataBinder.Eval(Container.DataItem,"AdName")%>”，确认吗？');">删除</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="<input type=&quot;checkbox&quot; onclick=&quot;_checkFormAll(this.checked)&quot;>">
        <ItemTemplate>
          <input type="checkbox" name="AdNameCollection" value='<%#DataBinder.Eval(Container.DataItem, "AdName")%>' />
        </ItemTemplate>
        <ItemStyle Width="20" cssClass="center"/>
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddAd" OnClick="AddAd_OnClick" Text="添加广告" runat="server" />
    <asp:Button class="btn" id="Delete" OnClick="Delete_OnClick" Text="删除广告" runat="server" />
  </ul>

</form>
</body>
</html>
