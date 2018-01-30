<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageView" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <!--#include file="../inc/head.html"-->
  </head>

  <body>
    <form id="main" class="m-l-15 m-r-15" runat="server">

      <div class="text-center" style="margin-top: 100px" v-bind:style="{ display: package.id ? 'none' : '' }">
        <img class="mt-3" src="../assets/layer/skin/default/xubox_loading0.gif" />
        <p class="lead mt-3 text-nowrap">载入中，请稍后...</p>
      </div>

      <div v-bind:style="{ display: package.id ? '' : 'none' }" style="display: none">

        <asp:PlaceHolder id="PhFailure" visible="false" runat="server">
          <div class="panel panel-border panel-danger">
            <div class="panel-heading">
              <h3 class="panel-title">插件安装失败</h3>
            </div>
            <div class="panel-body">
              <p>
                <asp:Literal id="LtlErrorMessage" runat="server" />
              </p>
            </div>
          </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder id="PhSuccess" visible="false" runat="server">
          <div class="panel panel-border panel-primary">
            <div class="panel-heading">
              <h3 class="panel-title">插件安装成功</h3>
            </div>
            <div class="panel-body">
              <p>恭喜，插件安装成功</p>
            </div>
          </div>
        </asp:PlaceHolder>

        <div class="card-box">
          <div class="widget-user" style="min-height: auto">
            <img v-bind:src="package.iconUrl" style="height: 110px; width: 110px;" class="img-responsive">
            <div class="wid-u-info m-b-5">
              <h4 class="m-t-0 m-b-10">
                {{ package.title }}
              </h4>

              <p class="lead m-b-5 font-13">
                <span>插件Id：
                  <span class="badge badge-primary">{{ package.id }}</span>
                </span>
                <span>作者：
                  <span class="badge badge-primary">{{ package.authors ? package.authors.join(',') : '' }}</span>
                </span>
                <span style="margin: 0 5px"></span>
                <span>版本号：
                  <span class="badge badge-primary">{{ package.version }}</span>
                </span>
                <span style="margin: 0 5px"></span>
                <span>上架日期：
                  <span class="badge badge-primary">{{ package.published }}</span>
                </span>
                <span style="margin: 0 5px"></span>
              </p>

              <p class="lead m-b-5 font-13" v-bind:style="{ display: package.tags ? '' : 'none' }" style="display: none">标签： {{ package.tags }}</p>

              <p class="lead">
                {{ package.description }}
              </p>

              <!-- <span>
                安装量：
                <i class="ion-ios-cloud-download-outline" style="font-size: 18px;"></i>
                <small style="font-size: 14px;">33K </small>
              </span>
              <span style="margin: 0 5px"></span>
              <span>
                综合评分：
                <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                <i class="ion-ios-star-half" style="color: #ffb900;font-size: 18px"></i>
                <i class="ion-ios-star-outline" style="color: #ffb900;font-size: 18px"></i>
              </span> -->
            </div>

            <hr />

            <div>
              <asp:Button cssClass="btn btn-primary" onClick="BtnInstall_Click" id="BtnInstall" Text="安装插件" runat="server" />
              <a class="btn btn-success m-l-5" v-bind:style="{ display: package.projectUrl ? '' : 'none' }" target="_blank" v-bind:href="package.projectUrl">插件主页</a>
              <asp:Button class="btn m-l-5" onClick="Return_Click" Text="返 回" runat="server" />
            </div>

          </div>

          <!-- <hr />

          <div v-html="package.readme" class="readme m-b-10"></div> -->
        </div>

      </div>
    </form>
  </body>

  </html>

  <script src="../assets/vue/vue.min.js"></script>
  <script src="../assets/cloudUtils.js"></script>
  <script>
    var api = new cloudUtils.Api();
    var pluginId = api.getQueryStringByName('pluginId');

    var allowNightlyBuild = <%=AllowNightlyBuild%>;
    var allowPrereleaseVersions = <%=AllowPrereleaseVersions%>;

    var data = {
      package: {}
    };

    api.get({
      allowNightlyBuild: allowNightlyBuild,
      allowPrereleaseVersions: allowPrereleaseVersions
    }, function (err, res) {
      if (!err && res) {
        data.package = res
      }
    }, 'packages/' + pluginId);

    new Vue({
      el: '#main',
      data: data
    });
  </script>
  <!--#include file="../inc/foot.html"-->