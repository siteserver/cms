<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageView" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <!--#include file="../inc/head.html"-->
  </head>

  <body>
    <form id="main" class="m-l-15 m-r-15" runat="server">

      <div class="text-center" style="margin-top: 100px" v-bind:style="{ display: plugin ? 'none' : '' }">
        <img class="mt-3" src="../assets/layer/skin/default/xubox_loading0.gif" />
        <p class="lead mt-3 text-nowrap">载入中，请稍后...</p>
      </div>

      <div v-bind:style="{ display: plugin ? '' : 'none' }" style="display: none">

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
            <img v-bind:src="'http://download.siteserver.cn/plugins/' + plugin.publisher + '-' + plugin.name + '/' + plugin.icon" class="img-responsive"
              alt="user">
            <div class="wid-u-info m-b-5">
              <h5 class="m-t-0 m-b-5">
                {{ plugin.displayName }}
                <code>{{ plugin.publisher + '-' + plugin.name }}</code>
              </h5>
              <p class="text-muted m-b-5 font-13" v-bind:title="plugin.description">{{ plugin.description }}</p>
              <span>作者：{{ plugin.publisher }}</span>
              <span style="margin: 0 5px"></span>
              <span>版本号：{{ plugin.version }}</span>
              <span style="margin: 0 5px"></span>
              <span>
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
              </span>
            </div>

            <asp:Button cssClass="btn btn-primary" onClick="BtnInstall_Click" id="BtnInstall" Text="安装插件" runat="server" />
            <a class="btn btn-success m-l-5" v-bind:style="{ display: plugin.homepage ? '' : 'none' }" v-bind:href="plugin.homepage">插件主页</a>
            <asp:Button class="btn m-l-5" onClick="Return_Click" Text="返 回" runat="server" />

          </div>

          <hr />

          <div v-html="plugin.readme" class="readme m-b-10"></div>
        </div>

      </div>
    </form>
  </body>

  </html>

  <script src="../assets/vue/vue.min.js"></script>
  <script src="../assets/cloudUtils.js"></script>
  <script>
    // var api = new cloudUtils.Api('http://localhost:5000/api');
    var api = new cloudUtils.Api('http://cloud.siteserver.cn/api');
    var pluginId = api.getQueryStringByName('pluginId');

    var data = {
      plugin: null,
    };

    api.get(null, function (err, res) {
      if (!err && res) {
        data.plugin = res
      }
    }, 'plugins/' + pluginId);

    new Vue({
      el: '#main',
      data: data
    });
  </script>
  <!--#include file="../inc/foot.html"-->