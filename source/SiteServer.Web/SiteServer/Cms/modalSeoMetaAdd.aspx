<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalSeoMetaAdd" Trace="false"%>
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
      <td width="150"><bairong:help HelpText="此页面元数据的名称" Text="名称：" runat="server" ></bairong:help></td>
      <td colspan="3"><asp:TextBox Columns="25" id="SeoMetaName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="SeoMetaName" errorMessage=" *" foreColor="red" display="Dynamic"
						runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="SeoMetaName"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="页面的标题" Text="页面标题：" runat="server" ></bairong:help></td>
      <td colspan="3"><asp:TextBox Rows="3" Width="350" TextMode="MultiLine" id="PageTitle" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="PageTitle"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="为搜索引擎提供的关键字列表" Text="关键字列表：" runat="server" ></bairong:help></td>
      <td colspan="3"><asp:TextBox Rows="3" Width="350" TextMode="MultiLine" id="Keywords" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Keywords"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /><br>
        <span class="gray">注意：各关键词间用英文逗号“,”隔开。</span></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="用来告诉搜索引擎网页的主要内容。" Text="页面描述：" runat="server" ></bairong:help></td>
      <td colspan="3"><asp:TextBox Width="350" Rows="4" TextMode="MultiLine" id="Description" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Description"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="标注页面的版权所有" Text="页面版权所有：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="25" id="Copyright" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Copyright"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
      <td width="120"><bairong:help HelpText="标注网页的作者或制作组。" Text="页面作者：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="25" id="Author" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Author"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="告诉搜索引擎网页内容什么时候失效。" Text="网页有效期：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="25" id="Expires" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Expires"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
      <td><bairong:help HelpText="告诉来访者怎样联系网站管理人" Text="电子邮件：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="25" id="Email" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Email"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="标注网页所使用的语言" Text="网页语言：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList ID="Language" runat="server">
          <asp:ListItem Text="<未设置>" Value=""></asp:ListItem>
          <asp:ListItem Text="中文" Value="chinese, ZH"></asp:ListItem>
          <asp:ListItem Text="英文" Value="english, EN"></asp:ListItem>
        </asp:DropDownList></td>
      <td><bairong:help HelpText="标注页面所使用的编码" Text="页面编码：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList ID="Charset" runat="server">
          <asp:ListItem Text="<未设置>" Value=""></asp:ListItem>
          <asp:ListItem Text="简体中文(GB2312)" Value="utf-8"></asp:ListItem>
          <asp:ListItem Text="繁体中文(Big5)" Value="Big5"></asp:ListItem>
          <asp:ListItem Text="Unicode(UTF-8)" Value="UTF-8"></asp:ListItem>
        </asp:DropDownList></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="说明网页是面向全球发布，或是只针对地区性用户。" Text="网页发布范围：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList ID="Distribution" runat="server">
          <asp:ListItem Text="<未设置>" Value=""></asp:ListItem>
          <asp:ListItem Text="全球用户" Value="Global"></asp:ListItem>
          <asp:ListItem Text="地区性用户" Value="Local"></asp:ListItem>
        </asp:DropDownList></td>
      <td><bairong:help HelpText="告诉搜索引擎你的网站适合哪些观众。" Text="网站级别：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList ID="Rating" runat="server">
          <asp:ListItem Text="<未设置>" Value=""></asp:ListItem>
          <asp:ListItem Text="都适合" Value="General"></asp:ListItem>
          <asp:ListItem Text="适合儿童" Value="Safe For Kids"></asp:ListItem>
          <asp:ListItem Text="14岁以上" Value="14 Years"></asp:ListItem>
          <asp:ListItem Text="只限成年人" Value="Mature Restricted"></asp:ListItem>
        </asp:DropDownList></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="用来告诉搜索爬虫机器人页面是否需要索引，哪些页面不需要索引。" Text="搜索引擎爬虫向导：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList ID="Robots" runat="server">
          <asp:ListItem Text="<未设置>" Value=""></asp:ListItem>
          <asp:ListItem Text="被检索，链接可查询" Value="INDEX,FOLLOW"></asp:ListItem>
          <asp:ListItem Text="被检索，链接不可查询" Value="INDEX,NOFOLLOW"></asp:ListItem>
          <asp:ListItem Text="不被检索，链接可查询" Value="NOINDEX,FOLLOW"></asp:ListItem>
          <asp:ListItem Text="不被检索，链接不可查询" Value="NOINDEX,NOFOLLOW"></asp:ListItem>
        </asp:DropDownList></td>
      <td><bairong:help HelpText="你根据网站的更新周期，告诉搜索引擎什么时候再次来访" Text="重访时间：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList ID="RevisitAfter" runat="server">
          <asp:ListItem Text="<未设置>" Value=""></asp:ListItem>
          <asp:ListItem Text="一天" Value="1 Day"></asp:ListItem>
          <asp:ListItem Text="一周" Value="7 Days"></asp:ListItem>
          <asp:ListItem Text="一个月" Value="31 Days"></asp:ListItem>
          <asp:ListItem Text="半年" Value="180 Days"></asp:ListItem>
          <asp:ListItem Text="一年" Value="365 Days"></asp:ListItem>
        </asp:DropDownList></td>
    </tr>
  </table>

</form>
</body>
</html>
