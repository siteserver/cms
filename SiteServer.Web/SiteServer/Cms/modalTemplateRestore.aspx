<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalTemplateRestore" Trace="false"%>
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
<bairong:alerts runat="server"></bairong:alerts>

  <style type="text/css">
  .CodeMirror-line-numbers { width: 22px; color: #aaa; background-color: #eee; text-align: right; padding-right: .3em; font-size: 10pt; font-family: monospace; padding-top: .4em; line-height: 16px; }
  </style>
  <script src="../assets/codeMirror/js/codemirror.js" type="text/javascript"></script>

  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          历史版本：
          <asp:DropDownList id="DdlLogId" AutoPostBack="true" OnSelectedIndexChanged="DdlLogId_SelectedIndexChanged" style="width:480px;" runat="server"></asp:DropDownList>
        </td>
      </tr>
    </table>
  </div>

  <div style="border: 1px solid #CCC; width:100%">
    <asp:TextBox width="100%" TextMode="MultiLine" id="TbContent" runat="server" Wrap="false" />
  </div>
  <script type="text/javascript">
    $(document).ready(function(){
      var isTextArea = false;
      var editor = CodeMirror.fromTextArea('TbContent', {
          height: "400px",
          parserfile: ["parsexml.js"],
          stylesheet: ["../assets/codeMirror/css/xmlcolors.css"],
          path: "../assets/codeMirror/js/",
          continuousScanning: 500,
          lineNumbers: true
      });
      $('#reindent').show().click(function(){
        if (!isTextArea) editor.reindent();
      });
    });
  </script>

  <hr />
  <table class="table noborder">
    <tr>
      <td class="center">
        <asp:Button class="btn btn-primary" id="btnSubmit" text="确 定"  runat="server" onClick="Submit_OnClick" />
      </td>
    </tr>
  </table>

</form>
</body>
</html>
