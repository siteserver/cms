<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalGatherTest" Trace="false"%>
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
    <tr>
      <td><bairong:help HelpText="选择采集地址开始测试" Text="采集地址：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList ID="GatherUrls" runat="server"></asp:DropDownList>
        &nbsp;
        <asp:Button class="btn" id="GetContentUrls"  style="margin-bottom:0px;" text="获取链接" OnClick="GatherUrls_Click" runat="server" /></td>
    </tr>
  </table>
  
  <table class="table table-noborder table-hover">
    <asp:Repeater ID="ContentUrlRepeater" runat="server">
      <itemtemplate>
        <tr>
          <td><bairong:NoTagText id="ContentItem" runat="server" />
          </td><td>
          <asp:Button class="btn" id="GetContent" text="获取内容" OnClick="GetContent_Click" runat="server" style="margin-bottom:0px;" /></td>
        </tr>
    </itemtemplate>
    </asp:Repeater>
  <table>

  <table class="table table-noborder table-hover">
    <tr>
      <td>
        <bairong:NoTagText id="Content" runat="server" />
      </td>
    </tr>
  </table>

</form>
</body>
</html>
