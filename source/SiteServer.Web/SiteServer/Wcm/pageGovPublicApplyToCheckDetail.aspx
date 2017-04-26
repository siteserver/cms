<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovPublicApplyToCheckDetail" %>
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
  <bairong:alerts runat="server" />

  <style>
  .applyTitle {margin:5px; padding:5px; font-size:18px; font-family:微软雅黑; text-align: center;}
  .applyTable td { text-align: left; border-bottom: 1px solid silver; border-right: 1px solid silver; }
  .tableBorder td { text-align: left; padding: 2px; margin: 2px; padding-left: 8px; border-bottom: 1px solid silver; border-right: 1px solid silver; }
  .tdBorder, .applyTable td { border-bottom: 1px solid silver; border-right: 1px solid silver; }
  .applyTable td { height: 30px; padding: 2px; margin: 2px; padding-left: 5px; }
  .applyTable .attribute { text-align: right; padding-right: 8px; width: 100px; background: F8F8F8; }
  .applyTable .normalText, .applyTable select { margin-left: 8px; width: 200px; height: 24px; line-height: 20px; border: 1px solid #9AABBB; }
  .applyTable textarea { margin-left: 8px; width: 700px; height: 50px; border: 1px solid #9AABBB; line-height: 20px; }
  table { font-size: 14px; }
  .disableTable .normalText { }
  .requireStyle { margin-left: 4px; color: red; }
  .leftAttribute { width: 30px; text-align: center; color: #333; font-size: 14px; }
  </style>
  <script>
  function showAction(divID){
    $('.action').hide();$('#' + divID).show();$('html,body').animate({scrollTop: $('#' + divID).offset().top}, 1000);
  }
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title">待审核申请</h3>
    <div class="popover-content">
    
      <div class="applyTitle">
        <asp:Literal ID="ltlName" runat="server"></asp:Literal>
        向
        <%=ApplyDepartment%>
        发出依申请公开请求【<asp:Literal ID="ltlState" runat="server"></asp:Literal>】</div>
      <table width="98%" class="center" border=0 cellspacing=0 cellpadding=0 style="border-left:1px solid silver;border-top:1px solid silver;">
        <tr>
          <td class="tdBorder leftAttribute">申<br>
            请<br>
            人<br>
            信<br>
            息</td>
          <td><table width="100%" border=0 cellspacing=0 cellpadding=0 style="width:100%;height:100%;">
              <tr>
                <td><table class="applyTable" border=0 cellspacing=0 cellpadding=0 style="width:100%;height:100%;">
                    <tbody>
                      <tr>
                        <td bgcolor="#f0f6fc" class="attribute">申请人类型</td>
                        <td><asp:Literal ID="ltlType" runat="server"></asp:Literal></td>
                      </tr>
                    </tbody>
                  </table></td>
              </tr>
              <asp:PlaceHolder ID="phCivic" runat="server">
                <tr>
                  <td><table class="applyTable" border=0 cellspacing=0 cellpadding=0 style="width:100%;height:100%;">
                      <tbody>
                        <tr>
                          <td bgcolor="#f0f6fc" class="attribute">姓名</td>
                          <td><asp:Literal ID="ltlCivicName" runat="server"></asp:Literal></td>
                          <td bgcolor="#f0f6fc" class="attribute">工作部门</td>
                          <td><asp:Literal ID="ltlCivicOrganization" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                          <td bgcolor="#f0f6fc" class="attribute">证件名称</td>
                          <td><asp:Literal ID="ltlCivicCardType" runat="server"></asp:Literal></td>
                          <td bgcolor="#f0f6fc" class="attribute">证件号码</td>
                          <td><asp:Literal ID="ltlCivicCardNo" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                          <td bgcolor="#f0f6fc" class="attribute">联系电话</td>
                          <td><asp:Literal ID="ltlCivicPhone" runat="server"></asp:Literal></td>
                          <td bgcolor="#f0f6fc" class="attribute">邮政编码</td>
                          <td><asp:Literal ID="ltlCivicPostCode" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                          <td bgcolor="#f0f6fc" class="attribute">联系地址</td>
                          <td colspan="3"><asp:Literal ID="ltlCivicAddress" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                          <td bgcolor="#f0f6fc" class="attribute">电子邮件</td>
                          <td><asp:Literal ID="ltlCivicEmail" runat="server"></asp:Literal></td>
                          <td bgcolor="#f0f6fc" class="attribute">传真</td>
                          <td><asp:Literal ID="ltlCivicFax" runat="server"></asp:Literal></td>
                        </tr>
                      </tbody>
                    </table></td>
                </tr>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="phOrg" runat="server">
                <tr>
                  <td><table class="applyTable" border=0 cellspacing=0 cellpadding=0 style="width:100%;height:100%;">
                      <tr>
                        <td bgcolor="#f0f6fc" class="attribute">名称</td>
                        <td><asp:Literal ID="ltlOrgName" runat="server"></asp:Literal></td>
                        <td bgcolor="#f0f6fc" class="attribute">组织机构代码</td>
                        <td><asp:Literal ID="ltlOrgUnitCode" runat="server"></asp:Literal></td>
                      </tr>
                      <tr>
                        <td bgcolor="#f0f6fc" class="attribute">法人代表</td>
                        <td><asp:Literal ID="ltlOrgLegalPerson" runat="server"></asp:Literal></td>
                        <td bgcolor="#f0f6fc" class="attribute">联系人姓名</td>
                        <td><asp:Literal ID="ltlOrgLinkName" runat="server"></asp:Literal></td>
                      </tr>
                      <tr>
                        <td bgcolor="#f0f6fc" class="attribute">联系电话</td>
                        <td><asp:Literal ID="ltlOrgPhone" runat="server"></asp:Literal></td>
                        <td bgcolor="#f0f6fc" class="attribute">邮政编码</td>
                        <td><asp:Literal ID="ltlOrgPostCode" runat="server"></asp:Literal></td>
                      </tr>
                      <tr>
                        <td bgcolor="#f0f6fc" class="attribute">联系地址</td>
                        <td colspan="3"><asp:Literal ID="ltlOrgAddress" runat="server"></asp:Literal></td>
                      </tr>
                      <tr>
                        <td bgcolor="#f0f6fc" class="attribute">电子邮件</td>
                        <td><asp:Literal ID="ltlOrgEmail" runat="server"></asp:Literal></td>
                        <td bgcolor="#f0f6fc" class="attribute">传真</td>
                        <td><asp:Literal ID="ltlOrgFax" runat="server"></asp:Literal></td>
                      </tr>
                    </table></td>
                </tr>
              </asp:PlaceHolder>
            </table></td>
        </tr>
      </table>
      <table width="98%" class="center" border=0 cellspacing=0 cellpadding=0 style="border-left:1px solid silver;">
        <tr>
          <td class="tdBorder leftAttribute" rowspan="3">所<br>
            需<br>
            信<br>
            息<br>
            情<br>
            况</td>
          <td><table border=0 cellspacing=0 cellpadding=0 style="width:100%;height:100%;" class="applyTable">
              <tr>
                <td bgcolor="#f0f6fc" class="attribute">提交部门</td>
                <td class="tdBorder"><%=ApplyDepartment%></td>
              </tr>
              <tr>
                <td bgcolor="#f0f6fc" class="attribute">查询号</td>
                <td class="tdBorder"><asp:Literal ID="ltlQueryCode" runat="server"></asp:Literal></td>
              </tr>
              <tr>
                <td bgcolor="#f0f6fc" class="attribute">标题</td>
                <td class="tdBorder"><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></td>
              </tr>
              <tr>
                <td bgcolor="#f0f6fc" class="attribute">所需信息的<br>
                  内容描述</td>
                <td class="tdBorder"><asp:Literal ID="ltlContent" runat="server"></asp:Literal></td>
              </tr>
              <tr>
                <td bgcolor="#f0f6fc" class="attribute">所需信息的<br>
                  用途</td>
                <td class="tdBorder"><asp:Literal ID="ltlPurpose" runat="server"></asp:Literal></td>
              </tr>
            </table></td>
        </tr>
        <tr>
          <td><table border=0 cellspacing=0 cellpadding=0 class="applyTable" style="width:100%;">
              <tr>
                <td bgcolor="#f0f6fc">是否申请减免费用</td>
                <td bgcolor="#f0f6fc">所需信息的指定提供方式</td>
                <td bgcolor="#f0f6fc">获取信息的方式</td>
              </tr>
              <tr>
                <td class="tdBorder"><asp:Literal ID="ltlIsApplyFree" runat="server"></asp:Literal></td>
                <td class="tdBorder"><asp:Literal ID="ltlProvideType" runat="server"></asp:Literal></td>
                <td class="tdBorder"><asp:Literal ID="ltlObtainType" runat="server"></asp:Literal></td>
              </tr>
            </table></td>
        </tr>
      </table>
      <hr />
      <table width="100%" border="0" class="center" cellpadding="8" cellspacing="0">
        <asp:PlaceHolder ID="phRemarks" Visible="false" runat="server">
        <tr>
        <td width="80" class="center">意见：</td>
          <td>
        <table border=0 cellspacing=0 cellpadding=0 class="applyTable" style="width:100%; border:1px solid silver">
          <tr>
            <td bgcolor="#f0f6fc" width="60">类型</td>
            <td bgcolor="#f0f6fc" width="100">日期</td>
            <td bgcolor="#f0f6fc" width="150">人员</td>
            <td bgcolor="#f0f6fc">意见</td>
          </tr>
          <asp:Repeater ID="rptRemarks" runat="server">
            <itemtemplate>
              <tr>
                <td class="tdBorder"><asp:Literal ID="ltlRemarkType" runat="server"></asp:Literal></td>
                <td class="tdBorder"><asp:Literal ID="ltlAddDate" runat="server"></asp:Literal></td>
                <td class="tdBorder"><asp:Literal ID="ltlDepartmentAndUserName" runat="server"></asp:Literal></td>
                <td class="tdBorder"><asp:Literal ID="ltlRemark" runat="server"></asp:Literal></td>
                </tr>
            </itemtemplate>
          </asp:Repeater>
        </table>
          </td>
          </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phReply" Visible="false" runat="server">
          <tr>
          <td width="80" class="center">办理回复：</td>
          <td>
              <table border=0 cellspacing=0 cellpadding=0 class="applyTable" style="width:100%; border:1px solid silver">
              <tr>
                <td width="80" bgcolor="#f0f6fc" class="attribute">办理人员</td>
                <td><asp:Literal ID="ltlDepartmentAndUserName" runat="server"></asp:Literal></td>
                </tr>
              <tr>
                  <td align="right" bgcolor="#f0f6fc" class="attribute">办理日期</td>
                  <td class="tdBorder"><asp:Literal ID="ltlAddDate" runat="server"></asp:Literal></td>
                </tr>
              <tr>
                <td align="right" bgcolor="#f0f6fc" class="attribute">回复内容</td>
                <td class="tdBorder"><asp:Literal ID="ltlReply" runat="server"></asp:Literal></td>
                </tr>
              <tr>
                <td align="right" bgcolor="#f0f6fc" class="attribute">附件</td>
                <td class="tdBorder"><asp:Literal ID="ltlFileUrl" runat="server"></asp:Literal></td>
              </tr>
              </table>
          </td>
          </tr>
        </asp:PlaceHolder>
        <tr>
          <td width="80" class="center">操作：</td>
          <td>
            <ul class="breadcrumb" style="text-align:left">
              <input type="button" value="审 核" onClick="showAction('divCheck');return false;" class="btn btn-success" />
              <input type="button" value="要求返工" onClick="showAction('divRedo');return false;" class="btn" />
              <input type="button" value="批 示" onClick="showAction('divComment');return false;" class="btn" />
              <input type="button" value="返 回" onClick="javascript:location.href='<%=ListPageUrl%>';return false;" class="btn" />
            </ul>
          </td>
        </tr>
      </table>

      <table id="divCheck" class="table table-bordered action" style="display:none">
        <tr class="info thead">
          <td colspan="2"><strong>审核申请</strong></td>
        </tr>
        <tr>
          <td>
            <table class="table table-noborder">
              <tr>
                <td colspan="2"><div class="tips">审核申请后信息将变为已审核状态</div></td>
              </tr>
              <tr>
                <td class="center" width="120">审核部门：</td>
                <td><%=MyDepartment%></td>
              </tr>
              <tr>
                <td class="center">审核人：</td>
                <td><%=MyDisplayName%></td>
              </tr>
              <tr>
                <td class="center">&nbsp;</td>
                <td><asp:Button class="btn btn-primary" OnClick="Check_OnClick" Text="提 交" runat="server"></asp:Button>
                  &nbsp;&nbsp;
                  <input type="button" value="取 消" onClick="$('#divCheck').hide();" class="btn" /></td>
              </tr>
            </table>
          </td>
        </tr>
      </table>

      <table id="divSwitchTo" class="table table-bordered action" style="display:none">
        <tr class="info thead">
          <td colspan="2"><strong>转办申请</strong></td>
        </tr>
        <tr>
          <td>
            <table class="table table-noborder">
              <tr>
                <td colspan="2"><div class="tips">受理申请后信息将变为待办理状态</div></td>
              </tr>
              <td class="center" width="120">转办到：</td>
              <td>
              <div class="fill_box" id="switchToDepartmentContainer" style="display:none">
                  <div class="addr_base addr_normal"> <b id="switchToDepartmentName"></b> <a class="addr_del" href="javascript:;" onClick="showswitchToDepartment('', '0')"></a>
                    <input id="switchToDepartmentID" name="switchToDepartmentID" value="0" type="hidden">
                  </div>
                </div>
                <div ID="divAddDepartment" class="btn_pencil" runat="server"><span class="pencil"></span>　选择</div>
                <script language="javascript">
              function showCategoryDepartment(departmentName, departmentID){
                  $('#switchToDepartmentName').html(departmentName);
                  $('#switchToDepartmentID').val(departmentID);
                  if (departmentID == '0'){
                    $('#switchToDepartmentContainer').hide();
                  }else{
                      $('#switchToDepartmentContainer').show();
                  }
              }
              </script>
              <asp:Literal ID="ltlScript" runat="server"></asp:Literal>
              </td>
            </tr>
              <tr>
                <td class="center">意见：</td>
                <td><asp:TextBox ID="tbSwitchToRemark" runat="server" TextMode="MultiLine" Columns="60" rows="4"></asp:TextBox></td>
              </tr>
              <tr>
                <td class="center">操作部门：</td>
                <td><%=MyDepartment%></td>
              </tr>
              <tr>
                <td class="center">操作人：</td>
                <td><%=MyDisplayName%></td>
              </tr>
              <tr>
                <td class="center">&nbsp;</td>
                <td><asp:Button class="btn btn-primary" OnClick="SwitchTo_OnClick" Text="提 交" runat="server"></asp:Button>
                  &nbsp;&nbsp;
                  <input type="button" value="取 消" onClick="$('#divSwitchTo').hide();" class="btn" /></td>
              </tr>
            </table>
          </td>
        </tr>
      </table>
         
      <table id="divComment" class="table table-bordered action" style="display:none">
        <tr class="info thead">
          <td colspan="2"><strong>批示申请</strong></td>
        </tr>
        <tr>
          <td>
            <table class="table table-noborder">
              <tr>
                <td class="center" width="120">批示意见：</td>
                <td><asp:TextBox ID="tbCommentRemark" runat="server" TextMode="MultiLine" Columns="60" rows="4"></asp:TextBox></td>
              </tr>
              <tr>
                <td class="center">批示部门：</td>
                <td><%=MyDepartment%></td>
              </tr>
              <tr>
                <td class="center">批示人：</td>
                <td><%=MyDisplayName%></td>
              </tr>
              <tr>
                <td class="center">&nbsp;</td>
                <td><asp:Button class="btn btn-primary" OnClick="Comment_OnClick" Text="提 交" runat="server"></asp:Button>
                  &nbsp;&nbsp;
                  <input type="button" value="取 消" onClick="$('#divComment').hide();" class="btn" /></td>
              </tr>
            </table>
          </td>
        </tr>
      </table>
      
      <table id="divRedo" class="table table-bordered action" style="display:none">
        <tr class="info thead">
          <td colspan="2"><strong>要求返工</strong></td>
        </tr>
        <tr>
          <td>
            <table class="table table-noborder">
              <tr>
                <td class="center" width="120">返工意见：</td>
                <td><asp:TextBox ID="tbRedoRemark" runat="server" TextMode="MultiLine" Columns="60" rows="4"></asp:TextBox></td>
              </tr>
              <tr>
                <td class="center">办理部门：</td>
                <td><%=MyDepartment%></td>
              </tr>
              <tr>
                <td class="center">办理人：</td>
                <td><%=MyDisplayName%></td>
              </tr>
              <tr>
                <td class="center">&nbsp;</td>
                <td><asp:Button class="btn btn-primary" OnClick="Redo_OnClick" Text="提 交" runat="server"></asp:Button>
                  &nbsp;&nbsp;
                  <input type="button" value="取 消" onClick="$('#divRedo').hide();" class="btn" /></td>
              </tr>
            </table>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

  <div class="popover popover-static">
    <h3 class="popover-title">流动轨迹（操作日志）</h3>
    <div class="popover-content">
    
      <table class="table table-bordered table-hover">
        <tr class="info thead">
          <td>操作部门</td>
          <td>操作人</td>
          <td width="120">操作时间</td>
          <td>操作内容</td>
        </tr>
        <asp:Repeater ID="rptLogs" runat="server">
          <itemtemplate>
            <tr>
              <td class="center"><asp:Literal ID="ltlDepartment" runat="server"></asp:Literal></td>
              <td class="center"><asp:Literal ID="ltlUserName" runat="server"></asp:Literal></td>
              <td class="center" style="width:120px;"><asp:Literal ID="ltlAddDate" runat="server"></asp:Literal></td>
              <td>
                <asp:Literal ID="ltlSummary" runat="server"></asp:Literal></td>
            </tr>
          </itemtemplate>
        </asp:Repeater>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
