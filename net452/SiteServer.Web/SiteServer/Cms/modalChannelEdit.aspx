<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelEdit" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">栏目名称</label>
          <div class="col-4">
            <asp:TextBox class="form-control" MaxLength="255" id="TbNodeName" runat="server" />
          </div>
          <div class="col-6">
            <asp:RequiredFieldValidator id="RequiredFieldValidator" ControlToValidate="TbNodeName" errorMessage=" *" foreColor="red"
              Display="Dynamic" runat="server" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">栏目索引</label>
          <div class="col-4">
            <asp:TextBox class="form-control" MaxLength="255" id="TbNodeIndexName" runat="server" />
          </div>
          <div class="col-6">
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbNodeIndexName" ValidationExpression="[^']+" errorMessage=" *"
              foreColor="red" Display="Dynamic" />
          </div>
        </div>

        <asp:PlaceHolder id="PhFilePath" runat="server">
          <div class="form-group form-row">
            <label class="col-2 col-form-label text-right">生成页面路径</label>
            <div class="col-4">
              <asp:TextBox class="form-control" MaxLength="200" id="TbFilePath" runat="server" />
            </div>
            <div class="col-6">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbFilePath" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" Display="Dynamic" />
            </div>
          </div>
        </asp:PlaceHolder>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">栏目图片地址</label>
          <div class="col-4">
            <asp:TextBox class="form-control" ID="TbImageUrl" runat="server" />
          </div>
          <div class="col-6">
            <asp:Literal id="LtlImageUrlButtonGroup" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbImageUrl" ValidationExpression="[^']+" errorMessage=" *"
              foreColor="red" Display="Dynamic" />
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">栏目内容</label>
          <div class="col-9">
            <ctrl:TextEditorControl id="TbContent" runat="server"></ctrl:TextEditorControl>
          </div>
          <div class="col-1 help-block"></div>
        </div>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">关键字列表</label>
          <div class="col-4">
            <asp:TextBox class="form-control" Rows="3" TextMode="MultiLine" id="TbKeywords" runat="server" />
          </div>
          <div class="col-6 help-block">
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbKeywords" ValidationExpression="[^']+" errorMessage=" *"
              foreColor="red" display="Dynamic" /> 注意：各关键词间用英文逗号“,”隔开。
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">页面描述</label>
          <div class="col-4">
            <asp:TextBox class="form-control" Rows="4" TextMode="MultiLine" id="TbDescription" runat="server" />
          </div>
          <div class="col-6 help-block">
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDescription" ValidationExpression="[^']+" errorMessage=" *"
              foreColor="red" display="Dynamic" />
          </div>
        </div>

        <asp:PlaceHolder id="PhLinkUrl" runat="server">
          <div class="form-group form-row">
            <label class="col-2 col-form-label text-right">外部链接</label>
            <div class="col-4">
              <asp:TextBox class="form-control" MaxLength="200" id="TbLinkUrl" runat="server" />
            </div>
            <div class="col-6 help-block">
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbLinkUrl" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" Display="Dynamic" />
            </div>
          </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder id="PhLinkType" runat="server">
          <div class="form-group form-row">
            <label class="col-2 col-form-label text-right">链接类型</label>
            <div class="col-4">
              <asp:DropDownList class="form-control" id="DdlLinkType" runat="server"></asp:DropDownList>
            </div>
            <div class="col-6 help-block">
              设置此栏目的链接与子栏目及内容的关系
            </div>
          </div>
        </asp:PlaceHolder>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">内容默认排序规则</label>
          <div class="col-4">
            <asp:DropDownList ID="DdlTaxisType" class="form-control" runat="server"></asp:DropDownList>
          </div>
          <div class="col-6">
            <small class="form-text text-muted">设置内容默认排序规则后，后台内容列表将改变排序显示规则</small>
          </div>
        </div>

        <asp:PlaceHolder id="PhChannelTemplateId" runat="server">
          <div class="form-group form-row">
            <label class="col-2 col-form-label text-right">栏目模版</label>
            <div class="col-4">
              <asp:DropDownList class="form-control" id="DdlChannelTemplateId" DataTextField="TemplateName" DataValueField="Id" runat="server"></asp:DropDownList>
            </div>
            <div class="col-6 help-block"></div>
          </div>
        </asp:PlaceHolder>

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">内容模版</label>
          <div class="col-4">
            <asp:DropDownList class="form-control" id="DdlContentTemplateId" DataTextField="TemplateName" DataValueField="Id" runat="server"></asp:DropDownList>
          </div>
          <div class="col-6 help-block"></div>
        </div>

        <ctrl:ChannelAuxiliaryControl id="CacAttributes" runat="server" />

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">栏目组</label>
          <div class="col-10">
            <asp:CheckBoxList id="CblNodeGroupNameCollection" RepeatDirection="Horizontal"
              RepeatLayout="Flow" class="checkbox checkbox-primary" runat="server" />
          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button id="BtnSubmit" class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->