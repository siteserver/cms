<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationComment" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form class="container" runat="server">
        <bairong:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>评论设置</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此修改内容评论设置
            </p>

            <ul class="nav nav-pills m-b-30">
              <li class="">
                <a href="pageConfigurationSite.aspx?publishmentSystemId=<%=PublishmentSystemId%>">站点配置</a>
              </li>
              <li class="">
                <a href="pageConfigurationContent.aspx?publishmentSystemId=<%=PublishmentSystemId%>">内容配置</a>
              </li>
              <li class="active">
                <a href="javascript:;">评论设置</a>
              </li>
              <li class="">
                <a href="pageConfigurationSiteAttributes.aspx?publishmentSystemId=<%=PublishmentSystemId%>">站点属性</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">是否启用评论功能</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCommentable" RepeatDirection="Horizontal" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlIsCommentable_OnSelectedIndexChanged"
                    runat="server">
                    <asp:ListItem Text="启用" Value="True" Selected="True" />
                    <asp:ListItem Value="False" Text="禁用" />
                  </asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block">
                  在此开启或禁用评论功能
                </div>
              </div>
              <asp:PlaceHolder ID="PhComments" runat="server">
                <div class="form-group">
                  <label class="col-sm-3 control-label">新评论是否需要审核</label>
                  <div class="col-sm-3">
                    <asp:DropDownList ID="DdlIsCheckComments" RepeatDirection="Horizontal" class="form-control" runat="server">
                      <asp:ListItem Text="需要审核" Value="True" Selected="True" />
                      <asp:ListItem Value="False" Text="不需要审核" />
                    </asp:DropDownList>
                  </div>
                  <div class="col-sm-6 help-block">
                    在此设置新评论是否需要审核
                  </div>
                </div>
                <div class="form-group">
                  <label class="col-sm-3 control-label">是否启用匿名评论</label>
                  <div class="col-sm-3">
                    <asp:DropDownList ID="DdlIsAnonymousComments" RepeatDirection="Horizontal" class="form-control" runat="server">
                      <asp:ListItem Value="True" Text="启用" Selected="True" />
                      <asp:ListItem Value="False" Text="禁用" />
                    </asp:DropDownList>
                  </div>
                  <div class="col-sm-6 help-block">
                    在此设置评论是否启用匿名评论, 设置更改之前的附件仍存放在原来位置
                  </div>
                </div>
              </asp:PlaceHolder>

              <hr />

              <div class="form-group m-b-0">
                <div class="col-sm-offset-3 col-sm-9">
                  <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
                </div>
              </div>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>