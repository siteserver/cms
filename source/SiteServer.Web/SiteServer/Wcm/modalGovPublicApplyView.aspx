<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.ModalGovPublicApplyView" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <style>
  .applyTable td { text-align: left; border-bottom: 1px solid silver; border-right: 1px solid silver; }
  .tableBorder td { text-align: left; padding: 2px; margin: 2px; padding-left: 8px; border-bottom: 1px solid silver; border-right: 1px solid silver; }
  .tdBorder, .applyTable td { border-bottom: 1px solid silver; border-right: 1px solid silver; }
  .applyTable td { height: 30px; padding: 2px; margin: 2px; padding-left: 5px; }
  .applyTable .attributes { text-align: right; padding-right: 8px; width: 100px; background: F8F8F8; }
  .applyTable .normalText, .applyTable select { margin-left: 8px; width: 200px; height: 24px; line-height: 20px; border: 1px solid #9AABBB; }
  .applyTable textarea { margin-left: 8px; width: 700px; height: 50px; border: 1px solid #9AABBB; line-height: 20px; }
  table { font-size: 14px; }
  .disableTable .normalText { }
  .requireStyle { margin-left: 4px; color: red; }
  .Rowattributes { width: 30px; text-align: center; color: #333; font-size: 14px; }
  </style>

  <div style="margin:5px; padding:5px; font-size:18px; font-family:微软雅黑;text-align:center">
    <asp:Literal ID="ltlName" runat="server"></asp:Literal>
    向
    <%=ApplyDepartment%>
    发出依申请公开请求【<asp:Literal ID="ltlState" runat="server"></asp:Literal>】</div>
  <table width="98%" class="center" border=0 cellspacing=0 cellpadding=0 style="border-left:1px solid silver;border-top:1px solid silver;">
    <tr>
      <td class="tdBorder Rowattributes">申<br>
        请<br>
        人<br>
        信<br>
        息</td>
      <td><table width="100%" border=0 cellspacing=0 cellpadding=0 style="width:100%;height:100%;">
          <tr>
            <td><table class="applyTable" border=0 cellspacing=0 cellpadding=0 style="width:100%;height:100%;">
                <tbody>
                  <tr>
                    <td bgcolor="#f0f6fc" class="attributes">申请人类型</td>
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
                      <td bgcolor="#f0f6fc" class="attributes">姓名</td>
                      <td><asp:Literal ID="ltlCivicName" runat="server"></asp:Literal></td>
                      <td bgcolor="#f0f6fc" class="attributes">工作部门</td>
                      <td><asp:Literal ID="ltlCivicOrganization" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                      <td bgcolor="#f0f6fc" class="attributes">证件名称</td>
                      <td><asp:Literal ID="ltlCivicCardType" runat="server"></asp:Literal></td>
                      <td bgcolor="#f0f6fc" class="attributes">证件号码</td>
                      <td><asp:Literal ID="ltlCivicCardNo" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                      <td bgcolor="#f0f6fc" class="attributes">联系电话</td>
                      <td><asp:Literal ID="ltlCivicPhone" runat="server"></asp:Literal></td>
                      <td bgcolor="#f0f6fc" class="attributes">邮政编码</td>
                      <td><asp:Literal ID="ltlCivicPostCode" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                      <td bgcolor="#f0f6fc" class="attributes">联系地址</td>
                      <td colspan="3"><asp:Literal ID="ltlCivicAddress" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                      <td bgcolor="#f0f6fc" class="attributes">电子邮件</td>
                      <td><asp:Literal ID="ltlCivicEmail" runat="server"></asp:Literal></td>
                      <td bgcolor="#f0f6fc" class="attributes">传真</td>
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
                    <td bgcolor="#f0f6fc" class="attributes">名称</td>
                    <td><asp:Literal ID="ltlOrgName" runat="server"></asp:Literal></td>
                    <td bgcolor="#f0f6fc" class="attributes">组织机构代码</td>
                    <td><asp:Literal ID="ltlOrgUnitCode" runat="server"></asp:Literal></td>
                  </tr>
                  <tr>
                    <td bgcolor="#f0f6fc" class="attributes">法人代表</td>
                    <td><asp:Literal ID="ltlOrgLegalPerson" runat="server"></asp:Literal></td>
                    <td bgcolor="#f0f6fc" class="attributes">联系人姓名</td>
                    <td><asp:Literal ID="ltlOrgLinkName" runat="server"></asp:Literal></td>
                  </tr>
                  <tr>
                    <td bgcolor="#f0f6fc" class="attributes">联系电话</td>
                    <td><asp:Literal ID="ltlOrgPhone" runat="server"></asp:Literal></td>
                    <td bgcolor="#f0f6fc" class="attributes">邮政编码</td>
                    <td><asp:Literal ID="ltlOrgPostCode" runat="server"></asp:Literal></td>
                  </tr>
                  <tr>
                    <td bgcolor="#f0f6fc" class="attributes">联系地址</td>
                    <td colspan="3"><asp:Literal ID="ltlOrgAddress" runat="server"></asp:Literal></td>
                  </tr>
                  <tr>
                    <td bgcolor="#f0f6fc" class="attributes">电子邮件</td>
                    <td><asp:Literal ID="ltlOrgEmail" runat="server"></asp:Literal></td>
                    <td bgcolor="#f0f6fc" class="attributes">传真</td>
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
      <td class="tdBorder Rowattributes" rowspan="3">所<br>
        需<br>
        信<br>
        息<br>
        情<br>
        况</td>
      <td><table border=0 cellspacing=0 cellpadding=0 style="width:100%;height:100%;" class="applyTable">
          <tr>
            <td bgcolor="#f0f6fc" class="attributes">提交部门</td>
            <td class="tdBorder"><%=ApplyDepartment%></td>
          </tr>
          <tr>
            <td bgcolor="#f0f6fc" class="attributes">查询号</td>
            <td class="tdBorder"><asp:Literal ID="ltlQueryCode" runat="server"></asp:Literal></td>
          </tr>
          <tr>
            <td bgcolor="#f0f6fc" class="attributes">标题</td>
            <td class="tdBorder"><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></td>
          </tr>
          <tr>
            <td bgcolor="#f0f6fc" class="attributes">所需信息的<br>
              内容描述</td>
            <td class="tdBorder"><asp:Literal ID="ltlContent" runat="server"></asp:Literal></td>
          </tr>
          <tr>
            <td bgcolor="#f0f6fc" class="attributes">所需信息的<br>
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
  <br />
  <table width="100%" border="0" class="center" cellpadding="8" cellspacing="0">
    <tr valign="top" style="background-color: #f0f6fc">
      <td colspan="2"></td>
    </tr>
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
            <td width="80" bgcolor="#f0f6fc" class="attributes">办理人员</td>
            <td><asp:Literal ID="ltlDepartmentAndUserName" runat="server"></asp:Literal></td>
            </tr>
          <tr>
              <td align="right" bgcolor="#f0f6fc" class="attributes">办理日期</td>
              <td class="tdBorder"><asp:Literal ID="ltlAddDate" runat="server"></asp:Literal></td>
            </tr>
          <tr>
            <td align="right" bgcolor="#f0f6fc" class="attributes">回复内容</td>
            <td class="tdBorder"><asp:Literal ID="ltlReply" runat="server"></asp:Literal></td>
            </tr>
          <tr>
            <td align="right" bgcolor="#f0f6fc" class="attributes">附件</td>
            <td class="tdBorder"><asp:Literal ID="ltlFileUrl" runat="server"></asp:Literal></td>
          </tr>
          </table>
      </td>
      </tr>
    </asp:PlaceHolder>
  </table>

</form>
</body>
</html>
