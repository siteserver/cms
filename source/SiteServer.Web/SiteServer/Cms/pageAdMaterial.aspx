<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageAdMaterial" %>
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
    
  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="AdMaterialID" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn
        HeaderText="广告物料名称">
        <ItemTemplate> <%#DataBinder.Eval(Container.DataItem,"AdMaterialName")%> </ItemTemplate>
        <ItemStyle  cssClass="center" />  
      </asp:TemplateColumn>
         <asp:TemplateColumn
        HeaderText="所属广告">
        <ItemTemplate> <%#GetAdvName((int)DataBinder.Eval(Container.DataItem, "AdvID"))%> </ItemTemplate>
       <ItemStyle Width="140" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="物料类型">
        <ItemTemplate> <%#GetAdMaterialType((string)DataBinder.Eval(Container.DataItem, "AdMaterialType"))%> </ItemTemplate>
       <ItemStyle Width="140" cssClass="center" />
      </asp:TemplateColumn>
       <asp:TemplateColumn HeaderText="是否有效">
        <ItemTemplate> <%#GetIsEnabled((string)DataBinder.Eval(Container.DataItem,"IsEnabled"))%> </ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn >
        <ItemTemplate><%#GetEditUrl((int)DataBinder.Eval(Container.DataItem,"AdMaterialID"))%></ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
     <asp:TemplateColumn >
        <ItemTemplate> <a href="pageAdMaterial.aspx?PublishmentSystemID=<%=Request.QueryString["PublishmentSystemID"]%>&AdMaterialID=<%# DataBinder.Eval(Container.DataItem,"AdMaterialID")%>&Delete=True" onClick="javascript:return confirm('此操作将删除广告物料“<%# DataBinder.Eval(Container.DataItem,"AdMaterialName")%>”，确认吗？');">删除</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="<input type=&quot;checkbox&quot; onclick=&quot;_checkFormAll(this.checked)&quot;>">
        <ItemTemplate>
          <input type="checkbox" name="AdMaterialIDCollection" value='<%#DataBinder.Eval(Container.DataItem, "AdMaterialID")%>' />
        </ItemTemplate>
        <ItemStyle Width="20" cssClass="center"/>
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddAdMaterial" Text="添加广告物料" runat="server" />
    &nbsp;
    <asp:Button class="btn" id="Delete" OnClick="Delete_OnClick" Text="删除广告物料" runat="server" />
  </ul>

</form>
</body>
</html>
