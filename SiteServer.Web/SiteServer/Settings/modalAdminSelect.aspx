<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalAdminSelect" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/header.aspx"-->
    </head>

    <body>
      <form class="form-inline" runat="server">
        <asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
        <ctrl:alerts runat="server" />

        <table class="table">
          <tr>
            <td style="border-top:none">

              <table class="table table-bordered table-hover">
                <tr class="info" treeItemLevel="2">
                  <td>
                    <img align="absmiddle" style="cursor:pointer" onClick="displayChildren(this);" isAjax="false" isOpen="true"
                      src="../assets/icons/tree/minus.gif" />
                    <img align="absmiddle" border="0" src="../assets/icons/tree/category.gif" />&nbsp;部门选择</td>
                </tr>
                <asp:Repeater ID="RptDepartment" runat="server">
                  <itemtemplate>
                    <asp:Literal id="ltlHtml" runat="server" />
                  </itemtemplate>
                </asp:Repeater>
              </table>

            </td>
            <td style="border-top:none">

              <table class="table table-bordered table-hover">
                <tr class="info" treeItemLevel="2">
                  <td>
                    <img align="absmiddle" src="../assets/icons/tree/minus.gif" />
                    <img align="absmiddle" border="0" src="../assets/icons/tree/category.gif" /> &nbsp;
                    <asp:Literal ID="LtlDepartment" runat="server"></asp:Literal>
                  </td>
                </tr>
                <asp:Repeater ID="RptUser" runat="server">
                  <itemtemplate>
                    <tr>
                      <td>
                        <img align="absmiddle" src="../assets/icons/tree/empty.gif" />
                        <img align="absmiddle" src="../assets/icons/tree/empty.gif" />
                        <img align="absmiddle" src="../assets/icons/menu/user.gif" /> &nbsp;
                        <asp:Literal ID="ltlUrl" runat="server"></asp:Literal>
                      </td>
                    </tr>
                  </itemtemplate>
                </asp:Repeater>
              </table>

            </td>
          </tr>
        </table>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->