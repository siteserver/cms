<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteSave" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">
    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item active">
          <a class="nav-link" href="pageSite.aspx">系统站点管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageSiteUrlWeb.aspx">Web地址</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageSiteUrlAssets.aspx">文件地址</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageSiteUrlApi.aspx">API地址</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="siteTables.cshtml">内容表管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageSiteKeyword.aspx">敏感词管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageSiteTemplate.aspx">站点模板管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="siteTemplateOnline.cshtml">在线站点模板</a>
        </li>
      </ul>
    </div>
    <ctrl:alerts runat="server" />
    <div class="card-box">
      <div class="m-t-0 header-title"> 保存站点模板 </div>
      <asp:PlaceHolder id="PhWelcome" runat="server">
        <p class="text-muted font-13 m-b-25"> 保存站点模板能够将此站点的文件、栏目、内容、模板、插件等保存在站点模板文件夹中 </p>
        <div class="form-group">
          <label class="col-form-label"> 站点模板名称
            <asp:RequiredFieldValidator ControlToValidate="TbSiteTemplateName" errorMessage=" *" foreColor="red"
              Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSiteTemplateName" ValidationExpression="[^']+"
              errorMessage=" *" foreColor="red" Display="Dynamic" />
          </label>
          <asp:TextBox id="TbSiteTemplateName" class="form-control" runat="server" />
        </div>
        <div class="form-group">
          <label class="col-form-label"> 站点模板文件夹名称
            <asp:RequiredFieldValidator ControlToValidate="TbSiteTemplateDir" errorMessage=" *" foreColor="red" Display="Dynamic"
              runat="server" />
            <asp:RegularExpressionValidator ControlToValidate="TbSiteTemplateDir" ValidationExpression="^T_.+"
              errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
          </label>
          <asp:TextBox id="TbSiteTemplateDir" class="form-control" runat="server" />
          <small class="form-text text-muted">文件名必须以T_开头，且以英文或拼音取名</small>
        </div>
        <div class="form-group">
          <label class="col-form-label"> 站点模板网站地址
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbWebSiteUrl" ValidationExpression="[^']+"
              errorMessage=" *" foreColor="red" Display="Dynamic" />
          </label>
          <asp:TextBox id="TbWebSiteUrl" class="form-control" runat="server" />
        </div>
        <div class="form-group">
          <label class="col-form-label"> 站点模板介绍
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDescription" ValidationExpression="[^']+"
              errorMessage=" *" foreColor="red" Display="Dynamic" />
          </label>
          <asp:TextBox id="TbDescription" class="form-control" Rows="4" TextMode="MultiLine" runat="server" />
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhSaveFiles" runat="server" Visible="false">
        <p class="text-muted font-13 m-b-25"> 保存站点文件，点击下一步将站点的文件保存到站点模板中 </p>
        <div class="form-group">
          <label class="col-form-label"> 是否保存全部文件 </label>
          <asp:RadioButtonList ID="RblIsSaveAllFiles" AutoPostBack="true" OnSelectedIndexChanged="RblIsSaveAllFiles_SelectedIndexChanged"
            RepeatDirection="Horizontal" class="radio radio-primary" runat="server"></asp:RadioButtonList>
        </div>
        <asp:PlaceHolder ID="PhDirectoriesAndFiles" runat="server" Visible="false">
          <div class="form-group">
            <label class="col-form-label"> 指定保存的文件及文件夹 </label>
            <asp:CheckBoxList ID="CblDirectoriesAndFiles" RepeatDirection="Horizontal" class="checkbox checkbox-primary"
              RepeatColumns="5" runat="server"></asp:CheckBoxList>
          </div>
        </asp:PlaceHolder>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhSaveSiteContents" runat="server" Visible="false">
        <p class="text-muted font-13 m-b-25"> 保存站点内容，点击下一步将站点的栏目及内容信息保存到站点模板中 </p>
        <div class="form-group">
          <label class="col-form-label"> 是否保存内容数据 </label>
          <asp:RadioButtonList ID="RblIsSaveContents" RepeatDirection="Horizontal" class="radio radio-primary" runat="server"></asp:RadioButtonList>
        </div>
        <div class="form-group">
          <label class="col-form-label"> 是否保存全部栏目 </label>
          <asp:RadioButtonList ID="RblIsSaveAllChannels" AutoPostBack="true" OnSelectedIndexChanged="RblIsSaveAllChannels_SelectedIndexChanged"
            RepeatDirection="Horizontal" class="radio radio-primary" runat="server"></asp:RadioButtonList>
        </div>
        <asp:PlaceHolder ID="PhChannels" runat="server" Visible="false">
          <div class="form-group">
            <label class="col-form-label"> 指定保存的栏目 </label>
            <small class="form-text text-muted">从下边选择需要保存的栏目，所选栏目的下级栏目将自动保存到站点模板中</small>
            <hr />
            <asp:Literal id="LtlChannelTree" runat="server" />
          </div>
        </asp:PlaceHolder>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhSaveSiteStyles" runat="server" Visible="false">
        <p class="text-muted font-13 m-b-25"> 保存站点数据，点击下一步将站点数据（包括模板、内容表、配置信息、插件等）保存到站点模板中 </p>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhUploadImageFile" runat="server" Visible="false">
        <p class="text-muted font-13 m-b-25"> 载入样图文件，选择样图文件的名称 </p>
        <div class="form-group">
          <label class="col-form-label"> 样图文件 </label>
          <input type="file" id="HifSamplePicFile" class="form-control" runat="server" />
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhDone" runat="server" Visible="false">
        <p class="text-muted font-13 m-b-25"> 站点模板保存成功，您已经完成保存站点模板的操作 </p>
        <div class="alert alert-success">
          <h4>保存成功，站点模版保存在"SiteFiles\SiteTemplates\
            <%=TbSiteTemplateDir.Text%>"文件夹中</h4>
        </div>
      </asp:PlaceHolder>
      <hr />
      <div class="text-center">
        <asp:Button class="btn btn-primary m-r-5" id="BtnWelcomeNext" onclick="BtnWelcomeNext_Click" runat="server"
          text="下一步"></asp:button>
        <asp:Button class="btn btn-primary m-r-5" id="BtnSaveFilesNext" onclick="BtnSaveFilesNext_Click" visible="false"
          runat="server" text="下一步"></asp:button>
        <asp:Button class="btn btn-primary m-r-5" id="BtnSaveSiteContentsNext" onclick="BtnSaveSiteContentsNext_Click"
          visible="false" runat="server" text="下一步"></asp:button>
        <asp:Button class="btn btn-primary m-r-5" id="BtnSaveSiteStylesNext" onclick="BtnSaveSiteStylesNext_Click"
          visible="false" runat="server" text="下一步"></asp:button>
        <asp:Button class="btn btn-primary m-r-5" id="BtnUploadImageFileNext" onclick="BtnUploadImageFileNext_Click"
          visible="false" runat="server" text="下一步"></asp:button>
        <asp:Button class="btn m-r-5" text="返 回" onclick="Return_OnClick" runat="server" />
      </div>
    </div>
  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->