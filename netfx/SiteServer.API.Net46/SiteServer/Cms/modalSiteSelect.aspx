<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalSiteSelect" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html class="modalPage">

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
</head>

<body>
  <form runat="server">
    <ctrl:alerts runat="server" />

    <table id="contents" class="table tablesaw table-hover m-0">
      <thead>
        <tr class="thead">
          <th>站点名称</th>
          <th>文件夹</th>
          <th>Web访问地址</th>
        </tr>
      </thead>
      <tbody>
        <asp:Repeater ID="RptContents" runat="server">
          <ItemTemplate>
            <tr>
              <td>
                <asp:Literal ID="ltlName" runat="server"></asp:Literal>
              </td>
              <td>
                <asp:Literal ID="ltlDir" runat="server"></asp:Literal>
              </td>
              <td>
                <asp:Literal ID="ltlWebUrl" runat="server"></asp:Literal>
              </td>
            </tr>
          </ItemTemplate>
        </asp:Repeater>
      </tbody>
    </table>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->