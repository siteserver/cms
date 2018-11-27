var $url = '/pages/plugins/add';

var data = {
  pageLoad: false,
  pageAlert: null,
  isNightly: null,
  pluginVersion: null,
  packageIds: null,
  word: null,
  featuredPackages: null,
  searchPackages: null
};

var methods = {
  getIconUrl: function (url) {
    return 'http://plugins.siteserver.cn/' + url;
  },

  load: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isNightly = res.isNightly;
      $this.pluginVersion = res.pluginVersion;
      $this.packageIds = res.packageIds;

      $apiCloud.get('plugins', {
        params: {
          isNightly: $this.isNightly,
          pluginVersion: $this.pluginVersion
        }
      }).then(function (response) {
        var res = response.data;

        $this.featuredPackages = res.value;
      }).catch(function (error) {
        this.pageAlert = utils.getPageAlert(error);
      }).then(function () {
        $this.pageLoad = true;
      });

    }).catch(function (error) {
      this.pageAlert = utils.getPageAlert(error);
    });
  },

  search: function () {
    var $this = this;

    if (this.word) {

      utils.loading(true);
      $apiCloud.get('plugins?keyword=' + encodeURIComponent(this.word), {
        params: {
          isNightly: $this.isNightly,
          pluginVersion: $this.pluginVersion
        }
      }).then(function (response) {
        var res = response.data;

        $this.searchPackages = res.value;
      }).catch(function (error) {
        this.pageAlert = utils.getPageAlert(error);
      }).then(function () {
        utils.loading(false);
      });

    } else {
      $this.searchPackages = null;
    }
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});