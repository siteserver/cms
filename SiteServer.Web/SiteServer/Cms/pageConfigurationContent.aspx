<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationContent" %>
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
              <b>内容配置</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此修改内容相关设置
            </p>

            <ul class="nav nav-pills m-b-30">
              <li class="">
                <a href="pageConfigurationSite.aspx?publishmentSystemId=<%=PublishmentSystemId%>">站点配置</a>
              </li>
              <li class="active">
                <a href="javascript:;">内容配置</a>
              </li>
              <li class="">
                <a href="pageConfigurationComment.aspx?publishmentSystemId=<%=PublishmentSystemId%>">评论设置</a>
              </li>
              <li class="">
                <a href="pageConfigurationSiteAttributes.aspx?publishmentSystemId=<%=PublishmentSystemId%>">站点属性</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">自动保存外部图片</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsSaveImageInTextEditor" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block"></div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">内容是否自动分页</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsAutoPageInTextEditor" AutoPostBack="true" OnSelectedIndexChanged="DdlIsAutoPageInTextEditor_OnSelectedIndexChanged"
                    class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block">
                  如果修改了自动分页选项，需要将所有内容页重新生成
                </div>
              </div>
              <asp:PlaceHolder id="PhAutoPage" runat="server">
                <div class="form-group">
                  <label class="col-sm-3 control-label">内容自动分页每页字数</label>
                  <div class="col-sm-3">
                    <asp:TextBox class="form-control" ID="TbAutoPageWordNum" runat="server" />
                  </div>
                  <div class="col-sm-6 help-block">
                    <asp:RequiredFieldValidator ControlToValidate="TbAutoPageWordNum" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                    />
                  </div>
                </div>
              </asp:PlaceHolder>
              <div class="form-group">
                <label class="col-sm-3 control-label">是否启用标题换行功能</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsContentTitleBreakLine" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block">
                  在标题中输入两连续的英文空格，内容页中标题将自动换行，列表页将忽略此空格
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">是否启用自动检测敏感词</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsAutoCheckKeywords" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block">
                  当点击确定按钮保存内容的时候，会自动检测敏感词，弹框提示。
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">内容审核机制</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCheckContentUseLevel" AutoPostBack="true" OnSelectedIndexChanged="DdlIsCheckContentUseLevel_OnSelectedIndexChanged"
                    class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block">
                  选择内容审核的机制，需要多级审核的请选择多级审核机制，否则选择默认审核机制
                </div>
              </div>
              <asp:PlaceHolder ID="PhCheckContentLevel" runat="server">
                <div class="form-group">
                  <label class="col-sm-3 control-label">内容审核级别</label>
                  <div class="col-sm-3">
                    <asp:DropDownList ID="DdlCheckContentLevel" class="form-control" runat="server">
                      <asp:ListItem Value="2" Text="二级" Selected="true"></asp:ListItem>
                      <asp:ListItem Value="3" Text="三级"></asp:ListItem>
                      <asp:ListItem Value="4" Text="四级"></asp:ListItem>
                      <asp:ListItem Value="5" Text="五级"></asp:ListItem>
                    </asp:DropDownList>
                  </div>
                  <div class="col-sm-6 help-block">
                    内容在添加后需要经多少次审核才能正式发布
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