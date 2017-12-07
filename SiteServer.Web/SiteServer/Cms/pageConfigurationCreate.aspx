<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationCreate" %>
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
              <b>页面命名规则</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此对生成页面进行详细设置
            </p>

            <ul class="nav nav-pills m-b-30">
              <li>
                <a href="pageTemplateFilePathRule.aspx?publishmentSystemId=<%=PublishmentSystemId%>">页面命名规则</a>
              </li>
              <li class="active">
                <a href="javascript:;">页面生成设置</a>
              </li>
              <li>
                <a href="pageConfigurationCreateTrigger.aspx?publishmentSystemId=<%=PublishmentSystemId%>">页面生成触发器</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">当内容变动时是否生成本页</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateContentIfContentChanged" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block"></div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">当栏目变动时是否生成本页</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateChannelIfChannelChanged" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block"></div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">生成页面中是否显示相关信息</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateShowPageInfo" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block"></div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">是否设置meta标签强制IE8兼容</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateIe8Compatible" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block"></div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">是否设置meta标签强制浏览器清除缓存</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateBrowserNoCache" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block"></div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">是否设置包含JS容错代码</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateJsIgnoreError" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block"></div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">内容列表及搜索是否可包含重复标题</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateSearchDuplicate" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block"></div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">是否生成页面中包含JQuery脚本引用</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateWithJQuery" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block"></div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">是否启用双击生成页面</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateDoubleClick" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block">
                  此功能通常用于制作调试期间，网站开发期间建议启用
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">翻页中生成的静态页面最大数</label>
                <div class="col-sm-3">
                  <asp:TextBox ID="TbCreateStaticMaxPage" class="form-control" runat="server" />
                </div>
                <div class="col-sm-6 help-block">
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbCreateStaticMaxPage" ValidationExpression="[^']+" ErrorMessage=" *"
                    ForeColor="red" Display="Dynamic" />页（设置翻页中生成的静态页面最大数，剩余页面将动态获取；设置为0代表将静态页面全部生成）
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">根据添加日期限制是否生成内容</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateStaticContentByAddDate" class="form-control" AutoPostBack="true"
                    OnSelectedIndexChanged="DdlIsCreateStaticContentByAddDate_SelectedIndexChanged" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block">
                  若启用此选项，系统将不再生成所选添加时间之前的内容页
                </div>
              </div>
              <asp:PlaceHolder ID="PhIsCreateStaticContentByAddDate" runat="server">
                <div class="form-group">
                  <label class="col-sm-3 control-label">生成内容添加日期限制</label>
                  <div class="col-sm-3">
                    <bairong:DateTimeTextBox id="TbCreateStaticContentAddDate" class="form-control" runat="server" />
                  </div>
                  <div class="col-sm-6 help-block">
                    在此设置内容添加日期，此日期之前的内容页将不再生成
                  </div>
                </div>
              </asp:PlaceHolder>
              <div class="form-group">
                <label class="col-sm-3 control-label">是否启用多线程生成页面</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsCreateMultiThread" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block">
                  此功能通常用于CMS服务器配置较高而且现在生成页面时CPU和内存利用率不太高（不超过60%）时建议启用
                </div>
              </div>

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