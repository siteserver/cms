<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationContent" %>
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
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationSite.aspx?siteId=<%=SiteId%>">站点设置</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="javascript:;">内容设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationSiteAttributes.aspx?siteId=<%=SiteId%>">站点属性</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">自动保存外部图片</label>
        <asp:DropDownList ID="DdlIsSaveImageInTextEditor" class="form-control" runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">内容是否自动分页</label>
        <asp:DropDownList ID="DdlIsAutoPageInTextEditor" AutoPostBack="true" OnSelectedIndexChanged="DdlIsAutoPageInTextEditor_OnSelectedIndexChanged"
          class="form-control" runat="server"></asp:DropDownList>
        <small class="form-text text-muted">
          如果修改了自动分页选项，需要将所有内容页重新生成
        </small>
      </div>

      <asp:PlaceHolder id="PhAutoPage" runat="server">
        <div class="form-group">
          <label class="col-form-label">内容自动分页每页字数
            <asp:RequiredFieldValidator ControlToValidate="TbAutoPageWordNum" ErrorMessage=" *" ForeColor="red" Display="Dynamic"
              runat="server" />
          </label>
          <asp:TextBox class="form-control" ID="TbAutoPageWordNum" runat="server" />
        </div>
      </asp:PlaceHolder>

      <div class="form-group">
        <label class="col-form-label">是否启用标题换行功能</label>
        <asp:DropDownList ID="DdlIsContentTitleBreakLine" class="form-control" runat="server"></asp:DropDownList>
        <small class="form-text text-muted">
          在标题中输入两连续的英文空格，内容页中标题将自动换行，列表页将忽略此空格
        </small>
      </div>

      <div class="form-group">
        <label class="col-form-label">是否启用自动检测敏感词</label>
        <asp:DropDownList ID="DdlIsAutoCheckKeywords" class="form-control" runat="server"></asp:DropDownList>
        <small class="form-text text-muted">
          当点击确定按钮保存内容的时候，会自动检测敏感词，弹框提示。
        </small>
      </div>

      <div class="form-group">
        <label class="col-form-label">内容审核机制</label>
        <asp:DropDownList ID="DdlIsCheckContentUseLevel" AutoPostBack="true" OnSelectedIndexChanged="DdlIsCheckContentUseLevel_OnSelectedIndexChanged"
          class="form-control" runat="server"></asp:DropDownList>
        <small class="form-text text-muted">
          选择内容审核的机制，需要多级审核的请选择多级审核机制，否则选择默认审核机制
        </small>
      </div>

      <asp:PlaceHolder ID="PhCheckContentLevel" runat="server">
        <div class="form-group">
          <label class="col-form-label">内容审核级别</label>
          <asp:DropDownList ID="DdlCheckContentLevel" class="form-control" runat="server">
            <asp:ListItem Value="2" Text="二级" Selected="true"></asp:ListItem>
            <asp:ListItem Value="3" Text="三级"></asp:ListItem>
            <asp:ListItem Value="4" Text="四级"></asp:ListItem>
            <asp:ListItem Value="5" Text="五级"></asp:ListItem>
          </asp:DropDownList>
          <small class="form-text text-muted">
            内容在添加后需要经多少次审核才能正式发布
          </small>
        </div>
      </asp:PlaceHolder>

      <hr />

      <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->