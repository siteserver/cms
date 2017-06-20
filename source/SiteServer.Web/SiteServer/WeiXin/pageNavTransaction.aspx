<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageNavTransaction" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <title>会员交易</title>
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0" />
    <script language="javascript" src="../../sitefiles/bairong/jquery/jquery-1.9.1.min.js"></script>
    <link rel="stylesheet" type="text/css" media="all" href="../scripts/lib/metro/metro.css" />
    <script src="../scripts/lib/metro/jquery.plugins.min.js"></script>
    <script src="../scripts/lib/metro/metro.js"></script>
    <bairong:Code Type="bootstrap" runat="server" />
    <!--[if lt IE 9]>
    <script src="../scripts/lib/metro/respond.min.js"></script>
  <![endif]-->
    <link rel="stylesheet" type="text/css" href="../scripts/lib/bootstrap/css/bootstrap.min.css" media="all" />
    <link href="../scripts/lib/font-awesome/css/font-awesome.css" rel="stylesheet">
</head>
<body>
    <!--#include file="../inc/openWindow.html"-->
    <div class="metro-layout horizontal">
        <div class="header">
            <h1>会员交易 <small>移动鼠标滑轮显示更多</small>    </h1>
        </div>
        <div class="content clearfix">

            <div class="items">

                <a class="box" href="javascript:;" onclick="openWindow('会员消费','/siteserver/weixin/modal_cardConsume.aspx?publishmentSystemID=<%=PublishmentSystemId%>',430,380,'false');return false;" style="background: #00acdc;">
                    <span>会员消费</span>
                    <i class="icon-yen"></i>
                </a>

                <a class="box" href="javascript:;" onclick="openWindow('会员卡充值','/siteserver/weixin/modal_cardRecharge.aspx?publishmentSystemID=<%=PublishmentSystemId%>',430,380,'false');return false;" style="background: #99251C;">
                    <span>会员卡充值</span>
                    <i class="icon-money"></i>
                </a>

                <a class="box" href="javascript:;" onclick="openWindow('修改积分','/siteserver/weixin/modal_cardCredits.aspx?publishmentSystemID=<%=PublishmentSystemId%>',430,380,'false');return false;" style="background: #19a657;">
                    <span>修改积分</span>
                    <i class="icon-pencil"></i>
                </a>

                <a class="box" href="../weixin/background_cardConsumeLog.aspx?publishmentSystemID=<%=PublishmentSystemId%>&cardSN=&userName=&mobile=" style="background: #00acdc;">
                    <span>会员消费查询</span>
                    <i class="icon-search"></i>
                </a>

                 <a class="box" href="../weixin/background_cardRechargeLog.aspx?publishmentSystemID=<%=PublishmentSystemId%>&cardSN=&userName=&mobile=" style="background: #cc2749;">
                    <span>会员卡充值查询</span>
                    <i class="icon-zoom-in"></i>
                </a>

                 <a class="box" href="../weixin/background_cardExchangeLog.aspx?publishmentSystemID=<%=PublishmentSystemId%>&cardSN=&userName=&mobile=" style="background: #9b50ba;">
                    <span>积分兑换查询</span>
                    <i class="icon-zoom-in"></i>
                </a>
                   
                 <a class="box" href="../weixin/background_cardCreditsLog.aspx?publishmentSystemID=<%=PublishmentSystemId%>&cardSN=&userName=&mobile=" style="background: #5133AB;">
                    <span>会员积分查询</span>
                    <i class="icon-zoom-out"></i>
                </a>
            </div>
        </div>
    </div>
</body>
</html>
