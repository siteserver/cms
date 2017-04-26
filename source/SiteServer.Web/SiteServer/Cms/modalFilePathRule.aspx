<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalFilePathRule" Trace="false"%>
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

	<script type="text/javascript">
	(function($) {
	  $.fn.caret = function(pos) {
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

	function AddOnPos(value)
	{
		var val = $('#tbRule').val();
		var i = $('#tbRule').caret();
		if (i == 0){
			val = val + value;
		}else{
			val = val.substr(0,i) + value + val.substr(i,val.length);
		}
		$('#tbRule').val(val);
		if (i > 0) {
			$('#tbRule').caret(i + value.length);
		}else{
			$('#tbRule').caret(val.length);
		}
	}
	</script>

	<table class="table table-bordered table-hover">
    <tr class="info">
      <td>规则</td>
      <td>含义</td>
      <td>规则</td>
      <td>含义</td>
      <td>规则</td>
      <td>含义</td>
    </tr>
    <asp:Literal ID="ltlRules" runat="server"></asp:Literal>
  </table>

  <table class="table noborder table-hover">
    <tr>
      <td width="120">页面命名规则：</td>
      <td>
      	<asp:TextBox class="input-xxlarge" id="tbRule" runat="server" />
      	<br>
      	<span class="gray">
      		<asp:Literal id="ltlTips" runat="server"></asp:Literal>
      	</span>
      </td>
    </tr>
  </table>

</form>
</body>
</html>
