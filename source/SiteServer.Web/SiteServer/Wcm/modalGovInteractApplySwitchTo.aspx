<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.ModalGovInteractApplySwitchTo" Trace="false"%>
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
<bairong:alerts text="转办申请后不会改变申请的状态，转办后将转至对应部门进行处理" runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td class="center" width="120">转办到：</td>
      <td><div class="fill_box" id="switchToDepartmentContainer" style="display:none">
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
      </td>
    </tr>
    <tr>
      <td class="center">意见：</td>
      <td><asp:TextBox ID="tbSwitchToRemark" runat="server" TextMode="MultiLine" Columns="60" rows="4"></asp:TextBox></td>
    </tr>
    <tr>
      <td class="center">操作部门：</td>
      <td><asp:Literal ID="ltlDepartmentName" runat="server"></asp:Literal></td>
    </tr>
    <tr>
      <td class="center">操作人：</td>
      <td><asp:Literal ID="ltlUserName" runat="server"></asp:Literal></td>
    </tr>
  </table>

</form>
</body>
</html>
