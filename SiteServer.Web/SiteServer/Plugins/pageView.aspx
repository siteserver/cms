<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageView" %>
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
  <style>
    .readme img {
      max-width: 100%;
    }
  </style>
</head>

<body class="fixed-left">
  <form class="form-inline" runat="server">
  <div id="wrapper">

    <div class="row" style="margin-top: 100px" v-bind:style="{ display: plugin ? 'none' : '' }">
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

    <div class="topbar" v-bind:style="{ display: plugin ? '' : 'none' }">
      <div class="navbar navbar-default" role="navigation">
        <div class="container">
          <div class="pull-left">
            <a href="pageAdd.aspx" class="button-menu-mobile open-left">
                <i class="ion-android-arrow-back"></i>
            </a>
            <span class="clearfix"></span>
          </div>
          <div class="pull-right">
            <asp:Button class="btn btn-success waves-light" onClick="BtnInstall_Click" id="BtnInstall" Text="安装插件" style="margin-top: 16px;" runat="server" />
            <span class="clearfix"></span>
          </div>
        </div>
      </div>
    </div>

    <div class="container" style="margin-top: 100px; width: 720px" v-bind:style="{ display: plugin ? '' : 'none' }">

      <asp:PlaceHolder id="PhFailure" visible="false" runat="server">
        <div class="panel panel-border panel-danger">
            <div class="panel-heading">
                <h3 class="panel-title">插件安装失败</h3>
            </div>
            <div class="panel-body">
                <p><asp:Literal id="LtlErrorMessage" runat="server" /></p>
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

      <div class="row">
        <div class="col-sm-12">
          <div class="card-box m-t-20">
            <div class="widget-user" style="min-height: auto">
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
            </div>
            <hr>

            <div class="media m-b-30 ">
              <div class="media-body">
                <a v-bind:style="{ display: plugin.homepage ? '' : 'none' }" v-bind:href="plugin.homepage" class="media-meta pull-right">插件主页</a>
                <h4 class="text-primary m-0">作者：{{ plugin.publisher }}</h4>
                <small class="text-muted">版本号：{{ plugin.version }}</small>
              </div>
            </div>

            <div class="row">
              <div v-html="plugin.readme" class="readme col-md-12  m-b-30"></div>
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