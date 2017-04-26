<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageSeoSiteMapBaidu" %>
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
    <h3 class="popover-title">生成百度新闻地图</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="160">百度站点地图文件：</td>
          <td>
            <asp:TextBox Columns="35" MaxLength="200" id="SiteMapBaiduPath" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="SiteMapBaiduPath" ErrorMessage="此项不能为空" foreColor="red" Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="SiteMapBaiduPath" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
        </tr>
        <tr>
          <td>负责人员的Email：</td>
          <td>
            <asp:TextBox Columns="35" MaxLength="200" id="SiteMapBaiduWebMaster" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="SiteMapBaiduWebMaster" ErrorMessage="此项不能为空" foreColor="red" Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="SiteMapBaiduWebMaster" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
        </tr>
        <tr>
          <td>更新周期：</td>
          <td>
            <asp:TextBox class="input-mini" MaxLength="200" id="SiteMapBaiduUpdatePeri" runat="server" />
            分钟
            <asp:RequiredFieldValidator ControlToValidate="SiteMapBaiduUpdatePeri" ErrorMessage="此项不能为空" foreColor="red" Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="SiteMapBaiduUpdatePeri" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
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
    <h3 class="popover-title">提交百度新闻地图</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="200"><bairong:help HelpText="百度站点地图的提交地址" Text="百度站点地图提交地址：" runat="server" ></bairong:help></td>
          <td><a href="http://news.baidu.com/newsop.html" target="_blank">http://news.baidu.com/newsop.html</a></td>
        </tr>
        <tr>
          <td><bairong:help HelpText="需要向Yahoo提交的站点地图文件地址" Text="站点地图文件地址：" runat="server" ></bairong:help></td>
          <td><asp:Literal id="ltlBaiduSiteMapUrl" runat="server" /></td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
