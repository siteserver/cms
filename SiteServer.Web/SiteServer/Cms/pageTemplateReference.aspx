<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateReference" enableviewstate="false"%>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            STL语言参考
          </div>
          <p class="text-muted font-13 m-b-25">
            STL语言为SiteServer模板语言(SiteServer Template Language)的缩写，是一种和HTML语言类似的服务器端语言。
          </p>

          <ul class="nav nav-pills m-b-20">
            <li class="nav-item active">
              <a class="nav-link" href="javascript:;" onclick="$('#containerElements').show();$('#containerEntities').hide();$('.nav-item').removeClass('active');$(this).parent().addClass('active');">STL 元素</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="javascript:;" onclick="$('#containerElements').hide();$('#containerEntities').show();$('.nav-item').removeClass('active');$(this).parent().addClass('active');">STL 实体</a>
            </li>
          </ul>

          <div id="containerElements">
            <div class="panel panel-default m-t-10">
              <div class="panel-body p-0">
                <div class="table-responsive">
                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr>
                        <th width="130">元素</th>
                        <th width="100">用途</th>
                        <th>简介</th>
                        <th>属性</th>
                      </tr>
                    </thead>
                    <tbody>
                      <asp:Literal ID="LtlTemplateElements" runat="server"></asp:Literal>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>

          <div id="containerEntities" style="display: none">
            <div class="panel panel-default m-t-10">
              <div class="panel-body p-0">
                <div class="table-responsive">
                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr>
                        <th width="130">实体</th>
                        <th width="100">用途</th>
                        <th>简介</th>
                        <th>属性</th>
                      </tr>
                    </thead>
                    <tbody>
                      <asp:Literal ID="LtlTemplateEntities" runat="server"></asp:Literal>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->