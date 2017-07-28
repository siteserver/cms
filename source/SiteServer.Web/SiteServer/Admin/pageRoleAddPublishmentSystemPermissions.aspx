<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.PageRoleAddPublishmentSystemPermissions" %>
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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts text="本页面设置对应站点的具体权限，确认后需在在角色编辑页面再次点击确认才能最终保存" runat="server" />

  <script type="text/javascript" language="javascript">
  function ChannelPermissions_CheckAll(chk){
    var oEvent = document.getElementById('ChannelPermissions');
    var chks = oEvent.getElementsByTagName("INPUT");
    for(var i=0; i<chks.length; i++)
    {
      if(chks[i].type=="checkbox") chks[i].checked=chk.checked;
    }
  }

  function WebsitePermissions_CheckAll(chk){
    var oEvent = document.getElementById('WebsitePermissions');
    var chks = oEvent.getElementsByTagName("INPUT");
    for(var i=0; i<chks.length; i++)
    {
      if(chks[i].type=="checkbox") chks[i].checked=chk.checked;
    }
  }
  </script>

  <asp:PlaceHolder ID="WebsitePermissionsPlaceHolder" runat="server">
    <div class="popover popover-static">
      <h3 class="popover-title">与站点相关权限设置</h3>
      <div class="popover-content">

        <label class="checkbox"><input type="checkbox" onClick="WebsitePermissions_CheckAll(this)" id="CheckAll2" name="CheckAll2"> 全选</label>
        <asp:CheckBoxList ID="WebsitePermissions" RepeatColumns="7" RepeatDirection="Horizontal" class="checkboxlist" runat="server"></asp:CheckBoxList>

      </div>
    </div>
  </asp:PlaceHolder>

  <asp:PlaceHolder ID="ChannelPermissionsPlaceHolder" runat="server">
    <div class="popover popover-static">
      <h3 class="popover-title">与栏目相关权限设置</h3>
      <div class="popover-content">

        <label class="checkbox"><input type="checkbox" onClick="ChannelPermissions_CheckAll(this)" id="CheckAll1" name="CheckAll1"> 全选</label>
        <asp:CheckBoxList ID="ChannelPermissions" RepeatColumns="7" RepeatDirection="Horizontal" class="checkboxlist" runat="server"></asp:CheckBoxList>

        <div class="tips">注：从下边选择需要管理的栏目，所选栏目下的所有子栏目都属于可管理范围。</div>

        <asp:Literal ID="NodeTree" runat="server"></asp:Literal>

      </div>
    </div>
  </asp:PlaceHolder>

  <hr />
  <table class="table noborder">
    <tr>
      <td class="center">
        <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server"/>
        <asp:Button class="btn" id="Return" text="返 回" onclick="Return_OnClick" runat="server"/>
      </td>
    </tr>
  </table>

</form>
</body>
</html>
