<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalContentImport" %>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts text="请选择Access文件，系统将导入Access文件对应的字段数据" runat="server"></bairong:alerts>

  <script type="text/javascript">
  $(document).ready(function(){
     $('#import input:radio').change(function() {
        var tips = '<button type="button" class="close" data-dismiss="alert">&times;</button><strong>提示!</strong>&nbsp;&nbsp; ';
         if (this.value == 'ContentZip'){
          $(".alert").html(tips +'请选择后台导出的压缩包，系统能够将内容以及内容相关的图片、附件等文件一道导入')
         } else if (this.value == 'ContentAccess'){
           $(".alert").html(tips +'请选择Access文件，系统将导入Access文件对应的字段数据');
         } else if (this.value == 'ContentExcel'){
           $(".alert").html(tips +'请选择Excel文件，系统将导入Excel文件对应的字段数据');
         } else if (this.value == 'ContentTxtZip'){
           $(".alert").html(tips +'请选择以.txt结尾的纯文本文件的压缩包，系统将压缩包内的每一个文件作为一篇内容，文件中第一行作为内容标题，其余作为内容正文导入');
         }
     });
  });
  </script>

  <table class="table table-noborder table-hover">
    <tr>
      <td><bairong:help HelpText="选择导入类型" Text="导入类型：" runat="server" ></bairong:help></td>
      <td id="import"><asp:RadioButtonList class="radiobuttonlist" ID="ImportType" runat="server" RepeatDirection="Horizontal">
          <asp:ListItem Text="导入压缩包" Value="ContentZip"></asp:ListItem>
          <asp:ListItem Text="导入Access" Value="ContentAccess" Selected="true"></asp:ListItem>
          <asp:ListItem Text="导入Excel" Value="ContentExcel"></asp:ListItem>
          <asp:ListItem Text="导入TXT压缩包" Value="ContentTxtZip"></asp:ListItem>
        </asp:RadioButtonList></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="选择需要上传的栏目文件" Text="栏目文件：" runat="server"></bairong:help></td>
      <td><input type=file  id=myFile size="35" runat="server"/>
        <asp:RequiredFieldValidator ControlToValidate="myFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="遇到同名标题是否覆盖" Text="是否覆盖同名标题：" runat="server" ></bairong:help></td>
      <td><asp:RadioButtonList ID="IsOverride" runat="server" RepeatDirection="Horizontal" class="noborder">
          <asp:ListItem Text="覆盖" Value="True" Selected="true"></asp:ListItem>
          <asp:ListItem Text="不覆盖" Value="False"></asp:ListItem>
        </asp:RadioButtonList></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="设置从第几条开始导入" Text="从第几条开始导入：" runat="server" ></bairong:help></td>
      <td><asp:TextBox class="input-mini" id="ImportStart" runat="server"/>
        (默认为第一条) </td>
    </tr>
    <tr>
      <td><bairong:help HelpText="设置共导入几条" Text="共导入几条：" runat="server" ></bairong:help></td>
      <td><asp:TextBox class="input-mini" id="ImportCount" runat="server"/>
        (默认为全部导入) </td>
    </tr>
    <tr>
      <td><bairong:help HelpText="设置内容的状态" Text='状态：'  runat="server" ></bairong:help></td>
      <td><asp:RadioButtonList ID="ContentLevel" RepeatDirection="Horizontal" class="noborder" runat="server"/></td>
    </tr>
  </table>

</form>
</body>
</html>
