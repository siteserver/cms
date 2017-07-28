<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageCreateFile" %>
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

  <bairong:alerts text="选择需要生成的文件页后点击“生成选定文件”即可生成对应的文件页面。" runat="server" />

  <script type="text/javascript" language="javascript">
  function selectAllFile(isChecked)
  {
    for(var i=0; i<document.getElementById('<%=FileCollectionToCreate.ClientID%>').options.length; i++)
    {
      document.getElementById('<%=FileCollectionToCreate.ClientID%>').options[i].selected = isChecked;
    }
  }
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title">生成文件页</h3>
    <div class="popover-content">

      <table class="table noborder">
        <tr>
          <td width="160">生成文件页：<br /><span class="gray">按住Ctrl可多选</span></td>
          <td>
            <asp:ListBox ID="FileCollectionToCreate" SelectionMode="Multiple" Rows="19" style="width:auto" runat="server"></asp:ListBox>
            &nbsp;&nbsp;
            <label class="checkbox inline" style="vertical-align:bottom">
              <input type="checkbox" onClick="selectAllFile(this.checked);"> 全选
            </label>
          </td>
        </tr>
        <tr>
          <td width="160">生成全部文件页：</td>
          <td colspan="3"><asp:Button class="btn btn-primary" style="margin-bottom:0px;" id="CreateFileButton" text="生成选定文件" onclick="CreateFileButton_OnClick" runat="server" /></td>
        </tr>
        <tr>
          <td width="160">删除全部文件页：</td>
          <td colspan="3"><asp:Button class="btn" id="DeleteAllFileButton" text="删 除" onclick="DeleteAllFileButton_OnClick" runat="server" /></td>
        </tr>
      </table>

    </div>
  </div>

</form>
</body>
</html>
