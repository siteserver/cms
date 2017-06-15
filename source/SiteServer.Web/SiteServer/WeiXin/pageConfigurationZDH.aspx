<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageConfigurationZDH" %>
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
  <bairong:alerts runat="server"></bairong:alerts>

  <script type="text/javascript" src="js/jquery.zclip.min.js"></script>

  <div class="popover popover-static">
    <h3 class="popover-title">站点信息</h3>
    <div class="popover-content">
    
      <table class="table noborder">
        <tr>
          <td width="160">是否开启直达号：</td>
          <td>
            <asp:RadioButtonList ID="RblIsEnabled" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server"></asp:RadioButtonList>
            <span>需要重新生成页面</span>
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
<!-- check for 3.6 html permissions -->