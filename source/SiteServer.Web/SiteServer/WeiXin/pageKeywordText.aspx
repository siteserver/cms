<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageKeywordText" %>
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
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts text="设置关键词自动回复，可以通过添加规则，用户发送的消息内如果有您设置的关键字，即可把您设置在此规则名中回复的内容自动发送给用户。" runat="server" />

  <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false"  HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:BoundColumn HeaderText="关键词" DataField="Keywords" >
        <ItemStyle Width="130" cssClass="center" />
      </asp:BoundColumn>
      <asp:TemplateColumn HeaderText="回复">
        <ItemTemplate>
          <asp:Literal ID="LtlReply" runat="server" />
        </ItemTemplate>
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="添加时间">
        <ItemTemplate>
          <asp:Literal ID="LtlAddDate" runat="server" />
        </ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="匹配规则">
        <ItemTemplate>
          <asp:Literal ID="LtlMatchType" runat="server" />
        </ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="启用">
        <ItemTemplate>
          <asp:Literal ID="LtlIsEnabled" runat="server" />
        </ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="上升">
        <ItemTemplate>
          <asp:HyperLink ID="hlUp" runat="server"><img src="../Pic/icon/up.gif" border="0" alt="上升" /></asp:HyperLink>
        </ItemTemplate>
        <ItemStyle Width="40" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="下降">
        <ItemTemplate>
          <asp:HyperLink ID="hlDown" runat="server"><img src="../Pic/icon/down.gif" border="0" alt="下降" /></asp:HyperLink>
        </ItemTemplate>
        <ItemStyle Width="40" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="LtlEditUrl" runat="server" />
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="LtlDeleteUrl" runat="server" />
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="BtnAdd" Text="添加关键词" runat="server" />
  </ul>

</form>
</body>
</html>
