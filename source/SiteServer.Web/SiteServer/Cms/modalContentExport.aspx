<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalContentExport" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" enctype="multipart/form-data" method="post" runat="server">
<%--<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />--%>
<bairong:alerts text="导出压缩包能够将内容以及内容相关的图片、附件等文件一道导出，导出Access或Excel则仅能导出数据。" runat="server"></bairong:alerts>

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
  $(document).ready(function(){
    $('#ddlPeriods').click(function(){
      if($('#ddlPeriods').val()=='-1'){
        $('#periods').show();
      }
      else{
        $('#periods').hide();
      }
    });

    if($('#ddlPeriods').val()=='-1'){
      $('#periods').show();
    }
    else{
      $('#periods').hide();
    }
  });
  </script>

  <table class="table table-noborder table-hover">
    <tr>
      <td width="100">导出类型：</td>
      <td>
        <asp:RadioButtonList ID="rblExportType" AutoPostBack="true" OnSelectedIndexChanged="rblExportType_SelectedIndexChanged" runat="server" RepeatDirection="Horizontal" class="noborder">
          <asp:ListItem Text="导出压缩包" Value="ContentZip"></asp:ListItem>
          <asp:ListItem Text="导出Access" Value="ContentAccess"></asp:ListItem>
          <asp:ListItem Text="导出Excel" Value="ContentExcel" Selected="true"></asp:ListItem>
        </asp:RadioButtonList>
      </td>
    </tr>
    <tr>
      <td>时间段选择：</td>
      <td>
        <asp:DropDownList id="ddlPeriods" class="input-small" runat="server">
          <asp:ListItem Text="全部" Value="0" selected="true"></asp:ListItem>
          <asp:ListItem Text="一周" Value="7"></asp:ListItem>
          <asp:ListItem Text="一月" Value="30"></asp:ListItem>
          <asp:ListItem Text="半年" Value="180"></asp:ListItem>
          <asp:ListItem Text="一年" Value="365"></asp:ListItem>
          <asp:ListItem Text="自定义" Value="-1"></asp:ListItem>
        </asp:DropDownList>
        <span id="periods" style="display:none">
          &nbsp;&nbsp;
          开始：
          <bairong:DateTimeTextBox id="tbStartDate" class="input-small" runat="server" />
          &nbsp;&nbsp;
          结束：
          <bairong:DateTimeTextBox id="tbEndDate" class="input-small" runat="server" />
        </span>
      </td>
    </tr>
    <asp:PlaceHolder ID="phDisplayAttributes" runat="server">
    <tr>
      <td colspan="2">
        选择需要导出的字段：<input type="checkbox" id="check_groups" onClick=checkAll(document.getElementById("Group"),this.checked);><label for="check_groups">全选</label>
        <span id="Group"><asp:CheckBoxList ID="cblDisplayAttributes" RepeatColumns="3" RepeatDirection="Horizontal" class="noborder" Width="100%" runat="server"/></span>
      </td>
    </tr>
    </asp:PlaceHolder>
    <tr>
      <td>内容状态：</td>
      <td>
        <asp:DropDownList id="ddlIsChecked" class="input-medium" runat="server">
          <asp:ListItem Text="全部内容" Value="All" selected="true"></asp:ListItem>
          <asp:ListItem Text="未审核内容" Value="False"></asp:ListItem>
          <asp:ListItem Text="已审核内容" Value="True"></asp:ListItem>
        </asp:DropDownList>
      </td>
    </tr>
  </table>
  
    <table class="table table-noborder">
        <tr>
          <td class="center">
              <asp:Button class="btn btn-primary" id="btnSubmit" text="确定" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" />
          </td>
        </tr>
      </table>
</form>
</body>
</html>
