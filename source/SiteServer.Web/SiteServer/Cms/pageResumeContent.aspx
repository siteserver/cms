<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageResumeContent" %>
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

  <table class="table table-bordered table-hover">
    <tr class="info thead">
	    <td>姓名</td>
	    <td>性别</td>
	    <td>手机号码</td>
	    <td>邮箱</td>
	    <td>学历</td>
	    <td>毕业院校</td>
	    <td>申请职位</td>
	    <td>添加时间</td>
		<td>&nbsp;</td>
		<td width="20">
			<input onclick="_checkFormAll(this.checked)" type="checkbox" />
		</td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <asp:Literal ID="ltlTr" runat="server"></asp:Literal>
			<td class="center"><asp:Literal ID="ltlRealName" runat="server"></asp:Literal></td>
            <td class="center"><asp:Literal ID="ltlGender" runat="server"></asp:Literal></td>
            <td class="center"><asp:Literal ID="ltlMobilePhone" runat="server"></asp:Literal></td>
            <td class="center"><asp:Literal ID="ltlEmail" runat="server"></asp:Literal></td>
            <td class="center"><asp:Literal ID="ltlEducation" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlLastSchoolName" runat="server"></asp:Literal></td>
            <td class="center">
                <asp:Literal ID="ltlJobTitle" runat="server"></asp:Literal>
            </td>
            <td class="center">
				<asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
			</td>
			<td class="center">
				<asp:Literal ID="ltlViewUrl" runat="server"></asp:Literal>
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
    <asp:Button class="btn" id="SetIsView" Text="标记为已读" runat="server" />
    <asp:Button class="btn" id="SetNotView" Text="标记为未读" runat="server" />
	<asp:Button class="btn" id="Delete" Text="删 除" runat="server" />
	<asp:Button class="btn" id="Return" OnClick="Return_OnClick" Text="返 回" runat="server" />
  </ul>

</form>
</body>
</html>
