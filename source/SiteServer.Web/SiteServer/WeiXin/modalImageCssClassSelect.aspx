<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalImageCssClassSelect" Trace="false"%>
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
<asp:Button id="BtnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>
  <bairong:Code type="ajaxupload" runat="server" />
  
 <link rel="stylesheet" href="../../SiteFiles/Services/WeiXin/components/lib/fontawesome/css/imageCssClass.css"/> 
 <link rel="stylesheet" href="../../SiteFiles/Services/WeiXin/components/lib/fontawesome/css/font-awesome.css"/>
 
   <script type="text/javascript">
       $(document).ready(function () {
           <asp:Literal id="LtlScript" runat="server" />
       })
   </script>
  <div id="new">
  
  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;" ><i class="fa fa-automobile"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bank"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-behance"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-behance-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bomb"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-building"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cab"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-car"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-child"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-circle-o-notch"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-circle-thin"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-codepen"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cube"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cubes"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-database"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-delicious"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-deviantart"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-digg"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-drupal"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-empire"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-envelope-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-fax"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-archive-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-audio-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-code-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-excel-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-image-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-movie-o"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-pdf-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-photo-o"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-picture-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-powerpoint-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-sound-o"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-video-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-word-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-zip-o"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-ge"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-git"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-git-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-google"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-graduation-cap"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-hacker-news"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-header"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-history"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-institution"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-joomla"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-jsfiddle"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-language"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-life-bouy"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-life-ring"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-life-saver"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-mortar-board"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-openid"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-paper-plane"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-paper-plane-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-paragraph"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-paw"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pied-piper"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pied-piper-alt"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pied-piper-square"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-qq"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-ra"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-rebel"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-recycle"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-reddit"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-reddit-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-send"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-send-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-share-alt"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-share-alt-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-slack"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sliders"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-soundcloud"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-space-shuttle"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-spoon"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-spotify"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-steam"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-steam-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-stumbleupon"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-stumbleupon-circle"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-support"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-taxi"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tencent-weibo"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tree"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-university"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-vine"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-wechat"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-weixin"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-wordpress"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-yahoo"></i></a></div>
    
  </div>

