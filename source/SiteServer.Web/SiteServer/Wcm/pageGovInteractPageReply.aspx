<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovInteractPageReply" %>
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
  .applyTitle {margin:5px; padding:5px; font-size:18px; font-family:微软雅黑;text-align:center}
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
  </style>
  <script>
  function showAction(divID){
    $('.action').hide();$('#' + divID).show();$('html,body').animate({scrollTop: $('#' + divID).offset().top}, 1000);
  }
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title">待办理办件</h3>
    <div class="popover-content">
    
      <div class="applyTitle"><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></div>
      <table width="98%" class="center" border="0" cellspacing="0" cellpadding="0" style="border-left:1px solid silver;border-top:1px solid silver;">
        <tr>
          <td><table width="100%" border="0" cellspacing="0" cellpadding="0" style="width:100%;height:100%;">
            <tr>
              <td><table class="applyTable" border="0" cellspacing="0" cellpadding="0" style="width:100%;height:100%;table-layout:fixed">
                <tbody>
                  <asp:Literal ID="ltlApplyAttributes" runat="server"></asp:Literal>
                  <tr>
                    <td bgcolor="#f0f6fc" style="width:80px;" class="attribute">提交时间</td>
                    <td class="tdBorder" style="width:300px"><asp:Literal ID="ltlAddDate" runat="server"></asp:Literal></td>
                    <td bgcolor="#f0f6fc" style="width:80px;" class="attribute">查询号</td>
                    <td class="tdBorder"><asp:Literal ID="ltlQueryCode" runat="server"></asp:Literal></td>
                  </tr>
                  <tr>
                    <td bgcolor="#f0f6fc" class="attribute">状态</td>
                    <td class="tdBorder" style="width:300px"><asp:Literal ID="ltlState" runat="server"></asp:Literal></td>
                    <td bgcolor="#f0f6fc" class="attribute">提交部门</td>
                    <td class="tdBorder"><asp:Literal ID="ltlDepartmentName" runat="server"></asp:Literal></td>
                  </tr>
                </tbody>
              </table></td>
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
                  <td class="tdBorder"><asp:Literal ID="ltlReplyAddDate" runat="server"></asp:Literal></td>
                </tr>
              <tr>
                <td align="right" bgcolor="#f0f6fc" class="attribute">回复内容</td>
                <td class="tdBorder"><asp:Literal ID="ltlReply" runat="server"></asp:Literal></td>
                </tr>
              <tr>
                <td align="right" bgcolor="#f0f6fc" class="attribute">附件</td>
                <td class="tdBorder"><asp:Literal ID="ltlReplyFileUrl" runat="server"></asp:Literal></td>
              </tr>
              </table>
          </td>
          </tr>
        </asp:PlaceHolder>
        <tr>
          <td width="80" class="center">操作：</td>
          <td>
            <ul class="breadcrumb breadcrumb-button" style="text-align:left">
              <asp:PlaceHolder ID="phBtnReply" runat="server">
              <input type="button" value="办 理" onClick="showAction('divReply');return false;" class="btn btn-success" />
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="phBtnSwitchToTranslate" runat="server">
              <input type="button" value="转 办" onClick="showAction('divSwitchTo');return false;" class="btn" />
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="phBtnComment" runat="server">
              <input type="button" value="批 示" onClick="showAction('divComment');return false;" class="btn" />
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="phBtnReturn" runat="server">
                <input type="button" value="返 回" onClick="javascript:location.href='<%=ListPageUrl%>';return false;" class="btn" />
              </asp:PlaceHolder>
            </ul>
          </td>
        </tr>
      </table>

      <table id="divReply" class="table table-bordered action" style="display:none">
        <tr class="info thead">
          <td colspan="2"><strong>办理办件</strong></td>
        </tr>
        <tr>
          <td>
            <table class="table noborder">
              <tr>
                <td colspan="2"><div class="tips">办理办件后信息将变为待审核状态</div></td>
              </tr>
              <tr>
                <td class="center" width="120">答复内容：</td>
                <td><asp:TextBox ID="tbReply" runat="server" TextMode="MultiLine" style="width:80%;height:120px;"></asp:TextBox></td>
              </tr>
              <tr>
                <td class="center">答复部门：</td>
                <td><%=MyDepartment%></td>
              </tr>
              <tr>
                <td class="center">答复人：</td>
                <td><%=MyDisplayName%></td>
              </tr>
              <tr>
                <td class="center">附件上传：</td>
                <td><input id="htmlFileUrl" runat="server" type="file" style="width:330px;" /></td>
              </tr>
              <tr>
                <td class="center">&nbsp;</td>
                <td><asp:Button class="btn btn-primary" OnClick="Reply_OnClick" Text="提 交" runat="server"></asp:Button>
                  &nbsp;&nbsp;
                  <input type="button" value="取 消" onClick="$('#divReply').hide();" class="btn" /></td>
              </tr>
            </table>
          </td>
        </tr>
      </table>

      <table id="divSwitchTo" class="table table-bordered action" style="display:none">
        <tr class="info thead">
          <td colspan="2"><strong>转办办件</strong></td>
        </tr>
        <tr>
          <td>
            <table class="table noborder">
              <tr>
                <td colspan="2"><div class="tips">受理办件后信息将变为待办理状态</div></td>
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
          <td colspan="2"><strong>批示办件</strong></td>
        </tr>
        <tr>
          <td>
            <table class="table noborder">
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
