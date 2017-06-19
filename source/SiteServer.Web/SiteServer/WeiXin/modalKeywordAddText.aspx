<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalKeywordAddText" Trace="false"%>
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
  
  <link href="css/emotion.css" rel="stylesheet">

  <table class="table table-noborder">
    <tr>
      <td width="120">关键词：</td>
      <td colspan="3"><asp:TextBox id="TbKeywords" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="TbKeywords" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <span class="gray">多个关键词请用空格格开：例如: 微信 腾讯</span> </td>
    </tr>
    <tr>
      <td>匹配规则：</td>
      <td width="240">
        <asp:DropDownList id="DdlMatchType" runat="server" />
      </td>
      <td width="80">是否启用：</td>
      <td class="checkbox">
        <asp:CheckBox id="CbIsEnabled" text="启用关键字"  runat="server" />
      </td>
    </tr>
    <tr>
      <td>自动回复内容：</td>
      <td colspan="3">

<div class="functionBar">
  <div class="opt">
    <a class="icon18C iconEmotion block" href="javascript:;">
      表情
    </a>
  </div>
  <div class="tip">
  </div>
  <div class="emotions" style="display: none;">
    <table cellspacing="0" cellpadding="0">
      <tbody>
        <tr>
          <td>
            <div class="eItem" style="background-position: 0px 0;" data-title="微笑"
            data-gifurl="images/face/0.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -24px 0;" data-title="撇嘴"
            data-gifurl="images/face/1.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -48px 0;" data-title="色"
            data-gifurl="images/face/2.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -72px 0;" data-title="发呆"
            data-gifurl="images/face/3.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -96px 0;" data-title="得意"
            data-gifurl="images/face/4.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -120px 0;" data-title="流泪"
            data-gifurl="images/face/5.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -144px 0;" data-title="害羞"
            data-gifurl="images/face/6.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -168px 0;" data-title="闭嘴"
            data-gifurl="images/face/7.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -192px 0;" data-title="睡"
            data-gifurl="images/face/8.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -216px 0;" data-title="大哭"
            data-gifurl="images/face/9.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -240px 0;" data-title="尴尬"
            data-gifurl="images/face/10.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -264px 0;" data-title="发怒"
            data-gifurl="images/face/11.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -288px 0;" data-title="调皮"
            data-gifurl="images/face/12.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -312px 0;" data-title="呲牙"
            data-gifurl="images/face/13.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -336px 0;" data-title="惊讶"
            data-gifurl="images/face/14.gif">
            </div>
          </td>
        </tr>
        <tr>
          <td>
            <div class="eItem" style="background-position: -360px 0;" data-title="难过"
            data-gifurl="images/face/15.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -384px 0;" data-title="酷"
            data-gifurl="images/face/16.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -408px 0;" data-title="冷汗"
            data-gifurl="images/face/17.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -432px 0;" data-title="抓狂"
            data-gifurl="images/face/18.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -456px 0;" data-title="吐"
            data-gifurl="images/face/19.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -480px 0;" data-title="偷笑"
            data-gifurl="images/face/20.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -504px 0;" data-title="可爱"
            data-gifurl="images/face/21.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -528px 0;" data-title="白眼"
            data-gifurl="images/face/22.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -552px 0;" data-title="傲慢"
            data-gifurl="images/face/23.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -576px 0;" data-title="饥饿"
            data-gifurl="images/face/24.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -600px 0;" data-title="困"
            data-gifurl="images/face/25.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -624px 0;" data-title="惊恐"
            data-gifurl="images/face/26.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -648px 0;" data-title="流汗"
            data-gifurl="images/face/27.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -672px 0;" data-title="憨笑"
            data-gifurl="images/face/28.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -696px 0;" data-title="大兵"
            data-gifurl="images/face/29.gif">
            </div>
          </td>
        </tr>
        <tr>
          <td>
            <div class="eItem" style="background-position: -720px 0;" data-title="奋斗"
            data-gifurl="images/face/30.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -744px 0;" data-title="咒骂"
            data-gifurl="images/face/31.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -768px 0;" data-title="疑问"
            data-gifurl="images/face/32.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -792px 0;" data-title="嘘"
            data-gifurl="images/face/33.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -816px 0;" data-title="晕"
            data-gifurl="images/face/34.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -840px 0;" data-title="折磨"
            data-gifurl="images/face/35.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -864px 0;" data-title="衰"
            data-gifurl="images/face/36.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -888px 0;" data-title="骷髅"
            data-gifurl="images/face/37.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -912px 0;" data-title="敲打"
            data-gifurl="images/face/38.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -936px 0;" data-title="再见"
            data-gifurl="images/face/39.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -960px 0;" data-title="擦汗"
            data-gifurl="images/face/40.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -984px 0;" data-title="抠鼻"
            data-gifurl="images/face/41.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1008px 0;" data-title="鼓掌"
            data-gifurl="images/face/42.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1032px 0;" data-title="糗大了"
            data-gifurl="images/face/43.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1056px 0;" data-title="坏笑"
            data-gifurl="images/face/44.gif">
            </div>
          </td>
        </tr>
        <tr>
          <td>
            <div class="eItem" style="background-position: -1080px 0;" data-title="左哼哼"
            data-gifurl="images/face/45.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1104px 0;" data-title="右哼哼"
            data-gifurl="images/face/46.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1128px 0;" data-title="哈欠"
            data-gifurl="images/face/47.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1152px 0;" data-title="鄙视"
            data-gifurl="images/face/48.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1176px 0;" data-title="委屈"
            data-gifurl="images/face/49.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1200px 0;" data-title="快哭了"
            data-gifurl="images/face/50.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1224px 0;" data-title="阴险"
            data-gifurl="images/face/51.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1248px 0;" data-title="亲亲"
            data-gifurl="images/face/52.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1272px 0;" data-title="吓"
            data-gifurl="images/face/53.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1296px 0;" data-title="可怜"
            data-gifurl="images/face/54.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1320px 0;" data-title="菜刀"
            data-gifurl="images/face/55.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1344px 0;" data-title="西瓜"
            data-gifurl="images/face/56.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1368px 0;" data-title="啤酒"
            data-gifurl="images/face/57.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1392px 0;" data-title="篮球"
            data-gifurl="images/face/58.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1416px 0;" data-title="乒乓"
            data-gifurl="images/face/59.gif">
            </div>
          </td>
        </tr>
        <tr>
          <td>
            <div class="eItem" style="background-position: -1440px 0;" data-title="咖啡"
            data-gifurl="images/face/60.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1464px 0;" data-title="饭"
            data-gifurl="images/face/61.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1488px 0;" data-title="猪头"
            data-gifurl="images/face/62.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1512px 0;" data-title="玫瑰"
            data-gifurl="images/face/63.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1536px 0;" data-title="凋谢"
            data-gifurl="images/face/64.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1560px 0;" data-title="示爱"
            data-gifurl="images/face/65.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1584px 0;" data-title="爱心"
            data-gifurl="images/face/66.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1608px 0;" data-title="心碎"
            data-gifurl="images/face/67.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1632px 0;" data-title="蛋糕"
            data-gifurl="images/face/68.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1656px 0;" data-title="闪电"
            data-gifurl="images/face/69.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1680px 0;" data-title="炸弹"
            data-gifurl="images/face/70.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1704px 0;" data-title="刀"
            data-gifurl="images/face/71.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1728px 0;" data-title="足球"
            data-gifurl="images/face/72.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1752px 0;" data-title="瓢虫"
            data-gifurl="images/face/73.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1776px 0;" data-title="便便"
            data-gifurl="images/face/74.gif">
            </div>
          </td>
        </tr>
        <tr>
          <td>
            <div class="eItem" style="background-position: -1800px 0;" data-title="月亮"
            data-gifurl="images/face/75.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1824px 0;" data-title="太阳"
            data-gifurl="images/face/76.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1848px 0;" data-title="礼物"
            data-gifurl="images/face/77.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1872px 0;" data-title="拥抱"
            data-gifurl="images/face/78.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1896px 0;" data-title="强"
            data-gifurl="images/face/79.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1920px 0;" data-title="弱"
            data-gifurl="images/face/80.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1944px 0;" data-title="握手"
            data-gifurl="images/face/81.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1968px 0;" data-title="胜利"
            data-gifurl="images/face/82.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -1992px 0;" data-title="抱拳"
            data-gifurl="images/face/83.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2016px 0;" data-title="勾引"
            data-gifurl="images/face/84.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2040px 0;" data-title="拳头"
            data-gifurl="images/face/85.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2064px 0;" data-title="差劲"
            data-gifurl="images/face/86.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2088px 0;" data-title="爱你"
            data-gifurl="images/face/87.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2112px 0;" data-title="NO"
            data-gifurl="images/face/88.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2136px 0;" data-title="OK"
            data-gifurl="images/face/89.gif">
            </div>
          </td>
        </tr>
        <tr>
          <td>
            <div class="eItem" style="background-position: -2160px 0;" data-title="爱情"
            data-gifurl="images/face/90.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2184px 0;" data-title="飞吻"
            data-gifurl="images/face/91.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2208px 0;" data-title="跳跳"
            data-gifurl="images/face/92.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2232px 0;" data-title="发抖"
            data-gifurl="images/face/93.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2256px 0;" data-title="怄火"
            data-gifurl="images/face/94.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2280px 0;" data-title="转圈"
            data-gifurl="images/face/95.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2304px 0;" data-title="磕头"
            data-gifurl="images/face/96.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2328px 0;" data-title="回头"
            data-gifurl="images/face/97.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2352px 0;" data-title="跳绳"
            data-gifurl="images/face/98.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2376px 0;" data-title="挥手"
            data-gifurl="images/face/99.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2400px 0;" data-title="激动"
            data-gifurl="images/face/100.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2424px 0;" data-title="街舞"
            data-gifurl="images/face/101.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2448px 0;" data-title="献吻"
            data-gifurl="images/face/102.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2472px 0;" data-title="右太极"
            data-gifurl="images/face/103.gif">
            </div>
          </td>
          <td>
            <div class="eItem" style="background-position: -2496px 0;" data-title="左太极"
            data-gifurl="images/face/104.gif">
            </div>
          </td>
        </tr>
      </tbody>
    </table>
    <div class="emotionsGif">
      <img src="images/face/0.gif">
    </div>
  </div>
  <div class="clr">
  </div>