</div>

  <section id="web-application">
  <h2 class="page-header"></h2>

  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-adjust"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-anchor"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-archive"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrows"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrows-h"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrows-v"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-asterisk"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-automobile"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-ban"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bank"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bar-chart-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-barcode"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bars"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-beer"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bell"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bell-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bolt"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bomb"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-book"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bookmark"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bookmark-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-briefcase"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bug"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-building"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-building-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bullhorn"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bullseye"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cab"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-calendar"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-calendar-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-camera"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-camera-retro"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-car"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-square-o-down"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-square-o-left"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-square-o-right"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-square-o-up"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-certificate"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-check"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-check-circle"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-check-circle-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-check-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-check-square-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-child"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-circle"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-circle-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-circle-o-notch"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-circle-thin"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-clock-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cloud"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cloud-download"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cloud-upload"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-code"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-code-fork"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-coffee"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cog"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cogs"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-comment"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-comment-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-comments"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-comments-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-compass"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-credit-card"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-crop"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-crosshairs"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cube"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cubes"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cutlery"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-dashboard"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-database"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-desktop"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-dot-circle-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-download"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-edit"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-ellipsis-h"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-ellipsis-v"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-envelope"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-envelope-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-envelope-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-eraser"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-exchange"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-exclamation"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-exclamation-circle"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-exclamation-triangle"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-external-link"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-external-link-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-eye"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-eye-slash"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-fax"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-female"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-fighter-jet"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-archive-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-audio-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-code-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-excel-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-image-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-movie-o"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-pdf-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-photo-o"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-picture-o"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-powerpoint-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-sound-o"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-video-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-word-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-zip-o"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-film"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-filter"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-fire"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-fire-extinguisher"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-flag"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-flag-checkered"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-flag-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-flash"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-flask"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-folder"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-folder-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-folder-open"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-folder-open-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-frown-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-gamepad"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-gavel"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-gear"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-gears"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-gift"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-glass"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-globe"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-graduation-cap"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-group"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-hdd-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-headphones"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-heart"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-heart-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-history"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-home"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-image"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-inbox"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-info"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-info-circle"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-institution"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-key"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-keyboard-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-language"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-laptop"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-leaf"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-legal"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-lemon-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-level-down"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-level-up"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-life-bouy"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-life-ring"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-life-saver"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-lightbulb-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-location-arrow"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-lock"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-magic"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-magnet"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-mail-forward"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-mail-reply"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-mail-reply-all"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-male"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-map-marker"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-meh-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-microphone"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-microphone-slash"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-minus"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-minus-circle"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-minus-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-minus-square-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-mobile"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-mobile-phone"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-money"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-moon-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-mortar-board"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-music"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-navicon"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"<i class="fa fa-paper-plane"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-paper-plane-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-paw"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pencil"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pencil-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pencil-square-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-phone"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-phone-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-photo"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-picture-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-plane"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-plus"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-plus-circle"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-plus-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-plus-square-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-power-off"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-print"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-puzzle-piece"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-qrcode"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-question"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-question-circle"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-quote-left"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-quote-right"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-random"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-recycle"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-refresh"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-reorder"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-reply"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-reply-all"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-retweet"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-road"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-rocket"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-rss"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-rss-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-search"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-search-minus"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-search-plus"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-send"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-send-o"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-share"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-share-alt"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-share-alt-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-share-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-share-square-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-shield"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-shopping-cart"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sign-in"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sign-out"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-signal"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sitemap"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sliders"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-smile-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort-alpha-asc"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort-alpha-desc"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort-amount-asc"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort-amount-desc"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort-asc"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort-desc"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort-down"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort-numeric-asc"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort-numeric-desc"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sort-up"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-space-shuttle"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-spinner"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-spoon"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-square-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-star"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-star-half"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-star-half-empty"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-star-half-full"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-star-half-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-star-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-suitcase"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-sun-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-support"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tablet"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tachometer"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tag"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tags"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tasks"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-taxi"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-terminal"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-thumb-tack"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-thumbs-down"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-thumbs-o-down"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-thumbs-o-up"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-thumbs-up"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-ticket"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-times"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-times-circle"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-times-circle-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tint"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-toggle-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-toggle-left"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-toggle-right"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-toggle-up"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-trash-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tree"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-trophy"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-truck"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-umbrella"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-university"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-unlock"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-unlock-alt"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-unsorted"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;" ><i class="fa fa-upload"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-user"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-users"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-video-camera"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-volume-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-volume-off"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-volume-up"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-warning"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-wheelchair"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-wrench"></i></a></div>
    
  </div>

</section>

  <section id="file-type">
  <h2 class="page-header"></h2>

  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-archive-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-audio-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-code-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-excel-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-image-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-movie-o"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-pdf-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-photo-o"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-picture-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-powerpoint-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-sound-o"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-text"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-text-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-video-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-word-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-zip-o"></i>   </a></div>
    
  </div>

</section>

  <section id="Spinner">
  <h2 class="page-header"></h2>
 
  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-circle-o-notch"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cog"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-gear"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-refresh"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-spinner"></i> </a></div>
    
  </div>
</section>

  <section id="form-control">
  <h2 class="page-header"></h2>

  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-check-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-check-square-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-circle"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-circle-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-dot-circle-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-minus-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-minus-square-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-plus-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-plus-square-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-square-o"></i> </a></div>
    
  </div>
</section>

  <section id="currency">
  <h2 class="page-header"></h2>

  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bitcoin"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-btc"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cny"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-dollar"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-eur"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-euro"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-gbp"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-inr"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-jpy"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-krw"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-money"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-rmb"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-rouble"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-rub"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-ruble"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-rupee"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-try"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-turkish-lira"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-usd"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-won"></i>   </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-yen"></i> </a></div>
    
  </div>

