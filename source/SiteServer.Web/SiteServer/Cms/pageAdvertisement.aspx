<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageAdvertisement" %>
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
          漂浮广告类型：
          <asp:DropDownList ID="AdvertisementType" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="ReFresh" runat="server"></asp:DropDownList>
        </td>
      </tr>
    </table>
  </div>

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="AdvertisementName" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn
        HeaderText="广告名称">
        <ItemTemplate> <%#DataBinder.Eval(Container.DataItem,"AdvertisementName")%> </ItemTemplate>
        <ItemStyle Width="140" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="广告显示">
        <ItemTemplate> <%#GetAdvertisementInString((string)DataBinder.Eval(Container.DataItem,"AdvertisementName"))%> </ItemTemplate>
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="广告类型">
        <ItemTemplate> <%#GetAdvertisementType((string)DataBinder.Eval(Container.DataItem,"AdvertisementType"))%> </ItemTemplate>
        <ItemStyle Width="140" cssClass="center" />
      </asp:TemplateColumn>
      <asp:BoundColumn
        HeaderText="添加日期"
        DataField="AddDate"
        DataFormatString="{0:yyyy-MM-dd}"
        ReadOnly="true">
        <ItemStyle Width="70" cssClass="center" />
      </asp:BoundColumn>
      <asp:TemplateColumn >
        <ItemTemplate> <a href="pageAdvertisementAdd.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&AdvertisementName=<%# DataBinder.Eval(Container.DataItem,"AdvertisementName")%>">编辑</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn >
        <ItemTemplate> <a href="pageAdvertisement.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&AdvertisementName=<%# DataBinder.Eval(Container.DataItem,"AdvertisementName")%>&Delete=True" onClick="javascript:return confirm('此操作将删除广告“<%# DataBinder.Eval(Container.DataItem,"AdvertisementName")%>”，确认吗？');">删除</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="<input type=&quot;checkbox&quot; onclick=&quot;_checkFormAll(this.checked)&quot;>">
        <ItemTemplate>
          <input type="checkbox" name="AdvertisementNameCollection" value='<%#DataBinder.Eval(Container.DataItem, "AdvertisementName")%>' />
        </ItemTemplate>
        <ItemStyle Width="20" cssClass="center"/>
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddAdvertisement" OnClick="AddAdvertisement_OnClick" Text="添加广告" runat="server" />
    &nbsp;
    <asp:Button class="btn" id="Delete" OnClick="Delete_OnClick" Text="删除广告" runat="server" />
  </ul>

</form>
</body>
</html>
