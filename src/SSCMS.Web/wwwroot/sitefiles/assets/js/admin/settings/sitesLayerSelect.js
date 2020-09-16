var $url = '/settings/sitesLayerSelect';

var data = utils.init({
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
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getMainUrl: function (site) {
    return utils.getIndexUrl({siteId: site.id});
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