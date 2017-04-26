<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageInstaller" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html PUBLIC "-//W3C//Dtd XHTML 1.0 transitional//EN" "http://www.w3.org/tr/xhtml1/Dtd/xhtml1-transitional.dtd">
    <html xmlns="http://www.w3.org/1999/xhtml">

    <head>
      <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
      <meta http-equiv="X-UA-Compatible" content="IE=7" />
      <title>SiteServer 系列产品安装向导</title>
      <link href="style/step.css" rel="stylesheet" type="text/css" />
      <bairong:Code type="JQuery" runat="server" />
      <script src="js/check_data.js"></script>
      <link href="../assets/showLoading/css/showLoading.css" rel="stylesheet" media="screen" />
      <script type="text/javascript" src="../assets/showLoading/js/jquery.showLoading.js"></script>
      <script>
        $(function () {
          $('.byellow').click(function () {
            $('#main').showLoading();
            return true;
          });
        });
      </script>
    </head>

    <body>
      <div class="wrap">
        <div class="top">
          <div class="top-logo"> </div>
          <div class="top-link">
            <UL>
              <LI>
                <A href="http://www.siteserver.cn/" target="_blank">官方网站</A> </LI>
              <LI>
                <A href="http://bbs.siteserver.cn/" target="_blank">技术论坛</A> </LI>
              <LI>
                <A href="http://cms.siteserver.cn/" target="_blank">系统帮助</A> </LI>
            </UL>
          </div>
          <div class="top-version">
            <H2>
              <asp:Literal ID="LtlVersionInfo" runat="server"></asp:Literal>
              安装向导 </H2>
          </div>
        </div>
        <div id="main">
          <div class="box">
            <h2>安装进度</h2>
            <ul class="list_step">
              <asp:Literal ID="LtlStepTitle" runat="server"></asp:Literal>
            </ul>
          </div>
          <div class="box noline">
            <form id="installForm" runat="server">
              <div class="form_detail">
                <div class="error">
                  <asp:Literal ID="LtlErrorMessage" runat="server"></asp:Literal>
                </div>
                <asp:PlaceHolder ID="PhStep1" runat="server">
                  <table cellpadding="0" cellspacing="0" width="660" border="0">
                    <tbody>
                      <tr>
                        <td>
                          <h3>SiteServer 系列产品许可协议</h3>
                        </td>
                        <td nowrap align="right"><img src="../Pic/Installer/printerIcon.gif"> <a href="eula.html" target="new"> 可打印版本</a></td>
                        <tr>
                          <td colspan="2">&nbsp;</td>
                        </tr>
                        <tr>
                          <td valign="top" class="center" colspan="2">
                            <iframe style="border-color:#999999; border-width:1px;" scrolling="yes" src="eula.html" height="264" width="660"></iframe>
                          </td>
                        </tr>
                        <tr>
                          <td colspan="2">&nbsp;</td>
                        </tr>
                        <tr>
                          <td valign="top" align="right" colspan="2"><span class="im">我已经阅读并同意此协议</span>
                            <asp:Checkbox id="ChkIAgree" runat="server" Checked="true" /> &nbsp;
                            <asp:button OnClick="btnStep1_Click" class="btn byellow" Text="继 续" runat="server"></asp:button>
                          </td>
                        </tr>
                    </tbody>
                  </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="PhStep2" runat="server">
                  <div>
                    <div class=pr-title>
                      <h3>服务器信息</h3>
                    </div>
                    <table class=twbox border=0 cellSpacing=0 cellPadding=0 align=center>
                      <tbody>
                        <tr>
                          <th align=middle>
                            <strong>参数</strong>
                          </th>
                          <th>
                            <strong>值</strong>
                          </th>
                        </tr>
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
                    <div class=pr-title>
                      <h3>目录权限检测</h3>
                    </div>
                    <div style="PADDING-BOTTOM: 0px; LINE-HEIGHT: 33px; PADDING-LEFT: 8px; PADDING-RIGHT: 8px; HEIGHT: 23px; COLOR: #666; OVERFLOW: hidden; PADDING-TOP: 2px">系统要求必须满足下列所有的目录权限全部可读写的需求才能使用，如果没有相关权限请添加。 </div>
                    <table class=twbox border=0 cellSpacing=0 cellPadding=0 width=512 align=center>
                      <tbody>
                        <tr>
                          <th width=300 align=middle>
                            <strong>目录名</strong>
                          </th>
                          <th width=212><strong>读写权限</strong></th>
                        </tr>
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

                    <p style="text-align:right; padding-right:50px;">
                      <asp:button OnClick="btnPrevious_Click" class="btn bnormal" Text="后 退" runat="server"></asp:button>
                      <asp:button ID="BtnStep2" OnClick="BtnStep2_Click" class="btn byellow" Text="下一步" runat="server"></asp:button>

                    </p>
                  </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="PhStep3" runat="server">
                  <div class=pr-title>
                    <h3>选择数据库类型</h3>
                  </div>
                  <div style="PADDING-BOTTOM: 10px; LINE-HEIGHT: 33px; PADDING-LEFT: 8px; PADDING-RIGHT: 8px; HEIGHT: 23px; COLOR: #666; OVERFLOW: hidden; PADDING-TOP: 2px">请选择需要安装的数据库类型。 </div>
                  <p>
                    <label>数据库类型：</label>
                    <asp:DropDownList ID="DdlSqlDatabaseType" runat="server"></asp:DropDownList>
                  </p>
                  <asp:PlaceHolder ID="PhSql1" runat="server">
                    <div class=pr-title>
                      <h3>数据库连接设置</h3>
                    </div>
                    <div style="PADDING-BOTTOM: 10px; LINE-HEIGHT: 33px; PADDING-LEFT: 8px; PADDING-RIGHT: 8px; HEIGHT: 23px; COLOR: #666; OVERFLOW: hidden; PADDING-TOP: 2px">在此设置数据库的连接字符串。 </div>
                    <p>
                      <label>数据库主机：</label>
                      <asp:TextBox style="width:285px" class="ipt_tx" ID="TbSqlServer" onblur="checkData(this, 'sqlserver_msg', '数据库主机');" Text=""
                        runat="server" />
                      <span id="sqlserver_msg" class="error" style="display:none"></span> <span class="info">IP地址或者服务器名</span>
                    </p>
                    <p>
                      <label>数据库用户：</label>
                      <asp:TextBox style="width:285px" class="ipt_tx" id="TbSqlUserName" onblur="checkData(this, 'sqlusername_msg', '数据库用户');"
                        runat="server" />
                      <span id="sqlusername_msg" class="error" style="display:none"></span> <span class="info">连接数据库的用户名</span>
                    </p>
                    <p>
                      <label>数据库密码：</label>
                      <asp:TextBox style="width:285px" TextMode="Password" class="ipt_tx" id="TbSqlPassword" onblur="checkData(this, 'sqlpassword_msg', '数据库密码');"
                        runat="server" />
                      <input type="hidden" runat="server" id="HihSqlHiddenPassword" />
                      <span id="sqlpassword_msg" class="error" style="display:none"></span> <span class="info">连接数据库的密码</span>
                    </p>
                  </asp:PlaceHolder>
                  <asp:PlaceHolder ID="PhSql2" runat="server" Visible="false">
                    <p>
                      <label>选择数据库：</label>
                      <asp:DropDownList ID="DdlSqlDatabaseName" runat="server"></asp:DropDownList>
                      <span class="info">选择安装的数据库</span>
                    </p>
                  </asp:PlaceHolder>
                  <p style="text-align:right; padding-right:50px;">
                    <asp:button OnClick="btnPrevious_Click" class="btn bnormal" Text="后 退" runat="server"></asp:button>
                    <asp:button OnClick="btnStep3_Click" class="btn byellow" Text="下一步" runat="server"></asp:button>
                  </p>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="PhStep4" runat="server">
                  <div class=pr-title>
                    <h3>管理员初始密码</h3>
                  </div>
                  <div style="PADDING-BOTTOM: 10px; LINE-HEIGHT: 33px; PADDING-LEFT: 8px; PADDING-RIGHT: 8px; HEIGHT: 23px; COLOR: #666; OVERFLOW: hidden; PADDING-TOP: 2px">在此设置总管理员的登录用户名与密码。</div>
                  <p>
                    <label>管理员用户名：</label>
                    <asp:TextBox style="width:285px" class="ipt_tx" ID="TbAdminName" onblur="checkAdminName();" runat="server" />
                    <span id="adminname_msg" class="error" style="display:none"></span> <span class="info">登录后台使用的用户名</span>
                  </p>
                  <p>
                    <label>管理员密码：</label>
                    <asp:TextBox style="width:285px" TextMode="Password" class="ipt_tx" id="TbAdminPassword" onblur="checkPassword();" runat="server"
                    />
                    <span id="password_msg" class="error" style="display:none"></span> <span class="rank_info">密码强度：
                      <input type="text" id="passwordLevel" class="rank r0" disabled="disabled" />
                    </span>
                  </p>
                  <p>
                    <label>确认密码：</label>
                    <asp:TextBox style="width:285px" TextMode="Password" class="ipt_tx" id="TbComfirmAdminPassword" onblur="checkConfirmPassword();"
                      runat="server" />
                    <span id="confirmPassword_msg" class="error" style="display:none"></span> <span class="info">6-16个字符，支持大小写字母、数字和符号</span>
                  </p>
                  <p>
                    <label>是否加密连接字符串：</label>
                    <asp:DropDownList ID="DdlIsProtectData" runat="server"></asp:DropDownList>
                    <span class="info">设置是否加密Web.Config中的数据库连接字符串</span>
                  </p>
                  <p style="text-align:right; padding-right:50px;">
                    <asp:button OnClick="btnPrevious_Click" class="btn bnormal" Text="后 退" runat="server"></asp:button>
                    <asp:button OnClick="btnStep4_Click" class="btn byellow" Text="下一步" runat="server"></asp:button>
                  </p>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="PhStep5" runat="server">
                  <p class="success" style="background-repeat:no-repeat; padding:15px; padding-left:50px;margin-right:100px;"> 恭喜，您已经完成了 SiteServer 系列产品的安装，并已正常运行，
                    <A href='<%=GetSiteServerUrl()%>'>进入后台</A>。
                  </p>
                </asp:PlaceHolder>
              </div>
            </form>
          </div>
        </div>
      </div>
      <div id="ft">
        <p> 北京百容千域软件技术开发有限公司 版权所有

        </p>
      </div>
      </div>
    </body>

    </html>