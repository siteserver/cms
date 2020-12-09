var $url = "/write/select";
var $defaultWidth = 160;

var data = utils.init({
  pageType: null,
  sites: null,
  root: null,
  siteId: 0,
  channelIds: []
});

var methods = {
  apiGet: function() {
    var $this = this;

    $api.get($url).then(function(response) {
      var res = response.data;

      if (res.unauthorized) {
        $this.pageType = 'Unauthorized';
        return;
      }

      $this.sites = res.sites;
      $this.siteId = res.siteId;
      $this.root = res.root;
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      utils.loading($this, false);
    });
  },

  apiSubmit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId
    }).then(function(response) {
      var res = response.data;

      $this.channelIds = [];
      $this.root = res.root;
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      utils.loading($this, false);
    });
  },

  handleSiteIdChange: function() {
    this.apiSubmit(this.siteId);
  },

  btnAddClick: function() {
    if (this.channelId == 0) return;

    location.href = utils.getRootUrl('write/editor', {
      siteId: this.siteId,
      channelId: this.channelId
    });
  },

  btnImportClick: function() {
    if (this.channelId == 0) return;

    utils.openLayer({
      title: '批量导入Word', 
      name: 'Word', 
      url: utils.getRootUrl('write/contentsLayerWord', {
        siteId: this.siteId,
        channelId: this.channelId
      }),
      full: true
    });
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  computed: {
    channelId: function() {
      return this.channelIds.length === 0 ? 0 : this.channelIds[this.channelIds.length - 1];
    },
  },
  created: function() {
    this.apiGet();
  }
});
