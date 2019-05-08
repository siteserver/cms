<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSiteEdit" %>
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
      <div class="m-t-0 header-title"> 修改站点 </div>
      <div class="form-group">
        <label class="col-form-label">站点名称
          <asp:RequiredFieldValidator ControlToValidate="TbSiteName" errorMessage=" *" foreColor="red" Display="Dynamic"
            runat="server" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSiteName" ValidationExpression="[^']+"
            errorMessage=" *" foreColor="red" Display="Dynamic" />
        </label>
        <asp:TextBox cssClass="form-control" id="TbSiteName" runat="server" />
      </div>
      <asp:PlaceHolder id="PhSiteDir" runat="server">
        <div class="form-group">
          <label class="col-form-label">文件夹名称
            <asp:RequiredFieldValidator ControlToValidate="TbSiteDir" errorMessage=" *" foreColor="red" Display="Dynamic"
              runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSiteDir" ValidationExpression="[\\.a-zA-Z0-9_-]+"
              foreColor="red" ErrorMessage=" 只允许包含字母、数字、下划线、中划线及小数点" Display="Dynamic" />
          </label>
          <asp:TextBox cssClass="form-control" id="TbSiteDir" runat="server" />
          <small class="form-text text-muted">实际在服务器中保存此网站的文件夹名称，此路径必须以英文或拼音命名。</small>
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhParentId" runat="server">
        <div class="form-group">
          <label class="col-form-label">上级站点</label>
          <asp:DropDownList ID="DdlParentId" class="form-control" runat="server"></asp:DropDownList>
        </div>
      </asp:PlaceHolder>
      <div class="form-group">
        <label class="col-form-label"> 内容表 </label>
        <asp:RadioButtonList id="RblTableRule" class="radio radio-primary" AutoPostBack="true" OnSelectedIndexChanged="RblTableRule_OnSelectedIndexChanged"
          RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
        <asp:PlaceHolder id="PhTableChoose" runat="server">
          <asp:DropDownList ID="DdlTableChoose" class="form-control" runat="server"></asp:DropDownList>
          <small class="form-text text-muted">请选择已存在的内容表</small>
        </asp:PlaceHolder>
        <asp:PlaceHolder id="PhTableHandWrite" runat="server">
          <asp:TextBox ID="TbTableHandWrite" class="form-control" runat="server"></asp:TextBox>
          <small class="form-text text-muted">请输入内容表名称，系统将检测数据库是否已存在指定的内容表，如果不存在系统将创建此内容表。</small>
        </asp:PlaceHolder>
      </div>
      <div class="form-group">
        <label class="col-form-label">站点排序
          <asp:RequiredFieldValidator ControlToValidate="TbTaxis" errorMessage=" *" foreColor="red" display="Dynamic"
            runat="server" />
          <asp:RegularExpressionValidator ControlToValidate="TbTaxis" ValidationExpression="\d+" Display="Dynamic"
            ErrorMessage="排序必须为数字" foreColor="red" runat="server" />
        </label>
        <asp:TextBox ID="TbTaxis" class="form-control" runat="server"></asp:TextBox>
        <small class="form-text text-muted">设置站点排序，排序数字大的站点将排在其他站点之前</small>
      </div>
      <div class="form-group">
        <label class="col-form-label">内容审核机制</label>
        <asp:RadioButtonList id="RblIsCheckContentUseLevel" AutoPostBack="true" OnSelectedIndexChanged="RblIsCheckContentUseLevel_OnSelectedIndexChanged"
          RepeatDirection="Horizontal" class="radio radio-primary" runat="server"></asp:RadioButtonList>
      </div>
      <asp:PlaceHolder id="PhCheckContentLevel" runat="server">
        <div class="form-group">
          <label class="col-form-label">内容审核级别</label>
          <asp:DropDownList id="DdlCheckContentLevel" class="form-control" runat="server">
            <asp:ListItem Value="2" Text="二级" Selected="true"></asp:ListItem>
            <asp:ListItem Value="3" Text="三级"></asp:ListItem>
            <asp:ListItem Value="4" Text="四级"></asp:ListItem>
            <asp:ListItem Value="5" Text="五级"></asp:ListItem>
          </asp:DropDownList>
        </div>
      </asp:PlaceHolder>
      <hr />
      <asp:Button class="btn btn-danger" id="BtnSubmit" text="修 改" OnClick="Submit_OnClick" runat="server" />
      <asp:Button class="btn" text="返 回" onclick="Return_OnClick" runat="server" />
    </div>
  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->