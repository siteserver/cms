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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">站点配置管理</h3>
    <div class="popover-content">
      <table class="table noborder table-hover">
        <tr>
            <td width="200">生成页面URL前缀：</td>
            <td>
                <asp:TextBox ID="tbPublishmentSystemUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="tbPublishmentSystemUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="tbPublishmentSystemUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                <br>
                <span>页面所有地址将保留此前缀，可以设置绝对路径（域名）或者相对路径（如：“/”）</span>
            </td>
        </tr>
        <tr>
            <td>网站部署方式：</td>
            <td>
                <asp:DropDownList ID="ddlIsMultiDeployment" AutoPostBack="true" OnSelectedIndexChanged="ddlIsMultiDeployment_SelectedIndexChanged" runat="server"></asp:DropDownList>
                <br />
                <span>如果是多服务器部署，请选择“内外网分离部署”</span>
            </td>
        </tr>
        <asp:PlaceHolder ID="phIsMultiDeployment" runat="server">
            <tr>
                <td>网站外部访问地址：</td>
                <td>
                    <asp:TextBox ID="tbOuterUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="tbOuterUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="tbOuterUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                    <br />
                    <span>外部访问的地址，通常填写网站域名</span>
                </td>
            </tr>
            <tr>
                <td>网站内部访问地址：</td>
                <td>
                    <asp:TextBox ID="tbInnerUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="tbInnerUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="tbInnerUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                    <br />
                    <span>内部访问的地址，后台访问将访问此地址</span>
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>API访问地址：</td>
            <td>
                <asp:TextBox ID="tbAPIUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="tbAPIUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="tbAPIUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
            </td>
        </tr>
        <tr>
            <td>用户中心访问地址：</td>
            <td>
                <asp:TextBox ID="tbHomeUrl" Columns="40" MaxLength="200" Style="ime-mode: disabled;" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="tbHomeUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="tbHomeUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
            </td>
        </tr>
        <tr>
          <td>网页编码：</td>
          <td>
            <asp:DropDownList id="Charset" runat="server"></asp:DropDownList>
            <br>
            <span>模板编码将同步修改</span>
          </td>
        </tr>
        <tr>
          <td>后台信息每页显示数目：</td>
          <td>
            <asp:TextBox Columns="25" Text="18" MaxLength="50" id="PageSize" class="input-mini" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="PageSize"
              errorMessage=" *" foreColor="red"
              Display="Dynamic"
              runat="server"/> <span>条</span>
           </td>
        </tr>
        <tr>
          <td>是否统计内容总点击量：</td>
          <td>
            <asp:RadioButtonList ID="IsCountHits" AutoPostBack="true" OnSelectedIndexChanged="IsCountHits_SelectedIndexChanged" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server"></asp:RadioButtonList>
            <span>需要重新生成页面</span>
          </td>
        </tr>
        <asp:PlaceHolder ID="phIsCountHitsByDay" runat="server">
          <tr>
            <td>是否统计内容日/周/月点击量：</td>
            <td><asp:RadioButtonList ID="IsCountHitsByDay" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server"></asp:RadioButtonList></td>
          </tr>
        </asp:PlaceHolder>
        <tr>
          <td>是否统计文件下载量：</td>
          <td><asp:RadioButtonList ID="IsCountDownload" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server"></asp:RadioButtonList></td>
        </tr>
        <tr>
          <td>是否启用双击生成页面：</td>
          <td>
            <asp:RadioButtonList ID="IsCreateDoubleClick" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server"></asp:RadioButtonList>
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
