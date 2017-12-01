<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalUploadImage" Trace="false" %>
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
<bairong:alerts text="如果不设置缩略图宽度或高度，宽度或高度将根据图片尺寸按比例缩放；如果小于此尺寸时不生成缩略图未勾选，生成的缩略图有可能大于实际尺寸" runat="server"></bairong:alerts>

	<table class="table table-noborder table-hover">
		<tr>
			<td width="120">选择上传的图片：</td>
			<td>
			  	<input type="file" id="hifUpload" size="45" runat="server"/> 
				<asp:RequiredFieldValidator ControlToValidate="hifUpload" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
			</td>
		</tr>
		<tr>
			<td colspan="2">
			<asp:CheckBox id="cbIsTitleImage" runat="server" Text="生成缩略图"/>
			</td>
		</tr>
		<tr id="rowTitleImageSize">
			<td colspan="2" style="padding-left:30px;">
			缩略图尺寸：
			&nbsp;宽&nbsp;<asp:TextBox ID="tbTitleImageWidth" class="input-mini" MaxLength="50" runat="server"></asp:TextBox>
			<asp:RegularExpressionValidator
				ControlToValidate="tbTitleImageWidth"
				ValidationExpression="\d+"
				Display="Dynamic"
				ErrorMessage=" *" foreColor="red"
				runat="server"/>
			&nbsp;高&nbsp;<asp:TextBox ID="tbTitleImageHeight" class="input-mini" MaxLength="50" runat="server"></asp:TextBox>
			<asp:RegularExpressionValidator
				ControlToValidate="tbTitleImageHeight"
				ValidationExpression="\d+"
				Display="Dynamic"
				ErrorMessage=" *" foreColor="red"
				runat="server"/>
			&nbsp;<asp:CheckBox id="cbIsTitleImageLessSizeNotThumb" runat="server" Text="小于此尺寸时不生成缩略图"/>
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<hr />
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<asp:CheckBox id="cbIsShowImageInTextEditor" runat="server" Checked="true" Text="将图片显示在内容编辑框中"/>
			</td>
		</tr>
		<tr id="rowIsSmallImage">
			<td colspan="2">
				<asp:CheckBox id="cbIsSmallImage" runat="server" Text="生成缩略图"/>
			</td>
		</tr>
		<tr id="rowSmallImageSize">
			<td colspan="2" style="padding-left:30px;">
				缩略图尺寸：
				&nbsp;宽&nbsp;<asp:TextBox ID="tbSmallImageWidth" class="input-mini" MaxLength="50" runat="server"></asp:TextBox>
				<asp:RegularExpressionValidator
					ControlToValidate="tbSmallImageWidth"
					ValidationExpression="\d+"
					Display="Dynamic"
					ErrorMessage=" *" foreColor="red"
					runat="server"/>
				&nbsp;高&nbsp;<asp:TextBox ID="tbSmallImageHeight" class="input-mini" MaxLength="50" runat="server"></asp:TextBox>
				<asp:RegularExpressionValidator
					ControlToValidate="tbSmallImageHeight"
					ValidationExpression="\d+"
					Display="Dynamic"
					ErrorMessage=" *" foreColor="red"
					runat="server"/>
				&nbsp;<asp:CheckBox id="cbIsSmallImageLessSizeNotThumb" runat="server" Text="小于此尺寸时不生成缩略图"/>
			</td>
		</tr>
		<tr id="rowIsLinkToOriginal">
			<td colspan="2">
				<asp:CheckBox id="cbIsLinkToOriginal" runat="server" Text="图片可链接到原图"/>
			</td>
		</tr>
	</table>
	
	<script language="javascript">
	function selectImage(textBoxUrl, imageUrl)
	{
		window.parent.document.getElementById('<%=Request.QueryString["TextBoxClientID"]%>').value = textBoxUrl;
		window.parent.closeWindow();
	}

	function checkBoxChange()
	{
		if (document.getElementById('<%=cbIsTitleImage.ClientID%>').checked)
		{
			$('#rowTitleImageSize').show();
		} else {
			$('#rowTitleImageSize').hide();
		}
		
		if (document.getElementById('<%=cbIsShowImageInTextEditor.ClientID%>').checked)
		{
			$('#rowIsLinkToOriginal').show();
			$('#rowIsSmallImage').show();
		} else {
			$('#rowIsLinkToOriginal').hide();
			$('#rowIsSmallImage').hide();
		}

		if (document.getElementById('<%=cbIsShowImageInTextEditor.ClientID%>').checked && document.getElementById('<%=cbIsSmallImage.ClientID%>').checked)
		{
			$('#rowSmallImageSize').show();
		} else {
			$('#rowSmallImageSize').hide();
		}
	}

	$(document).ready(function(){
		checkBoxChange();
	});

	<asp:Literal id="ltlScript" runat="server"></asp:Literal>

	</script>

</form>
</body>
</html>
