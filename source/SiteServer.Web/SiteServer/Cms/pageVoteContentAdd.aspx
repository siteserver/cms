<%@ Page Language="C#" validateRequest="false" enableEventValidation="false" Inherits="SiteServer.BackgroundPages.Cms.PageVoteContentAdd" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form id="myForm" enctype="multipart/form-data" class="form-inline" runat="server">
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <script type="text/javascript" charset="utf-8" src="../assets/validate.js"></script>
  <script type="text/javascript">
  function removeItem(itemIndex) {
    $('#item_' + itemIndex).remove();
    $('#ddlMaxSelectNum').children().remove();
    $('#ddlMaxSelectNum').append('<option value="1">单选</option>');
    for (i = 2; i < $('#vote_options li').length; i++){
      $('#ddlMaxSelectNum').append('<option value="' + i + '">至少选' + i + '项</option>');
    }
  }
  function addItem(title){
    var itemIndex = parseInt($('#itemCount').val());
    var itemCount = itemIndex + 1;
    $('#vote_options').append('<li id="item_' + itemIndex + '"> <span>' + itemCount + '.</span><div class="txt_des"><input id="options_' + itemIndex + '" name="options[' + itemIndex + ']" onfocus="this.className=\'colorfocus\';" onblur="this.className=\'colorblur\';" style="width:350px;" type="text" value="' + title + '" isValidate="true" errorMessage="" isRequire="true" /></div><span id="options_' + itemIndex +  '_msg" style="color:red;display:none; padding-left:5px; margin-top: 6px;"></span><a class="W_close" onclick="removeItem(' + itemIndex + ');" href="javascript:;">&nbsp;&nbsp;删除</a></li>');
    $('#itemCount').val(itemCount + '');
    $('#ddlMaxSelectNum').children().remove();
    $('#ddlMaxSelectNum').append('<option value="1">单选</option>');
    for (i = 2; i < $('#vote_options li').length; i++){
      $('#ddlMaxSelectNum').append('<option value="' + i + '">最多选' + i + '项</option>');
    } 
  }
  $(document).ready(function(e) {
    $('#addSummary').click(function(){
      $('#summary1').hide();$('#summary2').show();$('#IsSummary').val('True');
    });
    $('#closeSummary').click(function(){
      $('#summary1').show();$('#summary2').hide();$('#IsSummary').val('False');
    });
    $('#ddlEndDate').click(function(){
      if ($('#ddlEndDate').val() == ""){
        $('#endDate').show();
      }else{
      $('#endDate').hide();
      }
    });
    $('#add_more').click(function(e) {
      addItem('');
      });
  });
  $(document).keypress(function(e){
    if(e.ctrlKey && e.which == 13 || e.which == 10) { 
      e.preventDefault();
      $("#Submit").click();
    } else if (e.shiftKey && e.which==13 || e.which == 10) {
      e.preventDefault();
      $("#Submit").click();
    }
  });
  </script>
  <style>
  .vote_options li {clear:both; height:26px;}
  .vote_options li span, .txt_des { float:left;}
  .vote_addNew { line-height: 22px; padding-left: 5px; }
  .vote_addNew ul { padding-left: 5px; }
  .vote_addNew .vote_options li { margin-bottom: 15px; list-style: none; padding-left: 5px; }
  .vote_addNew .vote_options li:last-child { margin-bottom: 5px; }
  .vote_addNew .vote_options li span, .vote_addNew .vote_options li div, .pic_vote .upbtn .upic, .vote_addNew .vote_options .txt_des, .vote_addNew .vote_options .W_close, .vote_addNew .tips .add_more { vertical-align: top; float: left; }
  .vote_addNew .vote_options span { margin-right: 5px; }
  .vote_addNew .vote_options .W_close { margin-top: 8px; color: #666; text-decoration: none; }
  .vote_addNew .add_item {padding-left:10px;}
  .add_item {clear:both}
  </style>

  <div class="popover popover-static">
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server" /></h3>
    <div class="popover-content">
    
      <table class="table table-fixed noborder" style="position:relative;top:-30px;">
        <tr><td width="100">&nbsp;</td><td></td><td width="100"></td><td></td></tr>
        <bairong:AuxiliaryControl ID="acAttributes" runat="server"/>
        <tr>
          <td>说明：</td>
          <td colspan="3"><div id="summary1" style='<%=base.IsSummary ? "display:none" : ""%>'><a id="addSummary" href="javascript:;">添加说明</a></div>
            <div id="summary2" style='<%=base.IsSummary ? "" : "display:none"%>'>
              <div style="margin:5px 0"><a id="closeSummary" href="javascript:;">关闭说明</a></div>
              <div>
                <asp:TextBox ID="tbSummary" TextMode="MultiLine" style="width:90%; height:70px;" runat="server"></asp:TextBox>
              </div>
            </div>
            <input id="IsSummary" name="IsSummary" type="hidden" value="false" />
            <input id="itemCount" name="itemCount" type="hidden" value="2" />
           </td>
        </tr>
        <tr>
          <td>投票选项：</td>
          <td class="vote_addNew" colspan="3">
            <ul id="vote_options" class="vote_options txt_vote">
              <li>
                <span>1.</span>
                <div class="txt_des">
                  <input id="options_0" name="options[0]" style="width:350px;" type="text" value="<%=GetOptionTitle(0)%>" isValidate="true" errorMessage="" isRequire="true" />
                </div>
                <span id="options_0_msg" style="color:red;display:none; padding-left:5px;"></span>
                <script>event_observe('options_0', 'blur', checkAttributeValue);</script>
              </li>
              <li>
                <span>2.</span>
                <div class="txt_des">
                  <input id="options_1" name="options[1]" style="width:350px;" type="text" value="<%=GetOptionTitle(1)%>" isValidate="true" errorMessage="" isRequire="true" />
                </div>
                <span id="options_1_msg" style="color:red;display:none; padding-left:5px;"></span>
                <script>event_observe('options_1', 'blur', checkAttributeValue);</script>
              </li>
            </ul>
            <div class="add_item">
              <span class="add_more"><s></s><a id="add_more" href="javascript:;">再加一项</a></span>
              <span class="info_more gray">至少设置两项，每项最多20个字</span>
            </div>
            <asp:Literal ID="ltlScript" runat="server"></asp:Literal>
          </td>
        </tr>
        <tr>
          <td>单选/多选：</td>
          <td colspan="3">
            <asp:DropDownList ID="ddlMaxSelectNum" class="input-medium" runat="server">
              <asp:ListItem Text="单选" Value="1"></asp:ListItem>
            </asp:DropDownList>
          </td>
        </tr>
        <tr>
          <td>开始日期：</td>
          <td colspan="3">
            <bairong:DateTimeTextBox id="dtbAddDate" class="input-medium" showTime="true" now="true" Columns="20" runat="server" />
          </td>
        </tr>
        <tr>
          <td>结束日期：</td>
          <td colspan="3">
            <asp:DropDownList ID="ddlEndDate" style="width:100px;" runat="server"></asp:DropDownList>
            <span id="endDate" style='<%=string.IsNullOrEmpty(base.Request.QueryString["ID"]) ? "display:none" : ""%>'>
            <bairong:DateTimeTextBox id="dtbEndDate" class="input-medium" showTime="true" Columns="20" runat="server" />
            </span>
          </td>
        </tr>
        <tr>
          <td>投票结果：</td>
          <td colspan="3">
            <asp:RadioButtonList ID="rblIsVotedView" RepeatDirection="Horizontal" class="noborder" runat="server">
              <asp:ListItem Text="任何人可见" Value="False"></asp:ListItem>
              <asp:ListItem Text="投票后可见" Value="True" Selected="true"></asp:ListItem>
            </asp:RadioButtonList>
          </td>
        </tr>
        <tr>
          <td>隐藏文字：</td>
          <td colspan="3">
            <asp:TextBox ID="tbHiddenContent" TextMode="MultiLine" style="width:90%; height:70px;" runat="server"></asp:TextBox>
            <div class="gray">隐藏文字是参与投票成功后可见的内容，最多输入1000个字</div>
          </td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server"/>
            <input class="btn btn-info" type="button" onClick="submitPreview();" value="预 览" />
            <input class="btn" type="button" onClick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
            <br><span class="gray">提示：按CTRL+回车可以快速提交</span>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

  <script type="text/javascript">
  document.body.onkeydown = function (moz_ev) 
  { 
    var ev = (window.event) ? window.event : moz_ev; 
    if (ev != null && ev.ctrlKey && ev.keyCode == 13) 
    { 
      document.getElementById("Submit").click(); 
    } 
  }
  </script>

</form>
</body>
</html>
