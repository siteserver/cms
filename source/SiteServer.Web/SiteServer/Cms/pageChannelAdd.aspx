<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageChannelAdd" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <meta http-equiv="X-UA-Compatible" content="IE=7" />
      <!--#include file="../inc/header.aspx"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->
      <form class="form-inline" runat="server">
        <asp:Literal ID="LtlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <div class="popover popover-static">
          <h3 class="popover-title">添加栏目</h3>
          <div class="popover-content">

            <table class="table noborder table-hover">
              <tr>
                <td width="150">父栏目：</td>
                <td>
                  <asp:DropDownList ID="DdlParentNodeId" AutoPostBack="true" OnSelectedIndexChanged="DdlParentNodeId_SelectedIndexChanged"
                    runat="server"></asp:DropDownList>
                </td>
              </tr>
              <tr>
                <td>栏目名称：</td>
                <td>
                  <asp:TextBox Columns="45" MaxLength="255" ID="TbNodeName" runat="server" />
                  <asp:RequiredFieldValidator ControlToValidate="TbNodeName" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                  />
                </td>
              </tr>
              <tr>
                <td>栏目索引：</td>
                <td>
                  <asp:TextBox Columns="45" MaxLength="255" ID="TbNodeIndexName" runat="server" />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbNodeIndexName" ValidationExpression="[^']+" ErrorMessage=" *"
                    ForeColor="red" Display="Dynamic" />
                </td>
              </tr>
              <tr>
                <td>内容模型：</td>
                <td>
                  <asp:DropDownList ID="DdlContentModelId" runat="server"></asp:DropDownList>
                </td>
              </tr>
              <asp:PlaceHolder id="PhPlugins" runat="server">
                <tr>
                  <td>栏目插件：</td>
                  <td>
                    <asp:CheckBoxList ID="CblPlugins" CssClass="checkboxlist" RepeatDirection="Horizontal" runat="server"></asp:CheckBoxList>
                  </td>
                </tr>
              </asp:PlaceHolder>
              <tr>
                <td>栏目链接：</td>
                <td>
                  <asp:TextBox Columns="45" MaxLength="200" ID="TbLinkUrl" runat="server" />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbLinkUrl" ValidationExpression="[^']+" ErrorMessage=" *"
                    ForeColor="red" Display="Dynamic" />
                </td>
              </tr>
              <tr>
                <td>链接类型：</td>
                <td>
                  <asp:DropDownList ID="DdlLinkType" runat="server"></asp:DropDownList>
                </td>
              </tr>
              <tr>
                <td>栏目模板：</td>
                <td>
                  <asp:DropDownList ID="DdlChannelTemplateId" DataTextField="TemplateName" DataValueField="TemplateId" runat="server"></asp:DropDownList>
                </td>
              </tr>
              <tr>
                <td>本栏目内容模板：</td>
                <td>
                  <asp:DropDownList ID="DdlContentTemplateId" DataTextField="TemplateName" DataValueField="TemplateId" runat="server"></asp:DropDownList>
                </td>
              </tr>
              <tr>
                <td>生成页面路径：</td>
                <td>
                  <asp:TextBox Columns="45" MaxLength="200" ID="TbFilePath" runat="server" />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbFilePath" ValidationExpression="[^']+" ErrorMessage=" *"
                    ForeColor="red" Display="Dynamic" />
                </td>
              </tr>
              <tr>
                <td>栏目页面命名规则：</td>
                <td>
                  <asp:TextBox Columns="38" MaxLength="200" ID="TbChannelFilePathRule" runat="server" />
                  <asp:Button ID="BtnCreateChannelRule" class="btn" Text="构造" runat="server"></asp:Button>
                </td>
              </tr>
              <tr>
                <td>内容页面命名规则：</td>
                <td>
                  <asp:TextBox Columns="38" MaxLength="200" ID="TbContentFilePathRule" runat="server" />
                  <asp:Button ID="BtnCreateContentRule" class="btn" Text="构造" runat="server"></asp:Button>
                </td>
              </tr>
              <tr>
                <td>可以添加栏目：</td>
                <td>
                  <asp:RadioButtonList ID="RblIsChannelAddable" RepeatDirection="Horizontal" class="noborder" runat="server">
                    <asp:ListItem Text="是" Selected="True" />
                    <asp:ListItem Text="否" />
                  </asp:RadioButtonList>
                </td>
              </tr>
              <tr>
                <td>可以添加内容：</td>
                <td>
                  <asp:RadioButtonList ID="RblIsContentAddable" RepeatDirection="Horizontal" class="noborder" runat="server">
                    <asp:ListItem Text="是" Selected="True" />
                    <asp:ListItem Text="否" />
                  </asp:RadioButtonList>
                </td>
              </tr>
              <tr>
                <td>栏目图片地址：</td>
                <td>
                  <asp:TextBox ID="TbNavigationPicPath" MaxLength="100" Size="45" runat="server" />
                  <asp:Button ID="BtnSelectImage" class="btn" Text="选择" runat="server"></asp:Button>
                  <asp:Button ID="BtnUploadImage" class="btn" Text="上传" runat="server"></asp:Button>
                </td>
              </tr>
              <tr>
                <td colspan="2">
                  <bairong:TextEditorControl ID="TbContent" runat="server"></bairong:TextEditorControl>
                </td>
              </tr>
              <tr>
                <td>关键字列表：</td>
                <td>
                  <asp:TextBox Rows="3" Width="350" MaxLength="100" TextMode="MultiLine" ID="TbKeywords" runat="server" />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbKeywords" ValidationExpression="[^']+" ErrorMessage=" *"
                    ForeColor="red" Display="Dynamic" />
                  <=100 <br>
                    <span>注意：各关键词间用英文逗号“,”隔开。<span>
                        </td>
                    </tr>
                    <tr>
                        <td>页面描述：</td>
                        <td>
                            <asp:TextBox Width="350" Rows="4" MaxLength="200" TextMode="MultiLine" ID="TbDescription" runat="server" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDescription" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                            <=200
                        </td>
                    </tr>
                    <bairong:ChannelAuxiliaryControl ID="CacChannelControl" runat="server" />
                    <tr>
                        <td>栏目组：</td>
                        <td>
                            <asp:CheckBoxList CssClass="checkboxlist" ID="CblNodeGroupNameCollection" DataTextField="NodeGroupName" DataValueField="NodeGroupName" RepeatDirection="Horizontal" RepeatColumns="5" runat="server" />
                        </td>
                    </tr>
                </table>

                <hr />
                <table class="table noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" ID="Submit" Text="添 加" OnClick="Submit_OnClick" runat="server" />
                            <input class="btn" type="button" onclick="location.href='<%=ReturnUrl%>    ';return false;" value="返 回" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>