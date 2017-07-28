<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentCheck" %>
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

  <script type="text/javascript" >
  $(document).ready(function(){
    loopRows(document.getElementById('contents'), function(cur){ cur.onclick = chkSelect; });
    $(".popover-hover").popover({trigger:'hover',html:true});
  });
  </script>

  <div class="well well-small">
    <table class="table table-condensed noborder">
      <tr>
        <td width="80">
          状态：
        </td>
        <td>
          <asp:RadioButtonList ID="State" RepeatDirection="Horizontal" class="radiobuttonlist" AutoPostBack="true" OnSelectedIndexChanged="State_SelectedIndexChanged" runat="server"></asp:RadioButtonList>
        </td>
      </tr>
      <asp:PlaceHolder ID="PhContentModel" runat="server">
      <tr>
        <td>内容模型：</td>
        <td>
          <asp:DropDownList ID="DdlContentModelId" class="input-medium" AutoPostBack="true" OnSelectedIndexChanged="DdlContentModelId_SelectedIndexChanged" runat="server"></asp:DropDownList>
        </td>
      </tr>
      </asp:PlaceHolder>
    </table>
  </div>

  <table id="contents" class="table table-bordered table-hover">
    <tr class="info thead">
      <td>内容标题(点击查看) </td>
      <td>栏目</td>
      <asp:Literal ID="LtlColumnHeadRows" runat="server"></asp:Literal>
      <td width="50"> 状态 </td>
      <td width="30">&nbsp;</td>
      <asp:Literal ID="LtlCommandHeadRows" runat="server"></asp:Literal>
      <td width="20">
        <input type="checkbox" onClick="selectRows(document.getElementById('contents'), this.checked);">
      </td>
    </tr>
    <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
        <tr>
          <td>
            <asp:Literal ID="ltlItemTitle" runat="server"></asp:Literal>
          </td>
          <td>
            <asp:Literal ID="ltlChannel" runat="server"></asp:Literal>
          </td>
          <asp:Literal ID="ltlColumnItemRows" runat="server"></asp:Literal>
          <td class="center" nowrap>
            <asp:Literal ID="ltlItemStatus" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ltlItemEditUrl" runat="server"></asp:Literal>
          </td>
          <asp:Literal ID="ltlCommandItemRows" runat="server"></asp:Literal>
          <td class="center">
            <asp:Literal ID="ltlItemSelect" runat="server"></asp:Literal>
          </td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="SpContents" runat="server" class="table table-pager" />

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="BtnCheck" Text="审 核" runat="server" />
    <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />
  </ul>

</form>
</body>
</html>
