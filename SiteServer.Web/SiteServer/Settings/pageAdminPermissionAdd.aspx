<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAdminPermissionAdd" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
  <script type="text/javascript" language="javascript">
    function ChannelPermissions_CheckAll(chk) {
      var oEvent = document.getElementById('CblChannelPermissions');
      var chks = oEvent.getElementsByTagName("INPUT");
      for (var i = 0; i < chks.length; i++) {
        if (chks[i].type == "checkbox") chks[i].checked = chk.checked;
      }
    }

    function WebsitePermissions_CheckAll(eleId, chk) {
      var oEvent = document.getElementById(eleId);
      var chks = oEvent.getElementsByTagName("INPUT");
      for (var i = 0; i < chks.length; i++) {
        if (chks[i].type == "checkbox") chks[i].checked = chk.checked;
      }
    }
  </script>
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">

    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="pageAdministrator.aspx">管理员管理</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="pageAdminRole.aspx">角色管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminConfiguration.aspx">管理员设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminDepartment.aspx">所属部门管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAdminArea.aspx">所在区域管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="adminAccessTokens.cshtml">API密钥管理</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts text="本页面设置对应站点的具体权限，确认后需在在角色编辑页面再次点击确认才能最终保存" runat="server" />

    <div class="card-box">
      <asp:PlaceHolder ID="PhWebsiteSysPermissions" runat="server">
        <div class="form-group">
          <label class="col-form-label">
            站点权限设置
            <span class="checkbox checkbox-primary">
              <input type="checkbox" onClick="WebsitePermissions_CheckAll('CblWebsiteSysPermissions', this)" id="CheckSysPermissions"
                name="CheckSysPermissions">
              <label for="CheckSysPermissions">全选</label>
            </span>
          </label>
          <div class="m-3">
            <asp:CheckBoxList ID="CblWebsiteSysPermissions" RepeatColumns="7" RepeatDirection="Horizontal" class="checkbox checkbox-primary"
              runat="server"></asp:CheckBoxList>
          </div>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder ID="PhWebsitePluginPermissions" runat="server">

        <hr />

        <div class="form-group">
          <label class="col-form-label">
            插件权限设置
            <span class="checkbox checkbox-primary">
              <input type="checkbox" onClick="WebsitePermissions_CheckAll('CblWebsitePluginPermissions', this)" id="CheckPluginPermissions"
                name="CheckPluginPermissions">
              <label for="CheckPluginPermissions">全选</label>
            </span>
          </label>
          <div class="m-3">
            <asp:CheckBoxList ID="CblWebsitePluginPermissions" RepeatColumns="7" RepeatDirection="Horizontal" class="checkbox checkbox-primary"
              runat="server"></asp:CheckBoxList>
          </div>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder ID="PhChannelPermissions" runat="server">

        <hr />

        <div class="form-group">
          <label class="col-form-label">
            栏目权限设置
            <span class="checkbox checkbox-primary">
              <input type="checkbox" onClick="ChannelPermissions_CheckAll(this)" id="CheckChannels" name="CheckChannels">
              <label for="CheckChannels">全选</label>
            </span>
          </label>

          <div class="m-3">
            <asp:CheckBoxList ID="CblChannelPermissions" RepeatColumns="7" RepeatDirection="Horizontal" class="checkbox checkbox-primary"
              runat="server"></asp:CheckBoxList>
          </div>

          <small class="form-text text-muted">
            从下边选择需要管理的栏目，所选栏目下的所有子栏目都属于可管理范围
          </small>

          <div class="m-3">
            <asp:Literal ID="LtlNodeTree" runat="server"></asp:Literal>
          </div>

        </div>

      </asp:PlaceHolder>

      <hr />

      <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />
      <asp:Button class="btn" id="Return" text="返 回" onclick="Return_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->