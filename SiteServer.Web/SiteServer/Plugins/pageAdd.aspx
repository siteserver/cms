<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <link href="../assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
  <link href="../assets/css/core.css" rel="stylesheet" type="text/css">
  <link href="../assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  <link href="../assets/css/components.css" rel="stylesheet" type="text/css" />
  <link href="../assets/css/pages.css" rel="stylesheet" type="text/css" />
  <link href="../assets/css/menu.css" rel="stylesheet" type="text/css" />
</head>

<body class="fixed-left">
  <div id="wrapper">

    <div class="row" style="margin-top: 100px" v-bind:style="{ display: recentlyPlugins ? 'none' : '' }">
      <div class="col-sm-4"></div>
      <div class="col-sm-4">
        <div class="card-box">
          <div class="row">
            <h4 class="header-title m-t-0">载入中，请稍后...</h4>
            <div class="progress progress-lg m-b-5">
              <div class="progress-bar progress-bar-warning progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0"
                aria-valuemax="100" style="width: 100%;"></div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-sm-4"></div>
    </div>

    <div class="topbar" v-bind:style="{ display: recentlyPlugins ? '' : 'none' }">
      <div class="navbar navbar-default" role="navigation">
        <div class="container">
          <form role="search" class="navbar-left app-search pull-left" style="width: 40%">
            <input v-model="word" type="text" placeholder="搜索插件..." class="form-control app-search-input" style="width: 100%">
            <a href="javascript:;" v-on:click="search"><i class="ion-search"></i></a>
          </form>
        </div>
      </div>
    </div>

    <div class="container" style="margin-top: 100px">

      <div v-bind:style="{ display: searchPlugins ? '' : 'none' }">
        <div class="row">
          <div class="col-sm-12">
            <div class="page-title-box">
              <h4 class="page-title">搜索结果</h4>
            </div>
          </div>
        </div>

        <p class="lead m-t-0" v-bind:style="{ display: searchPlugins && searchPlugins.length === 0 ? '' : 'none' }">
          0 个插件，建议更换搜索词
        </p>

        <div class="row">
          <div class="col-sm-6 col-lg-4" v-for="plugin in searchPlugins">
            <div class="card-box widget-user">
              <a v-bind:href="'pageView.aspx?pluginId=' + plugin.publisher + '-' + plugin.name + '&version=' + plugin.version">
                  <img v-bind:src="'http://plugins.siteserver.cn/files/' + plugin.publisher + '-' + plugin.name + '/' + plugin.icon" class="img-responsive" alt="user">
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
        <div class="row">
          <div class="col-sm-12">
            <div class="page-title-box">
              <a href="#" class="pull-right">更多</a>
              <h4 class="page-title">精选</h4>
            </div>
          </div>
        </div>

        <div class="row">
          <div class="col-sm-6 col-lg-4" v-for="plugin in featuredPlugins">
            <div class="card-box widget-user">
              <a v-bind:href="'pageView.aspx?pluginId=' + plugin.publisher + '-' + plugin.name + '&version=' + plugin.version">
                  <img v-bind:src="'http://plugins.siteserver.cn/files/' + plugin.publisher + '-' + plugin.name + '/' + plugin.icon" class="img-responsive" alt="user">
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

      <div v-bind:style="{ display: popularPlugins  && !searchPlugins ? '' : 'none' }">
        <div class="row">
          <div class="col-sm-12">
            <div class="page-title-box">
              <a href="#" class="pull-right">更多</a>
              <h4 class="page-title">热门</h4>
            </div>
          </div>
        </div>

        <div class="row">
          <div class="col-sm-6 col-lg-4" v-for="plugin in popularPlugins">
            <div class="card-box widget-user">
              <a v-bind:href="'pageView.aspx?pluginId=' + plugin.publisher + '-' + plugin.name + '&version=' + plugin.version">
                  <img v-bind:src="'http://plugins.siteserver.cn/files/' + plugin.publisher + '-' + plugin.name + '/' + plugin.icon" class="img-responsive" alt="user">
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


      <div v-bind:style="{ display: recentlyPlugins  && !searchPlugins ? '' : 'none' }">
        <div class="row">
          <div class="col-sm-12">
            <div class="page-title-box">
              <a href="#" class="pull-right">更多</a>
              <h4 class="page-title">新增</h4>
            </div>
          </div>
        </div>

        <div class="row">
          <div class="col-sm-6 col-lg-4" v-for="plugin in recentlyPlugins">
            <div class="card-box widget-user">
              <a v-bind:href="'pageView.aspx?pluginId=' + plugin.publisher + '-' + plugin.name + '&version=' + plugin.version">
                  <img v-bind:src="'http://plugins.siteserver.cn/files/' + plugin.publisher + '-' + plugin.name + '/' + plugin.icon" class="img-responsive" alt="user">
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
    el: '#wrapper',
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