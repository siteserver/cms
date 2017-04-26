<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalGatherFileSet" Trace="false"%>
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
      <td><bairong:help HelpText="需要采集的网页地址" Text="采集网页地址：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="60" id="GatherUrl" runat="server"/>
        <asp:RequiredFieldValidator
							ControlToValidate="GatherUrl"
							errorMessage=" *" foreColor="red" 
							Display="Dynamic"
							runat="server"/></td>
    </tr>
    <asp:PlaceHolder ID="PlaceHolder_File" runat="server">
      <tr>
        <td><bairong:help HelpText="采集到的文件地址" Text="采集到文件地址：" runat="server" ></bairong:help></td>
        <td><asp:TextBox Columns="50" id="FilePath" runat="server"/>
          <br>
          <span class="gray">（以“~/”开头代表系统根目录，以“@/”开头代表站点根目录）</span></td>
      </tr>
      <tr>
        <td><bairong:help HelpText="是否删除文件中JS脚本" Text="删除JS脚本：" runat="server" ></bairong:help></td>
        <td><asp:RadioButtonList id="IsRemoveScripts" RepeatDirection="Horizontal" class="noborder" runat="server">
            <asp:ListItem Value="True">是</asp:ListItem>
            <asp:ListItem Value="False" Selected="true">否</asp:ListItem>
          </asp:RadioButtonList></td>
      </tr>
      <tr>
        <td><bairong:help HelpText="下载采集网址的相关文件（图片、CSS等）" Text="下载相关文件：" runat="server" ></bairong:help></td>
        <td><asp:RadioButtonList id="IsSaveRelatedFiles" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_SelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server">
            <asp:ListItem Value="True">是</asp:ListItem>
            <asp:ListItem Value="False" Selected="true">否</asp:ListItem>
          </asp:RadioButtonList></td>
      </tr>
      <asp:PlaceHolder ID="PlaceHolder_File_Directory" runat="server">
        <tr>
          <td><bairong:help HelpText="Css样式文件保存的文件夹地址" Text="Css样式保存地址：" runat="server" ></bairong:help></td>
          <td><asp:TextBox Text="@/css" Columns="25" MaxLength="200" id="StyleDirectoryPath" runat="server"/>
            <br /><span class="gray">（以“~/”开头代表系统根目录，以“@/”开头代表站点根目录）</span></td>
        </tr>
        <tr>
          <td><bairong:help HelpText="Js脚本文件保存的文件夹地址" Text="Js脚本保存地址：" runat="server" ></bairong:help></td>
          <td><asp:TextBox Text="@/js" Columns="25" MaxLength="200" id="ScriptDirectoryPath" runat="server"/>
            <br /><span class="gray">（以“~/”开头代表系统根目录，以“@/”开头代表站点根目录）</span></td>
        </tr>
        <tr>
          <td><bairong:help HelpText="图片文件保存的文件夹地址" Text="图片保存地址：" runat="server" ></bairong:help></td>
          <td><asp:TextBox Text="@/images" Columns="25" MaxLength="200" id="ImageDirectoryPath" runat="server"/>
            <br /><span class="gray">（以“~/”开头代表系统根目录，以“@/”开头代表站点根目录）</span></td>
        </tr>
      </asp:PlaceHolder>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="PlaceHolder_Content" runat="server">
      <tr>
        <td><bairong:help HelpText="选择栏目，采集到的内容将添加到此栏目中" Text="采集到栏目：" runat="server" ></bairong:help></td>
        <td><asp:DropDownList ID="NodeIDDropDownList" runat="server"></asp:DropDownList></td>
      </tr>
      <tr>
        <td><bairong:help HelpText="下载所采集内容的图片到服务器中" Text="下载内容图片：" runat="server" ></bairong:help></td>
        <td><asp:RadioButtonList id="IsSaveImage" RepeatDirection="Horizontal" class="noborder"
								runat="server">
            <asp:ListItem Value="True">是</asp:ListItem>
            <asp:ListItem Value="False" Selected="true">否</asp:ListItem>
          </asp:RadioButtonList></td>
      </tr>
    </asp:PlaceHolder>
  </table>

</form>
</body>
</html>
