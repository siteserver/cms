<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelMultipleSelect" Trace="false"%>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <bairong:alerts runat="server" />

        <div class="form-horizontal">

          <asp:PlaceHolder id="PhPublishmentSystemId" runat="server">
            <div class="form-group">
              <label class="col-xs-3 control-label text-right">选择站点</label>
              <div class="col-xs-8">
                <asp:DropDownList class="form-control" ID="DdlPublishmentSystemId" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlPublishmentSystemId_OnSelectedIndexChanged">
                </asp:DropDownList>
              </div>
              <div class="col-xs-1">

              </div>
            </div>
          </asp:PlaceHolder>

          <table class="table table-hover m-5">
            <tr class="info thead">
              <td>
                点击栏目名称进行选择
              </td>
            </tr>
            <tr treeItemLevel="0">
              <td>
                <img align="absmiddle" src="../assets/icons/tree/minus.gif" />
                <img align="absmiddle" border="0" src="../assets/icons/tree/folder.gif" />
                <asp:Literal ID="LtlChannelName" runat="server"></asp:Literal>
              </td>
            </tr>
            <asp:Repeater ID="RptChannel" runat="server">
              <itemtemplate>
                <asp:Literal id="ltlHtml" runat="server" />
              </itemtemplate>
            </asp:Repeater>
          </table>

        </div>

      </form>
    </body>

    </html>