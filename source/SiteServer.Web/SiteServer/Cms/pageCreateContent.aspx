<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageCreateContent" %>
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

  <bairong:alerts text="选择需要生成内容页面的栏目后点击“生成选定内容”即可生成对应的内容页面。" runat="server" />

  <script type="text/javascript" language="javascript">
  function selectAll(isChecked)
  {
    for(var i=0; i<document.getElementById('<%=NodeIDList.ClientID%>').options.length; i++)
    {
      document.getElementById('<%=NodeIDList.ClientID%>').options[i].selected = isChecked;
    }
  }
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title">生成内容页</h3>
    <div class="popover-content">
    
      <table class="table noborder">
        <tr>
          <td width="160">生成选定的内容：<br /><span class="gray">按住Ctrl可多选</span></td>
          <td><asp:ListBox ID="NodeIDList" SelectionMode="Multiple" Rows="19" style="width:auto" runat="server"></asp:ListBox>
            &nbsp;&nbsp;
            <label class="checkbox inline" style="vertical-align:bottom">
              <input type="checkbox" onClick="selectAll(this.checked);"> 全选
            </label>
        </tr>
        <tr>
          <td>生成内容：</td>
          <td> 生成范围：
            <asp:DropDownList ID="ChooseScope" runat="server"></asp:DropDownList>
            &nbsp;&nbsp;
            <asp:Button class="btn btn-primary" style="margin-bottom:0px;" id="CreateContentButton" text="生成选定内容" onclick="CreateContentButton_OnClick" runat="server" /></td>
        </tr>
        <tr>
          <td>删除全部内容页面：</td>
          <td><asp:Button class="btn" id="DeleteAllContentButton" text="删 除" onclick="DeleteAllContentButton_OnClick" runat="server" /></td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
