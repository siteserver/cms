<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelEdit" Trace="false"%>
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

  <table class="table table-noborder table-hover">
    <tr id="NodeNameRow" runat="server">
      <td width="120">栏目名称：</td>
      <td>
        <asp:TextBox Columns="45" MaxLength="255" id="NodeName" runat="server"/>
        <asp:RequiredFieldValidator id="RequiredFieldValidator" ControlToValidate="NodeName" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"/>
      </td>
      </tr>
      <tr id="NodeIndexNameRow" runat="server">
        <td>栏目索引：</td>
        <td>
          <asp:TextBox Columns="45" MaxLength="255" id="NodeIndexName" runat="server"/>
          <asp:RegularExpressionValidator
            runat="server"
            ControlToValidate="NodeIndexName"
            ValidationExpression="[^']+"
            errorMessage=" *" foreColor="red" 
            Display="Dynamic" />
        </td>
      </tr>
      <tr id="FilePathRow" runat="server">
        <td>生成页面路径：</td>
        <td>
          <asp:TextBox Columns="45" MaxLength="200" id="FilePath" runat="server"/>
          <asp:RegularExpressionValidator
            runat="server"
            ControlToValidate="FilePath"
            ValidationExpression="[^']+"
            errorMessage=" *" foreColor="red" 
            Display="Dynamic" />
        </td>
      </tr>
      <tr id="ImageUrlRow" runat="server">
        <td>栏目图片地址：</td>
        <td>
          <asp:TextBox ID="tbImageUrl" class="input-xlarge" runat="server"/>
          <asp:Literal id="ltlImageUrlButtonGroup" runat="server" />
          <asp:
        </td>
      </tr>
      <tr id="ContentRow" runat="server">
        <td colspan="2" class="center">
        <bairong:TextEditorControl id="Content" runat="server"></bairong:TextEditorControl>
        </td>
      </tr>
      <tr id="KeywordsRow" runat="server">
        <td>关键字列表：</td>
        <td>
          <asp:TextBox Rows="3" Width="350" MaxLength="100" TextMode="MultiLine" id="Keywords" runat="server" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="Keywords"
                ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />        
            <=100 <br>
          <span>注意：各关键词间用英文逗号“,”隔开。</span>
        </td>
      </tr>
      <tr id="DescriptionRow" runat="server">
        <td>页面描述：</td>
        <td>
          <asp:TextBox Width="350" Rows="4" MaxLength="200" TextMode="MultiLine" id="Description" runat="server" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="Description"
                ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />        
            <=200
        </td>
    </tr>
    <bairong:ChannelAuxiliaryControl ID="ControlForAuxiliary" runat="server"/>
    <tr id="LinkUrlRow" runat="server">
      <td>外部链接：</td>
      <td>
      <asp:TextBox MaxLength="200" id="LinkUrl" class="input-xlarge" runat="server"/>
      <asp:RegularExpressionValidator
        runat="server"
        ControlToValidate="LinkUrl"
        ValidationExpression="[^']+"
        errorMessage=" *" foreColor="red" 
        Display="Dynamic" />
      </td>
    </tr>
    <tr id="LinkTypeRow" runat="server">
      <td>链接类型：</td>
    <td>
      <asp:DropDownList id="LinkType" runat="server"></asp:DropDownList>
      <br>
      <span>指示此栏目的链接与栏目下子栏目及内容的关系</span>
    </td>
    </tr>
    <tr id="ChannelTemplateIDRow" runat="server">
      <td>栏目模版：</td>
      <td>
        <asp:DropDownList id="ChannelTemplateID" DataTextField="TemplateName" DataValueField="TemplateID" runat="server"></asp:DropDownList>
      </td>
    </tr>
    <tr id="ContentTemplateIDRow" runat="server">
      <td>内容模版：</td>
      <td>
        <asp:DropDownList id="ContentTemplateID" DataTextField="TemplateName" DataValueField="TemplateID" runat="server"></asp:DropDownList>
      </td>
    </tr>
    <tr id="NodeGroupNameCollectionRow" runat="server">
      <td>栏目组：</td>
      <td>
        <asp:CheckBoxList id="NodeGroupNameCollection" DataTextField="NodeGroupName" DataValueField="NodeGroupName" RepeatDirection="Horizontal" class="noborder" RepeatLayout="Flow" runat="server"/>
      </td>
    </tr>
  </table>

  <hr />
  <div class="center">
    <asp:Button class="btn btn-primary" id="btnSubmit" text="确 定"  runat="server" onClick="Submit_OnClick" />
  </div>

</form>
</body>
</html>
