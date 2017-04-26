<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalGatherSet" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

	<script language="JavaScript" type="text/JavaScript">
	function AddOnPos(obj, charvalue)
	{
		//obj代表要插入字符的输入框
		//value代表要插入的字符
		
		obj.focus();
		var r = document.selection.createRange();
		var ctr = obj.createTextRange();
		var i;
		var s = obj.value;
		
		//注释掉的这种方法只能用在单行的输入框input内
		//对多行输入框textarea无效
		//r.setEndPoint("StartToStart", ctr);
		//i = r.text.length;
		//取到光标位置----Start----
		var ivalue = "&^asdjfls2FFFF325%$^&"; 
		r.text = ivalue;
		i = obj.value.indexOf(ivalue);
		r.moveStart("character", -ivalue.length);
		r.text = "";
		//取到光标位置----End----
		//插入字符
		obj.value = s.substr(0,i) + charvalue + s.substr(i,s.length);
		ctr.collapse(true);
		ctr.moveStart("character", i + charvalue.length);
		ctr.select();
	}
	</script>

  <table class="table table-noborder table-hover">
	<tr>
	  <td><bairong:help HelpText="需要采集的起始网页地址" Text="起始网页地址：" runat="server" ></bairong:help></td>
	  <td><table cellpadding="4" cellspacing="4" width="100%">
		  <tr>
		  <td><asp:CheckBox ID="GatherUrlIsCollection" runat="server" AutoPostBack="true" OnCheckedChanged="GatherUrl_CheckedChanged" Text="从多个网址" Checked="true"></asp:CheckBox>
			  &nbsp;&nbsp;
			  <asp:CheckBox ID="GatherUrlIsSerialize" runat="server" AutoPostBack="true" OnCheckedChanged="GatherUrl_CheckedChanged" Text="从序列相似网址"></asp:CheckBox></td>
		</tr>
		  <tr id="GatherUrlCollectionRow" runat="server">
		  	<td><asp:TextBox class="input-large" TextMode="MultiLine" Rows="6" id="GatherUrlCollection" runat="server"/><br /><span class="gray">（以换行分割）</span>
			</td>
		</tr>
		  <tr id="GatherUrlSerializeRow" runat="server">
		  <td><asp:TextBox id="GatherUrlSerialize" runat="server"/>
			  变动数字: <a href="javascript:;" onCLICK="AddOnPos(document.getElementById('<%=GatherUrlSerialize.ClientID%>'), '*');" title="遇到变动数字用*代替"><font color="#0000FF">*</font>&nbsp;</a>
			  <br />
			  变动数字范围: 从
			  <asp:TextBox class="input-mini" id="SerializeFrom" Text="1" runat="server"/>
			  到
			  <asp:TextBox class="input-mini" id="SerializeTo" Text="10" runat="server"/>
			  数字变动倍数:
			  <asp:TextBox class="input-mini" Text="1" id="SerializeInterval" runat="server"/>
			  <br />
			  <asp:CheckBox ID="SerializeIsOrderByDesc" runat="server" Text="倒序"></asp:CheckBox>
			  <asp:CheckBox ID="SerializeIsAddZero" runat="server" Text="补零"></asp:CheckBox></td>
		</tr>
		</table></td>
	</tr>
	<tr>
	  <td width="110"><bairong:help HelpText="限定采集内容的地址必须包含的字符串" Text="内容地址包含：" runat="server" ></bairong:help></td>
	  <td><asp:TextBox Columns="40" MaxLength="200" id="UrlInclude" runat="server"/>
		<a href="javascript:;" onCLICK="AddOnPos(document.getElementById('<%=UrlInclude.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font>&nbsp;</a><a href="javascript:;" onCLICK="AddOnPos(document.getElementById('<%=UrlInclude.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font>&nbsp;</a></td>
	</tr>
	<tr>
	  <td><bairong:help HelpText="选择栏目，采集到的内容将添加到此栏目中" Text="采集到栏目：" runat="server" ></bairong:help></td>
	  <td><asp:DropDownList ID="NodeIDDropDownList" runat="server"></asp:DropDownList></td>
	</tr>
	<tr>
	  <td><bairong:help HelpText="需要采集的内容数，0代表采集所有内容" Text="采集内容数：" runat="server" ></bairong:help></td>
	  <td><asp:TextBox class="input-mini" MaxLength="4" id="GatherNum" Style="text-align:right" Text="10" runat="server"/>
		（0代表不限定）
		<asp:RequiredFieldValidator
			ControlToValidate="GatherNum"
			errorMessage=" *" foreColor="red" 
			Display="Dynamic"
			runat="server"/>
		<asp:RegularExpressionValidator
			ControlToValidate="GatherNum"
			ValidationExpression="\d+"
			ErrorMessage="采集数只能是数字"
			foreColor="red"
			Display="Dynamic"
			runat="server"/></td>
	</tr>
	<tr>
  </table>

</form>
</body>
</html>
