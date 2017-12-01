<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageMenu" %>
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
    <h3 class="popover-title"><lan>自定义菜单配置</lan></h3>
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

        <bairong:alerts text="主菜单最多4个汉字，子菜单最多7个汉字，多出来的部分将会以”...”代替，同步自定义菜单后，由于微信客户端缓存，最长需要24小时微信客户端才会展现出来，建议尝试取消关注公众账号后再次关注，则可以看到创建后的效果" runat="server" />
        
        <div class="popover popover-static">
          <h3 class="popover-title">
            自定义菜单设置
          </h3>
          <div class="popover-content">
          
            <table class="table noborder">
              <tr>
                <td class="center">
                  <asp:Button class="btn btn-success" text="同步菜单到微信中" onclick="Sync_OnClick" runat="server" />
                  <asp:Button class="btn btn-danger" text="禁用自定义菜单" onclick="Delete_OnClick" runat="server" />
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
