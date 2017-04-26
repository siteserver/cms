<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentAttributes" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <script language="javascript" type="text/javascript">
  function _toggleTab(no,totalNum){ 
    $("#tab"+no).removeClass("tabOff");
    $("#tab"+no).addClass("tabOn");
    $("#column"+no).show();
    
    document.getElementById("hdType").value = no + "";
    for (var i = 1; i <= totalNum; i++){
      if (i != no){
        $("#tab"+i).removeClass("tabOn");
        $("#tab"+i).addClass("tabOff");
        if (i != 2){
          $("#column"+i).hide();
        }
      }
    }
  }

  function _toggleTab2(no,totalNum){
    
    $("#tab"+no).removeClass("tabOff");
    $("#tab"+no).addClass("tabOn");
    $("#column"+1).show();
    
    document.getElementById("hdType").value = no + "";
    for (var i = 1; i <= totalNum; i++){
      if (i != no){
        $("#tab"+i).removeClass("tabOn");
        $("#tab"+i).addClass("tabOff");
        $("#column"+3).hide();
      }
    }
  }

  function _toggleTab(no, totalNum) {
    document.getElementById("hdType").value = no + "";
    $('#tab' + no).addClass("active");
    
    for (var i = 1; i <= totalNum; i++) {
      if (i != no) {
        $('#tab' + i).removeClass("active");
        $('#column' + i).hide();
      }
    }

    if (no == 2) no = 1;
    $('#column' + no).show();
  }
  </script>

  <input id="hdType" type="hidden" runat="server" value="1" />
    <ul class="nav nav-pills">
      <li id="tab1" class="active"><a href="javascript:;" onClick="_toggleTab(1,3);">设置属性</a></li>
      <li id="tab2"><a href="javascript:;" onClick="_toggleTab(2,3);">取消属性</a></li>
      <li id="tab3"><a href="javascript:;" onClick="_toggleTab(3,3);">设置点击量</a></li>
    </ul>
    <div id="column1">
      <div class="columncontent">
      <table class="table table-noborder table-hover">
        <tr>
          <td>
            <asp:CheckBox ID="IsRecommend" runat="server" Text="推荐"></asp:CheckBox>
            <asp:CheckBox ID="IsHot" runat="server" Text="热点"></asp:CheckBox>
            <asp:CheckBox ID="IsColor" runat="server" Text="醒目"></asp:CheckBox>
            <asp:CheckBox ID="IsTop" runat="server" Text="置顶"></asp:CheckBox></td>
        </tr>
      </table>
    </div>
    </div>
    <div id="column3" style="display:none">
      <div class="columncontent">
      <table class="table table-noborder table-hover">
        <tr>
          <td> 点击量：
            <asp:TextBox class="input-mini" MaxLength="50" id="Hits" Text="0" runat="server"/>
            <asp:RequiredFieldValidator
                ControlToValidate="Hits"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic"
                runat="server"/>
            <asp:RegularExpressionValidator
                ControlToValidate="Hits"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="点击量必须为整数"
                foreColor="red"
                runat="server"/></td>
        </tr>
      </table>
      </div>
    </div>

</form>
</body>
</html>
