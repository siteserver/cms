<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationCrossSiteTrans" %>
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
    <h3 class="popover-title">跨站转发审核设置</h3>
    <div class="popover-content">

      <div class="tips">提示：选择当栏目未设置跨站转发类型时采用的默认设置，同时设置跨站转发到本站点的内容是否需要审核</div>
    
      <table class="table noborder table-hover">
        <tr>
          <td width="260">跨站转发到本站点的内容是否需要审核：</td>
          <td><asp:RadioButtonList ID="IsCrossSiteTransChecked" RepeatDirection="Horizontal" class="noborder" runat="server"> </asp:RadioButtonList></td>
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

  <div class="popover popover-static">
    <h3 class="popover-title">栏目转发设置</h3>
    <div class="popover-content">

      <div class="tips">提示：在此设置各栏目中内容向其他站点跨站转发的选项，如果不指定跨站转发类型将使用站点的默认跨站转发设置</div>
    
      <table class="table table-bordered table-hover">
        <tr class="info thead">
          <td>栏目名</td>
          <td>跨站转发设置</td>
          <td width="50">&nbsp;</td>
        </tr>
        <asp:Repeater ID="rptContents" runat="server">
          <itemtemplate>
            <asp:Literal id="ltlHtml" runat="server" />
          </itemtemplate>
        </asp:Repeater>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
