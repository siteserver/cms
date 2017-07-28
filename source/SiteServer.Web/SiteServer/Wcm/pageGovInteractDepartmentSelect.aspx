<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovInteractDepartmentSelect" %>
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
  <bairong:alerts text="请从下边选择负责部门，所选部门下的所有分类都属于负责范围。" runat="server" />

  <script type="text/javascript" language="javascript">
  function DepartmentIDCollection_CheckAll(chk){
    var oEvent = document.getElementById('Departments');
    var chks = oEvent.getElementsByTagName("INPUT");
    for(var i=0; i<chks.length; i++)
    {
      if(chks[i].type=="checkbox") chks[i].checked=chk.checked;
    }
  }
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title">负责部门设置</h3>
    <div class="popover-content">
    
      <table class="table noborder">
        <tr>
          <td>
            <label class="checkbox inline"><input type="checkbox" onClick="DepartmentIDCollection_CheckAll(this)">全选</label></td>
        </tr>
        <tr>
          <td id="Departments"><asp:Literal ID="ltlDepartmentTree" runat="server"></asp:Literal></td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server"/>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
