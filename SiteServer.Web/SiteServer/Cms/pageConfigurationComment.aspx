<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationComment" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">

        <div class="card-box">
          <ul class="nav nav-pills">
            <li class="nav-item">
              <a class="nav-link" href="pageConfigurationSite.aspx?publishmentSystemId=<%=PublishmentSystemId%>">站点设置</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageConfigurationContent.aspx?publishmentSystemId=<%=PublishmentSystemId%>">内容设置</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="javascript:;">评论设置</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageConfigurationSiteAttributes.aspx?publishmentSystemId=<%=PublishmentSystemId%>">站点属性</a>
            </li>
          </ul>
        </div>

        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="form-group">
            <label class="col-form-label">是否启用评论功能</label>
            <asp:DropDownList ID="DdlIsCommentable" RepeatDirection="Horizontal" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlIsCommentable_OnSelectedIndexChanged"
              runat="server">
              <asp:ListItem Text="启用" Value="True" Selected="True" />
              <asp:ListItem Value="False" Text="禁用" />
            </asp:DropDownList>
            <small class="form-text text-muted">
              在此开启或禁用评论功能
            </small>
          </div>

          <asp:PlaceHolder ID="PhComments" runat="server">
            <div class="form-group">
              <label class="col-form-label">新评论是否需要审核</label>
              <asp:DropDownList ID="DdlIsCheckComments" RepeatDirection="Horizontal" class="form-control" runat="server">
                <asp:ListItem Text="需要审核" Value="True" Selected="True" />
                <asp:ListItem Value="False" Text="不需要审核" />
              </asp:DropDownList>
              <small class="form-text text-muted">
                在此设置新评论是否需要审核
              </small>
            </div>

            <div class="form-group">
              <label class="col-form-label">是否启用匿名评论</label>
              <asp:DropDownList ID="DdlIsAnonymousComments" RepeatDirection="Horizontal" class="form-control" runat="server">
                <asp:ListItem Value="True" Text="启用" Selected="True" />
                <asp:ListItem Value="False" Text="禁用" />
              </asp:DropDownList>
              <small class="form-text text-muted">
                在此设置评论是否启用匿名评论, 设置更改之前的附件仍存放在原来位置
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