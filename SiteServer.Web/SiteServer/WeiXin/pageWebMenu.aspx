<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageWebMenu" %>
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

  <link rel="stylesheet" type="text/css" href="css/menu.css">

  <script type="text/javascript">
    $(document).ready(function(){
      $('.mainMenu').click(function(){
        if($(this).next().is(':visible')){
          $(this).next().hide();
        }else{
          $(this).next().show();
        }
      });
    });
    var redirect = function(redirectUrl)
    {
    window.location.href = redirectUrl;
    }
    var contentSelect = function(title, nodeID, contentID, pageUrl){
      document.getElementById("menu").contentWindow.contentSelect(title, nodeID, contentID, pageUrl);
    };
    var selectChannel = function(nodeNames, nodeID, pageUrl){
      document.getElementById("menu").contentWindow.selectChannel(nodeNames, nodeID, pageUrl);
    };
    var selectKeyword = function(keyword){
      document.getElementById("menu").contentWindow.selectKeyword(keyword);
    };
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title"><lan>底部导航菜单管理</lan></h3>
    <div class="popover-content">
    
    <table class="table table-noborder">
    <tr>
    <td width="40%">
      
      <div id="iphone">
        <div id="iphone-view"></div>
        <div id="add-iphone-btn">

          <ul class="allMenus">
            <asp:Literal id="LtlMenu" runat="server"></asp:Literal>
          </ul>

        </div>
     </div>

    </td>
    <td width="60%">

      <div style="padding: 0 10px;margin: 0px;">

        <bairong:alerts text="菜单修改后，需要在生成管理中生成页面后，才能够看到创建后的效果" runat="server" />
        
        <div class="popover popover-static">
          <h3 class="popover-title">
            自定义菜单设置
          </h3>
          <div class="popover-content">
          
            <table class="table noborder">
              <tr>
                <td class="center">
                  <asp:Button id="BtnStatus" onclick="Status_OnClick" runat="server" />
                  <asp:Button class="btn btn-info" text="复制微信菜单到底部导航菜单" onclick="Sync_OnClick" runat="server" />
                  
                </td>
              </tr>
            </table>
        
          </div>
        </div>

      </div>

      <asp:Literal id="LtlIFrame" runat="server" />

    </td>
    </tr>
    </table>

    </div>
  </div>

</form>
</body>
</html>
