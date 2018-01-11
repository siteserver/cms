<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageConfig" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            插件设置
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="form-group">
            <label class="col-form-label">插件</label>
            <div class="form-control-plaintext">
              <asp:Literal id="LtlPlugin" runat="server" />
            </div>
          </div>

          <div class="form-group">
            <label class="col-form-label">数据库连接</label>
            <asp:DropDownList ID="DdlIsDefault" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlIsDefault_SelectedIndexChanged"
              runat="server"></asp:DropDownList>
          </div>

          <asp:PlaceHolder id="PhCustom" runat="server">
            <div class="form-group">
              <label class="col-form-label">数据库类型</label>
              <asp:DropDownList ID="DdlSqlDatabaseType" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlSqlDatabaseType_SelectedIndexChanged"
                runat="server"></asp:DropDownList>
            </div>

            <asp:PlaceHolder ID="PhSql1" runat="server">
              <div class="form-group">
                <label class="col-form-label">
                  数据库主机
                  <asp:RequiredFieldValidator runat="server" ControlToValidate="TbSqlServer" ErrorMessage="数据库主机为必填项。" foreColor="red" ToolTip="数据库主机为必填项。"
                    Display="Dynamic"></asp:RequiredFieldValidator>
                </label>
                <asp:TextBox ID="TbSqlServer" class="form-control" runat="server" />
                <small class="form-text text-muted">IP地址或者服务器名</small>
              </div>

              <div class="form-group">
                <label class="col-form-label">
                  身份验证
                </label>
                <asp:DropDownList ID="DdlIsTrustedConnection" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlIsTrustedConnection_SelectedIndexChanged"
                  runat="server"></asp:DropDownList>
              </div>

              <asp:PlaceHolder id="PhSqlUserNamePassword" runat="server">
                <div class="form-group">
                  <label class="col-form-label">
                    数据库用户
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TbSqlUserName" ErrorMessage="数据库用户为必填项。" foreColor="red" ToolTip="数据库用户为必填项。"
                      Display="Dynamic"></asp:RequiredFieldValidator>
                  </label>
                  <asp:TextBox ID="TbSqlUserName" class="form-control" runat="server" />
                  <small class="form-text text-muted">连接数据库的用户名</small>
                </div>

                <div class="form-group">
                  <label class="col-form-label">
                    数据库密码
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TbSqlPassword" ErrorMessage="数据库密码为必填项。" foreColor="red" ToolTip="数据库密码为必填项。"
                      Display="Dynamic"></asp:RequiredFieldValidator>
                    <input type="hidden" runat="server" id="HihSqlHiddenPassword" />
                  </label>
                  <asp:TextBox ID="TbSqlPassword" class="form-control" TextMode="Password" runat="server" />
                  <small class="form-text text-muted">连接数据库的密码</small>
                </div>
              </asp:PlaceHolder>

              <div class="form-group">
                <asp:Button id="BtnConnect" class="btn btn-succss" OnClick="Connect_Click" runat="server" Text="连接数据库" />
                <small class="form-text text-muted">点击按钮获取可用的数据库</small>
              </div>

            </asp:PlaceHolder>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhSql2" runat="server" Visible="false">
            <hr />

            <div class="form-group">
              <label class="col-form-label">
                选择数据库
              </label>
              <asp:DropDownList ID="DdlSqlDatabaseName" class="form-control" runat="server"></asp:DropDownList>
            </div>
          </asp:PlaceHolder>

          <hr />

          <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />
          <asp:Button class="btn" text="返 回" onclick="Return_OnClick" CausesValidation="false" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->