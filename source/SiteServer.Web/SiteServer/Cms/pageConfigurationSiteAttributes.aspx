<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationSiteAttributes" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script type="text/javascript" charset="utf-8" src="../assets/validate.js"></script>
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form class="container" runat="server">
        <bairong:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>站点属性</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此设置站点属性
            </p>

            <ul class="nav nav-pills m-b-30">
              <li class="">
                <a href="pageConfigurationSite.aspx?publishmentSystemId=<%=PublishmentSystemId%>">站点配置</a>
              </li>
              <li class="">
                <a href="pageConfigurationContent.aspx?publishmentSystemId=<%=PublishmentSystemId%>">内容配置</a>
              </li>
              <li class="">
                <a href="pageConfigurationComment.aspx?publishmentSystemId=<%=PublishmentSystemId%>">评论设置</a>
              </li>
              <li class="active">
                <a href="javascript:;">站点属性</a>
              </li>

            </ul>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">站点字段</label>
                <div class="col-sm-3">
                  <asp:Literal id="LtlSettings" runat="server" />
                </div>
                <div class="col-sm-6 help-block">
                  点击按钮进入站点字段设置界面
                </div>
              </div>

              <div class="form-group">
                <label class="col-sm-3 control-label">站点名称</label>
                <div class="col-sm-3">
                  <asp:TextBox Columns="25" MaxLength="50" id="TbPublishmentSystemName" runat="server" class="form-control" />
                </div>
                <div class="col-sm-6 help-block">
                  <asp:RequiredFieldValidator ControlToValidate="TbPublishmentSystemName" errorMessage=" *" foreColor="red" display="Dynamic"
                    runat="server" />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPublishmentSystemName" ValidationExpression="[^']+" errorMessage=" *"
                    foreColor="red" display="Dynamic" />
                </div>
              </div>

              <bairong:AuxiliaryControl ID="AcAttributes" runat="server" />

              <hr />

              <div class="form-group m-b-0">
                <div class="col-sm-offset-3 col-sm-9">
                  <asp:Button class="btn btn-primary" id="BtnSubmit" text="确 定" onclick="Submit_OnClick" runat="server" />
                </div>
              </div>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>