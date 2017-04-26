<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageTagStyleTemplate" %>
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
    <h3 class="popover-title">自定义模板</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="155"> 样式名称： </td>
          <td><asp:Literal ID="ltlStyleName" runat="server"></asp:Literal></td>
        </tr>
        <tr>
          <td width="155"> 调用标签： </td>
          <td><asp:Literal ID="ltlElement" runat="server"></asp:Literal></td>
        </tr>
        <tr>
          <td width="155"> 模板显示方式： </td>
          <td><asp:RadioButtonList ID="rblIsTemplate" AutoPostBack="true" OnSelectedIndexChanged="rblIsTemplate_SelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
        </tr>
        <asp:PlaceHolder ID="phTemplate" runat="server">
          <tr>
            <td width="155">生成默认模板：</td>
            <td><asp:CheckBox ID="cbIsCreateTemplate" AutoPostBack="true" OnCheckedChanged="cbIsCreateTemplate_CheckedChanged" runat="server" Text="生成默认模板"></asp:CheckBox>
              (提示：可以点此生成默认模板！) </td>
          </tr>
          <tr>
            <td width="155">模板内容：</td>
            <td><asp:TextBox Width="100%" Height="330" TextMode="MultiLine" ID="tbContent" runat="server" Wrap="false" />
              <asp:RequiredFieldValidator ControlToValidate="tbContent" ErrorMessage="模板内容必须填写，可以点击“生成默认模板”生成" foreColor="red" Display="Dynamic" runat="server" /></td>
          </tr>
          <asp:PlaceHolder ID="phSuccess" runat="server">
          <tr>
            <td width="155">提交成功模版内容：</td>
            <td><asp:TextBox Width="100%" Height="260" TextMode="MultiLine" ID="tbSuccess" runat="server" Wrap="false" />
              <asp:RequiredFieldValidator ControlToValidate="tbSuccess" ErrorMessage="模板内容必须填写，可以点击“生成默认模板”生成" foreColor="red" Display="Dynamic" runat="server" /></td>
          </tr>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="phFailure" runat="server">
          <tr>
            <td width="155">提交失败模版内容：</td>
            <td><asp:TextBox Width="100%" Height="260" TextMode="MultiLine" ID="tbFailure" runat="server" Wrap="false" />
              <asp:RequiredFieldValidator ControlToValidate="tbFailure" ErrorMessage="模板内容必须填写，可以点击“生成默认模板”生成" foreColor="red" Display="Dynamic" runat="server" /></td>
          </tr>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="phStyle" runat="server">
            <tr>
              <td width="155"><bairong:help HelpText="CSS样式" Text="CSS样式：" runat="server" ></bairong:help></td>
              <td><asp:TextBox Width="100%" TextMode="MultiLine" ID="tbStyle" runat="server" Height="165" Wrap="false" /></td>
            </tr>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="phScript" runat="server">
            <tr>
              <td width="155"><bairong:help HelpText="JS脚本" Text="JS脚本：" runat="server" ></bairong:help></td>
              <td><asp:TextBox Width="100%" TextMode="MultiLine" ID="tbScript" runat="server" Height="165" Wrap="false" /></td>
            </tr>
          </asp:PlaceHolder>
        </asp:PlaceHolder>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
            <asp:Button class="btn btn-info" id="Preview" text="预 览" runat="server" />
            <asp:PlaceHolder ID="phReturn" runat="server">
              <input type=button class="btn" onClick="location.href='<%=Request.QueryString["ReturnUrl"]%>';" value="返 回" />
            </asp:PlaceHolder>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
