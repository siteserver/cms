<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelEdit" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-horizontal">

          <asp:PlaceHolder id="PhNodeName" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">栏目名称</label>
              <div class="col-sm-4">
                <asp:TextBox class="form-control" MaxLength="255" id="TbNodeName" runat="server" />
              </div>
              <div class="col-sm-6">
                <asp:RequiredFieldValidator id="RequiredFieldValidator" ControlToValidate="TbNodeName" errorMessage=" *" foreColor="red"
                  Display="Dynamic" runat="server" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhNodeIndexName" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">栏目索引</label>
              <div class="col-sm-4">
                <asp:TextBox class="form-control" MaxLength="255" id="TbNodeIndexName" runat="server" />
              </div>
              <div class="col-sm-6">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbNodeIndexName" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" Display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhFilePath" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">生成页面路径</label>
              <div class="col-sm-4">
                <asp:TextBox class="form-control" MaxLength="200" id="TbFilePath" runat="server" />
              </div>
              <div class="col-sm-6">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbFilePath" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" Display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhImageUrl" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">栏目图片地址</label>
              <div class="col-sm-4">
                <asp:TextBox class="form-control" ID="TbImageUrl" runat="server" />
              </div>
              <div class="col-sm-6">
                <asp:Literal id="LtlImageUrlButtonGroup" runat="server" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbImageUrl" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" Display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhContent" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">栏目内容</label>
              <div class="col-sm-9">
                <ctrl:TextEditorControl id="TbContent" runat="server"></ctrl:TextEditorControl>
              </div>
              <div class="col-sm-1 help-block"></div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhKeywords" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">关键字列表</label>
              <div class="col-sm-4">
                <asp:TextBox class="form-control" Rows="3" TextMode="MultiLine" id="TbKeywords" runat="server" />
              </div>
              <div class="col-sm-6 help-block">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbKeywords" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" display="Dynamic" /> 注意：各关键词间用英文逗号“,”隔开。
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhDescription" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">页面描述</label>
              <div class="col-sm-4">
                <asp:TextBox class="form-control" Rows="4" TextMode="MultiLine" id="TbDescription" runat="server" />
              </div>
              <div class="col-sm-6 help-block">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDescription" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <ctrl:ChannelAuxiliaryControl id="AcAttributes" runat="server" />

          <asp:PlaceHolder id="PhLinkUrl" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">外部链接</label>
              <div class="col-sm-4">
                <asp:TextBox class="form-control" MaxLength="200" id="TbLinkUrl" runat="server" />
              </div>
              <div class="col-sm-6 help-block">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbLinkUrl" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" Display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhLinkType" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">链接类型</label>
              <div class="col-sm-4">
                <asp:DropDownList class="form-control" id="DdlLinkType" runat="server"></asp:DropDownList>
              </div>
              <div class="col-sm-6 help-block">
                指示此栏目的链接与栏目下子栏目及内容的关系
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhChannelTemplateId" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">栏目模版</label>
              <div class="col-sm-4">
                <asp:DropDownList class="form-control" id="DdlChannelTemplateId" DataTextField="TemplateName" DataValueField="TemplateId"
                  runat="server"></asp:DropDownList>
              </div>
              <div class="col-sm-6 help-block"></div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhContentTemplateId" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">内容模版</label>
              <div class="col-sm-4">
                <asp:DropDownList class="form-control" id="DdlContentTemplateId" DataTextField="TemplateName" DataValueField="TemplateId"
                  runat="server"></asp:DropDownList>
              </div>
              <div class="col-sm-6 help-block"></div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder id="PhNodeGroupNameCollection" runat="server">
            <div class="form-group">
              <label class="col-sm-2 control-label">栏目组</label>
              <div class="col-sm-10">
                <asp:CheckBoxList id="CblNodeGroupNameCollection" DataTextField="NodeGroupName" DataValueField="NodeGroupName" RepeatDirection="Horizontal"
                  RepeatLayout="Flow" class="checkbox checkbox-primary" runat="server" />
              </div>
            </div>
          </asp:PlaceHolder>

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