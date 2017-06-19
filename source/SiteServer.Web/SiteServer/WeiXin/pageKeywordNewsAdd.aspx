<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageKeywordNewsAdd" %>
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
  <bairong:alerts runat="server" />

  <link rel="stylesheet" type="text/css" href="css/keywordListAdd.css">
  <script type="text/javascript">
    $(document).ready(function(){
      $('#js_add_appmsg').click(function(){
        var html = $('#empty_item').html();
        $('#js_appmsg_preview').append(html);
        document.getElementById("resource").src = $('#empty_item .edit_gray').attr('href');
      });
    });
    var redirect = function(redirectUrl)
    {
    window.location.href = redirectUrl;
    }
    var deleteItem = function(e){
      $(e).parent().parent().remove();
    }
    var contentSelect = function(title, nodeID, contentID, pageUrl, imageUrl, imageSrc, summary){
      document.getElementById("resource").contentWindow.contentSelect(title, nodeID, contentID, pageUrl, imageUrl, imageSrc, summary);
    };
    var selectChannel = function(nodeNames, nodeID, pageUrl){
      document.getElementById("resource").contentWindow.selectChannel(nodeNames, nodeID, pageUrl);
    };
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title">
      <asp:Literal id="LtlPageTitle" runat="server" />
    </h3>
    <div class="popover-content">
    
      <!-- start -->
<div class="main_bd">
  <div class="media_preview_area">

    <asp:PlaceHolder id="PhSingle" runat="server">
    <div class="appmsg  editing">
      <div class="appmsg_content">
        <div class="js_appmsg_item ">
          <h4 class="appmsg_title">
            <asp:Literal id="LtlSingleTitle" runat="server" />
          </h4>
          <div class="appmsg_info">
            <em class="appmsg_date">
            </em>
          </div>
          <div class="appmsg_thumb_wrp">
            <asp:Literal id="LtlSingleImageUrl" runat="server" />
          </div>
          <p class="appmsg_desc">
            <asp:Literal id="LtlSingleSummary" runat="server" />
          </p>
        </div>
      </div>
    </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder id="PhMultiple" runat="server">
    <div class="appmsg multi editing">
      <div id="js_appmsg_preview" class="appmsg_content">
        <div class="js_appmsg_item ">
          <div class="appmsg_info">
            <em class="appmsg_date">
            </em>
          </div>
          <div class="cover_appmsg_item">
            <h4 class="appmsg_title">
              <asp:Literal id="LtlMultipleTitle" runat="server" />
            </h4>
            <div class="appmsg_thumb_wrp">
              <asp:Literal id="LtlMultipleImageUrl" runat="server" />
            </div>
            <div class="appmsg_edit_mask">
              <asp:Literal id="LtlMultipleEditUrl" runat="server" />
            </div>
          </div>
        </div>

        <asp:Repeater ID="RptMultipleContents" runat="server">
          <itemtemplate>
            <div class="appmsg_item js_appmsg_item ">
              <asp:Literal id="LtlImageUrl" runat="server" />
              <h4 class="appmsg_title">
                <asp:Literal id="LtlTitle" runat="server" />
              </h4>
              <div class="appmsg_edit_mask">
                <asp:Literal id="LtlEditUrl" runat="server" />
                <asp:Literal id="LtlDeleteUrl" runat="server" />
              </div>
            </div>
          </itemtemplate>
        </asp:Repeater>

        <!-- start -->
        <div id="empty_item" style="display:none">
          <div class="appmsg_item js_appmsg_item">
            <img class="js_appmsg_thumb appmsg_thumb" src="">
            <i class="appmsg_thumb default">
              缩略图
            </i>
            <h4 class="appmsg_title">
              <a onclick="return false;" href="javascript:void(0);" target="_blank">
                标题
              </a>
            </h4>
            <div class="appmsg_edit_mask">
              <asp:Literal id="LtlItemEditUrl" runat="server" />
              <a class="icon18_common del_gray js_del" href="javascript:;" onclick="deleteItem(this)">&nbsp;&nbsp;</a>
            </div>
          </div>
        </div>
        <!-- end -->
      </div>
      <div class="appmsg_add">
        <a onclick="return false;" id="js_add_appmsg" href="javascript:void(0);">
          &nbsp;
          <i class="icon24_common add_gray" style="margin-top:25px;">
            增加一条
          </i>
        </a>
      </div>
    </div>
    </asp:PlaceHolder>

    <div class="tool_area">
      <div class="tool_bar">
        <input type="button" value="返 回" onclick="javascript:location.href='pageKeywordNews.aspx?PublishmentSystemId=<%=base.PublishmentSystemId%>'" class="btn">
        <!-- <span id="js_preview" class="btn btn_input btn_primary">
          <asp:Literal id="LtlPreview" runat="server" />
        </span> -->
      </div>
    </div>

  </div>
  <div id="js_appmsg_editor" class="media_edit_area">

    <asp:Literal id="LtlIFrame" runat="server" />

  </div>

</div>
      <!-- end -->
  
    </div>
  </div>

</form>
</body>
</html>
