<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentArchive" %>
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
  });
  </script>

  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          栏目：
          <asp:DropDownList ID="NodeIDDropDownList" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" runat="server"></asp:DropDownList>
          每页显示条数：
          <asp:DropDownList ID="PageNum" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick" class="input-small" runat="server">
            <asp:ListItem Text="默认" Value="0" Selected="true"></asp:ListItem>
            <asp:ListItem Text="10" Value="10"></asp:ListItem>
            <asp:ListItem Text="16" Value="16"></asp:ListItem>
            <asp:ListItem Text="20" Value="20"></asp:ListItem>
            <asp:ListItem Text="30" Value="30"></asp:ListItem>
            <asp:ListItem Text="50" Value="50"></asp:ListItem>
            <asp:ListItem Text="100" Value="100"></asp:ListItem>
            <asp:ListItem Text="200" Value="200"></asp:ListItem>
            <asp:ListItem Text="300" Value="300"></asp:ListItem>
          </asp:DropDownList>
          时间：从
          <bairong:DateTimeTextBox id="DateFrom" class="input-small" Columns="12" runat="server" />
          &nbsp;到&nbsp;
          <bairong:DateTimeTextBox id="DateTo" class="input-small" Columns="12" runat="server" />
        </td>
      </tr>
      <tr>
        <td>
          目标：
          <asp:DropDownList ID="SearchType" class="input-small" runat="server"> </asp:DropDownList>
          关键字：
          <asp:TextBox id="Keyword"
            MaxLength="500"
            Size="37"
            runat="server"/>
          <asp:Button class="btn" OnClick="Search_OnClick" id="Search" text="搜 索"  runat="server"/>
        </td>
      </tr>
    </table>
  </div>

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td>内容标题(点击查看)</td>
      <td width="160">原位置</td>
      <td width="120">归档日期</td>
      <td width="20">
        <input type="checkbox" onClick="selectRows(document.getElementById('contents'), this.checked);">
      </td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <tr>
          <td>
            <asp:Literal ID="ItemTitle" runat="server"></asp:Literal>
          </td>
          <td>
            <asp:Literal ID="ItemChannelName" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ItemArchiveDate" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <input type="checkbox" name="ContentIDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
          </td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="spContents" runat="server" class="table table-pager" />

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn" id="Restore" Text="取消归档" runat="server" />
    <asp:Button class="btn" id="RestoreAll" Text="全部取消归档" runat="server" />
    <asp:Button class="btn" id="Delete" Text="删 除" runat="server" />
    <asp:Button class="btn" id="DeleteAll" Text="清空内容归档" runat="server" />
  </ul>

</form>
</body>
</html>
