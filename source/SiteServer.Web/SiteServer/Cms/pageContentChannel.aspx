<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentChannel" %>
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
   loopRows(document.getElementById('channels'), function(cur){ cur.onclick = chkSelect; });
   $(".popover-hover").popover({trigger:'hover',html:true});
  });
  </script>

  <div class="well well-small">
    <asp:Literal ID="LtlChannelButtons" runat="server"></asp:Literal>
  </div>

  <table id="channels" class="table table-bordered table-hover">
    <tr class="info thead">
      <td>栏目名称(点击查看)</td>
      <td width="100">栏目索引</td>
      <td width="30">上升</td>
      <td width="30">下降</td>
      <td width="50">&nbsp;</td>
      <td width="20">
        <input type="checkbox" onClick="selectRows(document.getElementById('channels'), this.checked);">
      </td>
    </tr>
    <asp:Repeater ID="RptChannels" runat="server">
      <itemtemplate>
          <tr>
            <td>
              <asp:Literal id="ltlNodeTitle" runat="server" />
            </td>
            <td>
              <asp:Literal id="ltlNodeIndexName" runat="server" />
            </td>
            <td class="center">
              <asp:Literal id="ltlUpLink" runat="server" />
            </td>
            <td class="center">
              <asp:Literal id="ltlDownLink" runat="server" />
            </td>
            <td class="center">
              <asp:Literal id="ltlEditLink" runat="server" />
            </td>
            <td class="center">
              <asp:Literal id="ltlCheckBoxHtml" runat="server" />
            </td>
          </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <br>

  <div class="well well-small">
    <asp:Literal ID="LtlContentButtons" runat="server"></asp:Literal>
    <div id="contentSearch" style="display:none;margin-top:10px;">
      时间从：
      <bairong:DateTimeTextBox id="DateFrom" class="input-small" runat="server" />
      目标：
      <asp:DropDownList ID="SearchType" class="input-medium" runat="server"></asp:DropDownList>
      关键字：
      <asp:TextBox id="Keyword" class="input-medium" runat="server"/>
      <asp:Button class="btn" OnClick="Search_OnClick" id="Search" text="搜 索"  runat="server"/>
    </div>
  </div>

  <table id="contents" class="table table-bordered table-hover">
    <tr class="info thead">
      <td>内容标题(点击查看) </td>
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
          <asp:Literal ID="ltlColumnItemRows" runat="server"></asp:Literal>
          <td class="center" nowrap>
            <asp:Literal ID="ltlItemStatus" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ltlItemEditUrl" runat="server"></asp:Literal>
          </td>
          <asp:Literal ID="ltlCommandItemRows" runat="server"></asp:Literal>
          <td class="center">
            <input type="checkbox" name="ContentIDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
          </td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="SpContents" runat="server" class="table table-pager" />

</form>
</body>
</html>
