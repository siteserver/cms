<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateAdd" validateRequest="false" %>
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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <style type="text/css">
  .CodeMirror-line-numbers { width: 22px; color: #aaa; background-color: #eee; text-align: right; padding-right: .3em; font-size: 10pt; font-family: monospace; padding-top: .4em; line-height: 16px; }
  </style>
  <script src="../assets/codeMirror/js/codemirror.js" type="text/javascript"></script>

  <div class="popover popover-static">
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server" /></h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="120">模板名称：</td>
          <td>
            <asp:TextBox Columns="35" class="input-xlarge" MaxLength="50" id="TemplateName" runat="server" />
            <asp:RequiredFieldValidator id="RequiredFieldValidator" ControlToValidate="TemplateName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator id="RegularExpressionValidator" runat="server" ControlToValidate="TemplateName" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
          <td colspan="2" class="align-right">模板类型：<%=TemplateTypeString%>
            <input type="hidden" runat="server" id="TemplateType" /></td>
        </tr>
        <tr>
          <td>模板文件：</td>
          <td colspan="3">
            <asp:TextBox class="input-xlarge" id="RelatedFileName" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="RelatedFileName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator ControlToValidate="RelatedFileName" ValidationExpression="(^T_[^'\.]+)|([^'\.]+[\\/]+T_[^'\.]+)" ErrorMessage="必须以T_开头，并且不能有文件扩展名" foreColor="red" Display="Dynamic" runat="server"/>
            <span>路径以/分隔，文件名以T_开头</span>
          </td>
        </tr>
        <tr id="CreatedFileFullNameRow" runat="server">
          <td>生成文件名：</td>
          <td colspan="3">
            <asp:TextBox class="input-xlarge" id="CreatedFileFullName" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="CreatedFileFullName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator ControlToValidate="CreatedFileFullName" ValidationExpression="[^'\.]+" ErrorMessage="不能有文件扩展名<br>" foreColor="red" Display="Dynamic" runat="server"/>
            <span>以“~/”开头代表系统根目录，以“@/”开头代表站点根目录</span>
          </td>
        </tr>
        <tr>
          <td>文件扩展名：</td>
          <td colspan="3">
            <asp:DropDownList ID="CreatedFileExtNameDropDownList" runat="server"></asp:DropDownList>
          </td>
        </tr>
        <tr>
          <td>网页编码：</td>
          <td colspan="3">
            <asp:DropDownList id="Charset" runat="server"></asp:DropDownList>
          </td>
        </tr>
        <tr>
          <td colspan="4" class="align-right">
            <asp:Literal id="LtlCommands" runat="server" />
            <asp:Button class="btn btn-info" id="btnEditorType" text="采用代码编辑模式" onclick="EditorType_OnClick" runat="server" />
            <input id="reindent" style="display:none" type="button" class="btn btn-info" onClick="javascript:;" value="对代码应用格式" />
          </td>
        </tr>
        <tr>
          <td colspan="4" class="center">
            <div style="border: 1px solid #CCC; width:100%">
              <asp:TextBox width="100%" TextMode="MultiLine" id="Content" runat="server" Height="500" Wrap="false" />
            </div>
            <asp:PlaceHolder id="phCodeMirror" runat="server">
            <script type="text/javascript">
              $(document).ready(function(){
                var isTextArea = false;
                var editor = CodeMirror.fromTextArea('Content', {
                    height: "500px",
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
            </asp:PlaceHolder>
          </td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="btnSubmit" text="确 定" onclick="Submit_OnClick" runat="server" />
            <input type="button" class="btn" onClick="location.href='pageTemplate.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&templateType=<%=Request.QueryString["templateType"]%>';" value="返 回" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
