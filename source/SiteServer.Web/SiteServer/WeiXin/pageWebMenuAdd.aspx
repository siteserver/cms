<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageWebMenuAdd" %>
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
  <bairong:alerts runat="server" />

  <script type="text/javascript">
    var contentSelect = function(title, nodeID, contentID, pageUrl){
      $('#siteTitle').show().html('内容页：' + title + '&nbsp;<a href="' + pageUrl + '" target="blank">查看</a>');
      $('#idsCollection').val(nodeID + '_' + contentID);
    };
    var selectChannel = function(nodeNames, nodeID, pageUrl){
      $('#siteTitle').show().html('栏目页：' + nodeNames + '&nbsp;<a href="' + pageUrl + '" target="blank">查看</a>');
      $('#idsCollection').val(nodeID + '_0');
    };
    var selectKeyword = function(val){
      var keywordType = val.split(',')[0];
      var functionID = val.split(',')[1];
      var functionName = val.split(',')[2];
      $('#functionTitle').show().html('微功能：' + functionName);
      $('#keywordType').val(keywordType);
      $('#functionID').val(functionID);
    };
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title">
      <asp:Literal id="LtlPageTitle" runat="server" />
    </h3>
    <div class="popover-content">
    
      <table class="table noborder">
        <tr>
          <td width="100">菜单名称：</td>
          <td>
            <asp:TextBox id="TbMenuName" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="TbMenuName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbMenuName" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
        </tr>
        <asp:PlaceHolder id="PhNavigationType" runat="server">
        <tr>
          <td>点击菜单触发：</td>
          <td><asp:DropDownList ID="DdlNavigationType" AutoPostBack="true" OnSelectedIndexChanged="ddlNavigationType_OnSelectedIndexChanged" runat="server"></asp:DropDownList></td>
        </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder id="PhFunction" visible="false" runat="server">
        <tr>
          <td>微功能页面：</td>
          <td>
            <div id="functionTitle" class="well well-small" style="display:none"></div>
            <asp:Button id="BtnFunctionSelect" class="btn btn-info" text="选择微功能页面" runat="server" />
            <input id="keywordType" name="keywordType" type="hidden" value="" />
            <input id="functionID" name="functionID" type="hidden" value="" />
          </td>
        </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder id="PhUrl" visible="false" runat="server">
        <tr>
          <td>网址：</td>
          <td>
          <asp:TextBox id="TbUrl" runat="server" style="width:300px;" />
            <asp:RequiredFieldValidator ControlToValidate="TbUrl" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbUrl" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
        </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder id="PhSite" visible="false" runat="server">
        <tr>
          <td>微网站页面：</td>
          <td>
            <div id="siteTitle" class="well well-small" style="display:none"></div>
            <asp:Button id="BtnContentSelect" class="btn btn-info" text="选择内容页" runat="server" />
            <asp:Button id="BtnChannelSelect" class="btn btn-info" text="选择栏目页" runat="server" />
            <input id="idsCollection" name="idsCollection" type="hidden" value="" />
          </td>
        </tr>
        </asp:PlaceHolder>
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

  <asp:Literal id="LtlScript" runat="server" />

</form>
</body>
</html>
