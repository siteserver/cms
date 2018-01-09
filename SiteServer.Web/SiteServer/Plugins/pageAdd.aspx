<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageAdd" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <div id="main" class="m-l-15 m-r-15">

        <div class="text-center" style="margin-top: 100px" v-bind:style="{ display: recentlyPlugins ? 'none' : '' }">
          <img class="mt-3" src="../assets/layer/skin/default/xubox_loading0.gif" />
          <p class="lead mt-3 text-nowrap">载入中，请稍后...</p>
        </div>

        <div v-bind:style="{ display: recentlyPlugins ? '' : 'none' }">
          <div class="card-box">

            <div class="form-inline">
              <div class="form-group">
                <label class="col-form-label m-r-10">关键字</label>
                <input v-model="word" type="text" placeholder="请输入关键字..." class="form-control" style="width: 400px">
              </div>

              <input type="button" value="搜索插件" v-on:click="search" class="btn btn-success m-l-10 btn-md">
            </div>

          </div>

          <div class="card-box">
            <div v-bind:style="{ display: searchPlugins ? '' : 'none' }">
              <div class="page-title-box">
                <h4 class="page-title">搜索结果</h4>
              </div>

              <p class="lead m-t-0" v-bind:style="{ display: searchPlugins && searchPlugins.length === 0 ? '' : 'none' }">
                0 个插件，建议更换搜索词
              </p>

              <div class="row">
                <div class="col-6 col-lg-4" v-for="plugin in searchPlugins">
                  <div class="card-box widget-user">
                    <a v-bind:href="'pageView.aspx?pluginId=' + plugin.publisher + '-' + plugin.name + '&version=' + plugin.version">
                      <img v-bind:src="'http://plugins.siteserver.cn/files/' + plugin.publisher + '-' + plugin.name + '/' + plugin.icon" class="img-responsive"
                        alt="user">
                      <div class="wid-u-info">
                        <h4 class="m-t-0 m-b-5">
                          {{ plugin.displayName }}
                          <code>{{ plugin.publisher + '-' + plugin.name }}</code>
                        </h4>
                        <p class="text-muted m-b-5 font-13" v-bind:title="plugin.description">{{ plugin.description }}</p>
                        <span title="插件安装量">
                          <i class="ion-ios-cloud-download-outline" style="font-size: 18px;"></i>
                          <small style="font-size: 14px;">33K </small>
                        </span>
                        <span style="margin: 0 5px"></span>
                        <span title="插件综合评分">
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star-half" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star-outline" style="color: #ffb900;font-size: 18px"></i>
                        </span>
                      </div>
                    </a>
                  </div>
                </div>
              </div>
            </div>

            <div v-bind:style="{ display: featuredPlugins && !searchPlugins ? '' : 'none' }">
              <div class="page-title-box">
                <a href="#" class="float-right">更多</a>
                <h4 class="page-title">精选</h4>
              </div>

              <div class="row">
                <div class="col-6 col-lg-4" v-for="plugin in featuredPlugins">
                  <div class="card-box widget-user">
                    <a v-bind:href="'pageView.aspx?pluginId=' + plugin.publisher + '-' + plugin.name + '&version=' + plugin.version">
                      <img v-bind:src="'http://plugins.siteserver.cn/files/' + plugin.publisher + '-' + plugin.name + '/' + plugin.icon" class="img-responsive"
                        alt="user">
                      <div class="wid-u-info">
                        <h4 class="m-t-0 m-b-5">
                          {{ plugin.displayName }}
                          <code>{{ plugin.publisher + '-' + plugin.name }}</code>
                        </h4>
                        <p class="text-muted m-b-5 font-13" v-bind:title="plugin.description">{{ plugin.description }}</p>
                        <span title="插件安装量">
                          <i class="ion-ios-cloud-download-outline" style="font-size: 18px;"></i>
                          <small style="font-size: 14px;">33K </small>
                        </span>
                        <span style="margin: 0 5px"></span>
                        <span title="插件综合评分">
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star-half" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star-outline" style="color: #ffb900;font-size: 18px"></i>
                        </span>
                      </div>
                    </a>
                  </div>
                </div>
              </div>
            </div>

            <div v-bind:style="{ display: popularPlugins && !searchPlugins ? '' : 'none' }">
              <div class="page-title-box">
                <a href="#" class="float-right">更多</a>
                <h4 class="page-title">热门</h4>
              </div>

              <div class="row">
                <div class="col-6 col-lg-4" v-for="plugin in popularPlugins">
                  <div class="card-box widget-user">
                    <a v-bind:href="'pageView.aspx?pluginId=' + plugin.publisher + '-' + plugin.name + '&version=' + plugin.version">
                      <img v-bind:src="'http://plugins.siteserver.cn/files/' + plugin.publisher + '-' + plugin.name + '/' + plugin.icon" class="img-responsive"
                        alt="user">
                      <div class="wid-u-info">
                        <h4 class="m-t-0 m-b-5">
                          {{ plugin.displayName }}
                          <code>{{ plugin.publisher + '-' + plugin.name }}</code>
                        </h4>
                        <p class="text-muted m-b-5 font-13" v-bind:title="plugin.description">{{ plugin.description }}</p>
                        <span title="插件安装量">
                          <i class="ion-ios-cloud-download-outline" style="font-size: 18px;"></i>
                          <small style="font-size: 14px;">33K </small>
                        </span>
                        <span style="margin: 0 5px"></span>
                        <span title="插件综合评分">
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star-half" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star-outline" style="color: #ffb900;font-size: 18px"></i>
                        </span>
                      </div>
                    </a>
                  </div>
                </div>
              </div>
            </div>

            <div v-bind:style="{ display: recentlyPlugins && !searchPlugins ? '' : 'none' }">
              <div class="page-title-box">
                <a href="#" class="float-right">更多</a>
                <h4 class="page-title">新增</h4>
              </div>

              <div class="row">
                <div class="col-6 col-lg-4" v-for="plugin in recentlyPlugins">
                  <div class="card-box widget-user">
                    <a v-bind:href="'pageView.aspx?pluginId=' + plugin.publisher + '-' + plugin.name + '&version=' + plugin.version">
                      <img v-bind:src="'http://plugins.siteserver.cn/files/' + plugin.publisher + '-' + plugin.name + '/' + plugin.icon" class="img-responsive"
                        alt="user">
                      <div class="wid-u-info">
                        <h4 class="m-t-0 m-b-5">
                          {{ plugin.displayName }}
                          <code>{{ plugin.publisher + '-' + plugin.name }}</code>
                        </h4>
                        <p class="text-muted m-b-5 font-13" v-bind:title="plugin.description">{{ plugin.description }}</p>
                        <span title="插件安装量">
                          <i class="ion-ios-cloud-download-outline" style="font-size: 18px;"></i>
                          <small style="font-size: 14px;">33K </small>
                        </span>
                        <span style="margin: 0 5px"></span>
                        <span title="插件综合评分">
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star-half" style="color: #ffb900;font-size: 18px"></i>
                          <i class="ion-ios-star-outline" style="color: #ffb900;font-size: 18px"></i>
                        </span>
                      </div>
                    </a>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

      </div>
    </body>

    </html>

    <script src="../assets/vue/vue.min.js"></script>
    <script src="../assets/cloudUtils.js"></script>
    <script>
      // var api = new cloudUtils.Api('http://localhost:5000/api');
      var api = new cloudUtils.Api('http://cloud.siteserver.cn/api');

      var data = {
        searching: false,
        word: null,
        featuredPlugins: null,
        popularPlugins: null,
        recentlyPlugins: null,
        searchPlugins: null,
      };

      api.get({
        name: 'featured'
      }, function (err, res) {
        if (!err && res && res.length > 0) {
          data.featuredPlugins = res
        }
      }, 'plugins');

      api.get({
        name: 'popular'
      }, function (err, res) {
        if (!err && res && res.length > 0) {
          data.popularPlugins = res
        }
      }, 'plugins');

      api.get({
        name: 'recently'
      }, function (err, res) {
        if (!err && res && res.length > 0) {
          data.recentlyPlugins = res
        }
      }, 'plugins');

      new Vue({
        el: '#main',
        data: data,
        methods: {
          search: function (event) {
            if (this.word) {
              this.searching = true;
              api.get(null, function (err, res) {
                data.searching = false;
                data.searchPlugins = res;
              }, 'plugins/search/' + this.word);
            } else {
              this.searching = false;
              data.searchPlugins = null;
            }
          }
        }
      });
    </script>