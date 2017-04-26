<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageRight" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<bairong:Code type="jQuery" runat="server" />
<bairong:Code type="calendar" runat="server" />
<bairong:Code type="bootstrap" runat="server" />
<bairong:Code type="toastr" runat="server" />
<bairong:Code type="layer" runat="server" />
<link rel="stylesheet" href="inc/style.css" type="text/css" />
<script language="javascript" src="inc/script.js"></script>
<link href="assets/font-awesome/css/font-awesome.css" rel="stylesheet">
</head>

<body>
<form class="form-inline" runat="server">
  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          <asp:Literal ID="LtlWelcome" runat="server"></asp:Literal>。
        </td>
      </tr>
    </table>
  </div>

  <div class="popover popover-static">
  <h3 class="popover-title">系统信息</h3>
  <div class="popover-content">

    <table class="table noborder table-hover">
      <tr>
        <td width="150">当前版本：</td>
        <td>
        	<asp:Literal ID="LtlVersionInfo" runat="server" />
        </td>
      </tr>
      <tr>
        <td>最近升级时间：</td>
        <td>
        	<asp:Literal ID="LtlUpdateDate" runat="server"></asp:Literal>
        </td>
      </tr>
      <tr>
        <td>上次登录时间：</td>
        <td>
        	<asp:Literal ID="LtlLastLoginDate" runat="server"></asp:Literal>
        </td>
      </tr>
    </table>

    </div>
  </div>

  <div id="checkList"></div>

</form>
</body>
</html>
<script type="text/javascript">
  $(function () {
    $.ajax({
        url     : "<%=ApiUrl%>",
          type    : "GET",
          dataType: "json",
          success : function (data) { renderList(data) }
         }
      );
   })

  function renderList(list) {
    var html = '';
    var totalCount = 0;
    for (i = 0; i < list.length; i++) {
        html += '<tr><td><a href="' + list[i].url + '">' + list[i].siteName + ' 有 <span style="color:#f00">' + list[i].count + '</span> 篇</a></td></tr>';
        totalCount += list[i].count;
    }
    html = '<tr class="info thead"><td>共有 <span style="color:#f00">' + totalCount + '</span> 篇内容待审核</td></tr>' + html;
    $("#checkList").html('<div class="popover popover-static"><h3 class="popover-title">待审核内容</h3><div class="popover-content"><table class="table table-bordered table-hover">' + html + '</table></div></div>');
  }
</script>
