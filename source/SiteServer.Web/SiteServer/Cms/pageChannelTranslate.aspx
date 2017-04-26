<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageChannelTranslate" %>
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
  function setOptionColor(obj)
  {
    for (var i=0;i<obj.options.length;i++)
    {
      if (obj.options[i].value=="")
      {
        obj.options[i].style.color="gray";
      }else{
        obj.options[i].style.color="black";
      }
    }
  }
  $(document).ready(function(){
    setOptionColor(document.getElementById('<%=NodeIDFrom.ClientID%>'));
  });
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title">批量转移</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="120">从栏目：</td>
          <td><asp:ListBox ID="NodeIDFrom" Height="360" style="width:auto" SelectionMode="Multiple" runat="server"></asp:ListBox>
            <asp:RequiredFieldValidator
              ControlToValidate="NodeIDFrom"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
            </td>
        </tr>
        <tr>
          <td>转移到：</td>
          <td> 站点：
            <asp:DropDownList ID="PublishmentSystemIDDropDownList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PublishmentSystemID_OnSelectedIndexChanged"></asp:DropDownList>
            &nbsp;&nbsp;栏目：
            <asp:DropDownList ID="NodeIDTo" runat="server"></asp:DropDownList>
            <asp:RequiredFieldValidator
              ControlToValidate="NodeIDTo"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
            </td>
        </tr>
        <tr>
          <td>转移类型：</td>
          <td><asp:DropDownList ID="TranslateType" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
          <td>转移后删除：</td>
          <td>
            <asp:RadioButtonList id="IsDeleteAfterTranslate" RepeatDirection="Horizontal" class="noborder" runat="server">
              <asp:ListItem Text="是" />
              <asp:ListItem Text="否" Selected="True"/>
            </asp:RadioButtonList>
          </td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="转 移" onclick="Submit_OnClick" runat="server"/>
            <asp:PlaceHolder ID="phReturn" runat="server">
              <input class="btn" type="button" onClick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
            </asp:PlaceHolder>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
