<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalUploadImage" Trace="false" %>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
		</head>

		<body>
			<form runat="server">
				<ctrl:alerts text="如果不设置缩略图宽度或高度，宽度或高度将根据图片尺寸按比例缩放" runat="server" />

				<div class="form-group form-row">
					<label class="col-3 col-form-label text-right">选择上传的图片</label>
					<div class="col-8">
						<input type="file" id="HifUpload" class="form-control" runat="server" />
					</div>
					<div class="col-1">
						<asp:RequiredFieldValidator ControlToValidate="HifUpload" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
						/>
					</div>
				</div>

				<div class="form-group form-row">
					<label class="col-3 col-form-label text-right"></label>
					<div class="col-8">
						<asp:CheckBox id="CbIsTitleImage" class="checkbox checkbox-primary" runat="server" Text="生成缩略图" />
					</div>
					<div class="col-1"></div>
				</div>

				<div class="form-group form-row" id="rowTitleImageSize">
					<label class="col-3 col-form-label text-right"></label>
					<label class="col-1 col-form-label text-right">
						宽
					</label>
					<div class="col-3">
						<asp:TextBox ID="TbTitleImageWidth" class="form-control" runat="server"></asp:TextBox>
					</div>
					<label class="col-1 col-form-label text-right">
						高
					</label>
					<div class="col-3">
						<asp:TextBox ID="TbTitleImageHeight" class="form-control" runat="server"></asp:TextBox>
					</div>
					<div class="col-1">
						<asp:RegularExpressionValidator ControlToValidate="TbTitleImageWidth" ValidationExpression="\d+" Display="Dynamic" ErrorMessage=" *"
						  foreColor="red" runat="server" />
						<asp:RegularExpressionValidator ControlToValidate="TbTitleImageHeight" ValidationExpression="\d+" Display="Dynamic" ErrorMessage=" *"
						  foreColor="red" runat="server" />
					</div>
				</div>

				<hr />

				<div class="form-group form-row">
					<label class="col-3 col-form-label text-right"></label>
					<div class="col-8">
						<asp:CheckBox id="CbIsShowImageInTextEditor" class="checkbox checkbox-primary" runat="server" Checked="true" Text="将图片显示在内容编辑框中"
						/>
					</div>
					<div class="col-1"></div>
				</div>

				<div class="form-group form-row" id="rowIsSmallImage">
					<label class="col-3 col-form-label text-right"></label>
					<div class="col-8">
						<asp:CheckBox id="CbIsSmallImage" class="checkbox checkbox-primary" runat="server" Text="生成缩略图" />
					</div>
					<div class="col-1"></div>
				</div>

				<div class="form-group form-row" id="rowSmallImageSize">
					<label class="col-3 col-form-label text-right"></label>
					<label class="col-1 col-form-label text-right">
						宽
					</label>
					<div class="col-3">
						<asp:TextBox ID="TbSmallImageWidth" class="form-control" runat="server"></asp:TextBox>
					</div>
					<label class="col-1 col-form-label text-right">
						高
					</label>
					<div class="col-3">
						<asp:TextBox ID="TbSmallImageHeight" class="form-control" runat="server"></asp:TextBox>
					</div>
					<div class="col-1">
						<asp:RegularExpressionValidator ControlToValidate="TbSmallImageWidth" ValidationExpression="\d+" Display="Dynamic" ErrorMessage=" *"
						  foreColor="red" runat="server" />
						<asp:RegularExpressionValidator ControlToValidate="TbSmallImageHeight" ValidationExpression="\d+" Display="Dynamic" ErrorMessage=" *"
						  foreColor="red" runat="server" />
					</div>
				</div>

				<div class="form-group form-row" id="rowIsLinkToOriginal">
					<label class="col-3 col-form-label text-right"></label>
					<div class="col-8">
						<asp:CheckBox id="CbIsLinkToOriginal" class="checkbox checkbox-primary" runat="server" Text="图片可链接到原图" />
					</div>
					<div class="col-1"></div>
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

				<div class="text-right mr-1">
					<asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
					<button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
				</div>

			</form>
		</body>

		</html>
		<!--#include file="../inc/foot.html"-->