</section>

  <section id="text-editor">
  <h2 class="page-header"></h2>

  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-align-center"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-align-justify"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-align-left"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-align-right"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bold"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-chain"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-chain-broken"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-clipboard"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-columns"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-copy"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-cut"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-dedent"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-eraser"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-o"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-text"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-file-text-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"<i class="fa fa-files-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-floppy-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-font"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-header"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-indent"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-italic"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-link"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-list"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-list-alt"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-list-ol"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-list-ul"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-outdent"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-paperclip"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-paragraph"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-paste"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-repeat"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-rotate-left"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-rotate-right"></i>  </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-save"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-scissors"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-strikethrough"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-subscript"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-superscript"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-table"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-text-height"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-text-width"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-th"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-th-large"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-th-list"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-underline"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-undo"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-unlink"></i></a></div>
    
  </div>

</section>

  <section id="directional">
  <h2 class="page-header"></h2>

  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-angle-double-down"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-angle-double-left"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-angle-double-right"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-angle-double-up"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-angle-down"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-angle-left"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-angle-right"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-angle-up"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-circle-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-circle-left"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-circle-o-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-circle-o-left"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-circle-o-right"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-circle-o-up"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-circle-right"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-circle-up"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-left"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-right"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrow-up"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrows"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrows-alt"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrows-h"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrows-v"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-left"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-right"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-square-o-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-square-o-left"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-square-o-right"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-square-o-up"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-caret-up"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-chevron-circle-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-chevron-circle-left"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-chevron-circle-right"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-chevron-circle-up"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-chevron-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-chevron-left"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-chevron-right"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-chevron-up"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-hand-o-down"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-hand-o-left"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-hand-o-right"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-hand-o-up"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-long-arrow-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-long-arrow-left"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-long-arrow-right"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-long-arrow-up"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-toggle-down"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-toggle-left"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-toggle-right"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-toggle-up"></i> </a></div>
    
  </div>

</section>

  <section id="video-player">
  <h2 class="page-header"></h2>

  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-arrows-alt"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-backward"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-compress"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-eject"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-expand"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-fast-backward"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-fast-forward"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-forward"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pause"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-play"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-play-circle"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-play-circle-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-step-backward"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-step-forward"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-stop"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-youtube-play"></i></a></div>
    
  </div>

</section>

  <section id="brand">
  <h2 class="page-header"></h2>
  
  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-adn"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-android"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-apple"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-behance"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-behance-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bitbucket"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bitbucket-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-bitcoin"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-btc"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-codepen"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-css3"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-delicious"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-deviantart"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-digg"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-dribbble"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-dropbox"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-drupal"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-empire"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-facebook"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-facebook-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-flickr"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-foursquare"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-ge"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-git"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-git-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-github"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-github-alt"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-github-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-gittip"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-google"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-google-plus"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-google-plus-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-hacker-news"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-html5"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-instagram"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-joomla"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-jsfiddle"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-linkedin"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-linkedin-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-linux"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-maxcdn"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-openid"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pagelines"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pied-piper"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pied-piper-alt"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pied-piper-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pinterest"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-pinterest-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-qq"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-ra"></i></a></div>
       
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-reddit"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-reddit-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-renren"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-share-alt"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-share-alt-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-skype"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-slack"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-soundcloud"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-spotify"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-stack-exchange"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-stack-overflow"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-steam"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-steam-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-stumbleupon"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-stumbleupon-circle"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tencent-weibo"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-trello"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tumblr"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-tumblr-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-twitter"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-twitter-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-vimeo-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-vine"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-vk"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-wechat"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-weibo"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-weixin"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-windows"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-wordpress"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-xing"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-xing-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-yahoo"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-youtube"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-youtube-play"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-youtube-square"></i> </a></div>
    
  </div>
</section>

  <section id="medical">
  <h2 class="page-header"></h2>

  <div class="row fontawesome-icon-list">
     
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-ambulance"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-h-square"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-hospital-o"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-medkit"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-plus-square"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-stethoscope"></i></a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-user-md"></i> </a></div>
    
      <div class="fa-hover col-md-3 col-sm-4"><a href="javascript:;"><i class="fa fa-wheelchair"></i></a></div>
    
  </div>

</section>

</form>
</body>
</html>
