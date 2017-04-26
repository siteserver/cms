<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageAdv" %>
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
          <asp:DropDownList ID="AdAreaNameList" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="ReFresh" runat="server"></asp:DropDownList>
        </td>
      </tr>
    </table>
  </div>

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="AdvID" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn
        HeaderText="广告名称">
        <ItemTemplate> <%#DataBinder.Eval(Container.DataItem,"AdvName")%> </ItemTemplate>
         <ItemStyle Width="140" cssClass="center" />
      </asp:TemplateColumn>
       <asp:TemplateColumn
        HeaderText="广告显示">
        <ItemTemplate> <%#GetAdvInString((int)DataBinder.Eval(Container.DataItem,"AdvID"))%> </ItemTemplate>
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="所属广告位">
        <ItemTemplate> <%#GetAdAreaName((int)DataBinder.Eval(Container.DataItem,"AdAreaID"))%> </ItemTemplate>
      <ItemStyle Width="120" cssClass="center" />
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
        <ItemTemplate> <a href="pageAdvAdd.aspx?PublishmentSystemID=<%=Request.QueryString["PublishmentSystemID"]%>&AdvID=<%# DataBinder.Eval(Container.DataItem,"AdvID")%>">编辑</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
       <asp:TemplateColumn >
        <ItemTemplate> <a href="pageAdMaterial.aspx?PublishmentSystemID=<%=Request.QueryString["PublishmentSystemID"]%>&AdvID=<%# DataBinder.Eval(Container.DataItem,"AdvID")%>">广告物料</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>  
      <asp:TemplateColumn >
        <ItemTemplate> <a href="pageAdv.aspx?PublishmentSystemID=<%=Request.QueryString["PublishmentSystemID"]%>&AdvID=<%# DataBinder.Eval(Container.DataItem,"AdvID")%>&Delete=True" onClick="javascript:return confirm('此操作将删除广告“<%# DataBinder.Eval(Container.DataItem,"AdvName")%>”，确认吗？');">删除</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="<input type=&quot;checkbox&quot; onclick=&quot;_checkFormAll(this.checked)&quot;>">
        <ItemTemplate>
          <input type="checkbox" name="AdvIDCollection" value='<%#DataBinder.Eval(Container.DataItem, "AdvID")%>' />
        </ItemTemplate>
        <ItemStyle Width="20" cssClass="center"/>
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddAdv" OnClick="AddAdv_OnClick" Text="添加广告" runat="server" />
    &nbsp;
    <asp:Button class="btn" id="Delete" OnClick="Delete_OnClick" Text="删除广告" runat="server" />
  </ul>

</form>
</body>
</html>
