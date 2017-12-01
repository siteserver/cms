<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageCardConfiguration" %>
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
      <script type="text/javascript">
             function removeItem(itemIndex) {
                 $('#item_' + itemIndex).remove();

             }
             function addItem(dayFrom, dayTo, credits) {
                 var itemIndex = parseInt($('#itemCount').val());
                 var itemCount = itemIndex + 1;
                 $('#options').append('<li id="item_' + itemIndex + '"> <span> 连续签到&nbsp;&nbsp;</span><div class="txt_des"><input id="optionsDayFrom_' + itemIndex + '" name="optionsDayFrom[' + itemIndex + ']"  class="input-mini" type="text" value="' + dayFrom + '" isValidate="true" errorMessage="" isRequire="true" /> &nbsp;到&nbsp; <input id="optionsDayTo_' + itemIndex + '" name="optionsDayTo[' + itemIndex + ']"  class="input-mini" type="text" value="' + dayTo + '" isValidate="true" errorMessage="" isRequire="true" />&nbsp;天，送&nbsp; <input id="optionsSignCredits_' + itemIndex + '" name="optionsSignCredits[' + itemIndex + ']"  class="input-mini" type="text" value="' + credits + '" isValidate="true" errorMessage="" isRequire="true" /> &nbsp;积分 </div><span id="optionsDayFrom_' + itemIndex + '_msg" style="color:red;display:none; padding-left:5px; margin-top: 6px;"></span><a class="W_close" onclick="removeItem(' + itemIndex + ');" href="javascript:;">&nbsp;&nbsp;删除</a></li>');
                 $('#itemCount').val(itemCount + '');
             }
           
             $(document).ready(function (e) {
                 $('#add_more').click(function (e) {
                     addItem('','','');
                 });
                  
             });
           
  </script>
  <style>
  .options li {clear:both; height:26px; list-style-type:none;}
  .options li span, .txt_des { float:left;}
  .addNew { line-height: 22px; padding-left: 5px; }
  .addNew ul { padding-left: 5px; }
  .addNew .options li { margin-bottom: 15px; list-style: none; padding-left: 5px; }
  .addNew .options li:last-child { margin-bottom: 5px; }
  .addNew .options li span, .addNew .options li div, .pic_radio .upbtn .upic, .addNew .options .txt_des, .addNew .options .W_close, .addNew .tips .add_more { vertical-align: top; float: left; }
  .addNew .options span { margin-right: 5px; }
  .addNew .options .W_close { margin-top: 8px; color: #666; text-decoration: none; }
  .addNew .add_item {padding-left:10px;}
  .add_item {clear:both}
  </style>
 
  <div class="popover popover-static">
    <h3 class="popover-title">会员卡设置</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="160"><bairong:help HelpText="是否启用领卡送积分" Text="是否启用领卡送积分：" runat="server" ></bairong:help></td>
          <td><asp:RadioButtonList ID="IsClaimCardCredits" runat="server" RepeatDirection="Horizontal" class="noborder" AutoPostBack="true" OnSelectedIndexChanged="Refrush"></asp:RadioButtonList></td>
        </tr>
        <tr id="ClaimCardCreditsRow" runat="server">
          <td width="160"><bairong:help HelpText="领卡送积分" Text="领卡送积分：" runat="server" ></bairong:help></td>
          <td> <asp:TextBox ID="TbClaimCardCredits" class="input-mini"  Columns="10" MaxLength="50" runat="server"></asp:TextBox> 
            <asp:RequiredFieldValidator ControlToValidate="TbClaimCardCredits" ErrorMessage="此项不能为空" Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator
              ControlToValidate="TbClaimCardCredits"
              ValidationExpression="\d+"
              Display="Dynamic"
              ErrorMessage="此项必须为数字"
              runat="server"/> 
          </td>
        </tr>
    
        <tr  runat="server">
          <td><bairong:help HelpText="是否启用消费赠送" Text="是否启用消费赠送：" runat="server" ></bairong:help></td>
          <td><asp:RadioButtonList ID="IsGiveConsumeCredits" runat="server" RepeatDirection="Horizontal" class="noborder" AutoPostBack="true" OnSelectedIndexChanged="Refrush"></asp:RadioButtonList></td>
        </tr>
        <tr id="GiveConsumeCreditsRow" runat="server">
          <td><bairong:help HelpText="赠送规则" Text="赠送规则：" runat="server" ></bairong:help></td>
          <td>消费&nbsp;<asp:TextBox class="input-mini" Columns="10" id="TbConsumeAmount" runat="server" />&nbsp;元&nbsp;赠送 &nbsp;<asp:TextBox class="input-mini" Columns="10" id="TbGivCredits" runat="server" />&nbsp;积分
              <br /><span>设置消费100元送50积分。当用户消费20元时,可得 20*(50/100)=10 积分;用户消费250元时,可得 250*(50/100)=125 积分。</span>
          </td>
         </tr>
         <tr  >
          <td><bairong:help HelpText="是否启用实体卡绑定" Text="是否启用实体卡绑定：" runat="server" ></bairong:help></td>
          <td><asp:RadioButtonList ID="IsBinding" runat="server" RepeatDirection="Horizontal" class="noborder" AutoPostBack="true" OnSelectedIndexChanged="Refrush"></asp:RadioButtonList></td>
        </tr>
         <tr  runat="server">
          <td><bairong:help HelpText="是否启用积分兑换" Text="是否启用积分兑换：" runat="server" ></bairong:help></td>
          <td><asp:RadioButtonList ID="IsExchange" runat="server" RepeatDirection="Horizontal" class="noborder" AutoPostBack="true" OnSelectedIndexChanged="Refrush"></asp:RadioButtonList></td>
        </tr>
        <tr id="ExchangeProportionRow"  runat="server">
          <td><bairong:help HelpText="积分兑换比例" Text="积分兑换比例：" runat="server" ></bairong:help></td>
          <td><asp:TextBox class="input-mini" Columns="10" MaxLength="50" id="TbExchangeProportion" runat="server" /> 
             <br /> <span>设置500积分可以兑换50元，兑换比例为10。（积分/人民币）</span>
          </td>
        </tr>

        <tr>
          <td width="160"><bairong:help HelpText="是否启用签到" Text="是否启用签到：" runat="server" ></bairong:help></td>
          <td><asp:RadioButtonList ID="IsSign" runat="server" RepeatDirection="Horizontal" class="noborder" AutoPostBack="true" OnSelectedIndexChanged="Refrush"></asp:RadioButtonList></td>
        </tr>
        <tr id="SignCreditsRow" runat="server" >
          <td width="160"><bairong:help HelpText="签到积分规则" Text="签到积分规则：" runat="server" ></bairong:help></td>
          <td> 
               <input id="itemCount" name="itemCount" type="hidden" value="2" />
               <ul id="options" class="options txt_radio">
                <li>
                <span>连续签到 &nbsp;</span>
                <div class="txt_des">
                  <input id="optionsDayFrom_0" name="optionsDayFrom[0]"  class="input-mini" type="text" value="<%=GetSignDayFrom(0)%>" isValidate="true" errorMessage="" isRequire="true" />
                  &nbsp;到&nbsp; <input id="optionsDayTo_0" name="optionsDayTo[0]"  class="input-mini" type="text" value="<%=GetSignDayTo(0)%>" isValidate="true" errorMessage="" isRequire="true" />
                  天，送&nbsp; <input id="optionsSignCredits_0" name="optionsSignCredits[0]"  class="input-mini" type="text" value="<%=GetSignCredits(0)%>" isValidate="true" errorMessage="" isRequire="true" /> &nbsp;积分
                </div>
                <span id="optionsDayFrom_0_msg" style="color:red;display:none; padding-left:5px;"></span>
                <script>event_observe('optionsDayFrom_0', 'blur', checkAttributeValue);</script>
              </li>
              <li>
                 <span>连续签到 &nbsp;</span>
                 <div class="txt_des">
                  <input id="optionsDayFrom_1" name="optionsDayFrom[1]"  class="input-mini" type="text" value="<%=GetSignDayFrom(1)%>" isValidate="true" errorMessage="" isRequire="true" />
                  &nbsp;到&nbsp; <input id="optionsDayTo_1" name="optionsDayTo[1]"  class="input-mini" type="text" value="<%=GetSignDayTo(1)%>" isValidate="true" errorMessage="" isRequire="true" />
                  天，送&nbsp; <input id="optionsSignCredits_1" name="optionsSignCredits[1]"  class="input-mini" type="text" value="<%=GetSignCredits(1)%>" isValidate="true" errorMessage="" isRequire="true" /> &nbsp;积分
                </div>
                <span id="optionsDayFrom_1_msg" style="color:red;display:none; padding-left:5px;"></span>
                <script>event_observe('optionsDayFrom_1', 'blur', checkAttributeValue);</script>
              </li> 
            </ul>
            <div class="add_item">
              <span class="add_more"><s></s><a id="add_more" href="javascript:;">再加一项</a></span>
              <span class="info_more gray">至少设置两项，连续签到1-5天，每天送10积分；连续签到6-10天，每天送20积分，则当用户第11天连续签到时，可得20积分。(超出天数取最大积分)</span>
            </div>
            <asp:Literal ID="LtlScript" runat="server"></asp:Literal>

          </td>
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

</form>
</body>
</html>
