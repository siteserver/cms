<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAdminRoleAdd" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
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

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">角色名称
          <asp:RequiredFieldValidator ControlToValidate="TbRoleName" ErrorMessage=" *" ForeColor="red" Display="Dynamic"
            runat="server" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbRoleName" ValidationExpression="[^',]+"
            ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
        </label>
        <asp:TextBox ID="TbRoleName" cssClass="form-control" runat="server" />
        <small class="form-text text-muted">唯一标识此角色的字符串</small>
      </div>

      <div class="form-group">
        <label class="col-form-label">备注
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDescription" ValidationExpression="[^']+"
            ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
        </label>
        <asp:TextBox class="form-control" TextMode="MultiLine" ID="TbDescription" runat="server" />
      </div>

      <div class="form-group">
        <label class="col-form-label">非站点权限设置</label>
        <div class="m-5">
          <asp:CheckBoxList class="checkbox checkbox-primary" RepeatDirection="Horizontal" RepeatColumns="3" ID="CblPermissions"
            runat="server" />
        </div>
      </div>

      <asp:PlaceHolder ID="PhSitePermissions" runat="server">

        <div class="form-group">
          <label class="col-form-label">站点权限设置
            <small class="form-text text-muted">
              点击网站进入站点权限设置界面
              <span class="bg-light" style="padding: 5px;">无权管理此网站</span>
              <span class="bg-primary" style="color: #fff; padding: 5px;">有权管理此网站</span>
            </small>
          </label>

          <div class="m-5">
            <asp:Literal ID="LtlSites" runat="server"></asp:Literal>

            <div class="clearfix"></div>
          </div>

        </div>

      </asp:PlaceHolder>

      <hr />

      <asp:Button class="btn btn-primary" Text="确 定" OnClick="Submit_OnClick" runat="server" />
      <asp:Button class="btn" ID="BtnReturn" Text="返 回" OnClick="Return_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>

<script language="javascript" type="text/javascript">
  function loadEvent() {

    var els = document.getElementsByTagName("input");
    var inputs = new Array(els.length + 1);
    for (var i = 0; i < els.length; i++) {
      inputs[i] = els[i];
    }

    inputs[els.length] = document.getElementById("TbDescription");

    for (var i = 0; i < inputs.length; i++) {
      inputs[i].onchange = function () {
        var ss_role = _getCookie("pageRoleAdd");
        var theV = (this.type == "checkbox") ? this.checked : this.value;
        if (!ss_role) {
          ss_role = this.id + ":" + theV;
        } else {
          ss_role += "," + this.id + ":" + theV;
        }
        _setCookie("pageRoleAdd", ss_role);
      };
    }

    <%=StrCookie %>

  }
  loadEvent();
</script>
<!--#include file="../inc/foot.html"-->