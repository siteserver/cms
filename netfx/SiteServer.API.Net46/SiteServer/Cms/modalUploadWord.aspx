<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalUploadWord" %>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->

			<script type="text/javascript">
				$(document).ready(function () {
					$('#CbIsFirstLineTitle').click(function (e) {
						if (!$("#CbIsFirstLineTitle").attr("checked")) {
							$("#CbIsFirstLineRemove").removeAttr("checked");
						};
					});
				});
			</script>
		</head>

		<body>
			<form runat="server">
				<ctrl:alerts runat="server" />

				<input id="HihFileNames" type="hidden" runat="server" />

				<div id="drop-area" style="height: 200px; line-height: 200px; text-align: center; font-size: 18px; color: #777; border: 2px dashed #0000004d;
						background: #fff;	border-radius: 6px; cursor: pointer; margin-bottom: 20px">
					点击批量上传Word文件或者将Word文件拖拽到此区域
				</div>

				<div id="main" class="row">

					<div class="col-sm-4 col-lg-3 col-xs-12" v-for="(file, index) in files">

						<div class="card m-b-20">

							<div class="card-body">
								<p class="card-text">
									{{ file.fileName }}
									<br /> 大小：{{ Math.round(file.length/1024) + ' KB' }}
								</p>
								<a @click="del(file)" href="javascript:;" class="card-link text-danger">删 除</a>
							</div>

						</div>

					</div>

				</div>

				<div class="form-group form-row">
					<label class="col-1 text-right col-form-label">设置</label>
					<div class="col-10">
						<div class="checkbox checkbox-primary">
							<asp:CheckBox ID="CbIsFirstLineTitle" Checked="true" runat="server" Text="将第一行作为标题" />
							<asp:CheckBox ID="CbIsFirstLineRemove" Checked="true" runat="server" Text="内容正文删除标题" />
							<asp:CheckBox id="CbIsClearFormat" Checked="true" runat="server" Text="清除格式" />
							<br />
							<asp:CheckBox id="CbIsFirstLineIndent" Checked="true" runat="server" Text="首行缩进" />
							<asp:CheckBox id="CbIsClearFontSize" Checked="true" runat="server" Text="清除字号" />
							<asp:CheckBox id="CbIsClearFontFamily" Checked="true" runat="server" Text="清除字体" />
							<asp:CheckBox id="CbIsClearImages" runat="server" Text="清除图片" />
						</div>
					</div>
					<div class="col-1 help-block"></div>
				</div>

				<div class="form-group form-row">
					<label class="col-1 text-right col-form-label">状态</label>
					<div class="col-10">
						<asp:DropDownList ID="DdlContentLevel" class="form-control" runat="server" />
					</div>
					<div class="col-1 help-block"></div>
				</div>

				<hr />

				<div class="text-right mr-1">
					<asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
					<button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
				</div>

			</form>
		</body>

		</html>
		<!--#include file="../inc/foot.html"-->

		<script type="text/javascript" src="../assets/vue/vue.min.js"></script>
		<script type="text/javascript" src="../assets/web-uploader/js/Q.js"></script>
		<script type="text/javascript" src="../assets/web-uploader/js/Q.Uploader.js"></script>
		<script type="text/javascript">
			var data = {
				op: '',
				file: null,
				indexOld: 0,
				indexNew: 0,
				files: []
			};

			var $vue = new Vue({
				el: '#main',
				data: data,
				methods: {
					upload: function (file) {
						if (file && file.fileName) {
							this.files.push(file);
						}
						$('#HihFileNames').val(this.getFileNames().join(','));
					},
					del: function (file) {
						this.files.splice(this.files.indexOf(file), 1);
						$('#HihFileNames').val(this.getFileNames().join(','));
					},
					getFileNames: function () {
						var arr = [];
						for (var i = 0; i < this.files.length; i++) {
							arr.push(this.files[i].fileName);
						}
						return arr;
					}
				}
			});

			var E = Q.event,
				Uploader = Q.Uploader;

			var boxDropArea = document.getElementById("drop-area");

			var uploader = new Uploader({
				url: '<%=UploadUrl%>',
				target: document.getElementById("drop-area"),
				allows: ".doc,.docx",
				on: {
					add: function (task) {
						if (task.disabled) {
							return swal({
								title: "允许上传的文件格式为：" + this.ops.allows,
								icon: 'warning',
								button: '关 闭'
							});
						}
					},
					complete: function (task) {
						var json = task.json;
						if (!json || json.ret != 1) {
							return swal({
								title: "上传失败！",
								icon: 'warning',
								button: '关 闭'
							});
						}

						$vue.upload(json);
					}
				}
			});

			function set_drag_drop() {
				//若浏览器不支持html5上传，则禁止拖拽上传
				if (!Uploader.support.html5 || !uploader.html5) {
					boxDropArea.innerHTML = "点击批量上传Word文件";
					return;
				}

				//阻止浏览器默认拖放行为
				E.add(boxDropArea, "dragleave", E.stop);
				E.add(boxDropArea, "dragenter", E.stop);
				E.add(boxDropArea, "dragover", E.stop);

				E.add(boxDropArea, "drop", function (e) {
					E.stop(e);

					//获取文件对象
					var files = e.dataTransfer.files;

					uploader.addList(files);
				});
			}

			set_drag_drop();
		</script>