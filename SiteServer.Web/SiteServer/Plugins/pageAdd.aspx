<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageAdd" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form id="main" class="m-l-15 m-r-15" runat="server">

        <div class="text-center" style="margin-top: 100px" v-bind:style="{ display: featuredPackages ? 'none' : '' }">
          <img class="mt-3" src="../assets/layer/skin/default/xubox_loading0.gif" />
          <p class="lead mt-3 text-nowrap">载入中，请稍后...</p>
        </div>

        <div v-bind:style="{ display: featuredPackages ? '' : 'none' }" style="display: none">
          <div class="card-box">

            <div class="row">
              <div class="col-8">
                <div class="form-inline">
                  <div class="form-group">
                    <label class="col-form-label m-r-10">关键字</label>
                    <input v-model="word" type="text" placeholder="请输入关键字..." class="form-control" style="width: 400px">
                  </div>

                  <input type="button" value="搜索插件" v-on:click="search" class="btn btn-success m-l-10 btn-md">
                </div>
              </div>
              <div class="col-4">
                <asp:Button id="BtnUpload" class="btn btn-primary float-right btn-md" Text="手动安装插件" runat="server" />
              </div>
            </div>

          </div>

          <div class="card-box">
            <div v-bind:style="{ display: searchPackages ? '' : 'none' }">
              <div class="page-title-box">
                <h4 class="page-title">搜索结果</h4>
              </div>

              <p class="lead m-t-0" v-bind:style="{ display: searchPackages && searchPackages.length === 0 ? '' : 'none' }">
                0 个插件，建议更换搜索词
              </p>

              <div class="row">
                <div class="col-6 col-lg-4" v-for="package in searchPackages">
                  <div class="card-box widget-user">
                    <a v-bind:href="'pageView.aspx?pluginId=' + package.id">
                      <img v-bind:src="package.iconUrl" class="img-responsive">
                      <div class="wid-u-info">
                        <h5 class="m-t-0 m-b-5">
                          {{ package.title }}
                          <br />
                          <code>{{ package.id }}</code>
                        </h5>
                        <p class="text-muted m-b-5 font-13" v-bind:title="package.description">{{ package.description }}</p>
                        <!-- <span title="插件安装量">
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
                        </span> -->
                      </div>
                    </a>
                  </div>
                </div>
              </div>
            </div>

            <div v-bind:style="{ display: featuredPackages && !searchPackages ? '' : 'none' }">
              <div class="page-title-box">
                <!-- <a href="#" class="float-right">更多</a> -->
                <h4 class="page-title">精选</h4>
              </div>

              <div class="row">
                <div class="col-6 col-lg-4" v-for="package in featuredPackages">
                  <div class="card-box widget-user">
                    <a v-bind:href="'pageView.aspx?pluginId=' + package.id">
                      <img v-bind:src="package.iconUrl" class="img-responsive">
                      <div class="wid-u-info">
                        <h5 class="m-t-0 m-b-5">
                          {{ package.title }}
                          <br />
                          <code>{{ package.id }}</code>
                        </h5>
                        <p class="text-muted m-b-5 font-13" v-bind:title="package.description">{{ package.description }}</p>
                        <!-- <span title="插件安装量">
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
                        </span> -->
                      </div>
                    </a>
                  </div>
                </div>
              </div>
            </div>

          </div>
        </div>

      </form>
    </body>

    </html>

    <script src="../assets/vue/vue.min.js"></script>
    <script src="../assets/js/apiUtils.js"></script>
    <script>
      var api = new apiUtils.Api();
      var isNightly = <%=IsNightly%>;
      var version = '<%=Version%>';

      var data = {
        searching: false,
        word: null,
        featuredPackages: null,
        searchPackages: null,
      };

      api.get({
        isNightly: isNightly,
        version: version,
        $filter: "category eq 'featured'"
      }, function (err, res) {
        if (!err && res && res.length > 0) {
          data.featuredPackages = res
        }
      }, 'packages');

      new Vue({
        el: '#main',
        data: data,
        methods: {
          search: function (event) {
            if (this.word) {
              this.searching = true;
              api.get({
                isNightly: isNightly
              }, function (err, res) {
                data.searching = false;
                data.searchPackages = res;
              }, 'packages/actions/search/' + this.word);
            } else {
              this.searching = false;
              data.searchPackages = null;
            }
          }
        }
      });
    </script>
    <!--#include file="../inc/foot.html"-->