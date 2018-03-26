<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageInstaller" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <title>SiteServer CMS 安装向导</title>
      <!--#include file="../inc/head.html"-->
      <link href="../assets/showLoading/css/showLoading.css" rel="stylesheet" />
      <script type="text/javascript" src="../assets/showLoading/js/jquery.showLoading.js"></script>
      <link href="password.css" rel="stylesheet" />
      <script type="text/javascript" src="password.js"></script>
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">

        <div id="main" class="card-box">
          <h4 class="text-dark  header-title m-t-0">
            SiteServer CMS
            <asp:Literal ID="LtlVersionInfo" runat="server"></asp:Literal> 安装向导
          </h4>
          <p class="text-muted m-b-25 font-13">
            欢迎来到SiteServer CMS 安装向导！只要进行以下几步操作，你就可以开始使用强大且可扩展的CMS系统了。
          </p>

          <ctrl:alerts runat="server" />

          <!-- step 1 place -->
          <asp:PlaceHolder ID="PhStep1" runat="server">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">许可协议</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">环境检测</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">数据库设置</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">安装产品</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">安装完成</a>
              </li>
            </ul>

            <div class="form-group">
              <label class="col-form-label">
                SiteServer CMS 开源协议（GPL-3.0）
                <img src="../Pic/Installer/printerIcon.gif">
                <a href="eula.html" target="new"> 可打印版本</a>
              </label>
            </div>

            <iframe style="border-color:#F5F5F5; border-width:1px;" scrolling="yes" src="eula.html" height="320" width="100%"></iframe>

            <hr />

            <div class="text-center">
              <asp:Checkbox id="ChkIAgree" class="checkbox checkbox-primary" runat="server" Text="我已经阅读并同意此协议" Checked="true" />
              <asp:Button OnClick="BtnStep1_Click" class="btn btn-primary" Text="下一步" runat="server"></asp:Button>
            </div>

          </asp:PlaceHolder>

          <!-- step 2 place -->
          <asp:PlaceHolder ID="PhStep2" runat="server">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">许可协议</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">环境检测</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">数据库设置</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">安装产品</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">安装完成</a>
              </li>
            </ul>

            <div class="form-group">
              <label class="col-form-label">
                服务器信息
              </label>
              <small class="form-text text-muted">下表显示当前服务器环境</small>
            </div>

            <div class="panel panel-default">
              <div class="panel-body p-0">
                <div class="table-responsive">
                  <table class="tablesaw table table-hover m-b-0 tablesaw-stack">
                    <thead>
                      <tr>
                        <th>参数</th>
                        <th>值</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr>
                        <td>服务器域名</td>
                        <td>
                          <asp:Literal ID="LtlDomain" runat="server"></asp:Literal>
                        </td>
                      </tr>
                      <tr>
                        <td>SiteServer 版本</td>
                        <td>
                          <asp:Literal ID="LtlVersion" runat="server"></asp:Literal>
                        </td>
                      </tr>
                      <tr>
                        <td>.NET版本</td>
                        <td>
                          <asp:Literal ID="LtlNetVersion" runat="server"></asp:Literal>
                        </td>
                      </tr>
                      <tr>
                        <td>系统根目录</td>
                        <td>
                          <asp:Literal ID="LtlPhysicalApplicationPath" runat="server"></asp:Literal>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <div class="form-group">
              <label class="col-form-label">
                目录权限检测
              </label>
              <small class="form-text text-muted">
                系统要求必须满足下列所有的目录权限全部可读写的需求才能使用，如果没有相关权限请添加。
              </small>
            </div>

            <div class="panel panel-default">
              <div class="panel-body p-0">
                <div class="table-responsive">
                  <table class="tablesaw table table-hover m-b-0 tablesaw-stack">
                    <thead>
                      <tr>
                        <th>目录名</th>
                        <th>读写权限</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr>
                        <td>/*</td>
                        <td>
                          <asp:Literal ID="LtlRootWrite" runat="server"></asp:Literal>
                        </td>
                      </tr>
                      <tr>
                        <td>/SiteFiles/*</td>
                        <td>
                          <asp:Literal ID="LtlSiteFielsWrite" runat="server"></asp:Literal>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <hr />

            <div class="text-center">
              <asp:Button OnClick="BtnPrevious_Click" CausesValidation="false" class="btn" Text="上一步" runat="server"></asp:Button>
              <asp:Button ID="BtnStep2" OnClick="BtnStep2_Click" class="btn btn-primary ml-2" Text="下一步" runat="server"></asp:Button>
            </div>

          </asp:PlaceHolder>

          <!-- step 3 place -->
          <asp:PlaceHolder ID="PhStep3" runat="server">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">许可协议</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">环境检测</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">数据库设置</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">安装产品</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">安装完成</a>
              </li>
            </ul>

            <div class="form-group">
              <label class="col-form-label">
                数据库类型
              </label>
              <asp:DropDownList ID="DdlSqlDatabaseType" cssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlSqlDatabaseType_SelectedIndexChanged"
                runat="server"></asp:DropDownList>
              <small class="form-text text-muted">
                请选择需要安装的数据库类型。
              </small>
            </div>

            <asp:PlaceHolder ID="PhSql1" runat="server">
              <div class="form-group">
                <label class="col-form-label">
                  数据库主机
                  <asp:RequiredFieldValidator ControlToValidate="TbSqlServer" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
                  />
                </label>
                <asp:TextBox ID="TbSqlServer" class="form-control" runat="server" />
                <small class="form-text text-muted">
                  IP地址或者服务器名
                </small>
              </div>
              <div class="form-group">
                <label class="col-form-label">
                  数据库端口
                  <asp:RequiredFieldValidator ControlToValidate="TbSqlServer" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
                  />
                </label>
                <asp:DropDownList ID="DdlIsDefaultPort" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlIsDefaultPort_SelectedIndexChanged"
                  runat="server"></asp:DropDownList>
              </div>

              <asp:PlaceHolder id="PhSqlPort" runat="server">
                <div class="form-group">
                  <label class="col-form-label">
                    自定义端口
                    <asp:RequiredFieldValidator ControlToValidate="TbSqlPort" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
                    />
                  </label>
                  <asp:TextBox ID="TbSqlPort" class="form-control" runat="server" />
                  <small class="form-text text-muted">
                    连接数据库的端口
                  </small>
                </div>
              </asp:PlaceHolder>

              <div class="form-group">
                <label class="col-form-label">
                  数据库用户名
                  <asp:RequiredFieldValidator ControlToValidate="TbSqlUserName" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
                  />
                </label>
                <asp:TextBox ID="TbSqlUserName" class="form-control" runat="server" />
                <small class="form-text text-muted">
                  连接数据库的用户名
                </small>
              </div>
              <div class="form-group">
                <label class="col-form-label">
                  数据库密码
                  <asp:RequiredFieldValidator ControlToValidate="TbSqlPassword" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
                  />
                  <input type="hidden" runat="server" id="HihSqlHiddenPassword" />
                </label>
                <asp:TextBox ID="TbSqlPassword" TextMode="Password" class="form-control" runat="server" />
                <small class="form-text text-muted">
                  连接数据库的密码
                </small>
              </div>

              <asp:PlaceHolder id="PhSqlOracleDatabase" visible="false" runat="server">
                <div class="form-group">
                  <label class="col-form-label">
                    数据库名称
                    <asp:RequiredFieldValidator ControlToValidate="TbSqlOracleDatabase" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
                    />
                  </label>
                  <asp:TextBox ID="TbSqlOracleDatabase" class="form-control" runat="server" />
                  <small class="form-text text-muted">
                    指定需要安装的Oracle数据库名称
                  </small>
                </div>
              </asp:PlaceHolder>

            </asp:PlaceHolder>

            <asp:PlaceHolder ID="PhSql2" runat="server" Visible="false">

              <div class="form-group">
                <label class="col-form-label">
                  选择数据库
                </label>
                <asp:DropDownList ID="DdlSqlDatabaseName" class="form-control" runat="server"></asp:DropDownList>
                <small class="form-text text-muted">
                  选择安装的数据库
                </small>
              </div>

            </asp:PlaceHolder>

            <hr />

            <div class="text-center">
              <asp:Button OnClick="BtnPrevious_Click" CausesValidation="false" class="btn" Text="上一步" runat="server"></asp:Button>
              <asp:Button OnClick="BtnStep3_Click" class="btn btn-primary ml-2" Text="下一步" runat="server"></asp:Button>
            </div>

          </asp:PlaceHolder>

          <!-- step 4 place -->
          <asp:PlaceHolder ID="PhStep4" runat="server">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">许可协议</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">环境检测</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">数据库设置</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">安装产品</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">安装完成</a>
              </li>
            </ul>

            <div class="form-group">
              <label class="col-form-label">
                总管理员用户名
                <asp:RequiredFieldValidator ControlToValidate="TbAdminName" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
                />
              </label>
              <asp:TextBox class="form-control" ID="TbAdminName" runat="server" />
              <small class="form-text text-muted">
                在此设置总管理员的登录用户名
              </small>
            </div>

            <div class="form-group">
              <label class="col-form-label">
                总管理员密码
                <asp:RequiredFieldValidator ControlToValidate="TbAdminPassword" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
                />
              </label>
              <asp:TextBox id="TbAdminPassword" TextMode="Password" onblur="checkPassword();" class="form-control" runat="server" />
              <small class="form-text text-muted">
                密码强度：
                <input type="text" id="passwordLevel" class="rank r0" disabled="disabled" />
              </small>
            </div>
            <div class="form-group">
              <label class="col-form-label">
                确认密码
                <asp:RequiredFieldValidator ControlToValidate="TbComfirmAdminPassword" errorMessage=" *" foreColor="red" Display="Dynamic"
                  runat="server" />
                <asp:CompareValidator runat="server" ControlToCompare="TbAdminPassword" ControlToValidate="TbComfirmAdminPassword" Display="Dynamic"
                  ForeColor="red" ErrorMessage=" 两次输入的新密码不一致！请再输入一遍您上面填写的新密码。"></asp:CompareValidator>
              </label>
              <asp:TextBox id="TbComfirmAdminPassword" TextMode="Password" class="form-control" runat="server" />
              <small class="form-text text-muted">
                6-16个字符，支持大小写字母、数字和符号
              </small>
            </div>
            <div class="form-group">
              <label class="col-form-label">
                是否加密连接字符串
              </label>
              <asp:DropDownList ID="DdlIsProtectData" class="form-control" runat="server"></asp:DropDownList>
              <small class="form-text text-muted">
                设置是否加密Web.Config中的数据库连接字符串
              </small>
            </div>

            <hr />

            <div class="text-center">
              <asp:Button OnClick="BtnPrevious_Click" CausesValidation="false" class="btn" Text="上一步" runat="server"></asp:Button>
              <asp:Button OnClick="BtnStep4_Click" class="btn btn-primary ml-2" Text="下一步" runat="server"></asp:Button>
            </div>

          </asp:PlaceHolder>

          <!-- step 5 place -->
          <asp:PlaceHolder ID="PhStep5" runat="server">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">许可协议</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">环境检测</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">数据库设置</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">安装产品</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">安装完成</a>
              </li>
            </ul>

            <div class="alert alert-success" role="alert">
              <h4 class="alert-heading">安装完成！</h4>
              <p>
                恭喜，您已经完成了 SiteServer CMS 的安装
                <asp:Literal id="LtlGo" runat="server" />
              </p>
              <hr>
              <p class="mb-0">
                获取更多使用帮助请访问
                <a href="http://docs.siteserver.cn" target="_blank">SiteServer CMS 文档中心</a>
              </p>
            </div>

          </asp:PlaceHolder>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->
    <script>
      var validate = window.Page_ClientValidate;
      $(function () {
        $('.btn-primary').click(function () {
          if (!validate || validate()) {
            $('#main').showLoading();
          }
          return true;
        });
      });
    </script>