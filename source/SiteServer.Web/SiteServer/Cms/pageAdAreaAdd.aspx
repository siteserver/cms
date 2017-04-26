<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageAdAreaAdd" %>
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
 <bairong:alerts text="广告位的调用方法：&amp;lt;stl:ad area=&quot;广告位名称&quot; &gt;&amp;lt;/stl:ad&gt;" runat="server"></bairong:alerts> 
 
   <div class="popover popover-static">
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server" /></h3>
    <div class="popover-content">
      <table class="table noborder table-hover">
        <tr>
          <td width="160">广告位名称：</td>
          <td colspan="3">
            <asp:TextBox Columns="45" MaxLength="50" id="AdAreaName" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="AdAreaName"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
          </td>
        </tr>
        <tr>
          <td>是否生效：</td>
          <td colspan="3">
            <asp:RadioButtonList ID="IsEnabled" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
          </td>
        </tr>
        <tr>
            <td>广告位宽度：</td>
            <td>
              <asp:TextBox class="input-mini" id="Width" runat="server"/>
              <asp:RegularExpressionValidator
                ControlToValidate="Width"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="宽度必须为数字"
                foreColor="red"
                runat="server"/>
            </td>
            <td>高度：</td>
            <td>
              <asp:TextBox class="input-mini" id="Height" runat="server"/>
              <asp:RegularExpressionValidator
                ControlToValidate="Height"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="高度必须为数字"
                foreColor="red"
                runat="server"/>
            </td>
          </tr>
            <tr>
            <td>广告位描述：</td>
            <td colspan="3">
              <asp:TextBox style="height:100px; width:70%" TextMode="MultiLine" id="Summary" runat="server" Wrap="false" />
             </td>
          </tr>
       </table>
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
            <input class="btn" type="button" onClick="location.href='pageAdArea.aspx?PublishmentSystemID=<%=PublishmentSystemId%>';return false;" value="返 回" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
