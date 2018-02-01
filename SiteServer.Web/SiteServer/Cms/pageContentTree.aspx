<%@ Page Language="C#" Trace="false" EnableViewState="false" Inherits="SiteServer.BackgroundPages.BasePageCms" %>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html style="background-color: #fff;">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
			<style>
				.scroll {
					overflow-x: scroll !important
				}

				.list-group {
					margin-bottom: 0;
				}

				.table>tbody>tr>td,
				.table>tbody>tr>th,
				.table>tfoot>tr>td,
				.table>tfoot>tr>th,
				.table>thead>tr>td,
				.table>thead>tr>th {
					border-top: none;
				}

				.table a,
				.table span,
				.table div {
					font-size: 12px !important;
				}
			</style>
			<script type="text/javascript">
				$(document).ready(function () {
					$('body').height($(window).height());
					$('body').addClass('scroll');
				});
			</script>
		</head>

		<body style="margin: 0; padding: 0; background-color: #fff;">
			<form class="m-0" runat="server">
				<div class="list-group mail-list">
					<div onclick="location.reload(true);" style="cursor: pointer;" class="list-group-item b-0 active">
						栏目列表
					</div>
				</div>
				<table class="table table-sm table-hover">
					<tbody>
						<ctrl:NodeTree runat="server"></ctrl:NodeTree>
					</tbody>
				</table>
			</form>
		</body>

		</html>
		<!--#include file="../inc/foot.html"-->