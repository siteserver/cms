<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelAdd" Trace="false" %>
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
        <bairong:Alerts runat="server"></bairong:Alerts>

        <script language="javascript">
          function selectChannel(nodeNames, nodeID) {
            $('#nodeNames').html(nodeNames);
            $('#nodeID').val(nodeID);
          }
        </script>

        <table class="table noborder">
          <tr>
            <td width="100">父栏目： </td>
            <td colspan="3">
              <div class="fill_box">
                <div class="addr_base addr_normal">
                  <b id="nodeNames"></b>
                  <input id="nodeID" name="nodeID" value="0" type="hidden">
                </div>
              </div>
              <asp:HyperLink id="HlSelectChannel" class="btn_pencil" runat="server"><span class="pencil"></span>　选择</asp:HyperLink>
              <asp:Literal ID="LtlSelectChannelScript" runat="server"></asp:Literal>
            </td>
          </tr>
          <tr>
            <td width="100">内容模型：</td>
            <td width="230">
              <asp:DropDownList ID="DdlContentModelId" runat="server"></asp:DropDownList>
            </td>
            <asp:PlaceHolder id="PhPlugins" runat="server">
                <td width="100">栏目插件：</td>
                <td>
                  <asp:CheckBoxList ID="CblPlugins" CssClass="checkboxlist" RepeatDirection="Horizontal" runat="server"></asp:CheckBoxList>
                </td>
            </asp:PlaceHolder>
          </tr>
          <tr>
            <td>栏目模板：</td>
            <td>
              <asp:DropDownList ID="DdlChannelTemplateId" DataTextField="TemplateName" DataValueField="TemplateId" runat="server"></asp:DropDownList>
            </td>
            <td width="100">内容模板：</td>
            <td>
              <asp:DropDownList ID="DdlContentTemplateId" DataTextField="TemplateName" DataValueField="TemplateId" runat="server"></asp:DropDownList>
            </td>
          </tr>
          <tr>
            <td colspan="4">
              <asp:CheckBox Text="将栏目名称作为栏目索引" ID="CbIsNameToIndex" runat="server" />
            </td>
          </tr>
          <tr>
            <td colspan="4">
              <div class="alert alert-info">
                栏目之间用换行分割，下级栏目在栏目前添加“－”字符，索引可以放到括号中，如：
                <br> 栏目一(栏目索引)
                <br> －下级栏目(下级索引)
                <br> －－下下级栏目
              </div>
            </td>
          </tr>
          <tr>
            <td colspan="4" class="center">
              <asp:TextBox Style="width: 98%; height: 240px" TextMode="MultiLine" ID="TbNodeNames" runat="server" />
            </td>
          </tr>
        </table>

      </form>
    </body>

    </html>