<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateAdd" validateRequest="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <style type="text/css">
        .CodeMirror-line-numbers {
          min-width: 22px;
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
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            <asp:Literal id="LtlPageTitle" runat="server" />
          </div>
          <p class="text-muted font-13 m-b-25">
            模板类型：
            <asp:Literal id="LtlTemplateType" runat="server" />
            <input type="hidden" runat="server" id="HihTemplateType" />
          </p>

          <div class="form-row">
            <div class="col-md-4 mb-3">
              <label for="TbTemplateName">模板名称
                <asp:RequiredFieldValidator ControlToValidate="TbTemplateName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTemplateName" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </label>
              <asp:TextBox class="form-control" id="TbTemplateName" onChange="validateControls()" runat="server" />
            </div>
            <div class="col-md-4 mb-3">
              <label for="DdlCharset">文件编码</label>
              <asp:DropDownList id="DdlCharset" class="form-control" runat="server"></asp:DropDownList>
            </div>
          </div>

          <div class="form-row">
            <div class="col-md-4 mb-3">
              <label for="TbRelatedFileName">模板文件
                <asp:RequiredFieldValidator ControlToValidate="TbRelatedFileName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                />
                <asp:RegularExpressionValidator ControlToValidate="TbRelatedFileName" ValidationExpression="(^T_[^'\.]+)|([^'\.]+[\\/]+T_[^'\.]+)"
                  ErrorMessage="必须以T_开头，并且不能有文件扩展名" foreColor="red" Display="Dynamic" runat="server" />
              </label>
              <asp:TextBox class="form-control" id="TbRelatedFileName" onChange="validateControls()" runat="server" />
              <small class="form-text text-muted">路径以/分隔，文件名以T_开头</small>
            </div>
            <asp:PlaceHolder id="PhCreatedFileFullName" runat="server">
              <div class="col-md-4 mb-3">
                <label for="TbCreatedFileFullName">生成文件名
                  <asp:RequiredFieldValidator ControlToValidate="TbCreatedFileFullName" errorMessage=" *" foreColor="red" display="Dynamic"
                    runat="server" />
                  <asp:RegularExpressionValidator ControlToValidate="TbCreatedFileFullName" ValidationExpression="[^'\.]+" ErrorMessage="不能有文件扩展名"
                    foreColor="red" Display="Dynamic" runat="server" />
                </label>
                <asp:TextBox class="form-control" id="TbCreatedFileFullName" onChange="validateControls()" runat="server" />
                <small class="form-text text-muted">以“~/”开头代表系统根目录，以“@/”开头代表站点根目录</small>
              </div>
            </asp:PlaceHolder>
            <div class="col-md-4 mb-3">
              <label for="DdlCreatedFileExtName">文件扩展名</label>
              <asp:DropDownList ID="DdlCreatedFileExtName" class="form-control" runat="server"></asp:DropDownList>
              <small class="form-text text-muted">文件扩展名将决定模板文件以及生成文件的文件类型</small>
            </div>
          </div>

          <div class="form-group form-row">
            <div class="btn-group btn-group-sm">
              <asp:Literal id="LtlCommands" runat="server" />
              <asp:Button class="btn" id="BtnEditorType" text="采用代码编辑模式" onclick="EditorType_OnClick" runat="server" />
              <button id="reindent" style="display:none" class="btn" onClick="javascript:;">对代码应用格式</button>
            </div>

            <div class="m-t-10" style="border: 1px solid #CCC; width:100%">
              <asp:TextBox class="form-control" TextMode="MultiLine" id="TbContent" runat="server" Height="500" Wrap="false" />
            </div>
            <asp:PlaceHolder id="PhCodeMirror" runat="server">
              <script type="text/javascript">
                $(document).ready(function () {
                  var editor = CodeMirror.fromTextArea('TbContent', {
                    height: "500px",
                    parserfile: ["parsecss.js", "tokenizejavascript.js", "parsejavascript.js", "parsexml.js", "parsehtmlmixed.js"],
                    stylesheet: ["../assets/codeMirror/css/xmlcolors.css", "../assets/codeMirror/css/csscolors.css", "../assets/codeMirror/css/jscolors.css"],
                    path: "../assets/codeMirror/js/",
                    continuousScanning: 500,
                    lineNumbers: true
                  });
                  $('#reindent').show().click(function (e) {
                    editor.reindent();
                    e.preventDefault();
                  });
                });
              </script>
            </asp:PlaceHolder>
          </div>

          <hr />

          <div class="text-center">
            <asp:Button class="btn btn-primary m-r-5" text="确 定" onclick="Submit_OnClick" runat="server" />
            <asp:Button class="btn" text="返 回" CausesValidation="false" onclick="Return_OnClick" runat="server" />
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->