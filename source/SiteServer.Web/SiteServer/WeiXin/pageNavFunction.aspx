<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageNavFunction" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html lang="en">
<head> 
  <meta charset="utf-8">
  <title>微功能导航</title> 
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
      <h1>微功能导航 <small>移动鼠标滑轮显示更多</small>    </h1>
    </div>
    <div class="content clearfix">

      <div class="items">
        
        <a class="box" href="../weixin/background_coupon.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #00acdc;">
          <span>优惠券</span>
          <i class="icon-money"></i>
        </a>

        <a class="box" href="../weixin/background_lottery.aspx?publishmentSystemID=<%=PublishmentSystemID%>&lotteryType=Scratch" style="background: #cc2749;">
          <span>刮刮卡</span>
          <i class="icon-credit-card"></i>
        </a>

        <a class="box" href="../weixin/background_lottery.aspx?publishmentSystemID=<%=PublishmentSystemID%>&lotteryType=BigWheel" style="background: #3399ff;">
          <span>大转盘</span>
          <i class="icon-bullseye"></i>
        </a>

        <a class="box" href="../weixin/background_lottery.aspx?publishmentSystemID=<%=PublishmentSystemID%>&lotteryType=GoldEgg" style="background: #f2ca27;">
          <span>砸金蛋</span>
          <i class="icon-legal"></i>
        </a>

        <a class="box" href="../weixin/background_lottery.aspx?publishmentSystemID=<%=PublishmentSystemID%>&lotteryType=Flap" style="background: #00A3A3;">
          <span>大翻牌</span>
          <i class="icon-th-large"></i>
        </a>

        <a class="box" href="../weixin/background_lottery.aspx?publishmentSystemID=<%=PublishmentSystemID%>&lotteryType=YaoYao" style="background: #5133AB;">
          <span>摇摇乐</span>
          <i class="icon-bell-alt"></i>
        </a>

        <a class="box" href="../weixin/background_vote.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #19a657;">
          <span>微投票</span>
          <i class="icon-reorder"></i>
        </a>

        <a class="box" href="../weixin/background_message.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #eb7126;">
          <span>微留言</span>
          <i class="icon-comment"></i>
        </a>

        <a class="box" href="../weixin/background_appointment.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #99251C;">
          <span>微预约</span>
          <i class="icon-calendar"></i>
        </a>

        <a class="box" href="../weixin/background_conference.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #1e7145;">
          <span>微会议</span>
          <i class="icon-ticket"></i>
        </a>
        
        <a class="box" href="../weixin/background_map.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #034380;">
          <span>微导航</span>
          <i class="icon-location-arrow"></i>
        </a>

        <a class="box" href="../weixin/background_view360.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #56b10e;">
          <span>360全景</span>
          <i class="icon-compass"></i>
        </a>

        <a class="box" href="../weixin/background_album.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #cc3333;">
          <span>微相册</span>
          <i class="icon-picture"></i>
        </a>

        <a class="box" href="../weixin/background_search.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #9b50ba;">
          <span>微搜索</span>
          <i class="icon-search"></i>
        </a>

        <a class="box" href="javascript:;" onclick="$('.items').hide();$('#items_store').show();" style="background: #2980B9;">
          <span>微门店</span>
          <i class="icon-building"></i>
        </a>

        <a class="box" href="javascript:;" onclick="$('.items').hide();$('#items_wifi').show();" style="background: #348055;">
          <span>微信WIFI</span>
          <i class="icon-signal"></i>
        </a>

       <a class="box" href="../weixin/background_card.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #9b50ba;">
          <span>会员卡</span>
          <i class="icon-check-minus"></i>
       </a>
     <asp:PlaceHolder id="PhQCloud" visible="false" runat="server">
          <a class="box" href="../weixin/background_right.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #3498db;">
            <span>账户信息</span>
            <i class="icon-user"></i>
          </a>

          <a class="box" href="../weixin/background_menu.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #1abc9c;">
            <span>自定义菜单</span>
            <i class="icon-align-center"></i>
          </a>

          <a class="box" href="../weixin/background_keywordText.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #9B59B6;">
            <span>关键词文本回复</span>
            <i class="icon-tumblr"></i>
          </a>

          <a class="box" href="../weixin/background_keywordNews.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #9B59B6;">
            <span>关键词图文回复</span>
            <i class="icon-picture"></i>
          </a>

          <a class="box" href="../weixin/background_keywordConfiguration.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #9B59B6;">
            <span>智能回复设置</span>
            <i class="icon-cog"></i>
          </a>
        </asp:PlaceHolder>
      </div>

      <div class="items" id="items_store" style="display:none">
        
        <a class="box" href="../weixin/background_storecategory.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #2980B9;">
          <span>门店区域管理</span>
          <i class="icon-building"></i>
        </a>

        <a class="box" href="../weixin/background_store.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #2980B9;">
          <span>微门店管理</span>
          <i class="icon-building"></i>
        </a>
      </div>

      <div class="items" id="items_wifi"  style="display:none">
        
        <a class="box" href="../weixin/background_wifiAdd.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #348055;">
          <span>商家设置</span>
          <i class="icon-signal"></i>
        </a>

        <a class="box" href="../weixin/background_wifiNode.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #348055;">
          <span>路由器管理</span>
          <i class="icon-signal"></i>
        </a>

        <a class="box" href="../weixin/background_wifiNodeReport.aspx?publishmentSystemID=<%=PublishmentSystemID%>" style="background: #348055;">
          <span>路由器报表</span>
          <i class="icon-signal"></i>
        </a>
      </div>

    </div>
  </div>
</body>
</html>