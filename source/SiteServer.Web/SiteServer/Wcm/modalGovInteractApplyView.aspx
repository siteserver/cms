<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.ModalGovInteractApplyView" Trace="false"%>
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
  .RowLabel { width: 30px; text-align: center; color: #333; font-size: 14px; }
  </style>
      
  <div style="margin:5px; padding:5px; font-size:18px; font-family:微软雅黑;text-align:center;"><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></div>
  
  <table width="98%" class="center" border="0" cellspacing="0" cellpadding="0" style="border-left:1px solid silver;border-top:1px solid silver;">
    <tr>
      <td><table width="100%" border="0" cellspacing="0" cellpadding="0" style="width:100%;height:100%;">
        <tr>
          <td><table class="applyTable" border="0" cellspacing="0" cellpadding="0" style="width:100%;height:100%;">
            <tbody>
              <asp:Literal ID="ltlApplyAttributes" runat="server"></asp:Literal>
              <tr>
                <td bgcolor="#f0f6fc" class="attributes">提交时间</td>
                <td class="tdBorder" style="width:300px"><asp:Literal ID="ltlAddDate" runat="server"></asp:Literal></td>
                <td bgcolor="#f0f6fc" class="attributes">查询号</td>
                <td class="tdBorder"><asp:Literal ID="ltlQueryCode" runat="server"></asp:Literal></td>
              </tr>
              <tr>
                <td bgcolor="#f0f6fc" class="attributes">状态</td>
                <td class="tdBorder" style="width:300px"><asp:Literal ID="ltlState" runat="server"></asp:Literal></td>
                <td bgcolor="#f0f6fc" class="attributes">提交部门</td>
                <td class="tdBorder"><asp:Literal ID="ltlDepartmentName" runat="server"></asp:Literal></td>
              </tr>
            </tbody>
          </table></td>
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
              <td class="tdBorder"><asp:Literal ID="ltlReplyAddDate" runat="server"></asp:Literal></td>
            </tr>
          <tr>
            <td align="right" bgcolor="#f0f6fc" class="attributes">回复内容</td>
            <td class="tdBorder"><asp:Literal ID="ltlReply" runat="server"></asp:Literal></td>
            </tr>
          <tr>
            <td align="right" bgcolor="#f0f6fc" class="attributes">附件</td>
            <td class="tdBorder"><asp:Literal ID="ltlReplyFileUrl" runat="server"></asp:Literal></td>
          </tr>
          </table>
      </td>
      </tr>
    </asp:PlaceHolder>
  </table>

</form>
</body>
</html>
