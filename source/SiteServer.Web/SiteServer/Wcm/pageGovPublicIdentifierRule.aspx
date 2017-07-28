<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovPublicIdentifierRule" %>
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
  <bairong:alerts text="如果修改了索引号生成规则，之前添加的信息将不受影响。" runat="server" />

  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          索引号预览：<asp:Literal ID="ltlPreview" runat="server"></asp:Literal>
        </td>
      </tr>
    </table>
  </div>

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
  	<Columns>
        <asp:TemplateColumn HeaderText="规则排序">
			<ItemTemplate>
				<asp:Literal ID="ltlIndex" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle cssClass="center" Width="70" />
		</asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="规则名称">
			<ItemTemplate>
				<asp:Literal ID="ltlRuleName" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle cssClass="center" />
		</asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="规则类型">
			<ItemTemplate>
				<asp:Literal ID="ltlIdentifierType" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle cssClass="center" />
		</asp:TemplateColumn>
         <asp:TemplateColumn HeaderText="最小位数">
			<ItemTemplate>
				<asp:Literal ID="ltlMinLength" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle Width="100" cssClass="center" />
		</asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="分隔符">
			<ItemTemplate>
				<asp:Literal ID="ltlSuffix" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle Width="100" cssClass="center" />
		</asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="上升">
			<ItemTemplate>
				<asp:HyperLink ID="hlUpLinkButton" runat="server"><img src="../assets/icons/up.gif" border="0" alt="上升" /></asp:HyperLink>
			</ItemTemplate>
			<ItemStyle Width="40" cssClass="center" />
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="下降">
			<ItemTemplate>
				<asp:HyperLink ID="hlDownLinkButton" runat="server"><img src="../assets/icons/down.gif" border="0" alt="下降" /></asp:HyperLink>
			</ItemTemplate>
			<ItemStyle Width="40" cssClass="center" />
		</asp:TemplateColumn>
        <asp:TemplateColumn>
			<ItemTemplate>
				<asp:Literal ID="ltlSettingUrl" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle Width="120" cssClass="center" />
		</asp:TemplateColumn>
        <asp:TemplateColumn>
			<ItemTemplate>
				<asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle Width="50" cssClass="center" />
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<ItemTemplate>
				<asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle Width="50" cssClass="center" />
		</asp:TemplateColumn>
	</Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddButton" Text="新增索引规则" runat="server" />
  </ul>

</form>
</body>
</html>
