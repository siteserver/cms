<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplate" %>
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
          模板类型：
          <asp:DropDownList ID="DdlTemplateType" AutoPostBack="true" OnSelectedIndexChanged="DdlTemplateType_OnSelectedIndexChanged" class="input-medium" runat="server"></asp:DropDownList>
          模板名称/文件名：
          <asp:TextBox ID="TbKeywords" Columns="35" runat="server"></asp:TextBox>
          <asp:Button class="btn" onclick="btnSearch_Click" runat="server" Text="搜 索"></asp:Button>
        </td>
      </tr>
    </table>
  </div>

  <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="TemplateID" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn
      HeaderText="模板名称">
        <ItemTemplate>
          <asp:Literal ID="ltlTemplateName" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle cssClass="center" />
      </asp:TemplateColumn>
      <asp:BoundColumn
      HeaderText="模板文件"
      DataField="RelatedFileName"
      ReadOnly="true">
        <ItemStyle Wrap="false" HorizontalAlign="left" />
      </asp:BoundColumn>
      <asp:TemplateColumn HeaderText="生成文件名">
        <ItemTemplate>
          <asp:Literal ID="ltlFileName" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Wrap="false" HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="使用次数">
        <ItemTemplate>
          <asp:Literal ID="ltlUseCount" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="60" Wrap="false" HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="模板类型">
        <ItemTemplate>
          <asp:Literal ID="ltlTemplateType" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="60" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlDefaultUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlCopyUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlLogUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlCreateUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="30" cssClass="center" />
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Literal id="LtlCommands" runat="server" />
  </ul>

</form>
</body>
</html>
