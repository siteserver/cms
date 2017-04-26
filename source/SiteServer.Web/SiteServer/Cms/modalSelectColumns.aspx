<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalSelectColumns" Trace="false"%>
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

  <script type="text/javascript" language="javascript">
  function checkAll(layer, bcheck)
  {
    for(var i=0; i<layer.children.length; i++)
    {
      if (layer.children[i].children.length>0)
      {
        checkAll(layer.children[i],bcheck);
      }else{
        if (layer.children[i].type=="checkbox")
        {
            layer.children[i].checked = bcheck;
        }
      }
    }
  }
  </script>

  <table width="100%">
    <tr>
      <td>
        需要显示的项：<input type="checkbox" id="check_groups" onClick=checkAll(document.getElementById("Group"),this.checked);><label for="check_groups">全选</label>
        <span id="Group">
          <asp:CheckBoxList ID="DisplayAttributeCheckBoxList" RepeatColumns="3" RepeatDirection="Horizontal" class="table table-noborder table-hover" Width="100%" runat="server"/>
        </span>
      </td>
    </tr>
  </table>

</form>
</body>
</html>
