<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalFilePathRule" Trace="false"%>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
			<script type="text/javascript">
				(function ($) {
					$.fn.caret = function (pos) {
						var target = this[0];
						//get
						if (arguments.length == 0) {
							//HTML5
							if (window.getSelection) {
								//contenteditable
								if (target.contentEditable == 'true') {
									target.focus();
									var range1 = window.getSelection().getRangeAt(0),
										range2 = range1.cloneRange();
									range2.selectNodeContents(target);
									range2.setEnd(range1.endContainer, range1.endOffset);
									return range2.toString().length;
								}
								//textarea
								return target.selectionStart;
							}
							//IE<9
							if (document.selection) {
								target.focus();
								//contenteditable
								if (target.contentEditable == 'true') {
									var range1 = document.selection.createRange(),
										range2 = document.body.createTextRange();
									range2.moveToElementText(target);
									range2.setEndPoint('EndToEnd', range1);
									return range2.text.length;
								}
								//textarea
								var pos = 0,
									range = target.createTextRange(),
									range2 = document.selection.createRange().duplicate(),
									bookmark = range2.getBookmark();
								range.moveToBookmark(bookmark);
								while (range.moveStart('character', -1) !== 0) pos++;
								return pos;
							}
							//not supported
							return 0;
						}
						//set
						//HTML5
						if (window.getSelection) {
							//contenteditable
							if (target.contentEditable == 'true') {
								target.focus();
								window.getSelection().collapse(target.firstChild, pos);
							}
							//textarea
							else
								target.setSelectionRange(pos, pos);
						}
						//IE<9
						else if (document.body.createTextRange) {
							var range = document.body.createTextRange();
							range.moveToElementText(target)
							range.moveStart('character', pos);
							range.collapse(true);
							range.select();
						}
					}
				})(jQuery)

				function AddOnPos(value) {
					var val = $('#TbRule').val();
					var i = $('#TbRule').caret();
					if (i == 0) {
						val = val + value;
					} else {
						val = val.substr(0, i) + value + val.substr(i, val.length);
					}
					$('#TbRule').val(val);
					if (i > 0) {
						$('#TbRule').caret(i + value.length);
					} else {
						$('#TbRule').caret(val.length);
					}
				}
			</script>
		</head>

		<body>
			<!--#include file="../inc/openWindow.html"-->

			<form runat="server">
				<ctrl:alerts runat="server" />

				<table class="table table-bordered table-hover">
					<tr class="info">
						<td>规则</td>
						<td>含义</td>
						<td>规则</td>
						<td>含义</td>
						<td>规则</td>
						<td>含义</td>
					</tr>
					<asp:Literal ID="LtlRules" runat="server"></asp:Literal>
				</table>

				<div class="form-horizontal">

					<div class="form-group">
						<label class="col-xs-3 control-label text-right">页面命名规则</label>
						<div class="col-xs-8">
							<asp:TextBox class="form-control" id="TbRule" runat="server" />
						</div>
						<div class="col-xs-1"></div>
					</div>

					<hr />

					<div class="form-group m-b-0">
						<div class="col-xs-11 text-right">
							<asp:Button class="btn btn-primary m-l-10" ID="BtnCheck" Text="确 定" OnClick="Submit_OnClick" runat="server" />
							<button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
						</div>
						<div class="col-xs-1"></div>
					</div>

				</div>

			</form>
		</body>

		</html>