<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalTemplateView" Trace="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <style type="text/css">
        .CodeMirror-line-numbers {
          width: 22px;
          color: #aaa;
          background-color: #eee;
          text-align: right;
          padding-right: .3em;
          font-size: 10pt;
          font-family: monospace;
          padding-top: .4em;
          line-height: 16px;
        }
      </style>
      <script src="../assets/codeMirror/js/codemirror.js" type="text/javascript"></script>
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <div class="col-12">

            <div style="border: 1px solid #CCC; width:100%">
              <asp:TextBox width="100%" TextMode="MultiLine" id="TbContent" runat="server" Wrap="false" />
            </div>
            <script type="text/javascript">
              $(document).ready(function () {
                var isTextArea = false;
                var editor = CodeMirror.fromTextArea('TbContent', {
                  height: "400px",
                  parserfile: ["parsexml.js"],
                  stylesheet: ["../assets/codeMirror/css/xmlcolors.css"],
                  path: "../assets/codeMirror/js/",
                  continuousScanning: 500,
                  lineNumbers: true
                });
                $('#reindent').show().click(function () {
                  if (!isTextArea) editor.reindent();
                });
              });
            </script>

          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">关 闭</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->