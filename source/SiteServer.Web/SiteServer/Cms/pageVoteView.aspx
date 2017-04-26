<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageVoteView" %>
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

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="VoteItemID" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" runat="server">
    <Columns>
      <asp:TemplateColumn>
        <ItemTemplate> <%# Container.ItemIndex + 1%> </ItemTemplate>
        <ItemStyle HorizontalAlign="left"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlItemHtml" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="left"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <TABLE height=13 cellSpacing=0 borderColorDark=#ffffff cellPadding=0 width=<%# GetDisplayColorWidth(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"VoteNum")))%> bgColor=<%#DataBinder.Eval(Container.DataItem,"DisplayColor")%> borderColorLight=#000000 border=0>
            <TBODY>
              <TR>
                <TD style="BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; BORDER-LEFT: 1px solid; BORDER-BOTTOM: 1px solid" vAlign=center width="100%"></TD>
              </TR>
            </TBODY>
          </TABLE>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="left" Width="300"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn >
        <ItemTemplate> <%# DataBinder.Eval(Container.DataItem,"VoteNum")%>票（<%# GetDisplayColorWidth(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"VoteNum")))%>） </ItemTemplate>
        <ItemStyle Width="100" cssClass="center" />
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <input type=button class="btn" onClick="location.href='pageVote.aspx?PublishmentSystemID=<%=PublishmentSystemId%>';" value="返 回" />
  </ul>

</form>
</body>
</html>
