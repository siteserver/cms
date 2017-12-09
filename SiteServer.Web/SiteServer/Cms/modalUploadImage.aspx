<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalUploadImage" Trace="false" %>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
		</head>

		<body>
			<!--#include file="../inc/openWindow.html"-->

			<form runat="server">
				<ctrl:alerts text="如果不设置缩略图宽度或高度，宽度或高度将根据图片尺寸按比例缩放" runat="server" />

				<div class="form-horizontal">

					<div class="form-group">
						<label class="col-xs-3 control-label text-right">选择上传的图片</label>
						<div class="col-xs-8">
							<input type="file" id="HifUpload" class="form-control" runat="server" />
						</div>
						<div class="col-xs-1">
							<asp:RequiredFieldValidator ControlToValidate="HifUpload" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
							/>
						</div>
					</div>

					<div class="form-group">
						<label class="col-xs-3 control-label text-right"></label>
						<div class="col-xs-8">
							<asp:CheckBox id="CbIsTitleImage" class="checkbox checkbox-primary" runat="server" Text="生成缩略图" />
						</div>
						<div class="col-xs-1"></div>
					</div>

					<div class="form-group" id="rowTitleImageSize">
						<label class="col-xs-3 control-label text-right"></label>
						<label class="col-xs-1 control-label text-right">
							宽
						</label>
						<div class="col-xs-3">
							<asp:TextBox ID="TbTitleImageWidth" class="form-control" runat="server"></asp:TextBox>
						</div>
						<label class="col-xs-1 control-label text-right">
							高
						</label>
						<div class="col-xs-3">
							<asp:TextBox ID="TbTitleImageHeight" class="form-control" runat="server"></asp:TextBox>
						</div>
						<div class="col-xs-1">
							<asp:RegularExpressionValidator ControlToValidate="TbTitleImageWidth" ValidationExpression="\d+" Display="Dynamic" ErrorMessage=" *"
							  foreColor="red" runat="server" />
							<asp:RegularExpressionValidator ControlToValidate="TbTitleImageHeight" ValidationExpression="\d+" Display="Dynamic" ErrorMessage=" *"
							  foreColor="red" runat="server" />
						</div>
					</div>

					<hr />

					<div class="form-group">
						<label class="col-xs-3 control-label text-right"></label>
						<div class="col-xs-8">
							<asp:CheckBox id="CbIsShowImageInTextEditor" class="checkbox checkbox-primary" runat="server" Checked="true" Text="将图片显示在内容编辑框中"
							/>
						</div>
						<div class="col-xs-1"></div>
					</div>

					<div class="form-group" id="rowIsSmallImage">
						<label class="col-xs-3 control-label text-right"></label>
						<div class="col-xs-8">
							<asp:CheckBox id="CbIsSmallImage" class="checkbox checkbox-primary" runat="server" Text="生成缩略图" />
						</div>
						<div class="col-xs-1"></div>
					</div>

					<div class="form-group" id="rowSmallImageSize">
						<label class="col-xs-3 control-label text-right"></label>
						<label class="col-xs-1 control-label text-right">
							宽
						</label>
						<div class="col-xs-3">
							<asp:TextBox ID="TbSmallImageWidth" class="form-control" runat="server"></asp:TextBox>
						</div>
						<label class="col-xs-1 control-label text-right">
							高
						</label>
						<div class="col-xs-3">
							<asp:TextBox ID="TbSmallImageHeight" class="form-control" runat="server"></asp:TextBox>
						</div>
						<div class="col-xs-1">
							<asp:RegularExpressionValidator ControlToValidate="TbSmallImageWidth" ValidationExpression="\d+" Display="Dynamic" ErrorMessage=" *"
							  foreColor="red" runat="server" />
							<asp:RegularExpressionValidator ControlToValidate="TbSmallImageHeight" ValidationExpression="\d+" Display="Dynamic" ErrorMessage=" *"
							  foreColor="red" runat="server" />
						</div>
					</div>

					<div class="form-group" id="rowIsLinkToOriginal">
						<label class="col-xs-3 control-label text-right"></label>
						<div class="col-xs-8">
							<asp:CheckBox id="CbIsLinkToOriginal" class="checkbox checkbox-primary" runat="server" Text="图片可链接到原图" />
						</div>
						<div class="col-xs-1"></div>
					</div>

					<script language="javascript">
						function selectImage(textBoxUrl, imageUrl) {
							window.parent.document.getElementById('<%=Request.QueryString["TextBoxClientID"]%>').value = textBoxUrl;
							window.parent.closeWindow();
						}

						function checkBoxChange() {
							if (document.getElementById('<%=CbIsTitleImage.ClientID%>').checked) {
								$('#rowTitleImageSize').show();
							} else {
								$('#rowTitleImageSize').hide();
							}

							if (document.getElementById('<%=CbIsShowImageInTextEditor.ClientID%>').checked) {
								$('#rowIsLinkToOriginal').show();
								$('#rowIsSmallImage').show();
							} else {
								$('#rowIsLinkToOriginal').hide();
								$('#rowIsSmallImage').hide();
							}

							if (document.getElementById('<%=CbIsShowImageInTextEditor.ClientID%>').checked && document.getElementById(
									'<%=CbIsSmallImage.ClientID%>').checked) {
								$('#rowSmallImageSize').show();
							} else {
								$('#rowSmallImageSize').hide();
							}
						}

						$(document).ready(function () {
							checkBoxChange();
						});
					</script>

					<asp:Literal id="LtlScript" runat="server"></asp:Literal>

					<hr />

					<div class="form-group m-b-0">
						<div class="col-xs-11 text-right">
							<asp:Button class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
							<button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
						</div>
						<div class="col-xs-1"></div>
					</div>
				</div>

			</form>
		</body>

		</html>