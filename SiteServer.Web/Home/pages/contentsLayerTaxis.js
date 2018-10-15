var $api = new apiUtils.Api(apiUrl + '/home/contentsLayerTaxis');

var data = {
  siteId: parseInt(pageUtils.getQueryString('siteId')),
  channelId: parseInt(pageUtils.getQueryString('channelId')),
  contentIds: pageUtils.getQueryString('contentIds'),
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

    parent.pageUtils.loading(true);
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