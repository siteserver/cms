<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTemplateFilePathRule" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <asp:PlaceHolder id="phFilePath" runat="server">
    <tr>
      <td width="140">生成页面路径：</td>
      <td><asp:TextBox class="input-xlarge" id="tbFilePath" runat="server"/>
        <asp:RegularExpressionValidator
					runat="server"
					ControlToValidate="tbFilePath"
					ValidationExpression="[^']+"
					errorMessage=" *" foreColor="red" 
					Display="Dynamic" /></td>
    </tr>
    </asp:PlaceHolder>
    <tr>
      <td width="140">下级栏目页面命名规则：</td>
      <td>
        <asp:TextBox class="input-xxlarge" id="tbChannelFilePathRule" runat="server"/>
        <asp:Button ID="btnCreateChannelRule" class="btn" runat="server" text="构造" />
        <br><span class="gray">系统生成文件时采取的下级栏目页文件名规则</span>
      </td>
    </tr>
    <tr>
      <td>下级内容页面命名规则：</td>
      <td>
        <asp:TextBox class="input-xxlarge" id="tbContentFilePathRule" runat="server"/>
        <asp:Button ID="btnCreateContentRule" class="btn" runat="server" text="构造" />
        <br><span class="gray">系统生成文件时采取的下级内容页文件名规则</span>
      </td>
    </tr>
  </table>

</form>
</body>
</html>
