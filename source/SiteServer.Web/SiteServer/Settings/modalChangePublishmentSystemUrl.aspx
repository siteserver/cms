<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalChangePublishmentSystemUrl" Trace="false" %>
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
        <asp:Button ID="btnSubmit" UseSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" Style="display: none" />
        <bairong:Alerts Text="此操作将修改站点的访问地址，修改前请先确认此地址能够被访问。" runat="server"></bairong:Alerts>

        <table class="table table-noborder table-hover">
          <asp:PlaceHolder id="PhUrlSettings" runat="server">
            <tr>
              <td width="160">Web部署方式：</td>
              <td>
                <asp:DropDownList id="DdlIsSeparatedWeb" AutoPostBack="true" OnSelectedIndexChanged="DdlIsSeparatedWeb_SelectedIndexChanged"
                  runat="server"></asp:DropDownList>
                <br />
                <span>设置网站页面部署方式</span>
              </td>
            </tr>

            <asp:PlaceHolder ID="PhSeparatedWeb" runat="server">
              <tr>
                <td width="160">独立部署Web访问地址：</td>
                <td>
                  <asp:TextBox id="TbSeparatedWebUrl" class="form-control" runat="server"></asp:TextBox>
                  <asp:RequiredFieldValidator ControlToValidate="TbSeparatedWebUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                  />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSeparatedWebUrl" ValidationExpression="[^']+" ErrorMessage=" *"
                    ForeColor="red" Display="Dynamic" />
                </td>
              </tr>
            </asp:PlaceHolder>
          </asp:PlaceHolder>

        </table>

      </form>
    </body>

    </html>