var $url = '/admin/settings/sitesLayerSelect';

var data = utils.initData({
  sites: null,
  rootSiteId: null,
  tableNames: null,
  site: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.sites = res.sites;
      $this.rootSiteId = res.rootSiteId;
      $this.tableNames = res.tableNames;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getMainUrl: function (site) {
    return '../main.cshtml?siteId=' + site.id;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});