</div>

        <asp:TextBox id="TbReply" textMode="MultiLine" class="textarea" width="95%" rows="8" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="TbReply" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <br>
        <span class="gray">超链接添加形式，如：&lt;a href="http://www.baidu.com"&gt;百度&lt;/a&gt;</span>
        <hr>
        <div id="titles" class="well well-small" style="display:none"></div>
        <asp:Button id="BtnContentSelect" class="btn btn-info" text="添加微官网内容链接" runat="server" />
        <asp:Button id="BtnChannelSelect" class="btn btn-info" text="添加微官网栏目链接" runat="server" />

        <input id="idsCollection" name="idsCollection" type="hidden" value="" />
        <script type="text/javascript">
        var contentSelect = function(title, nodeID, contentID, pageUrl){
          $('#tbReply').val($('#tbReply').val() + '<a href="' + pageUrl + '">' + title + '</a>');
        };
        var selectChannel = function(nodeNames, nodeID, pageUrl){
          $('#tbReply').val($('#tbReply').val() + '<a href="' + pageUrl + '">' + nodeNames + '</a>');
        };

        $('.iconEmotion').click(function(){
          $('.emotions').toggle();
        });
        $('.eItem').hover(function(){
          $('.emotionsGif img').attr('src', $(this).attr('data-gifurl'));
        });
        $('.eItem').click(function(){
          $('#tbReply').val($('#tbReply').val() + '/' + $(this).attr('data-title'));
          $('.emotions').hide();
        });
        </script>
      </td>
    </tr>
  </table>

</form>
</body>
</html>
