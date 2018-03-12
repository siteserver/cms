<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelSelect" Trace="false"%>
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

        <table class="table tablesaw table-hover m-0">
          <thead>
            <tr class="thead">
              <th>
                点击栏目名称进行选择
              </th>
            </tr>
          </thead>
          <tbody>
            <tr treeItemLevel="1">
              <td>
                <img align="absmiddle" style="cursor:pointer" onClick="displayChildren(this);" isAjax="false" isOpen="true" src="../assets/icons/tree/minus.png"
                />
                <img align="absmiddle" border="0" src="../assets/icons/tree/folder.gif" />
                <asp:Literal ID="LtlSite" runat="server"></asp:Literal>
              </td>
            </tr>
            <asp:Repeater ID="RptChannel" runat="server">
              <itemtemplate>
                <asp:Literal id="ltlHtml" runat="server" />
              </itemtemplate>
            </asp:Repeater>
          </tbody>
        </table>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->