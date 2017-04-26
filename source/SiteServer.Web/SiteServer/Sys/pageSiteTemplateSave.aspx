<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PageSiteTemplateSave" %>
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

  <div class="popover popover-static">
    <h3 class="popover-title">保存站点模板</h3>
    <div class="popover-content">
    
      <asp:PlaceHolder id="PhWelcome" runat="server" Visible="false">

        <blockquote>
          <p>欢迎使用保存为站点模板向导</p>
          <small>您能够将此站点的站点文件、站点内容、模板、菜单显示方式、采集规则、投票信息等保存在站点模板中。</small>
        </blockquote>

        <table class="table table-noborder table-hover">
          <tr>
            <td width="200">站点模板名称：</td>
            <td><asp:TextBox width="300" MaxLength="50" id="TbSiteTemplateName" runat="server"/>
              <asp:RequiredFieldValidator
                ControlToValidate="TbSiteTemplateName"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic"
                runat="server"/>
              <asp:RegularExpressionValidator
                runat="server"
                ControlToValidate="TbSiteTemplateName"
                ValidationExpression="[^']+"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic" /></td>
          </tr>
          <tr>
            <td width="200">站点模板文件夹名称：</td>
            <td><asp:TextBox Columns="25" MaxLength="500" id="TbSiteTemplateDir" runat="server"/>
              <asp:RequiredFieldValidator
                ControlToValidate="TbSiteTemplateDir"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic"
                runat="server"/>
              <asp:RegularExpressionValidator 
                 ControlToValidate="TbSiteTemplateDir" ValidationExpression="^T_.+" 
                 errorMessage=" *" foreColor="red" display="Dynamic" runat="server"/>
              （文件名必须以T_开头，且以英文或拼音取名） </td>
          </tr>
          <tr>
            <td width="200">站点模板网站地址：</td>
            <td><asp:TextBox width="300" MaxLength="200" id="TbWebSiteUrl" runat="server"/>
              <asp:RegularExpressionValidator
                runat="server"
                ControlToValidate="TbWebSiteUrl"
                ValidationExpression="[^']+"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic" /></td>
          </tr>
          <tr>
            <td width="200">站点模板介绍：</td>
            <td><asp:TextBox width="300" Rows="4" TextMode="MultiLine" id="TbDescription" runat="server"/>
              <asp:RegularExpressionValidator
                runat="server"
                ControlToValidate="TbDescription"
                ValidationExpression="[^']+"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic" /></td>
          </tr>
        </table>

      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhSaveFiles" runat="server" Visible="false">

        <blockquote>
          <p>保存站点文件</p>
          <small>点击下一步将站点的文件保存到站点模板中。</small>
        </blockquote>

        <table class="table table-noborder table-hover">
          <tr>
            <td width="200">是否保存全部文件：</td>
            <td><asp:RadioButtonList ID="RblIsSaveAllFiles" AutoPostBack="true" OnSelectedIndexChanged="RblIsSaveAllFiles_SelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
          </tr>
          <asp:PlaceHolder ID="PhDirectoriesAndFiles" runat="server" Visible="false">
          <tr>
            <td width="200">指定保存的文件及文件夹：</td>
            <td><asp:CheckBoxList ID="CblDirectoriesAndFiles" RepeatDirection="Horizontal" class="noborder" RepeatColumns="5" runat="server"></asp:CheckBoxList></td>
          </tr>
          </asp:PlaceHolder>
        </table>

      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhSaveSiteContents" runat="server" Visible="false">

        <blockquote style="margin-top:20px;">
          <p>保存站点内容</p>
          <small>点击下一步将站点的栏目及内容信息保存到站点模板中。</small>
        </blockquote>

        <table class="table table-noborder table-hover">
          <tr>
            <td width="200">是否保存内容数据：</td>
            <td><asp:RadioButtonList ID="RblIsSaveContents" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
          </tr>
          <tr>
            <td width="200">是否保存全部栏目：</td>
            <td><asp:RadioButtonList ID="RblIsSaveAllChannels" AutoPostBack="true" OnSelectedIndexChanged="RblIsSaveAllChannels_SelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
          </tr>
          <asp:PlaceHolder ID="PhChannels" runat="server" Visible="false">
          <tr>
            <td width="200" valign="top">
              指定保存的栏目：
              <br />
              <span>从下边选择需要保存的栏目，所选栏目的上级栏目将自动保存到站点模板中</span>
            </td>
            <td><asp:Literal id="LtlChannelTree" runat="server" /></td>
          </tr>
          </asp:PlaceHolder>
        </table>

      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhSaveSiteStyles" runat="server" Visible="false">

        <blockquote>
          <p>保存站点信息</p>
          <small>点击下一步将站点信息（包括模板、辅助表、菜单显示方式、采集规则及投票信息等）保存到站点模板中。</small>
        </blockquote>

      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhUploadImageFile" runat="server" Visible="false">
        
        <blockquote>
          <p>载入样图文件</p>
          <small>选择样图文件的名称</small>
        </blockquote>

        <table class="table table-noborder table-hover">
          <tr>
            <td width="150">选择样图文件：</td>
            <td><input type=file id=HifSamplePicFile size="35" runat="server"/></td>
          </tr>
        </table>

      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhDone" runat="server" Visible="false">

        <blockquote>
          <p>站点模板保存成功</p>
          <small>您已经完成保存站点模板的操作。</small>
        </blockquote>

        <div class="alert alert-success">
          <h4>站点模版保存在"SiteFiles\SiteTemplates\<%=TbSiteTemplateDir.Text%>"文件夹中。</h4>
        </div>

      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhOperatingError" runat="server" Visible="false">

        <blockquote>
          <p>发生错误</p>
          <small>执行向导过程中出错</small>
        </blockquote>

        <div class="alert alert-error">
          <h4><asp:Literal ID="LtlErrorMessage" runat="server"></asp:Literal></h4>
        </div>

      </asp:PlaceHolder>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn" ID="BtnPrevious" OnClick="BtnPrevious_Click" runat="server" Text="< 上一步"></asp:Button>
            <asp:Button class="btn btn-primary" id="BtnNext" onclick="BtnNext_Click" runat="server" text="下一步 >"></asp:button>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
