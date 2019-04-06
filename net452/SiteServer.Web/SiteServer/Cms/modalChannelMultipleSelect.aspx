<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelMultipleSelect" Trace="false"%>
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

        <asp:PlaceHolder id="PhSiteId" runat="server">
          <div class="form-group form-row">
            <label class="col-2 col-form-label text-right m-t-5">选择站点</label>
            <div class="col-6">
              <asp:DropDownList class="form-control" ID="DdlSiteId" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlSiteId_OnSelectedIndexChanged">
              </asp:DropDownList>
            </div>
            <div class="col-4"></div>
          </div>
        </asp:PlaceHolder>

        <table class="table table-tree tablesaw table-hover m-0">
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
                <i class="ion-ios-home"></i>
                <asp:Literal ID="LtlChannelName" runat="server"></asp:Literal>
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