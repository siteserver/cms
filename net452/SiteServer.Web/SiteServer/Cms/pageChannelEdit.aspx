<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageChannelEdit" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">
    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="m-t-0 header-title">
        编辑栏目
      </div>
      <p class="text-muted font-13 m-b-25"></p>

      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">栏目名称</label>
        <div class="col-sm-4">
          <asp:TextBox class="form-control" ID="TbNodeName" runat="server" />
        </div>
        <div class="col-sm-6">
          <asp:RequiredFieldValidator ControlToValidate="TbNodeName" ErrorMessage=" *" ForeColor="red" Display="Dynamic"
            runat="server" />
        </div>
      </div>

      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">栏目索引</label>
        <div class="col-sm-4">
          <asp:TextBox class="form-control" ID="TbNodeIndexName" runat="server" />
        </div>
        <div class="col-sm-6">
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbNodeIndexName" ValidationExpression="[^']+"
            ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
        </div>
      </div>

      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">内容模型插件</label>
        <div class="col-sm-4">
          <asp:DropDownList ID="DdlContentModelPluginId" class="form-control" runat="server"></asp:DropDownList>
        </div>
        <div class="col-sm-6"></div>
      </div>

      <asp:PlaceHolder id="PhContentRelatedPluginIds" runat="server">
        <div class="form-group form-row">
          <label class="col-sm-2 col-form-label text-right">内容关联插件</label>
          <div class="col-sm-10">
            <asp:CheckBoxList ID="CblContentRelatedPluginIds" CssClass="checkbox checkbox-primary" RepeatDirection="Horizontal"
              runat="server"></asp:CheckBoxList>
          </div>
        </div>
      </asp:PlaceHolder>

      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">外部链接</label>
        <div class="col-sm-4">
          <asp:TextBox ID="TbLinkUrl" class="form-control" runat="server" />
        </div>
        <div class="col-sm-6">
          <small class="form-text text-muted">设置后链接将指向此地址</small>
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbLinkUrl" ValidationExpression="[^']+"
            ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
        </div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">链接类型</label>
        <div class="col-sm-4">
          <asp:DropDownList ID="DdlLinkType" class="form-control" runat="server"></asp:DropDownList>
        </div>
        <div class="col-sm-6">
          <small class="form-text text-muted">设置此栏目的链接与子栏目及内容的关系</small>
        </div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">内容默认排序规则</label>
        <div class="col-sm-4">
          <asp:DropDownList ID="DdlTaxisType" class="form-control" runat="server"></asp:DropDownList>
        </div>
        <div class="col-sm-6">
          <small class="form-text text-muted">设置内容默认排序规则后，后台内容列表将改变排序显示规则</small>
        </div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">栏目模板</label>
        <div class="col-sm-4">
          <asp:DropDownList ID="DdlChannelTemplateId" class="form-control" DataTextField="TemplateName" DataValueField="Id"
            runat="server"></asp:DropDownList>
        </div>
        <div class="col-sm-6"></div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">本栏目内容模板</label>
        <div class="col-sm-4">
          <asp:DropDownList ID="DdlContentTemplateId" class="form-control" DataTextField="TemplateName" DataValueField="Id"
            runat="server"></asp:DropDownList>
        </div>
        <div class="col-sm-6"></div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">生成页面路径</label>
        <div class="col-sm-4">
          <asp:TextBox ID="TbFilePath" class="form-control" runat="server" />
        </div>
        <div class="col-sm-6">
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbFilePath" ValidationExpression="[^']+"
            ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
        </div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">栏目页面命名规则</label>
        <div class="col-sm-4">
          <asp:TextBox class="form-control" ID="TbChannelFilePathRule" runat="server" />
        </div>
        <div class="col-sm-6">
          <asp:Button ID="BtnCreateChannelRule" class="btn" Text="构造" runat="server"></asp:Button>
        </div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">内容页面命名规则</label>
        <div class="col-sm-4">
          <asp:TextBox class="form-control" ID="TbContentFilePathRule" runat="server" />
        </div>
        <div class="col-sm-6">
          <asp:Button ID="BtnCreateContentRule" class="btn" Text="构造" runat="server"></asp:Button>
        </div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">可以添加栏目</label>
        <div class="col-sm-4">
          <asp:RadioButtonList ID="RblIsChannelAddable" RepeatDirection="Horizontal" class="radio radio-primary" runat="server">
            <asp:ListItem Text="是" Value="True" Selected="True" />
            <asp:ListItem Text="否" Value="False" />
          </asp:RadioButtonList>
        </div>
        <div class="col-sm-6"></div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">可以添加内容</label>
        <div class="col-sm-4">
          <asp:RadioButtonList ID="RblIsContentAddable" RepeatDirection="Horizontal" class="radio radio-primary" runat="server">
            <asp:ListItem Text="是" Value="True" Selected="True" />
            <asp:ListItem Text="否" Value="False" />
          </asp:RadioButtonList>
        </div>
        <div class="col-sm-6"></div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">栏目图片地址</label>
        <div class="col-sm-4">
          <asp:TextBox ID="TbImageUrl" class="form-control" runat="server" />
        </div>
        <div class="col-sm-6">
          <asp:Button ID="BtnSelectImage" class="btn" Text="选择" runat="server"></asp:Button>
          <asp:Button ID="BtnUploadImage" class="btn" Text="上传" runat="server"></asp:Button>
        </div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">栏目内容</label>
        <div class="col-sm-9">
          <ctrl:TextEditorControl ID="TbContent" runat="server"></ctrl:TextEditorControl>
        </div>
        <div class="col-sm-1"></div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">关键字列表</label>
        <div class="col-sm-4">
          <asp:TextBox Rows="3" TextMode="MultiLine" ID="TbKeywords" class="form-control" runat="server" />
        </div>
        <div class="col-sm-6">
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbKeywords" ValidationExpression="[^']+"
            ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
          <small class="form-text text-muted">注意：各关键词间用英文逗号“,”分割。</small>
        </div>
      </div>
      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">页面描述</label>
        <div class="col-sm-4">
          <asp:TextBox Rows="4" TextMode="MultiLine" ID="TbDescription" class="form-control" runat="server" />
        </div>
        <div class="col-sm-6">
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDescription" ValidationExpression="[^']+"
            ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
        </div>
      </div>

      <ctrl:ChannelAuxiliaryControl ID="CacAttributes" runat="server" />

      <div class="form-group form-row">
        <label class="col-sm-2 col-form-label text-right">栏目组</label>
        <div class="col-sm-10">
          <asp:CheckBoxList CssClass="checkbox checkbox-primary" ID="CblNodeGroupNameCollection" RepeatDirection="Horizontal"
            RepeatColumns="5" runat="server" />
        </div>
      </div>

      <hr />

      <div class="text-center">
        <asp:Button class="btn btn-primary" ID="BtnSubmit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
        <input class="btn m-l-5" type="button" onclick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
      </div>

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->