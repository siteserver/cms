<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageSeoSiteMapGoogle" %>
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
    <h3 class="popover-title">生成站点地图</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="160"><bairong:help HelpText="站点地图文件地址" Text="站点地图文件：" runat="server" ></bairong:help></td>
          <td>
            <asp:TextBox Columns="35" MaxLength="200" id="SiteMapGooglePath" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="SiteMapGooglePath" ErrorMessage="此项不能为空" foreColor="red" Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="SiteMapGooglePath" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
        </tr>
        <tr>
          <td width="160"><bairong:help HelpText="设置页面的更新频率" Text="页面更新频率：" runat="server" ></bairong:help></td>
          <td><asp:DropDownList ID="SiteMapGoogleChangeFrequency" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
          <td width="160"><bairong:help HelpText="设置是否显示最后更新日期" Text="是否显示最后更新日期：" runat="server" ></bairong:help></td>
          <td><asp:RadioButtonList ID="SiteMapGoogleIsShowLastModified" runat="server" RepeatDirection="Horizontal" class="noborder"></asp:RadioButtonList></td>
        </tr>
        <tr>
          <td width="160"><bairong:help HelpText="站点地图分页数" Text="站点地图分页数" runat="server" ></bairong:help></td>
          <td><asp:TextBox class="input-mini" Columns="8" id="SiteMapGooglePageCount" Style="text-align:right" runat="server"/>
            篇
            <asp:RequiredFieldValidator
                    ControlToValidate="SiteMapGooglePageCount"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic"
                    runat="server"/>
            <asp:RegularExpressionValidator
                    ControlToValidate="SiteMapGooglePageCount"
                    ValidationExpression="\d+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic"
                    runat="server"/></td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" ID="Submit" Text="生成站点地图" OnClick="Submit_OnClick" runat="server" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

  <div class="popover popover-static">
    <h3 class="popover-title">Google 提交地址</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="200" class="center"><bairong:help HelpText="Google站点地图的提交地址" Text="Google站点地图提交地址：" runat="server" ></bairong:help></td>
          <td><A href="http://www.google.com/webmasters/sitemaps" target="_blank">http://www.google.com/webmasters/sitemaps</A></td>
        </tr>
        <tr>
          <td width="160" class="center"><bairong:help HelpText="需要向Google提交的站点地图文件地址" Text="站点地图文件地址：" runat="server" ></bairong:help></td>
          <td><asp:Literal id="ltlGoogleSiteMapUrl" runat="server" /></td>
        </tr>
      </table>
  
    </div>
  </div>

  <div class="popover popover-static">
    <h3 class="popover-title">Yahoo 提交地址</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="200" class="center"><bairong:help HelpText="Yahoo站点地图的提交地址" Text="Yahoo站点地图提交地址：" runat="server" ></bairong:help></td>
          <td><A href="https://siteexplorer.search.yahoo.com/" target="_blank">https://siteexplorer.search.yahoo.com</A></td>
        </tr>
        <tr>
          <td width="160" class="center"><bairong:help HelpText="需要向Yahoo提交的站点地图文件地址" Text="站点地图文件地址：" runat="server" ></bairong:help></td>
          <td><asp:Literal id="ltlYahooSiteMapUrl" runat="server" /></td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
