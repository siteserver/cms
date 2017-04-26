<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTrackerEdit" %>
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
    <h3 class="popover-title">修改统计设置</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="200">是否统计页面访问量：</td>
          <td><asp:RadioButtonList ID="IsTracker" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
            <span>需要重新生成页面</span>
          </td>
        </tr>
        <tr>
          <td>原有统计天数：</td>
          <td>
            <asp:TextBox class="input-mini" Columns="25" MaxLength="50" id="TrackerDays" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="TrackerDays"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
            <asp:RegularExpressionValidator
              runat="server"
              ControlToValidate="TrackerDays"
              ValidationExpression="\d+"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic" />
            天 </td>
        </tr>
        <tr>
          <td>原有访问量(PageView)：</td>
          <td><asp:TextBox class="input-mini" Columns="25" MaxLength="50" id="TrackerPageView" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="TrackerPageView"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
            <asp:RegularExpressionValidator
              runat="server"
              ControlToValidate="TrackerPageView"
              ValidationExpression="\d+"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic" />
            <br>
            <span>设置原有访问量，系统将把原有访问量和现有访问量相加作为总的访问量</span>
          </td>
        </tr>
        <tr>
          <td>原有访客数(UniqueVisitor)：</td>
          <td>
            <asp:TextBox class="input-mini" Columns="25" MaxLength="50" id="TrackerUniqueVisitor" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="TrackerUniqueVisitor"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
            <asp:RegularExpressionValidator
              runat="server"
              ControlToValidate="TrackerUniqueVisitor"
              ValidationExpression="\d+"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic" />
            <br>
            <span>设置原有访客数，系统将把原有访客数和现有访客数相加作为总的访客数</span>
          </td>
        </tr>
        <tr>
          <td>浏览者离线时间：</td>
          <td>
            <asp:TextBox class="input-mini" Columns="25" MaxLength="50" id="TrackerCurrentMinute" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="TrackerCurrentMinute"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
            <asp:RegularExpressionValidator
              runat="server"
              ControlToValidate="TrackerCurrentMinute"
              ValidationExpression="\d+"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic" />
            分钟 
            <br>
            <span>设置浏览者最后一次浏览页面后多久视为离线</span>
          </td>
        </tr>
        <tr>
          <td>统计显示样式：</td>
          <td>
            <asp:DropDownList ID="TrackerStyle" runat="server"></asp:DropDownList>
            <br>
            <span>设置前台页面显示统计数据的样式</span>
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
