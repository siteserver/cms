<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageUtilityAccessTokensAdd" %>
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
              <a class="nav-link" href="pageUtilityCache.aspx">系统缓存</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUtilityParameter.aspx">系统参数查看</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageUtilityAccessTokens.aspx">API密钥</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUtilityEncrypt.aspx">加密字符串</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUtilityJsMin.aspx">JS脚本压缩</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageUtilityDbLogDelete.aspx">清空数据库日志</a>
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
            <label class="col-form-label">授权范围
              <small class="form-text text-muted">
                授权范围定义API密钥的访问权限
                <a href="https://docs.siteserver.cn/api/" target="_blank">阅读更多</a>
              </small>
            </label>

            <div class="m-5">

              <div class="row">
                <div class="col-12">
                  <asp:CheckBoxList ID="CblScopes" CssClass="checkbox checkbox-primary" RepeatDirection="Horizontal" runat="server" />
                </div>
              </div>

            </div>

          </div>

          <hr />

          <asp:Button class="btn btn-primary" id="BtnSubmit" text="确 定" OnClick="Submit_OnClick" runat="server" />
          <asp:Button class="btn" text="返 回" CausesValidation="false" onclick="Return_OnClick" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->