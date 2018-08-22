<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageAdd" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form id="main" defaultbutton="BtnSearch" class="m-l-15 m-r-15" runat="server">

        <template v-if="!pageLoad">
          <div class="text-center" style="margin-top: 100px">
            <img class="mt-3" src="../assets/images/loading.gif" />
            <p class="lead mt-3 text-nowrap">载入中，请稍后...</p>
          </div>
        </template>
        <template v-else>
          <div class="card-box">

            <div class="row">
              <div class="col-10">
                <div class="form-inline">
                  <div class="form-group">
                    <label class="col-form-label m-r-10">关键字</label>
                    <input v-model="word" type="text" placeholder="请输入关键字..." class="form-control" style="width: 400px">
                  </div>

                  <asp:Button id="BtnSearch" text="搜索插件" onClientClick="$vue.search();return false;" class="btn btn-success m-l-10 btn-md"
                    runat="server" />
                </div>
              </div>
              <div class="col-2">
                <a class="btn btn-default float-right btn-md" href="pageManagement.aspx">管理插件</a>
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
                          <small class="text-primary">{{ package.id }}</small>
                          <small v-if="packageIds.indexOf(package.id) !== -1" class="text-danger">（已安装）</small>
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
                          <small class="text-primary">{{ package.id }}</small>
                          <small v-if="packageIds.indexOf(package.id) !== -1" class="text-danger">（已安装）</small>
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
        </template>

      </form>
    </body>

    </html>

    <script src="../assets/vue/vue.min.js"></script>
    <script src="../assets/js/apiUtils.js"></script>
    <script>
      var ssApi = new apiUtils.Api();
      var isNightly = <%=IsNightly%>;
      var version = '<%=Version%>';

      var data = {
        pageLoad: false,
        searching: false,
        word: null,
        packageIds: '<%=PackageIds%>',
        featuredPackages: null,
        searchPackages: null,
        nextLink: null
      };

      var methods = {
        load: function () {
          var $this = this;

          ssApi.get({
            isNightly: isNightly,
            version: version,
            $top: 30,
            $filter: "category eq 'featured'"
          }, function (err, res) {
            if (err || !res || !res.value) return;

            $this.pageLoad = true;
            $this.featuredPackages = res.value;
          }, 'packages');
        },
        search: function (event) {
          if (this.word) {
            this.searching = true;
            ssApi.get({
              isNightly: isNightly,
              version: version,
              $top: 30,
              $filter: "keyword eq '" + this.word + "'"
            }, function (err, res) {
              data.searching = false;
              if (err || !res || !res.value) return;

              data.searchPackages = res.value;
            }, 'packages');
          } else {
            this.searching = false;
            data.searchPackages = null;
          }
        }
      };

      var $vue = new Vue({
        el: '#main',
        data: data,
        methods: methods
      });

      $vue.load();
    </script>
    <!--#include file="../inc/foot.html"-->