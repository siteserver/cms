<%@ Page Language="C#" Inherits="SiteServer.CMS.BackgroundPages.BackgroundBasePage" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!doctype html>
<html lang="en">
<head> 
  <meta charset="utf-8">
  <title>微官网功能导航</title> 
  <meta name="description" content="" />
  <meta name="keywords" content="" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0" />
  <script language="javascript" src="../../sitefiles/bairong/jquery/jquery-1.9.1.min.js"></script>
  <link rel="stylesheet" type="text/css" media="all" href="../scripts/lib/metro/metro.css" />
  <script src="../scripts/lib/metro/jquery.plugins.min.js"></script>
  <script src="../scripts/lib/metro/metro.js"></script>
  <!--[if lt IE 9]>
    <script src="../scripts/lib/metro/respond.min.js"></script>
  <![endif]-->
  <link rel="stylesheet" type="text/css" href="../scripts/lib/bootstrap/css/bootstrap.min.css" media="all" />
  <link href="../scripts/lib/font-awesome/css/font-awesome.css" rel="stylesheet">
</head> 
<body>
  <div class="metro-layout horizontal">
    <div class="header">
      <h1>微官网功能导航 <small>移动鼠标滑轮显示更多</small>    </h1>
    </div>
    <div class="content clearfix">
      <div class="items">
        <a class="box" href="../cms/background_contentMain.aspx?publishmentSystemID=<%=PublishmentSystemId%>">
          <span>内容管理</span>
          <i class="icon-file-text"></i>
        </a>
        <a class="box" href="../cms/background_channel.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #43b51f;">
          <span>栏目管理</span>
          <i class="icon-folder-close"></i>
        </a>
        <a class="box" href="../cms/background_contentCheck.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #00aeef;">
          <span>内容审核  </span>
          <i class="icon-shield"></i>
        </a>
        <a class="box" href="../cms/background_contentSearch.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #f58d00;">
          <span>内容搜索</span>
          <i class="icon-search"></i>
        </a>
        <a class="box" href="../cms/background_contentTrash.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #00aba9;">
          <span>内容回收站</span>
          <i class="icon-trash"></i>
        </a>
        <a class="box height2" href="../stl/background_templateMain.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #d32c2c;">
          <span>模板管理</span>
          <i class="icon-dashboard big"></i>
        </a>
        <a class="box" href="../stl/background_templateMatch.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #3c5b9b;">
          <span>匹配模板</span>
          <i class="icon-refresh"></i>
        </a>
        <a class="box" href="../stl/background_templateInclude.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #ffc808;">
          <span>包含文件管理</span>
          <i class="icon-file"></i>
        </a>
        <a class="box height2" href="../stl/background_progressBar.aspx?CreateIndex=True&publishmentSystemID=<%=PublishmentSystemId%>" style="background: #00a300;">
          <span>生成首页</span>
          <i class="icon-globe big"></i>
        </a>
        <a class="box" href="../stl/background_createChannel.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #00aeef;">
          <span>生成栏目页  </span>
          <i class="icon-globe"></i>
        </a>
        <a class="box" href="../stl/background_createContent.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #f58d00;">
          <span>生成内容页</span>
          <i class="icon-globe"></i>
        </a>
        <a class="box" href="../stl/background_createFile.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #640f6c;">
          <span>生成文件页</span>
          <i class="icon-globe"></i>
        </a>
        <a class="box" href="../cms/background_configurationSite.aspx?publishmentSystemID=<%=PublishmentSystemId%>" style="background: #0C7710;">
          <span>站点设置</span>
          <i class="icon-cog"></i>
        </a>

      </div>
    </div>
  </div>
</body>
</html>