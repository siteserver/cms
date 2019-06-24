<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationCreate" %>
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
          <a class="nav-link" href="pageConfigurationCreateRule.aspx?siteId=<%=SiteId%>">页面命名规则</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="javascript:;">页面生成设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationCreateTrigger.aspx?siteId=<%=SiteId%>">页面生成触发器</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">当内容变动时是否生成本页</label>
        <asp:DropDownList ID="DdlIsCreateContentIfContentChanged" class="form-control" runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">当栏目变动时是否生成本页</label>
        <asp:DropDownList ID="DdlIsCreateChannelIfChannelChanged" class="form-control" runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">生成页面中是否显示相关信息</label>
        <asp:DropDownList ID="DdlIsCreateShowPageInfo" class="form-control" runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">是否设置meta标签强制IE8兼容</label>
        <asp:DropDownList ID="DdlIsCreateIe8Compatible" class="form-control" runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">是否设置meta标签强制浏览器清除缓存</label>
        <asp:DropDownList ID="DdlIsCreateBrowserNoCache" class="form-control" runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">是否设置包含JS容错代码</label>
        <asp:DropDownList ID="DdlIsCreateJsIgnoreError" class="form-control" runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">是否生成页面中包含JQuery脚本引用</label>
        <asp:DropDownList ID="DdlIsCreateWithJQuery" class="form-control" runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">是否启用双击生成页面</label>
        <asp:DropDownList ID="DdlIsCreateDoubleClick" class="form-control" runat="server"></asp:DropDownList>
        <small class="form-text text-muted">
          此功能通常用于制作调试期间，网站开发期间建议启用
        </small>
      </div>

      <div class="form-group">
        <label class="col-form-label">翻页中生成的静态页面最大数（页）
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbCreateStaticMaxPage" ValidationExpression="[^']+"
            ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
        </label>
        <asp:TextBox ID="TbCreateStaticMaxPage" class="form-control" runat="server" />
        <small class="form-text text-muted">
          设置翻页中生成的静态页面最大数，剩余页面将动态获取；设置为0代表将静态页面全部生成
        </small>
      </div>

      <div class="form-group">
        <label class="col-form-label">是否启用默认文件名</label>
        <asp:DropDownList ID="DdlIsCreateUseDefaultFileName" class="form-control" AutoPostBack="true"
          OnSelectedIndexChanged="DdlIsCreateUseDefaultFileName_SelectedIndexChanged" runat="server"></asp:DropDownList>
        <small class="form-text text-muted">
          若启用此选项，文件名如果是默认文件名，文件名将省略
        </small>
      </div>

      <asp:PlaceHolder ID="PhIsCreateUseDefaultFileName" runat="server">
        <div class="form-group">
          <label class="col-form-label">默认文件名</label>
          <asp:TextBox id="TbCreateDefaultFileName" class="form-control" runat="server" />
          <small class="form-text text-muted">
            需要确保Web服务器支持默认文件名
          </small>
        </div>
      </asp:PlaceHolder>

      <div class="form-group">
        <label class="col-form-label">根据添加日期限制是否生成内容</label>
        <asp:DropDownList ID="DdlIsCreateStaticContentByAddDate" class="form-control" AutoPostBack="true"
          OnSelectedIndexChanged="DdlIsCreateStaticContentByAddDate_SelectedIndexChanged" runat="server"></asp:DropDownList>
        <small class="form-text text-muted">
          若启用此选项，系统将不再生成所选添加时间之前的内容页
        </small>
      </div>

      <asp:PlaceHolder ID="PhIsCreateStaticContentByAddDate" runat="server">
        <div class="form-group">
          <label class="col-form-label">生成内容添加日期限制</label>
          <ctrl:DateTimeTextBox id="TbCreateStaticContentAddDate" class="form-control" runat="server" />
          <small class="form-text text-muted">
            在此设置内容添加日期，此日期之前的内容页将不再生成
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