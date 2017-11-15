<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelEdit" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <bairong:alerts runat="server" />

        <div class="form-horizontal">

          <div class="form-group" id="NodeNameRow" runat="server">
            <label class="col-sm-2 control-label">栏目名称</label>
            <div class="col-sm-4">
              <asp:TextBox class="form-control" Columns="45" MaxLength="255" id="NodeName" runat="server" />
            </div>
            <div class="col-sm-6">
              <asp:RequiredFieldValidator id="RequiredFieldValidator" ControlToValidate="NodeName" errorMessage=" *" foreColor="red" Display="Dynamic"
                runat="server" />
            </div>
          </div>
          <div class="form-group" id="NodeIndexNameRow" runat="server">
            <label class="col-sm-2 control-label">栏目索引</label>
            <div class="col-sm-3">
              <asp:TextBox class="form-control" Columns="45" MaxLength="255" id="NodeIndexName" runat="server" />
            </div>
            <div class="col-sm-6">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="NodeIndexName" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" Display="Dynamic" />
            </div>
          </div>
          <div class="form-group" id="FilePathRow" runat="server">
            <label class="col-sm-2 control-label">生成页面路径</label>
            <div class="col-sm-4">
              <asp:TextBox class="form-control" Columns="45" MaxLength="200" id="FilePath" runat="server" />
            </div>
            <div class="col-sm-6">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="FilePath" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" Display="Dynamic" />
            </div>
          </div>
          <div class="form-group" id="ImageUrlRow" runat="server">
            <label class="col-sm-2 control-label">栏目图片地址</label>
            <div class="col-sm-4">
              <asp:TextBox class="form-control" ID="tbImageUrl" runat="server" />
            </div>
            <div class="col-sm-6">
              <asp:Literal id="ltlImageUrlButtonGroup" runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="tbImageUrl" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" Display="Dynamic" />
            </div>
          </div>
          <div class="form-group" id="ContentRow" runat="server">
            <label class="col-sm-2 control-label">栏目内容</label>
            <div class="col-sm-9">
              <bairong:TextEditorControl id="Content" runat="server"></bairong:TextEditorControl>
            </div>
            <div class="col-sm-1 help-block"></div>
          </div>
          <div class="form-group" id="KeywordsRow" runat="server">
            <label class="col-sm-2 control-label">关键字列表</label>
            <div class="col-sm-4">
              <asp:TextBox class="form-control" Rows="3" Width="350" TextMode="MultiLine" id="Keywords" runat="server" />
            </div>
            <div class="col-sm-6 help-block">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="Keywords" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" display="Dynamic" /> 注意：各关键词间用英文逗号“,”隔开。
            </div>
          </div>
          <div class="form-group" id="DescriptionRow" runat="server">
            <label class="col-sm-2 control-label">页面描述</label>
            <div class="col-sm-4">
              <asp:TextBox class="form-control" Width="350" Rows="4" TextMode="MultiLine" id="Description" runat="server" />
            </div>
            <div class="col-sm-6 help-block">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="Description" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>
          <bairong:ChannelAuxiliaryControl ID="ControlForAuxiliary" runat="server" />
          <div class="form-group" id="LinkUrlRow" runat="server">
            <label class="col-sm-2 control-label">外部链接</label>
            <div class="col-sm-4">
              <asp:TextBox class="form-control" MaxLength="200" id="LinkUrl" runat="server" />
            </div>
            <div class="col-sm-6 help-block">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="LinkUrl" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" Display="Dynamic" />
            </div>
          </div>
          <div class="form-group" id="LinkTypeRow" runat="server">
            <label class="col-sm-2 control-label">链接类型</label>
            <div class="col-sm-4">
              <asp:DropDownList class="form-control" id="LinkType" runat="server"></asp:DropDownList>
            </div>
            <div class="col-sm-6 help-block">
              指示此栏目的链接与栏目下子栏目及内容的关系
            </div>
          </div>
          <div class="form-group" id="ChannelTemplateIDRow" runat="server">
            <label class="col-sm-2 control-label">栏目模版</label>
            <div class="col-sm-4">
              <asp:DropDownList class="form-control" id="ChannelTemplateID" DataTextField="TemplateName" DataValueField="TemplateID" runat="server"></asp:DropDownList>
            </div>
            <div class="col-sm-6 help-block"></div>
          </div>
          <div class="form-group" id="ContentTemplateIDRow" runat="server">
            <label class="col-sm-2 control-label">内容模版</label>
            <div class="col-sm-4">
              <asp:DropDownList class="form-control" id="ContentTemplateID" DataTextField="TemplateName" DataValueField="TemplateID" runat="server"></asp:DropDownList>
            </div>
            <div class="col-sm-6 help-block"></div>
          </div>
          <div class="form-group" id="NodeGroupNameCollectionRow" runat="server">
            <label class="col-sm-2 control-label">栏目组</label>
            <div class="col-sm-10">
              <asp:CheckBoxList id="NodeGroupNameCollection" DataTextField="NodeGroupName" DataValueField="NodeGroupName" RepeatDirection="Horizontal"
                RepeatLayout="Flow" class="checkbox checkbox-primary" runat="server" />
            </div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-sm-11 text-right">
              <asp:Button id="BtnSubmit" class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-sm-1"></div>
          </div>

        </div>

      </form>
    </body>

    </html>