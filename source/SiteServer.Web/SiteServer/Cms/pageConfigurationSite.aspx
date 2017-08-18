<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationSite" %>
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
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">站点配置管理</h3>
    <div class="popover-content">
      <table class="table noborder table-hover">
        <tr>
            <td width="200">生成页面URL前缀：</td>
            <td>
                <asp:TextBox ID="TbPublishmentSystemUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="TbPublishmentSystemUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPublishmentSystemUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                <br>
                <span>页面所有地址将保留此前缀，可以设置绝对路径（域名）或者相对路径（如：“/”）</span>
            </td>
        </tr>
        <tr>
            <td>网站部署方式：</td>
            <td>
                <asp:DropDownList ID="DdlIsMultiDeployment" AutoPostBack="true" OnSelectedIndexChanged="DdlIsMultiDeployment_SelectedIndexChanged" runat="server"></asp:DropDownList>
                <br />
                <span>如果是多服务器部署，请选择“内外网分离部署”</span>
            </td>
        </tr>
        <asp:PlaceHolder ID="PhSingle" runat="server">
            <tr>
                <td>站点访问地址：</td>
                <td>
                    <asp:TextBox ID="TbSiteUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TbSiteUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSiteUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td>API访问地址：</td>
                <td>
                    <asp:TextBox ID="TbApiUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TbApiUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbApiUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="PhMulti" runat="server">
            <tr>
                <td>外部站点访问地址：</td>
                <td>
                    <asp:TextBox ID="TbOuterSiteUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TbOuterSiteUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbOuterSiteUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td>内部站点访问地址：</td>
                <td>
                    <asp:TextBox ID="TbInnerSiteUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TbInnerSiteUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbInnerSiteUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td>外部API访问地址：</td>
                <td>
                    <asp:TextBox ID="TbOuterApiUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TbOuterApiUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbOuterApiUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td>内部API访问地址：</td>
                <td>
                    <asp:TextBox ID="TbInnerApiUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="TbInnerApiUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="TbInnerApiUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                </td>
            </tr>
        </asp:PlaceHolder>
        
        <tr>
            <td>用户中心访问地址：</td>
            <td>
                <asp:TextBox ID="TbHomeUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="TbHomeUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbHomeUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
            </td>
        </tr>
        <tr>
          <td>网页编码：</td>
          <td>
            <asp:DropDownList id="DdlCharset" runat="server"></asp:DropDownList>
            <br>
            <span>模板编码将同步修改</span>
          </td>
        </tr>
        <tr>
          <td>后台信息每页显示数目：</td>
          <td>
            <asp:TextBox Columns="25" Text="18" MaxLength="50" id="TbPageSize" class="input-mini" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="TbPageSize"
              errorMessage=" *" foreColor="red"
              Display="Dynamic"
              runat="server"/> <span>条</span>
           </td>
        </tr>
        <tr>
          <td>是否启用双击生成页面：</td>
          <td>
            <asp:RadioButtonList ID="RblIsCreateDoubleClick" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server"></asp:RadioButtonList>
            <span>此功能通常用于制作调试期间，网站正式上线后不建议启用</span>
          </td>
        </tr>
      </table>
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
          </td>
        </tr>
      </table>
    </div>
  </div>

</form>
</body>
</html>
