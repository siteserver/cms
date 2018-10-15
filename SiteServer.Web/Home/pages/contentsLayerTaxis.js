var $api = new utils.Api('/home/contentsLayerTaxis');

var data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  contentIds: utils.getQueryString('contentIds'),
  pageLoad: false,
  pageAlert: null,
  isUp: true,
  taxis: 1
};

var methods = {
  loadConfig: function () {
    this.pageLoad = true;
  },
  btnSubmitClick: function () {
    var $this = this;

    parent.utils.loading(true);
    $api.post({
        siteId: $this.siteId,
        channelId: $this.channelId,
        contentIds: $this.contentIds,
        isUp: $this.isUp,
        taxis: $this.taxis
      },
      function (err, res) {
        if (err || !res || !res.value) return;

        parent.location.reload(true);
      }
    );
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.loadConfig();
  }
});