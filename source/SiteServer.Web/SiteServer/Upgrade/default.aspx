<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageUpgrade" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta http-equiv="X-UA-Compatible" content="IE=7" />
<title>SiteServer 系列产品升级向导</title>
<link href="../installer/style/step.css" rel="stylesheet" type="text/css" />
<script src="../installer/js/check_data.js"></script></head>
<body>
<div class="wrap">
  <DIV class="top">
    <DIV class="top-logo"> </DIV>
    <DIV class="top-link">
      <UL>
        <LI> <A href="http://www.siteserver.cn/" target="_blank">官方网站</A> </LI>
        <LI> <A href="http://bbs.siteserver.cn/" target="_blank">技术论坛</A> </LI>
        <LI> <A href="http://cms.siteserver.cn/" target="_blank">系统帮助</A> </LI>
      </UL>
    </DIV>
    <DIV class="top-version">
      <H2> <asp:Literal ID="LtlVersionInfo" runat="server"></asp:Literal> 升级向导 </H2>
    </DIV>
  </DIV>
  <div id="main">
    <div class="box">
      <h2>升级进度</h2>
      <ul class="list_step">
        <asp:Literal ID="LtlStepTitle" runat="server"></asp:Literal>
      </ul>
    </div>
    <div class="box noline">
      <form runat="server">
        <div class="form_detail">
          <div class="error">
              <asp:Literal ID="LtlErrorMessage" runat="server"></asp:Literal>
            </div>
            <asp:PlaceHolder ID="PhStep1" runat="server">
              <table cellpadding="0" cellspacing="0" width="660" border="0">
                <TBODY>
                  <tr>
                    <td><H3 style="position:absolute; margin-top:0px; top: 205px;">SITESERVER<span style="font-size:6px; position:relative; top:-10px">TM</span> 系列产品许可协议</H3></td>
                    <td nowrap align="right"><img src="../Pic/Installer/printerIcon.gif"> <a href="../installer/eula.html" target="new"> 可打印版本</a></td>
                  <tr>
                    <td colspan="2">&nbsp;</td>
                  </tr>
                  <tr>
                    <td valign="top" class="center" colspan="2"><iframe style="border-color:#999999; border-width:1px;" scrolling="yes" src="../installer/eula.html" height="264" width="660"></iframe></td>
                  </tr>
                  <tr>
                    <td colspan="2">&nbsp;</td>
                  </tr>
                  <tr>
                    <td valign="top" align="right" colspan="2"><span class="im">我已经阅读并同意此协议</span>
                      <asp:Checkbox id="ChkIAgree" runat="server" Checked="true" />
                      &nbsp;
                      <asp:button OnClick="PhStep1_Click" class="btn byellow" tabindex="3" Text="开始升级" runat="server"></asp:button></td>
                  </tr>
                </tbody>
              </table>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="PhStep2" runat="server">
              <DIV id="Upgrade1">
                <DIV class=pr-title>
                  <H3 style="color:#06F">分析软件信息</H3>
                  <DIV>&nbsp;<img src="../assets/icons/waiting.gif" width="220" height="19" align="middle" /></DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3 style="color:#06F">升级系统文件</H3>
                  <DIV>&nbsp;升级系统文件。 </DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3 style="color:#06F">升级平台数据库</H3>
                  <DIV>&nbsp;升级平台数据库。 </DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3 style="color:#06F">升级模块数据库</H3>
                  <DIV>&nbsp;升级模块数据库。 </DIV>
                </DIV>
                </DIV>
                <DIV id="Upgrade2" style="display:none">
                <DIV class=pr-title>
                  <H3>分析软件信息(已完成)</H3>
                  <DIV>&nbsp;分析软件完整性</DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3 style="color:#06F">升级系统文件</H3>
                  <img src="../assets/icons/waiting.gif" width="220" height="19" align="middle" />
                </DIV>
                <DIV class=pr-title>
                  <H3 style="color:#06F">升级平台数据库</H3>
                  <DIV>&nbsp;升级平台数据库。 </DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3 style="color:#06F">升级模块数据库</H3>
                  <DIV>&nbsp;升级模块数据库。 </DIV>
                </DIV>
              </DIV>
              <DIV id="Upgrade3" style="display:none">
                <DIV class=pr-title>
                  <H3>分析软件信息(已完成)</H3>
                  <DIV>&nbsp;分析软件完整性</DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3>升级系统文件(已完成)</H3>
                  <DIV>&nbsp;升级系统文件。 </DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3 style="color:#06F">升级平台数据库</H3>
                  <DIV>&nbsp;<img src="../assets/icons/waiting.gif" width="220" height="19" align="middle" /></DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3 style="color:#06F">升级模块数据库</H3>
                  <DIV>&nbsp;升级模块数据库。 </DIV>
                </DIV>
              </DIV>
              <DIV id="Upgrade4" style="display:none">
                <DIV class=pr-title>
                  <H3>分析软件信息(已完成)</H3>
                  <DIV>&nbsp;分析软件完整性</DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3>升级系统文件(已完成)</H3>
                  <DIV>&nbsp;升级系统文件。 </DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3>升级平台数据库(已完成)</H3>
                  <DIV>&nbsp;升级平台数据库。 </DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3 style="color:#06F">升级模块数据库</H3>
                  <DIV>&nbsp;<img src="../assets/icons/waiting.gif" width="220" height="19" align="middle" /></DIV>
                </DIV>
              </DIV>
              <DIV id="Upgrade5" style="display:none">
                <DIV class=pr-title>
                  <H3>分析软件信息(已完成)</H3>
                  <DIV>&nbsp;分析软件完整性</DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3>升级系统文件(已完成)</H3>
                  <DIV>&nbsp;升级系统文件。 </DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3>升级平台数据库(已完成)</H3>
                  <DIV>&nbsp;升级平台数据库。 </DIV>
                </DIV>
                <DIV class=pr-title>
                  <H3>升级模块数据库(已完成)</H3>
                  <DIV>&nbsp;升级模块数据库。 </DIV>
                </DIV>
              </DIV>
              <script>
			  var k = 1;
			  var timeout;
			  function upgrade(){
				  for(j=1;j<=5;j++){
					  document.getElementById("Upgrade" + j).style.display = "none";
				  }
				  k = k + 1;
				  document.getElementById("Upgrade" + k).style.display = "";
				  if (k == 5){
					window.clearInterval(timeout);
					location.href="default.aspx?done=true";
				  }
			  }
			  timeout = window.setInterval("upgrade()", 2000);
			  </script>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="PhStep3" runat="server">
            <p class="success" style="background-repeat:no-repeat; padding:15px; padding-left:50px;margin-right:100px;">
              <span style="position:absolute; margin-top:0px;">恭喜，您已经完成了 SITESERVER<span style="font-size:6px; position:relative; top:-10px">TM</span>&nbsp;&nbsp;系列产品的升级，<A href='<%=GetSiteServerUrl()%>'>进入后台</A>。</span>
            </p>
          </asp:PlaceHolder>
        </div>
      </form>
    </div>
  </div>
</div>
<div id="ft">
  <p> 北京百容千域软件技术开发有限公司 版权所有 </p>
</div>
</div>
</body>
</html>
