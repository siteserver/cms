<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateJsAdd" %>
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

  <script>
  function changeExtension(sel,tb,holder){
      tb.value = sel.options[sel.options.selectedIndex].value;
      if (sel.options[sel.options.selectedIndex].value==""){
          holder.style.display = '';
      }else{
          holder.style.display = 'none';
      }
  }
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server" /></h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="155"><bairong:help HelpText="样式文件名" Text="样式文件名：" runat="server" ></bairong:help></td>
          <td colspan="3"><asp:TextBox Columns="45" MaxLength="50" id="RelatedFileName" runat="server" />
            <asp:RequiredFieldValidator
                       ControlToValidate="RelatedFileName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator 
                       ControlToValidate="RelatedFileName" ValidationExpression="[^'\.]+" 
                       ErrorMessage="不能有文件扩展名" foreColor="red" Display="Dynamic" runat="server"/></td>
        </tr>
        <tr>
          <td width="155"><bairong:help HelpText="模板对应的文件扩展名" Text="文件扩展名：" runat="server" ></bairong:help></td>
          <td colspan="3">
            <asp:Literal id="ltlCreatedFileExtName" runat="server"></asp:Literal>
          </td>
        </tr>
        <tr>
          <td width="155"><bairong:help HelpText="网页的编码" Text="网页编码：" runat="server" ></bairong:help></td>
          <td colspan="3"><asp:DropDownList id="Charset" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
          <td width="155"><bairong:help HelpText="放置Js代码及需要嵌入的标签" Text="模板文件内容：" runat="server" ></bairong:help></td>
          <td colspan="3">&nbsp;</td>
        </tr>
        <tr>
          <td colspan="4"><asp:TextBox TextMode="MultiLine" id="Content" runat="server" style="height:450px; width:98%" Wrap="false" /></td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
            <input type=button class="btn" onClick="location.href='pageTemplateJs.aspx?PublishmentSystemID=<%=PublishmentSystemId%>';" value="返 回" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
