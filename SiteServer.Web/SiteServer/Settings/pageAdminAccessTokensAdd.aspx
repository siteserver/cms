<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAdminAccessTokensAdd" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">

        <div class="card-box">
          <ul class="nav nav-pills">
            <li class="nav-item">
              <a class="nav-link" href="pageAdministrator.aspx">管理员管理</a>
            </li>
            <li class="nav-item">
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
            <li class="nav-item active">
              <a class="nav-link" href="pageAdminAccessTokens.cshtml">API密钥管理</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            <asp:Literal id="LtlPageTitle" runat="server" />
          </div>

          <div class="form-group">
            <label class="col-form-label">
              名称
              <asp:RequiredFieldValidator ControlToValidate="TbTitle" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTitle" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" Display="Dynamic" />
            </label>
            <asp:TextBox cssClass="form-control" id="TbTitle" runat="server" />
          </div>

          <div class="form-group">
            <label class="col-form-label">
              关联管理员
            </label>
            <asp:DropDownList ID="DdlAdministrators" CssClass="form-control" runat="server" />
            <small class="form-text text-muted">
              关联管理员定义API密钥的访问权限，API密钥的访问权限将被限制在此管理员的权限范围内
              <a href="https://docs.siteserver.cn/api/" target="_blank">阅读更多</a>
            </small>
          </div>

          <div class="form-group">
            <label class="col-form-label">
              授权范围
            </label>
            <asp:CheckBoxList ID="CblScopes" CssClass="checkbox checkbox-primary" RepeatDirection="Horizontal" runat="server" />
            <small class="form-text text-muted">
              授权范围定义API密钥可访问的API地址，API密钥能够访问的API地址将被限制在授权范围内
              <a href="https://docs.siteserver.cn/api/" target="_blank">阅读更多</a>
            </small>

          </div>

          <hr />

          <asp:Button class="btn btn-primary" id="BtnSubmit" text="确 定" OnClick="Submit_OnClick" runat="server" />
          <asp:Button class="btn" text="返 回" CausesValidation="false" onclick="Return_OnClick" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->